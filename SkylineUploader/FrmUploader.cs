﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HelperClasses;
using Microsoft.Win32;
using SkylineUploader.Classes;
using SkylineUploaderDomain.DataModel;
using SkylineUploaderDomain.DataModel.Classes;
using Telerik.WinControls.UI;
using System.ServiceProcess;
using System.Xml.Linq;
using Telerik.WinControls;
using ServiceSettings = SkylineUploaderDomain.DataModel.Classes.ServiceSettings;

namespace SkylineUploader
{
    public partial class FrmUploader : RadForm
    {
        private List<GridData> _folderData;
        private BackgroundWorker _bwCheckSql;
        private List<SqlHelper> _sqlInstances;
        private bool _sqlFound = false;
        private bool _dataSourceSet = false;
        private string _dataSource = string.Empty;


        public FrmUploader()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            //Database.SetInitializer<UploaderDbContext>(null);

            if (!FileHelper.CreateProgramDataFolder())
            {
                //Debug.Error("Unable to create the ProgramData Folder");
                MessageBox.Show("Unable to create the ProgramData Folder. Closing application", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            Debug.CheckLogFileSizes();

            Debug.Log("","","" );
            Debug.Log("","","" );
            Debug.Log("***********","***********","" );
            Debug.Log("FrmUploader","Starting up","" );
            Debug.Log("***********","***********","" );
            Debug.Log("","","" );

            CheckAppConfig();

            if (_sqlFound && _dataSourceSet)
            {
                InitialiseServiceSettings();
            }

            timer1.Enabled = true;
        }

        private void InitialiseServiceSettings()
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                ServiceSettings serviceSettings = (from s in context.ServiceSettings select s).FirstOrDefault() ?? new ServiceSettings();

                serviceSettings.ServiceMessage = string.Empty;
                serviceSettings.LastUpdate = DateTime.Now;
                serviceSettings.Progress = 0;
                serviceSettings.Running = false;
                serviceSettings.Transferring = false;
                serviceSettings.Uploading = false;
                serviceSettings.ProgressMaximum = 100;

                context.SaveChanges();
            }
        }


        private void CheckAppConfig()
        {
            string connectionString = SqlHelper.GetConnectionString("UploaderDbContext");
            if (connectionString.Contains("*error*"))
            {
                Debug.WriteEventLogError("ConnectionString missing from app.config. Closing application. " + connectionString);
                MessageBox.Show("ConnectionString missing from app.config. Closing application\n\n" + connectionString, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FileHelper.DeleteProgramDataFolder();
                Environment.Exit(0);
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            var dataSource = builder["Data Source"].ToString();
            if (string.IsNullOrEmpty(dataSource) || dataSource == "NotSet")
            {
                Debug.Log("FrmUploader","CheckAppConfig","ConnectionString = " + connectionString);

                Debug.Log("FrmUploader","CheckAppConfig","Datasource not found in ConnectionString. Calling CheckSqlInstance()");
                uxLabelStatus.Visibility = ElementVisibility.Visible;
                _bwCheckSql.RunWorkerAsync();
            }
            else
            {
                uxLabelStatus.Text = "Ready";
                uxButtonNew1.Enabled = true;

                if (!InitialiseFoldersGrid())
                {
                    Environment.Exit(0);
                }
            }

            string value = ConfigurationManager.AppSettings["TimerInterval"];
            if (!string.IsNullOrEmpty(value))
            {
                int timerInterval = 0;
                if (int.TryParse(value, out timerInterval))
                {
                    if (timerInterval > 500 && timerInterval < 60000)
                    {
                        timer1.Interval = timerInterval;
                    }
                }
            }
            else
            {
                timer1.Interval = 2000;
            }
        }

        private void CheckServiceStatus()
        {

            uxLabelStatus.Visibility = ElementVisibility.Visible;
            uxProgressBar.Visibility = ElementVisibility.Collapsed;
            UxWaitingBar.Visibility = ElementVisibility.Collapsed;
            UxWaitingBar.StopWaiting();


            var pcName = Environment.MachineName;

            if (!DoesServiceExist("SkylineUploaderService", pcName))
            {
                uxLabelStatus.Image = Properties.Resources.error_warning;
                uxLabelStatus.Text = "Skyline Uploader Service not found";
                return;
            }

            string serviceStatus = GetServiceStatus("SkylineUploaderService", pcName);
            switch (serviceStatus)
            {
                case "Running":
                    using (UploaderDbContext context = new UploaderDbContext())
                    {
                        ServiceSettings serviceSettings = (from s in context.ServiceSettings select s).FirstOrDefault();
                        if (serviceSettings == null)
                        {
                            uxLabelStatus.Text = DateTime.Now.ToString("T") + " The Skyline Uploader Service status is not set";
                            uxLabelStatus.Image = Properties.Resources.error_warning;
                        }
                        else
                        {
                            if (serviceSettings.Running && !string.IsNullOrEmpty(serviceSettings.ServiceMessage))
                            {
                                uxLabelStatus.Text = DateTime.Now.ToString("T") + " " + serviceSettings.ServiceMessage;
                                uxLabelStatus.Image = Properties.Resources.clock_16;
                            }

                            if (serviceSettings.Running && string.IsNullOrEmpty(serviceSettings.ServiceMessage))
                            {
                                uxLabelStatus.Text = DateTime.Now.ToString("T") + " Skyline Uploader Service Running";
                                uxLabelStatus.Image = Properties.Resources.clock_16;
                            }

                            if (!serviceSettings.Running)
                            {
                                uxLabelStatus.Text = DateTime.Now.ToString("T") + " The Skyline Uploader Service is starting up";
                                uxLabelStatus.Image = Properties.Resources.clock_16;
                            }

                            if (serviceSettings.Uploading)
                            {
                                uxProgressBar.Visibility = ElementVisibility.Visible;
                                uxProgressBar.Maximum = serviceSettings.ProgressMaximum;
                                uxProgressBar.Value1 = serviceSettings.Progress;
                            }

                            if (serviceSettings.Transferring)
                            {
                                UxWaitingBar.Visibility = ElementVisibility.Visible;
                                UxWaitingBar.StartWaiting();
                            }
                        }


                    }
                    break;
                case "Stopped":
                    uxLabelStatus.Text = "Skyline Uploader service is Stopped";
                    uxLabelStatus.Image = Properties.Resources.error_warning;
                    return;
                case "Paused":
                    uxLabelStatus.Text = "Skyline Uploader service is Paused";
                    uxLabelStatus.Image = Properties.Resources.error_warning;
                    return;
                case "Stopping":
                    uxLabelStatus.Text = "Skyline Uploader service is Stopping";
                    uxLabelStatus.Image = Properties.Resources.error_warning;
                    return;
                case "Starting":
                    uxLabelStatus.Text = "Skyline Uploader service is Starting";
                    uxLabelStatus.Image = Properties.Resources.error_warning;
                    return;
            }


        }

        private bool DoesServiceExist(string serviceName, string machineName)
        {
            ServiceController[] services = ServiceController.GetServices(machineName);
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
            return service != null;
        }

        private string GetServiceStatus(string serviceName, string machineName)
        {
            ServiceController sc = new ServiceController(serviceName);

            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    return "Running";
                case ServiceControllerStatus.Stopped:
                    return "Stopped";
                case ServiceControllerStatus.Paused:
                    return "Paused";
                case ServiceControllerStatus.StopPending:
                    return "Stopping";
                case ServiceControllerStatus.StartPending:
                    return "Starting";
                default:
                    return "Status Changing";
            }
        }

        private void InitializeBackgroundWorker()
        {
            _bwCheckSql = new BackgroundWorker();
            _bwCheckSql.WorkerReportsProgress = true;
            _bwCheckSql.WorkerSupportsCancellation = true;

            _bwCheckSql.DoWork += new DoWorkEventHandler(bwCheckSQL_DoWork);
            _bwCheckSql.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCheckSQL_RunWorkerCompleted);
            _bwCheckSql.ProgressChanged += new ProgressChangedEventHandler(bwCheckSQL_ProgressChanged);
        }



        private void bwCheckSQL_DoWork(object sender, DoWorkEventArgs e)
        {
            _bwCheckSql.ReportProgress(-1, "Looking for SQL instances. Please wait");

            _sqlInstances = new List<SqlHelper>();

            string ServerName = Environment.MachineName;
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        string serverInstance = ServerName + "\\" + instanceName;
                        _bwCheckSql.ReportProgress(-1, "Found SQL Instance: " + serverInstance);
                        _sqlInstances.Add(new SqlHelper()
                        {
                            ServerInstance = serverInstance
                        });
                    }
                }
            }

            if (_sqlInstances.Count > 0)
            {
                _sqlFound = true;
            }
        }

        private void bwCheckSQL_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progress = e.ProgressPercentage;
            if (progress == -1)
            {
                uxLabelStatus.Text = e.UserState.ToString();
                Debug.Log("FrmUploader","bwCheckSQL_ProgressChanged",e.UserState.ToString());
            }
        }

        private void bwCheckSQL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.Log("FrmUploader","bwCheckSQL_RunWorkerCompleted","sqlFound = " + _sqlFound);
            Debug.Log("FrmUploader","bwCheckSQL_RunWorkerCompleted","dataSourceSet = " + _dataSourceSet);
            if (_sqlFound && !_dataSourceSet)
            {
                Debug.Log("FrmUploader","bwCheckSQL_RunWorkerCompleted","Deleting ProgramData folder");
                FileHelper.DeleteProgramDataFolder();

                Debug.Log("FrmUploader","bwCheckSQL_RunWorkerCompleted","sqlInstances.Count = " + _sqlInstances.Count);
                if (_sqlInstances.Count > 0)
                {
                    using (var frmSelectSqlInstance = new FrmSelectSqlInstance())
                    {
                        frmSelectSqlInstance.SqlInstances = _sqlInstances;
                        frmSelectSqlInstance.StartPosition = FormStartPosition.CenterParent;
                        frmSelectSqlInstance.ShowDialog(this);

                        _dataSource = frmSelectSqlInstance.SelectedInstance;
                        _dataSourceSet = frmSelectSqlInstance.DataSourceSet;
                        Debug.Log("FrmUploader","bwCheckSQL_RunWorkerCompleted","dataSourceSet = " + _dataSourceSet);

                        if (string.IsNullOrEmpty(_dataSource))
                        {
                            MessageBox.Show("No SQL Server instance selected. Shutting down", "SQL Server not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Debug.Error("FrmUploader","bwCheckSQL_RunWorkerCompleted","No SQL Server instance selected. Shutting down");
                            Application.Exit();
                        }
                    }
                }
            }

            if (_sqlFound && _dataSourceSet)
            {
                uxLabelStatus.Text = "Configuration complete";
                uxButtonNew1.Enabled = false;

                //MessageBox.Show("Configuration complete. Skyline Uploader starting up", "Configuration complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Application.Restart();
                //Environment.Exit(0);

                MessageBox.Show("Configuration complete. Please restart the Skyline Uploader", "Configuration complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();

            }
            else
            {
                MessageBox.Show("Unable to find any instance of SQL Server. Shutting down", "SQL Server not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.Error("FrmUploader","bwCheckSQL_RunWorkerCompleted","Unable to find any instance of SQL Server. Shutting down");
                Application.Exit();
            }

        }


        private bool InitialiseFoldersGrid()
        {
            string connectionString = string.Empty;
            try
            {
                Debug.Log("FrmUploader","InitialiseFoldersGrid","Creating UploaderDbContext");
                using (UploaderDbContext context = new UploaderDbContext())
                {
                    Debug.Log("FrmUploader","InitialiseFoldersGrid","This should create the database if it does not exist");

                    connectionString = SqlHelper.GetConnectionString("UploaderDbContext");
                    //context.Database.Connection.ConnectionString = connectionString;
                    //Debug.Log("ConnectionString = "+ connectionString);

                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                    var dataSource = builder["Data Source"].ToString();
                    Debug.Log("FrmUploader","InitialiseFoldersGrid","DataSource = " + dataSource);

                    //bool dbCreated = context.Database.CreateIfNotExists();
                    //if (dbCreated) Debug.Log("FrmUploader","InitialiseFoldersGrid","Database created");
                }

            }
            catch (Exception e)
            {
                Debug.Error("FrmUploader","InitialiseFoldersGrid","Unexpected Error trying to create the Database: " + e.Message);
                Debug.Error("FrmUploader","InitialiseFoldersGrid","ConnectionString = " + connectionString);
                MessageBox.Show("Unexpected Error trying to create the Database\n\n" + e.Message, "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            if (!GetGridData())
            {
                return false;
            }

            uxGridViewFolders.DataSource = _folderData;
            uxGridViewFolders.Columns["FolderId"].IsVisible = false;
            uxGridViewFolders.Columns["PortalId"].IsVisible = false;
            if (uxGridViewFolders.Columns["AdminUsername"] != null) uxGridViewFolders.Columns["AdminUsername"].IsVisible = false;
            if (uxGridViewFolders.Columns["AdminPassword"] != null) uxGridViewFolders.Columns["AdminPassword"].IsVisible = false;
            if (uxGridViewFolders.Columns["LibraryId"] != null) uxGridViewFolders.Columns["LibraryId"].IsVisible = false;
            if (uxGridViewFolders.Columns["InEditMode"] != null) uxGridViewFolders.Columns["InEditMode"].IsVisible = false;
            if (uxGridViewFolders.Columns["DeleteAfterUpload"] != null) uxGridViewFolders.Columns["DeleteAfterUpload"].IsVisible = false;
            if (uxGridViewFolders.Columns["FileTypes"] != null) uxGridViewFolders.Columns["FileTypes"].IsVisible = false;
            if (uxGridViewFolders.Columns["UserId"] != null) uxGridViewFolders.Columns["UserId"].IsVisible = false;
            if (uxGridViewFolders.Columns["LibraryUserId"] != null) uxGridViewFolders.Columns["LibraryUserId"].IsVisible = false;
            if (uxGridViewFolders.Columns["WaitForXml"] != null) uxGridViewFolders.Columns["WaitForXml"].IsVisible = false;
            if (uxGridViewFolders.Columns["EmailUser"] != null) uxGridViewFolders.Columns["EmailUser"].IsVisible = false;
            uxGridViewFolders.Columns["PortalUrl"].BestFit();
            uxGridViewFolders.Columns["PortalUrl"].BestFit();
            uxGridViewFolders.Columns["Files"].Width = 30;
            uxGridViewFolders.Columns["Files"].TextAlignment = ContentAlignment.MiddleCenter;

            uxGridViewFolders.Columns["PortalUrl"].HeaderText = "Portal URL";
            uxGridViewFolders.Columns["FolderName"].HeaderText = "Folder Name";
            uxGridViewFolders.Columns["LibraryUsername"].HeaderText = "Library Username";
            uxGridViewFolders.Columns["LibraryName"].HeaderText = "Library Name";
            uxGridViewFolders.Columns["SourceFolder"].HeaderText = "Source Folder";
            uxGridViewFolders.Columns["InEditMode"].HeaderText = "In Edit Mode";
            uxGridViewFolders.Columns["DeleteAfterUpload"].HeaderText = "Delete After Upload";
            uxGridViewFolders.Columns["Status"].HeaderText = "Status";
            uxGridViewFolders.Columns["Status"].TextAlignment = ContentAlignment.MiddleCenter;

            GridViewCommandColumn editColumn = new GridViewCommandColumn();
            editColumn.Name = "Edit";
            editColumn.UseDefaultText = true;
            editColumn.DefaultText = string.Empty;
            editColumn.FieldName = "FolderId";
            editColumn.HeaderText = string.Empty;
            editColumn.TextAlignment = ContentAlignment.MiddleCenter;
            editColumn.Width = 18;
            editColumn.BestFit();
            editColumn.TextImageRelation = TextImageRelation.ImageBeforeText;
            editColumn.ImageAlignment = ContentAlignment.MiddleCenter;
            editColumn.Image = Properties.Resources.Edit;
            uxGridViewFolders.MasterTemplate.Columns.Add(editColumn);

            GridViewCommandColumn deleteColumn = new GridViewCommandColumn();
            deleteColumn.Name = "Delete";
            deleteColumn.UseDefaultText = true;
            deleteColumn.DefaultText = string.Empty;
            deleteColumn.FieldName = "FolderId";
            deleteColumn.HeaderText = string.Empty;
            deleteColumn.TextAlignment = ContentAlignment.MiddleCenter;
            deleteColumn.Width = 18;
            deleteColumn.BestFit();
            deleteColumn.TextImageRelation = TextImageRelation.ImageBeforeText;
            deleteColumn.ImageAlignment = ContentAlignment.MiddleCenter;
            deleteColumn.Image = Properties.Resources.Delete;
            uxGridViewFolders.MasterTemplate.Columns.Add(deleteColumn);


            uxGridViewFolders.CommandCellClick += new CommandCellClickEventHandler(uxGridViewFolders_CommandCellClick);

            return true;

        }

        private bool GetGridData()
        {
            //Debug.Log("Getting Grid data");
            string connectionString = string.Empty;
            try
            {
                uxGridViewFolders.BeginUpdate();

                using (UploaderDbContext context = new UploaderDbContext())
                {
                    connectionString = context.Database.Connection.ConnectionString;
                    //connectionString = SqlHelper.GetConnectionString("UploaderDbContext");
                    //context.Database.Connection.ConnectionString = connectionString;

                    _folderData = (from f in context.Folders
                                   join l in context.Login on f.FolderId equals l.FolderId
                                   join ul in context.UserLibraries on f.FolderId equals ul.FolderId
                                   join sf in context.SourceFolders on f.FolderId equals sf.FolderId
                                   select new GridData
                                   {
                                       FolderId = f.FolderId,
                                       PortalId = f.PortalId,
                                       PortalUrl = l.PortalUrl,
                                       FolderName = f.FolderName,
                                       LibraryUsername = ul.Username,
                                       LibraryName = ul.LibraryName,
                                       Status = f.Status,
                                       Files = f.Files,
                                       Enabled = f.Enabled,
                                       SourceFolder = sf.FolderPath

                                   }).ToList();
                }
                uxGridViewFolders.DataSource = _folderData;
                uxGridViewFolders.EndUpdate();
            }


            catch (SqlException sqlExc)
            {
                var error = sqlExc.Message;
                var inner = sqlExc.InnerException;
                var errorNumber = sqlExc.Number;

                MessageBox.Show("Error connecting to the database. Closing application\n\n" + sqlExc.Message, "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                MessageBox.Show("ConnectionString: " + connectionString, "Diagnostic Message", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Debug.Error("FrmUploader","GetGridData","Error connecting to the database.", sqlExc);
                Debug.Error("FrmUploader","GetGridData","SQL Error number: " + errorNumber);

                if (errorNumber == 262)
                {
                    //reset the app.config ConnectionString back to blank Data Source
                    SqlConnectionStringBuilder sqlConBuilder = new SqlConnectionStringBuilder();
                    sqlConBuilder.ConnectionString = "Data Source=NotSet;Initial Catalog=SkylineUploader;Integrated Security=SSPI;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                    string errorMessage = SqlHelper.ModifyConnectionString("UploaderDbContext", sqlConBuilder.ConnectionString);
                    Debug.Error("FrmUploader","GetGridData",errorMessage);
                    MessageBox.Show(errorMessage, "Run Once As Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }

            catch (Exception e)
            {
                MessageBox.Show("Error connecting to the database. Closing application\n\n" + e.Message, "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                MessageBox.Show("ConnectionString = " + connectionString, "Diagnostic Message", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Debug.Error("FrmUploader","GetGridData","ConnectionString = " + connectionString);

                Debug.Error("FrmUploader","GetGridData","Error connecting to the database. Closing application", e);
                Environment.Exit(0);
            }

            return true;

        }

        private void uxGridViewFolders_CommandCellClick(object sender, GridViewCellEventArgs e)
        {
            string folderValue = ((sender as GridCommandCellElement)).Value.ToString();
            if (string.IsNullOrEmpty(folderValue))
            {
                return;
            }

            Guid folderId = Guid.Empty;
            if (!Guid.TryParse(folderValue, out folderId))
            {
                return;
            }

            if (folderId == Guid.Empty)
            {
                return;
            }

            timer1.Enabled = false;

            //int selectedIndex = -1;
            var commandName = e.Column.Name;
            if (commandName == "Edit")
            {

                //selectedIndex = uxGridViewFolders.Rows.IndexOf(this.uxGridViewFolders.CurrentRow);
                using (var frmFolderDetails = new FrmFolderDetails())
                {
                    frmFolderDetails.FolderId = folderId;
                    frmFolderDetails.StartPosition = FormStartPosition.CenterParent;
                    frmFolderDetails.ShowDialog(this);
                }

                timer1.Enabled = true;
                return;
            }

            if (commandName == "Delete")
            {
                DialogResult res = MessageBox.Show("Are you sure that you want to delete this folder?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != DialogResult.Yes)
                {
                    timer1.Enabled = true;
                    return;

                }

                using (var context = new UploaderDbContext())
                {
                    Folder folder = (from f in context.Folders where f.FolderId == folderId select f).FirstOrDefault();
                    Login login = (from l in context.Login where l.FolderId == folderId select l).FirstOrDefault();
                    UserLibrary userLibrary = (from ul in context.UserLibraries where ul.FolderId == folderId select ul).FirstOrDefault();
                    SourceFolder sourceFolder = (from sf in context.SourceFolders where sf.FolderId == folderId select sf).FirstOrDefault();
                    if (folder != null) context.Folders.Remove(folder);
                    if (login != null) context.Login.Remove(login);
                    if (userLibrary != null) context.UserLibraries.Remove(userLibrary);
                    if (sourceFolder != null) context.SourceFolders.Remove(sourceFolder);

                    context.SaveChanges();
                }
                timer1.Enabled = true;
                return;
            }
        }


        //https://stackoverflow.com/questions/34748589/how-to-create-a-database-user-in-entity-framework
        private void AddDbUser(UploaderDbContext myDB)
        {
            string accountDomainName = "AccountDomainName";  // replace with user's login domain
            string accountLoginID = "AccountLoginID";  // replace with user's login ID

            string sql =
                "USE [MyDB]" +
                "CREATE USER [MyNewUser] FOR LOGIN [" + accountDomainName + "\\" + accountLoginID + "]" +
                "ALTER AUTHORIZATION ON SCHEMA::[db_datareader] TO [" + accountLoginID + "]" +
                "ALTER AUTHORIZATION ON SCHEMA::[db_datawriter] TO [" + accountLoginID + "]" +
                "EXEC sp_addrolemember N'db_datawriter', N'" + accountLoginID + "'" +
                "EXEC sp_addrolemember N'db_datareader', N'" + accountLoginID + "'";

            myDB.Database.ExecuteSqlCommand(sql);
        }

        private void uxMenuItemDebug_Click(object sender, EventArgs e)
        {
            string logDir = Settings.GetLogDir();
            string debugFile = Path.Combine(logDir, "Debug.txt");
            if (File.Exists(debugFile))
            {
                Process.Start("notepad.exe", debugFile);
            }
        }

        private void uxMenuItemError_Click(object sender, EventArgs e)
        {
            string logDir = Settings.GetLogDir();
            string debugFile = Path.Combine(logDir, "Error.txt");
            if (File.Exists(debugFile))
            {
                Process.Start("notepad.exe", debugFile);
            }
        }

        private void uxMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void uxGridViewFolders_CellFormatting(object sender, CellFormattingEventArgs e)
        {


            if (e.Column.Name != "Files" && e.Column.Name != "Status" && e.Column.Name != "Enabled" && e.Column.Name != "Edit" && e.Column.Name != "Delete" && e.CellElement.Value != null)
            {
                e.CellElement.ToolTipText = e.CellElement.Value.ToString();
            }

            if (e.Column.Name == "Edit")
            {
                e.CellElement.ToolTipText = "Edit profile";
            }

            if (e.Column.Name == "Delete")
            {
                e.CellElement.ToolTipText = "Delete profile";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;


            if (uxGridViewFolders.CurrentRow != null)
            {
                int currentRowIndex = uxGridViewFolders.CurrentRow.Index;
                CheckServiceStatus();
                GetGridData();
                try
                {
                    uxGridViewFolders.CurrentRow = uxGridViewFolders.Rows[currentRowIndex];
                }
                catch (Exception )
                {

                }

            }

            timer1.Enabled = true;
        }

        private void uxButtonNew1_Click(object sender, EventArgs e)
        {
            using (var frmFolderDetails = new FrmFolderDetails())
            {
                frmFolderDetails.FolderId = Guid.NewGuid();
                frmFolderDetails.StartPosition = FormStartPosition.CenterParent;
                frmFolderDetails.ShowDialog(this);
            }

            if (!GetGridData())
            {
                MessageBox.Show("There was a problem getting the data for the grid. closing application", "Error getting data",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.Error("FrmUploader","uxButtonNew1_Click","Error connecting to the database. Closing application");
                Environment.Exit(0);
            }
        }

        private void uxButtonClose1_Click(object sender, EventArgs e)
        {
            if (_bwCheckSql.IsBusy)
            {
                _bwCheckSql.CancelAsync();
            }
            Application.Exit();
        }

        private void uxMenuItemSQL_Click(object sender, EventArgs e)
        {
            //var res = MessageBox.Show("Do you want reset the connection to the database?", "Reset Database Connection", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            //if (res == DialogResult.OK)
            //{
            //    SqlConnectionStringBuilder sqlConBuilder = new SqlConnectionStringBuilder();
            //    sqlConBuilder.ConnectionString = "Data Source=NotSet;Initial Catalog=SkylineUploader;Integrated Security=SSPI;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            //    string errorMessage = SqlHelper.ModifyConnectionString("UploaderDbContext", sqlConBuilder.ConnectionString);
            //    if (!string.IsNullOrEmpty(errorMessage))
            //    {
            //        Debug.Error("Error resetting the ConnectionString. Error: " + errorMessage);
            //        MessageBox.Show("Error resetting the ConnectionString. Error: \n\n" + errorMessage, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }


            //    var deletedOk = SettingsHelper.DeleteSettingsFile();
            //    if (deletedOk)
            //    {
            //        MessageBox.Show("Database connection reset. Please restart the Skyline Uploader App",
            //            "Database Connection Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        Application.Exit();
            //    }
            //    else
            //    {
            //        MessageBox.Show(
            //            "Unable to delete the configuration file " + Global.SettingsPath +
            //            "\n\nTry to delete it manually", "Error deleting the configuration file", MessageBoxButtons.OK,
            //            MessageBoxIcon.Error);
            //    }
            //}
        }

        private void uxMenuDebugOn_Click(object sender, EventArgs e)
        {
            if (SettingsHelper.UpdateDebugMode("true"))
            { 
                var debugMode = SettingsHelper.GetDebugMode();
                if (debugMode)
                {
                    MessageBox.Show("Debug mode turned ON. Please restart the Skyline Uploader Service", "Restart Service", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem turning Debug mode ON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                
            }
            else
            {
                MessageBox.Show("There was a problem turning on debug mode", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Text = GetTitleText();

        }

        private void uxMenuDebugOff_Click(object sender, EventArgs e)
        {
            if (SettingsHelper.UpdateDebugMode("false"))
            {
                var debugMode = SettingsHelper.GetDebugMode();
                if (!debugMode)
                {
                    MessageBox.Show("Debug mode turned OFF. Please restart the Skyline Uploader Service", "Restart Service", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem turning Debug mode OFF.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            else
            {
                MessageBox.Show("There was a problem turning off debug mode", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Text = GetTitleText();
        }

        private void uxMenuItemLogging_Click(object sender, EventArgs e)
        {

        }

        private void uxMenuSetServiceLogin_Click(object sender, EventArgs e)
        {
            using ( var frmServiceLogin = new FrmServiceLogin())
            {
                frmServiceLogin.ShowDialog();
            }
        }

        private void FrmUploader_Load(object sender, EventArgs e)
        {
            this.Text =  GetTitleText();
            
        }

        private string GetTitleText()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string titleText = "Skyline Uploader version "+ version.Major +"." + version.Minor +"." + version.Build;

            string settingsPath = Global.SettingsPath;
            bool serviceDebugMode = false;

            if (File.Exists(settingsPath))
            {
                XDocument doc = XDocument.Load(Global.SettingsPath);
                if (doc.Root != null)
                {
                    var xElement = doc.Root.Element("DebugMode");
                    if (xElement != null)
                    {
                        var debugMode = xElement.Value;
                        if (!string.IsNullOrEmpty(debugMode))
                        {
                            serviceDebugMode = debugMode.ToLower() == "true";
                        }
                    }
                }
            }

            if (serviceDebugMode)
            {
                titleText += " :: Service Debug ON";
            }

            return titleText;
        }
    }
}
