using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using SkylineUploader.Classes;
using SkylineUploaderDomain.DataModel;
using SkylineUploaderDomain.DataModel.Classes;
using Telerik.WinControls.UI;

namespace SkylineUploader
{
    public partial class FrmUploader : RadForm
    {
        private List<GridData> _folderData;
        private BackgroundWorker bwCheckSQL;
        private List<SqlServerInstance> SqlInstances;
        private bool sqlFound = false;
        private bool dataSourceSet = false;

        public FrmUploader()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            Database.SetInitializer<UploaderDbContext>(null);
            CheckAppConfig();
        }

        private void CheckAppConfig()
        {
            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["UploaderDbContext"].ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectString);
            var dataSource = builder["Data Source"].ToString();
            if (string.IsNullOrEmpty(dataSource))
            {
                Debug.Log("ConnectionString = " + connectString);
                Debug.Log("Datasource not found in ConnectionString. Calling CheckSqlInstance()");
                CheckSqlInstance();
            }
            else
            {
                uxLabelStatus.Text = "Ready";
                uxButtonNew.Enabled = true;
                if (!InitialiseFoldersGrid())
                {
                    Application.Exit();
                }
            }

        }

        private void InitializeBackgroundWorker()
        {
            bwCheckSQL = new BackgroundWorker();
            bwCheckSQL.WorkerReportsProgress = true;
            bwCheckSQL.WorkerSupportsCancellation = true;

            bwCheckSQL.DoWork += new DoWorkEventHandler(bwCheckSQL_DoWork);
            bwCheckSQL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCheckSQL_RunWorkerCompleted);
            bwCheckSQL.ProgressChanged += new ProgressChangedEventHandler(bwCheckSQL_ProgressChanged);
        }



        private void bwCheckSQL_DoWork(object sender, DoWorkEventArgs e)
        {
            bwCheckSQL.ReportProgress(-1, "Looking for SQL instances. Please wait");

            SqlInstances = new List<SqlServerInstance>();

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
                        Console.WriteLine(serverInstance);
                        SqlInstances.Add(new SqlServerInstance()
                        {
                            ServerInstance = serverInstance
                        });
                    }
                }
            }


            if (SqlInstances.Count > 0)
            {
                sqlFound = true;
            }
        }

        private void bwCheckSQL_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progress = e.ProgressPercentage;
            if (progress == -1)
            {
                uxLabelStatus.Text = e.UserState.ToString();
                Debug.Log(e.UserState.ToString());
            }
        }

        private void bwCheckSQL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            uxWaitingBar.StopWaiting();
            uxWaitingBar.Visible = false;

            Debug.Log("sqlFound = " + sqlFound);
            Debug.Log("dataSourceSet = " + dataSourceSet);
            if (sqlFound && !dataSourceSet)
            {
                Debug.Log("sqlInstances.Count = " + SqlInstances.Count);
                if (SqlInstances.Count > 0)
                {
                    using (var frmSelectSqlInstance = new FrmSelectSqlInstance())
                    {
                        frmSelectSqlInstance.sqlInstances = SqlInstances;
                        frmSelectSqlInstance.StartPosition = FormStartPosition.CenterParent;
                        frmSelectSqlInstance.ShowDialog(this);

                        string dataSource = frmSelectSqlInstance.SelectedInstance;
                        dataSourceSet = frmSelectSqlInstance.DataSourceSet;
                        Debug.Log("dataSourceSet = " + dataSourceSet);

                        if (string.IsNullOrEmpty(dataSource))
                        {
                            MessageBox.Show("No SQL Server instance selected. Shutting down", "SQL Server not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Debug.Error("No SQL Server instance selected. Shutting down");
                            Application.Exit();
                        }
                    }
                }
            }

            if (sqlFound && dataSourceSet)
            {
                uxLabelStatus.Text = "Ready";
                uxButtonNew.Enabled = true;
                if(!InitialiseFoldersGrid())
                {
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("Unable to find any instance of SQL Server. Shutting down", "SQL Server not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.Error("Unable to find any instance of SQL Server. Shutting down");
                Application.Exit();
            }

        }

        private void CheckSqlInstance()
        {
            uxLabelStatus.Visible = true;
            uxWaitingBar.Visible = true;
            uxWaitingBar.StartWaiting();
            bwCheckSQL.RunWorkerAsync();
        }

        private bool InitialiseFoldersGrid()
        {
            var ok = GetGridData();
            if (ok == false)
            {
                return false;
            }

            uxGridViewFolders.DataSource = _folderData;
            uxGridViewFolders.Columns["FolderId"].IsVisible = false;
            uxGridViewFolders.Columns["PortalId"].IsVisible = false;
            uxGridViewFolders.Columns["PortalUrl"].BestFit();
            uxGridViewFolders.Columns["PortalUrl"].BestFit();
            uxGridViewFolders.Columns["Files"].Width = 30;
            uxGridViewFolders.Columns["Files"].TextAlignment = ContentAlignment.MiddleCenter;

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
            Debug.Log("Getting Grid data");
            try
            {
                uxGridViewFolders.BeginUpdate();
                using (UploaderDbContext context = new UploaderDbContext())
                {
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
                                       Files = 0,
                                       Enabled = f.Enabled
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

                MessageBox.Show("Error connectiing to the database. Closing application\n\n" + sqlExc.Message, "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Debug.Error("Error connectiing to the database.", sqlExc);
                Debug.Error("SQL Error number: " + errorNumber);

                if (errorNumber == 262)
                {
                    SqlConnectionStringBuilder sqlConBuilder = new SqlConnectionStringBuilder();
                    sqlConBuilder.ConnectionString = "Data Source=;Initial Catalog=SkylineUploader;Integrated Security=SSPI;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                    SqlServerInstance.ModifyConnectionString("UploaderDbContext", sqlConBuilder.ConnectionString);
                }

                return false;
            }

            catch (Exception e)
            {
                MessageBox.Show("Error connectiing to the database. Closing application\n\n" + e.Message, "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Debug.Error("Error connectiing to the database. Closing application", e);
                return false;
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

            int selectedIndex = -1;
            var commandName = e.Column.Name;
            if (commandName == "Edit")
            {
                selectedIndex = uxGridViewFolders.Rows.IndexOf(this.uxGridViewFolders.CurrentRow);
                using (var frmFolderDetails = new FrmFolderDetails())
                {
                    frmFolderDetails.FolderId = folderId;
                    frmFolderDetails.StartPosition = FormStartPosition.CenterParent;
                    frmFolderDetails.ShowDialog(this);
                }
            }

            if (commandName == "Delete")
            {
                DialogResult res = MessageBox.Show("Are you sure that you want to delete this folder?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != DialogResult.Yes)
                {
                    return;
                }

                using (var context = new UploaderDbContext())
                {
                    Folder folder = (from f in context.Folders where f.FolderId == folderId select f).FirstOrDefault();
                    Login login = (from l in context.Login where l.FolderId == folderId select l).FirstOrDefault();
                    UserLibrary userLibrary = (from ul in context.UserLibraries where ul.FolderId == folderId select ul).FirstOrDefault();
                    SourceFolder sourceFolder = (from sf in context.SourceFolders where sf.FolderId == folderId select sf).FirstOrDefault();
                    var documentType = (from dt in context.DocumentTypes where dt.FolderId == folderId select dt).FirstOrDefault();
                    if (folder != null) context.Folders.Remove(folder);
                    if (login != null) context.Login.Remove(login);
                    if (userLibrary != null) context.UserLibraries.Remove(userLibrary);
                    if (sourceFolder != null) context.SourceFolders.Remove(sourceFolder);
                    if (documentType != null) context.DocumentTypes.Remove(documentType);
                    context.SaveChanges();
                }
            }

            var ok = GetGridData();
            if (ok == false)
            {

            }

            if (selectedIndex > -1)
            {
                uxGridViewFolders.CurrentRow = null;
                uxGridViewFolders.Rows[selectedIndex].IsSelected = true;
            }
        }

        private void uxButtonClose_Click(object sender, EventArgs e)
        {
            if (bwCheckSQL.IsBusy)
            {
                bwCheckSQL.CancelAsync();
            }
            Application.Exit();
        }

        private void uxButtonNew_Click(object sender, EventArgs e)
        {
            using (var frmFolderDetails = new FrmFolderDetails())
            {
                frmFolderDetails.FolderId = Guid.NewGuid();
                frmFolderDetails.StartPosition = FormStartPosition.CenterParent;
                frmFolderDetails.ShowDialog(this);
            }

            var ok = GetGridData();
            if (ok == false)
            {

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


    }
}
