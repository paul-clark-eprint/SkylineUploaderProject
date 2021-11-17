using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using HelperClasses;
using SkylineUploader.Classes;
using SkylineUploaderDomain.DataModel;
using SkylineUploaderDomain.DataModel.Classes;
using ServiceSettings = SkylineUploaderDomain.DataModel.Classes.ServiceSettings;

namespace SkylineUploaderService
{
    //https://docs.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer
    
    public partial class SkylineUploaderService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        public static string _connectionString;
        private static Timer _timer;
        private static List<GridData> _folderData;
        private static Guid _loginUserId = Guid.Empty;
        private static Guid _portalId;
        private static Guid _docId = Guid.Empty;
        private static int _totalFiles = 0;
        private static string _portalVersion;
        private static string _serviceVersion;
        private static string _errorMessage = string.Empty;
        private static bool _uploadOK = false;
        private static bool _globalUser;
        private static bool _portalFound;
        private static bool _validUser;

        public SkylineUploaderService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists("Skyline Uploader Service"))
            {
                EventLog.CreateEventSource("Skyline Uploader Service", "Skyline Uploader Service Log");
            }
            eventLog.Source = "Skyline Uploader Service";
            eventLog.Log = "Skyline Uploader Service Log";
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Skyline Uploader Service Starting");

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            //serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            //serviceStatus.dwWaitHint = 100000;
            //SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            _connectionString =  GetConnectionString();

            if (string.IsNullOrEmpty(_connectionString))
            {
                StopService();
            }

            eventLog.WriteEntry("Starting up");

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);
                var dataSource = builder["Data Source"].ToString();
                if (string.IsNullOrEmpty(dataSource))
                {
                    eventLog.WriteEntry("The ConnectionString dataSource is empty",EventLogEntryType.Error);
                    StopService();
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Unexpected error looking for the ConnectionString dataSource: "+ ex.Message,EventLogEntryType.Error);
                StopService();
            }

            InitialiseServiceSettings();

            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(TimerEvent);
            _timer.Interval = 1000;
            _timer.Enabled = true;


        }

        private static void InitialiseServiceSettings()
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                var serviceSettings = (from ss in context.ServiceSettings select ss).FirstOrDefault();
                if (serviceSettings == null)
                {
                    serviceSettings = new ServiceSettings();
                    serviceSettings.ServiceMessage = "Skyline Uploader service starting";
                    serviceSettings.LastUpdate = DateTime.Now;
                    serviceSettings.Running = true;
                    context.ServiceSettings.Add(serviceSettings);
                    context.SaveChanges();
                }
                else
                {
                    serviceSettings.ServiceMessage = "Skyline Uploader service starting";
                    serviceSettings.LastUpdate = DateTime.Now;
                    serviceSettings.Running = true;
                    context.SaveChanges();
                }
            }
        }

        private string GetConnectionString([CallerLineNumber] int lineNumber = 0)
        {
            string connectionString= string.Empty;
            try
            {
                //eventLog.WriteEntry("Looking for ConnectionString");
                //connectionString = SettingsHelper.GetConnectionString();
                //if (string.IsNullOrEmpty(connectionString))
                //{
                //    eventLog.WriteEntry("ConnectionString empty. Stopping service",EventLogEntryType.Error,lineNumber);
                //    StopService();
                //    return null;
                //}

                //if (connectionString.Contains("*error*"))
                //{
                //    eventLog.WriteEntry("Error getting the ConnectionString: "+ connectionString +". Stopping service",EventLogEntryType.Error,lineNumber);
                //    StopService();
                //    return null;
                //}

                string settingsPath = Global.SettingsPath;
                if (settingsPath.StartsWith("*error*"))
                {
                    eventLog.WriteEntry("Error reading the settings path: " + settingsPath, EventLogEntryType.Error);
                    StopService();
                    return string.Empty;
                }

                if (File.Exists(settingsPath))
                {
                    XDocument doc = XDocument.Load(Global.SettingsPath);
                    if (doc.Root != null)
                    {
                        var xElement = doc.Root.Element("ConnectionString");
                        if (xElement != null)
                        {
                            connectionString = SettingsHelper.Decrypt(xElement.Value);
                        }
                    }
                }

                eventLog.WriteEntry("ConnectionString = "+ connectionString);
                

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                var dataSource = builder["Data Source"].ToString();
                if (string.IsNullOrEmpty(dataSource))
                {
                    eventLog.WriteEntry("Data Source not defined in ConnectionString. Shutting down",EventLogEntryType.Error,lineNumber);
                    StopService();
                }
                eventLog.WriteEntry("Data Source: "+ dataSource);
                return connectionString;
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Unexpected error in GetConnectionStringFromRegistry(). Error = "+ex.Message,EventLogEntryType.Error,lineNumber);
                eventLog.WriteEntry("ConnectionString = "+ connectionString,EventLogEntryType.Error,lineNumber);
                StopService();
            }

            return null;
        }

        private void StopService()
        {
            eventLog.WriteEntry("Stopping service",EventLogEntryType.Error);
            Environment.Exit(1);
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("Skyline Uploader Service stopping");

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            //serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            //serviceStatus.dwWaitHint = 100000;
            //SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private static void TimerEvent(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            _folderData = GetFolderData();

            if (!_folderData.Any())
            {
                _timer.Start();
                return;
            }

            var count = _folderData.Count();
            //Console.WriteLine("Number of profiles: " + count);

            int totalFiles = 0;

            foreach (var profile in _folderData)
            {
                if (profile.InEditMode)
                {
                    Console.WriteLine("Profile " + profile.FolderName + " is in edit mode. Skipping it");
                    SetFolderStatus("Edit Mode", profile.FolderId);
                    continue;
                }

                var portalUrl = profile.PortalUrl;
                var username = profile.AdminUsername;
                var password = SettingsHelper.Decrypt(profile.AdminPassword);
                var sourceFolder = profile.SourceFolder;
                var fileTypes = profile.FileTypes;

                if (!Directory.Exists(sourceFolder))
                {
                    Console.WriteLine("profile Source Folder does not exist: '" + sourceFolder + "'. Skipping profile");
                    continue;
                }

                int profileFiles = 0;
                if (fileTypes != null)
                {
                    var supplortedfileTypes = fileTypes.Split(',');
                    foreach (var supplortedfileType in supplortedfileTypes)
                    {
                        switch (supplortedfileType)
                        {
                            case "PDF":
                                profileFiles += GetFileTypes(sourceFolder, "*.pdf");
                                break;
                            case "Word":
                                profileFiles += GetFileTypes(sourceFolder, "*.doc");
                                profileFiles += GetFileTypes(sourceFolder, "*.docx");
                                break;

                        }
                    }
                }

                if (profileFiles == 0)
                {
                    SetFolderStatus("Idle", profile.FolderId);
                    SetFileCount(profile.FolderId, 0);
                    continue;
                }

                SetFileCount(profile.FolderId, profileFiles);
                SetServiceMessage("Checking folder " + profile.FolderName + ". " + profileFiles + " files found");
                totalFiles += profileFiles;



                if (!CheckUrl(portalUrl))
                {
                    SetServiceMessage("Profile URL is not valid: '" + portalUrl + "'. Skipping profile");
                    continue;
                }



                if (DoLogin(portalUrl, username, password))
                {
                    if (_loginUserId == Guid.Empty)
                    {
                        continue;
                    }

                    FileInfo[] files = GetFilesToUpload(profile.SourceFolder);

                    if (files != null && files.Length > 0)
                    {
                        var oldestDate = DateTime.MaxValue;
                        int fileIndex = 0;
                        int oldestIndex = 0;
                        foreach (FileInfo file in files)
                        {
                            if (FileLocked(file))
                            {
                                Console.WriteLine("File " + file.FullName + " is locked. Skipping this file");
                                fileIndex++;
                                continue;
                            }

                            var fileDate = file.LastWriteTime;
                            if (fileDate < oldestDate)
                            {
                                oldestDate = fileDate;
                                oldestIndex = fileIndex;
                            }

                            fileIndex++;
                        }

                        var fileName = files[oldestIndex].Name;
                        Console.WriteLine("Oldest modified file is " + fileName);

                        Webcalls.UploadParams uploadParams = new Webcalls.UploadParams();
                        uploadParams.UploadUrl = portalUrl;
                        uploadParams.username = username;
                        uploadParams.Password = password;
                        uploadParams.DocumentName = fileName;
                        uploadParams.PdfPath = profile.SourceFolder;
                        uploadParams.UserId = _loginUserId;
                        uploadParams.LibraryId = profile.LibraryId;

                        SetServiceMessage("Uploading " + fileName + " in folder " + profile.FolderName);
                        SetFolderStatus("Uploading", profile.FolderId);

                        bool uploadedOk = UploadDocument(uploadParams);
                        if (uploadedOk)
                        {
                            SetServiceMessage(fileName + " uploaded OK");
                        }
                        else
                        {
                            SetServiceMessage("There was a problem uploading " + fileName);
                        }

                        if (uploadedOk && profile.DeleteAfterUpload)
                        {
                            var filePath = Path.Combine(profile.SourceFolder, fileName);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }

                    }
                }
                else
                {
                    Console.WriteLine("Unable to log in to the portal: '" + portalUrl + "'. Skipping profile");
                    continue;
                }
            }


            if (totalFiles == 0)
            {
                SetServiceMessage("No files to upload");
            }
            else
            {
                SetServiceMessage(totalFiles + " files to upload");
            }
            _timer.Start();
        }

        private static List<GridData> GetFolderData()
        {
            List<GridData> folderData;

            try
            {
                using (UploaderDbContext context = new UploaderDbContext())
                {
                    context.Database.Connection.ConnectionString = _connectionString;

                    folderData = (from f in context.Folders
                                  join l in context.Login on f.FolderId equals l.FolderId
                                  join ul in context.UserLibraries on f.FolderId equals ul.FolderId
                                  join sf in context.SourceFolders on f.FolderId equals sf.FolderId

                                  select new GridData
                                  {
                                      FolderId = f.FolderId,
                                      PortalId = f.PortalId,
                                      PortalUrl = l.PortalUrl,
                                      AdminUsername = l.Username,
                                      AdminPassword = l.Password,
                                      FolderName = f.FolderName,
                                      LibraryUsername = ul.Username,
                                      LibraryName = ul.LibraryName,
                                      LibraryId = ul.LibraryId,
                                      Enabled = f.Enabled,
                                      SourceFolder = sf.FolderPath,
                                      InEditMode = f.InEditMode,
                                      DeleteAfterUpload = f.DeleteAfterUpload,
                                      FileTypes = f.FileType
                                  }).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return folderData;
        }

        private static int GetFileTypes(string sourceFolder, string extension)
        {
            var files = Directory.GetFiles(sourceFolder, extension, SearchOption.TopDirectoryOnly);
            return files.Length;
        }

        private static bool UploadDocument(Webcalls.UploadParams uploadParams)
        {
            string filename = uploadParams.DocumentName;
            string uploadDir = uploadParams.UserId.ToString();
            //Upload the document          
            int Offset = 0; // starting offset.

            //define the chunk size
            int ChunkSize = 1048576; // 64 * 1024 kb
            //define the buffer array according to the chunksize.
            byte[] Buffer = new byte[ChunkSize];

            string pdfPath = Path.Combine(uploadParams.PdfPath, filename);
            string url = uploadParams.UploadUrl;
            Guid userId = uploadParams.UserId;
            Guid libraryId = uploadParams.LibraryId;


            //_bwUpload.ReportProgress(0); //Set ProgressBar to 0


            //opening the file for read.
            FileStream fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

            //creating the ServiceSoapClient which will allow to connect to the service.
            var webSvc = new SkylineWebService.SkylineWebService();
            webSvc.Url = url + "/WebServices/SkylineWebService.asmx";

            IWebProxy proxy = WebRequest.DefaultWebProxy;
            if (proxy != null)
            {
                webSvc.Proxy = proxy;
            }

            try
            {
                long fileSize = new FileInfo(pdfPath).Length; // File size of file being uploaded.
                                                              // reading the file.

                SetProgressBarMaximum(Convert.ToInt32(fileSize));
                fs.Position = Offset;
                int bytesRead = 0;
                while (Offset != fileSize) // continue uploading the file chunks until offset = file size.
                {

                    bytesRead = fs.Read(Buffer, 0, ChunkSize); // read the next chunk 
                                                               // (if it exists) into the buffer. 
                                                               // the while loop will terminate if there is nothing left to read
                                                               // check if this is the last chunk and resize the buffer as needed 
                                                               // to avoid sending a mostly empty buffer 
                                                               // (could be 10Mb of 000000000000s in a large chunk)
                    if (bytesRead != Buffer.Length)
                    {
                        ChunkSize = bytesRead;
                        byte[] TrimmedBuffer = new byte[bytesRead];
                        Array.Copy(Buffer, TrimmedBuffer, bytesRead);
                        Buffer = TrimmedBuffer; // the trimmed buffer should become the new 'buffer'
                    }
                    // send this chunk to the server. it is sent as a byte[] parameter, 
                    // but the client and server have been configured to encode byte[] using MTOM. 
                    bool ChunkAppened = webSvc.UploadFile(Path.GetFileName(pdfPath), Buffer, Offset, uploadDir);

                    if (!ChunkAppened)
                    {
                        break;
                    }

                    // Offset is only updated AFTER a successful send of the bytes. 
                    Offset += bytesRead; // save the offset position for resume

                    //if (_bwUpload.CancellationPending)
                    //{
                    //    _uploadCancelled = true;
                    //    break;
                    //}

                    ReportProgress(Offset, true);
                }
            }
            catch (Exception ex)
            {
                //Debug.Error("Error uploading file " + pdfPath + " to " + url, ex);
                fs.Close();
                _errorMessage = "Error uploading file " + pdfPath + " to " + url + "\n\n" + ex.Message;
                _uploadOK = false;
                ReportProgress(0, false);
                //timer1.Enabled = true;
                return false;
            }
            finally
            {
                fs.Close();
            }
            ReportProgress(0, false);
            ReportTransferring(true);

            try
            {
                string docIdOrError = webSvc.MoveTempDocumentsToSpecificLibrary(userId, libraryId, false);
                try
                {
                    _docId = new Guid(docIdOrError);
                }
                catch (Exception)
                {
                    _docId = Guid.Empty;
                }

                if (_docId == Guid.Empty)
                {
                    _uploadOK = false;
                    _errorMessage = docIdOrError;
                    //Debug.Error(docIdOrError);
                    return false;
                }

                _uploadOK = true;

            }
            catch (Exception ex)
            {
                //possible timeout
                //Debug.Error("Error calling MoveTempDocumentsToUserLibrary", ex);
                _errorMessage = "Error copying your document to your online library:\n\n" + ex.Message;
                _uploadOK = false;
                ReportTransferring(false);
                return false;
            }
            ReportTransferring(false);
            return true;
        }

        private static void ReportTransferring(bool transferring)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                var serviceSettings = (from ss in context.ServiceSettings select ss).FirstOrDefault();
                serviceSettings.Transferring = transferring;
                serviceSettings.Uploading = false;
                context.SaveChanges();
            }
        }

        private static void ReportProgress(int offset, bool uploading)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                var serviceSettings = (from ss in context.ServiceSettings select ss).FirstOrDefault();
                serviceSettings.Uploading = uploading;
                if (uploading)
                {
                    serviceSettings.Progress = offset;
                }
                else
                {
                    serviceSettings.Progress = 0;
                    serviceSettings.ProgressMaximum = 100;
                }
                context.SaveChanges();
            }
        }

        private static void SetProgressBarMaximum(int maxValue)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                var serviceSettings = (from ss in context.ServiceSettings select ss).FirstOrDefault();
                serviceSettings.ProgressMaximum = maxValue;
                context.SaveChanges();
            }
        }

        private static bool FileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        private static FileInfo[] GetFilesToUpload(string sourceFolder)
        {
            if (!Directory.Exists(sourceFolder)) return null;

            //int totalFiles = 0;
            //var _extensions = new List<string>();
            //DropDownCheckedItemsCollection documentTypes = uxCheckedDropDownListFileTypes.CheckedItems;
            //foreach (var documentType in documentTypes)
            //{
            //    var type = documentType.Text;
            //    switch (type)
            //    {
            //        case "PDF":
            //            _extensions.Add("*.pdf");
            //            break;
            //        case "Word":
            //            _extensions.Add("*.docx");
            //            _extensions.Add("*.doc");
            //            break;
            //        case "Excel":
            //            _extensions.Add("*.xlsx");
            //            _extensions.Add("*.xls");
            //            break;
            //        case "PowerPoint":
            //            _extensions.Add("*.pptx");
            //            _extensions.Add("*.ppt");
            //            break;
            //        case "Image":
            //            _extensions.Add("*.jpg");
            //            _extensions.Add("*.jpeg");
            //            _extensions.Add("*.png");
            //            _extensions.Add("*.bmp");
            //            _extensions.Add("*.gif");
            //            //extensions.Add("*.tiff");
            //            break;
            //    }
            //}


            DirectoryInfo dinfo = new DirectoryInfo(sourceFolder);
            FileInfo[] files = dinfo.GetFiles("*.pdf");
            //totalFiles += files.Length;
            //foreach (FileInfo file in Files)
            //{
            //    //var name = file.Name;
            //}
            //}


            return files;
        }

        private static bool CheckUrl(string portalUrl)
        {
            var urlValid = Webcalls.CheckUrl(portalUrl);
            return urlValid;
        }

        private static bool DoLogin(string portalUrl, string username, string password)
        {
            if (string.IsNullOrEmpty(portalUrl) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            

            if (Program.PricingService == null)
            {
                Program.PricingService = new SkylineUploader.PricingService.PricingService();
            }

            if (Program.SkylineService == null)
            {
                Program.SkylineService = new SkylineUploader.SkylineWebService.SkylineWebService();
            }

            try
            {
                Guid portalId = Guid.Empty;

                string rawUrl = portalUrl.Replace("http://", "");
                rawUrl = rawUrl.Replace("https://", "");

                try
                {
                    Program.PricingService.Url = "https://" + rawUrl + "/webservices/PricingService.asmx";
                    //_bwLogin.ReportProgress(0, "Looking for https://" + rawUrl);
                    portalId = Program.PricingService.GetPortalGuidFromUrl(rawUrl);
                }
                catch (WebException)
                {
                    //if (worker.CancellationPending == true)
                    //{
                    //    e.Cancel = true;
                    //    return;
                    //}

                    //_bwLogin.ReportProgress(0, "Looking for http://" + rawUrl);
                    Program.PricingService.Url = "http://" + rawUrl + "/webservices/PricingService.asmx";
                    portalId = Program.PricingService.GetPortalGuidFromUrl(rawUrl);
                }

                //portalId = Program.PricingService.GetPortalGuidFromUrl(rawUrl);
                if (portalId == Guid.Empty)
                {
                    //_bwLogin.ReportProgress(0, "Error connecting to " + rawUrl);
                    _portalFound = false;
                    return false;
                }
                _portalId = portalId;

                bool useHttps = Program.PricingService.UseHttps(_portalId);

                string urlType = useHttps == true ? "https://" : "http://";

                //_bwLogin.ReportProgress(0, "Connected to " + _portalUrl);


                Program.PricingService.Url = urlType + rawUrl + "/webservices/PricingService.asmx";


                _loginUserId = Program.PricingService.ValidateUser(username, password, out _validUser, out _globalUser);

                //_portalId = Program.SkylineService.GetPortalIdForUrl(_portalUrl);

                _portalFound = true;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    _portalFound = false;
                    //_bwLogin.ReportProgress(0, "Error connecting to " + uxTextBoxPortalUrl.Text);
                    return false;
                }
            }
            catch (Exception)
            {
                _portalFound = false;
                //_bwLogin.ReportProgress(0, "Error connecting to " + uxTextBoxPortalUrl.Text);
                return false;
            }

            _portalVersion = Program.PricingService.GetAssemblyVersion();
            _serviceVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            try
            {
                Version ver = Version.Parse(_portalVersion);
                int major = ver.Major;
                int minor = ver.Minor;
                int revision = ver.Revision;
                if (major >= 7 && minor >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return false;
        }

        private static void SetServiceMessage(string message)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                var serviceSettings = (from ss in context.ServiceSettings select ss).FirstOrDefault();

                serviceSettings.ServiceMessage = message;
                serviceSettings.LastUpdate = DateTime.Now;
                serviceSettings.Running = true;
                context.SaveChanges();
            }
        }

        private static void SetFolderStatus(string status, Guid folderId)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                Folder folder = (from f in context.Folders where f.FolderId == folderId select f).FirstOrDefault();
                if (folder != null)
                {
                    folder.Status = status;
                    context.SaveChanges();
                }
            }
        }

        private static void SetFileCount(Guid folderId, int count)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                Folder folder = (from f in context.Folders where f.FolderId == folderId select f).FirstOrDefault();
                if (folder != null)
                {
                    folder.Files = count;
                    context.SaveChanges();
                }
            }
        }
    }
}
