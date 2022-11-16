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
using System.Xml;
using System.Xml.Linq;
using HelperClasses;
using SkylineUploader.Classes;
using SkylineUploader.SkylineWebService;
using SkylineUploaderDomain.DataModel;
using SkylineUploaderDomain.DataModel.Classes;
using Debug = SkylineUploader.Debug;
using ServiceSettings = SkylineUploaderDomain.DataModel.Classes.ServiceSettings;
using UserLibraryIds = SkylineUploaderService.SkylineWebService.UserLibraryIds;

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
        //private static string _errorMessage = string.Empty;
        private static bool _uploadOK = false;
        private static bool _globalUser;
        private static bool _portalFound;
        private static bool _validUser;
        private static bool _lastFileUploaded;
        private static bool _debugMode = false;
        
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

        public enum MessageLevel
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Information = 4
        }

        protected override void OnStart(string[] args)
        {
            WriteEventLogAlways("Starting Service " + ServiceName, EventLogEntryType.Information);

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            //serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            //serviceStatus.dwWaitHint = 100000;
            //SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            _connectionString = GetConnectionString();
            //eventLog.WriteEntry("_connectionString = "+ _connectionString);

            if (string.IsNullOrEmpty(_connectionString))
            {
                WriteEventLog("The ConnectionString is empty", EventLogEntryType.Error);
                CallStopService();
            }

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);
                var dataSource = builder["Data Source"].ToString();
                if (string.IsNullOrEmpty(dataSource))
                {
                    WriteEventLog("The ConnectionString dataSource is empty", EventLogEntryType.Error);
                    CallStopService();
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("Unexpected error looking for the ConnectionString dataSource: " + ex.Message, EventLogEntryType.Error);
                CallStopService();
            }

            WriteEventLog("Initializing Service Settings", EventLogEntryType.Information);
            InitialiseServiceSettings();

            WriteEventLog("Starting timer", EventLogEntryType.Information);

            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(TimerEvent);
            _timer.Interval = 1000;
            _timer.Enabled = true;


        }

        private static void InitialiseServiceSettings()
        {
            try
            {
                using (UploaderDbContext context = new UploaderDbContext())
                {
                    context.Database.Connection.ConnectionString = _connectionString;

                    var serviceSettings = (from ss in context.ServiceSettings select ss).FirstOrDefault();
                    if (serviceSettings == null)
                    {
                        serviceSettings = new ServiceSettings();
                        serviceSettings.ServiceMessage = string.Empty;
                        serviceSettings.LastUpdate = DateTime.Now;
                        serviceSettings.Running = true;
                        context.ServiceSettings.Add(serviceSettings);
                        context.SaveChanges();
                    }
                    else
                    {
                        serviceSettings.ServiceMessage = string.Empty;
                        serviceSettings.LastUpdate = DateTime.Now;
                        serviceSettings.Running = true;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("Unexpected error in InitialiseServiceSettings:" + ex, EventLogEntryType.Error);
                CallStopService();
            }
        }

        private string GetConnectionString()
        {
            string connectionString = string.Empty;
            try
            {
                string settingsPath = Global.SettingsPath;
                WriteEventLog("settingsPath = " + settingsPath,EventLogEntryType.Information);

                if (settingsPath.StartsWith("*error*"))
                {
                    WriteEventLog("Error reading the settings path: " + settingsPath, EventLogEntryType.Error);
                    CallStopService();
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

                        xElement = doc.Root.Element("DebugMode");
                        if (xElement != null)
                        {
                            var debugMode = xElement.Value;
                            if (!string.IsNullOrEmpty(debugMode))
                            {
                                _debugMode = debugMode.ToLower() == "true";
                                WriteEventLogAlways("Debug mode " + _debugMode, EventLogEntryType.Information);
                            }
                        }

                    }
                }
                else
                {
                    WriteEventLog("settingsPath file not found. Closing", EventLogEntryType.Error);
                }

                WriteEventLog("ConnectionString  '" + connectionString + "'",EventLogEntryType.Information);


                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                var dataSource = builder["Data Source"].ToString();
                if (string.IsNullOrEmpty(dataSource))
                {
                    WriteEventLog("Data Source not defined in ConnectionString. Shutting down", EventLogEntryType.Error);
                    CallStopService();
                }
                WriteEventLog("Data Source: " + dataSource, EventLogEntryType.Information);
                return connectionString;
            }
            catch (Exception ex)
            {
                WriteEventLog("Unexpected error in GetConnectionString. Error = " + ex.Message, EventLogEntryType.Error);
                WriteEventLog("ConnectionString = " + connectionString, EventLogEntryType.Error);
                CallStopService();
            }

            return null;
        }

        //private void StopService()
        //{
        //    eventLog.WriteEntry("Stopping service " + ServiceName, EventLogEntryType.Error);

        //    ServiceController sc = new ServiceController(ServiceName);
        //    sc.Stop();
        //}

        private static void CallStopService()
        {
            var serviceName = new ServiceBase().ServiceName;
            WriteEventLog("Stopping service " + serviceName, EventLogEntryType.Error);

            ServiceController sc = new ServiceController(serviceName);
            sc.Stop();
        }

        protected override void OnStop()
        {
            WriteEventLogAlways("Skyline Uploader Service stopping",EventLogEntryType.Information);

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

            //var count = _folderData.Count();
            //Console.WriteLine("Number of profiles: " + count);

            int totalFiles = 0;

            foreach (var profile in _folderData)
            {
                if (profile.InEditMode)
                {
                    WriteEventLog("Profile " + profile.FolderName + " is in edit mode. Skipping it", EventLogEntryType.Warning);
                    SetFolderStatus("Edit Mode", profile.FolderId);
                    continue;
                }

                if (!profile.Enabled)
                {
                    WriteEventLog("Profile " + profile.FolderName + " is disabled. Skipping it", EventLogEntryType.Warning);
                    SetFolderStatus("Disabled", profile.FolderId);
                    continue;
                }

                var portalUrl = profile.PortalUrl;
                var username = profile.AdminUsername;
                var password = SettingsHelper.Decrypt(profile.AdminPassword);
                var sourceFolder = profile.SourceFolder;
                var fileTypes = profile.FileTypes;
                var LibraryUserId = profile.LibraryUserId;

                if (!Directory.Exists(sourceFolder))
                {
                    WriteEventLog("profile Source Folder does not exist: '" + sourceFolder + "'. Skipping profile", EventLogEntryType.Warning);
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


                if (!CheckFolderPermissiongs(sourceFolder))
                {
                    WriteEventLog("Read, Write or Delete permission problem on Source Folder: '" + sourceFolder + "'. Disabling profile "+ profile.FolderName, EventLogEntryType.Error);
                    DisableProfile(profile.FolderId);
                    continue;
                }

                SetFileCount(profile.FolderId, profileFiles);

                if (profileFiles == 1)
                {
                    SetServiceMessage("Checking folder " + profile.FolderName + ". " + profileFiles + " file found");
                    WriteEventLog("Checking folder " + profile.FolderName + ". " + profileFiles + " file found",EventLogEntryType.Information);
                }
                else
                {
                    SetServiceMessage("Checking folder " + profile.FolderName + ". " + profileFiles + " files found");
                    WriteEventLog("Checking folder " + profile.FolderName + ". " + profileFiles + " files found",EventLogEntryType.Information);
                }
                
                totalFiles += profileFiles;



                if (!CheckUrl(portalUrl))
                {
                    WriteEventLog("Profile URL is not valid: '" + portalUrl + "'. Skipping profile", EventLogEntryType.Error);
                    SetFolderStatus("URL not valid", profile.FolderId);
                    SetServiceMessage("Profile URL is not valid: '" + portalUrl + "'. Skipping profile");
                    continue;
                }

                WriteEventLog("Profile URL is valid. Logging in to " + portalUrl,EventLogEntryType.Information);

                if (DoLogin(portalUrl, username, password))
                {
                    if (_loginUserId == Guid.Empty)
                    {
                        WriteEventLog("Unable to log in to portal "+ portalUrl +". Disabling the profile "+ profile.FolderName, EventLogEntryType.Error);
                        SetFolderStatus("Login error", profile.FolderId);
                        DisableProfile(profile.FolderId);
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
                                WriteEventLog("File " + file.FullName + " is locked. Skipping this file", EventLogEntryType.Warning);
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
                        WriteEventLog("Uploading " + fileName, EventLogEntryType.Information);

                        Webcalls.UploadParams uploadParams = new Webcalls.UploadParams();
                        uploadParams.UploadUrl = portalUrl;
                        uploadParams.username = username;
                        uploadParams.Password = password;
                        uploadParams.DocumentName = fileName;
                        uploadParams.PdfPath = profile.SourceFolder;
                        uploadParams.LibraryUserId = profile.LibraryUserId;
                        uploadParams.LibraryId = profile.LibraryId;
                        uploadParams.PortalId = _portalId;
                        uploadParams.LibraryName = profile.LibraryName;
                        uploadParams.FolderName = profile.FolderName;

                        SetServiceMessage("Uploading " + fileName + " in folder " + profile.FolderName);
                        SetFolderStatus("Uploading", profile.FolderId);

                        bool uploadedOk = UploadDocument(uploadParams);
                        if (uploadedOk)
                        {
                            WriteEventLog(fileName + " uploaded OK", EventLogEntryType.Information);
                            SetServiceMessage(fileName + " uploaded OK");
                        }
                        else
                        {
                            WriteEventLog("There was a problem uploading " + fileName, EventLogEntryType.Error);
                            SetServiceMessage("There was a problem uploading " + fileName);
                            LogMessage(portalUrl, _portalId, MessageLevel.Error, 0, "There was a problem uploading " + fileName);
                        }

                        if (profile.DeleteAfterUpload)
                        {
                            var filePath = Path.Combine(profile.SourceFolder, fileName);
                            try
                            {
                                if (File.Exists(filePath))
                                {
                                    WriteEventLog("DeleteAfterUpload: Deleting file " + filePath, EventLogEntryType.Information);
                                    File.Delete(filePath);
                                }
                                else
                                {
                                    WriteEventLog("DeleteAfterUpload: File not found " + filePath, EventLogEntryType.Error);
                                    LogMessage(portalUrl, _portalId, MessageLevel.Error, 103, "DeleteAfterUpload: File not found " + filePath);
                                }
                                

                            }
                            catch (Exception exception)
                            {
                                WriteEventLog("DeleteAfterUpload: Error deleting file " + filePath, EventLogEntryType.Error);
                                LogMessage(portalUrl, _portalId, MessageLevel.Error, 103, "DeleteAfterUpload: Error deleting file " + filePath);

                                WriteEventLog(exception.ToString(), EventLogEntryType.Error);
                                LogMessage(portalUrl, _portalId, MessageLevel.Error, 103, exception.ToString());

                                DisableProfile(profile.FolderId);
                                LogMessage(portalUrl, _portalId, MessageLevel.Critical, 103, "Profile has been disabled");
                            }

                            var xmlName = Path.GetFileNameWithoutExtension(fileName) + ".xml";
                            var xmlPath = Path.Combine(profile.SourceFolder, xmlName);
                            try
                            {
                                if (File.Exists(xmlPath))
                                {
                                    WriteEventLog("DeleteAfterUpload: Deleting file " + xmlPath, EventLogEntryType.Information);
                                    File.Delete(xmlPath);
                                }
                            }
                            catch (Exception exception)
                            {
                                WriteEventLog("DeleteAfterUpload: Error deleting file " + xmlPath, EventLogEntryType.Error);

                                WriteEventLog(exception.ToString(), EventLogEntryType.Error);

                                DisableProfile(profile.FolderId);
                            }
                        }
                    }
                }
                else
                {
                    WriteEventLog("Unable to log in to the portal: '" + portalUrl + "'. Skipping profile", EventLogEntryType.Error);
                    continue;
                }
            }

            if (totalFiles == 0)
            {
                SetServiceMessage("No files to upload");
                if (_lastFileUploaded)
                {
                    _lastFileUploaded = false;
                    //WriteEventLog("No files to upload");
                }
            }
            else
            {
                if (totalFiles == 1)
                {
                    SetServiceMessage(totalFiles + " file to upload");
                    WriteEventLog(totalFiles + " file to upload",EventLogEntryType.Information);
                    _lastFileUploaded = true;
                }
                else
                {
                    SetServiceMessage(totalFiles + " files to upload");
                    WriteEventLog(totalFiles + " files to upload",EventLogEntryType.Information);
                    _lastFileUploaded = false;
                }

            }
            _timer.Start();
        }

        private static bool CheckFolderPermissiongs(string sourceFolder)
        {
            var testFile = Path.Combine(sourceFolder, "__test__.txt");
            try
            {
                if (File.Exists(testFile))
                {
                    File.Delete(testFile);
                }
            }
            catch (Exception e)
            {
                WriteEventLog("CheckFolderPermissiongs: Error deleting file " + testFile, EventLogEntryType.Error);
                WriteEventLog(e.ToString(), EventLogEntryType.Error);
                return false;
            }

            try
            {
                using (FileStream fs = new FileStream(testFile, FileMode.Create))
                {    
                    byte[] messageByte = Encoding.ASCII.GetBytes("testing write access.");
                    // Write the number of bytes to the file.
                    fs.WriteByte((byte)messageByte.Length);

                    // Write the bytes to the file.
                    fs.Write(messageByte, 0, messageByte.Length);

                    // Close the stream.
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                WriteEventLog("CheckFolderPermissiongs: Unable to create files in the folder  " + sourceFolder, EventLogEntryType.Error);
                WriteEventLog(e.ToString(), EventLogEntryType.Error);
                return false;
            }

            try
            {
                if (File.Exists(testFile))
                {
                    File.Delete(testFile);
                }
                else
                {
                    WriteEventLog("CheckFolderPermissiongs: Unable to read files in the folder  " + sourceFolder, EventLogEntryType.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                WriteEventLog("CheckFolderPermissiongs: Unable to delete files in the folder  " + sourceFolder, EventLogEntryType.Error);
                WriteEventLog(e.ToString(), EventLogEntryType.Error);

                return false;
            }

            WriteEventLog("Permissions check OK on folder " + sourceFolder, EventLogEntryType.Information);
            return true;
        }

        private static void WriteEventLog(string message, EventLogEntryType logType)
        {
            
            
            if (logType == EventLogEntryType.Information && _debugMode == false)
            {
                return;
            }

            using (EventLog eventLog = new EventLog())
            {
                eventLog.Source = "Skyline Uploader Service";
                eventLog.Log = "Skyline Uploader Service Log";
                eventLog.WriteEntry(message, logType);
            }
        }

        private static void WriteEventLogAlways(string message, EventLogEntryType logType)
        {
            using (EventLog eventLog = new EventLog())
            {
                eventLog.Source = "Skyline Uploader Service";
                eventLog.Log = "Skyline Uploader Service Log";
                eventLog.WriteEntry(message, logType);
            }
        }

        private static List<GridData> GetFolderData()
        {
            List<GridData> folderData = new List<GridData>();

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
                                      LibraryUserId = ul.UserId,
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
                WriteEventLog("Unexpected error in GetFolderData " + e, EventLogEntryType.Error);
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
            string uploadDir = uploadParams.LibraryUserId.ToString();
            string url = uploadParams.UploadUrl;
            Guid libraryUserId = uploadParams.LibraryUserId;
            Guid libraryId = uploadParams.LibraryId;
            var portalId = uploadParams.PortalId;

            //creating the ServiceSoapClient which will allow to connect to the service.
            var webSvc = new SkylineWebService.SkylineWebService();
            webSvc.Url = url + "/WebServices/SkylineWebService.asmx";

            IWebProxy proxy = WebRequest.DefaultWebProxy;
            if (proxy != null)
            {
                webSvc.Proxy = proxy;
            }


            LogMessage(webSvc, portalId, MessageLevel.Information, 0, "Uploading file " + filename);
            var xmlName = Path.GetFileNameWithoutExtension(filename) + ".xml";
            var xmlPath = Path.Combine(uploadParams.PdfPath, xmlName);
            if (File.Exists(xmlPath))
            {
                LogMessage(webSvc, portalId, MessageLevel.Information, 0, "Found XML file " + xmlName);
                XmlDocument doc = new XmlDocument();
                        doc.Load(xmlPath);
                        var node = doc.SelectSingleNode("/Skyline/Email");
                        if (node != null)
                        {
                            var email = node.InnerText;
                            if (!string.IsNullOrEmpty(email))
                            {
                                LogMessage(webSvc, portalId, MessageLevel.Information, 0, "Found email address " + email);
                                
                                UserLibraryIds userLibraryIds = webSvc.GetUserDefaultLibraryIds(portalId, email);
                                if (userLibraryIds != null)
                                {
                                    LogMessage(webSvc, portalId, MessageLevel.Information, 0, "Found user and user's default library");

                                    Guid userId = userLibraryIds.UserId;
                                    bool userActive = webSvc.IsUserActive(userId);
                                    if (userActive)
                                    {
                                        uploadDir =userLibraryIds.UserId.ToString();
                                        libraryUserId = userLibraryIds.UserId;
                                        libraryId = userLibraryIds.UserDefaultLibraryId;
                                    }
                                    else
                                    {
                                        WriteEventLog("The user with the email address "+ email + " is not activated. The document will be uploaded to the default library "+ uploadParams.LibraryName, EventLogEntryType.Warning);
                                        LogMessage(webSvc, portalId, MessageLevel.Warning, 0, "The user with the email address "+ email + " is not activated. The document will be uploaded to the default library " + uploadParams.LibraryName);
                                    }
                                    
                                }
                                else
                                {
                                    WriteEventLog("Unable to get the default library for the email address "+ email + ". The document will be uploaded to the default library "+ uploadParams.LibraryName,EventLogEntryType.Warning);
                                    LogMessage(webSvc, portalId, MessageLevel.Warning, 0, "Unable to get the default library for the email address "+ email + ". The document will be uploaded to the default library " + uploadParams.LibraryName);
                                }
                            }
                        }
            }

            //Upload the document          
            int Offset = 0; // starting offset.

            //define the chunk size
            int ChunkSize = 1048576; // 64 * 1024 kb
            //define the buffer array according to the chunksize.
            byte[] Buffer = new byte[ChunkSize];

            string pdfPath = Path.Combine(uploadParams.PdfPath, filename);

            //opening the file for read.
            FileStream fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

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

                    ReportProgress(Offset, true);
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("Error uploading file " + pdfPath + " to " + url + " Error message: " + ex.Message,EventLogEntryType.Error);
                fs.Close();
                _uploadOK = false;
                ReportProgress(0, false);

                LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Error uploading file " + pdfPath + " to " + url + " Error message: " + ex.Message);
                
                return false;
            }
            finally
            {
                fs.Close();
            }
            ReportProgress(0, false);
            ReportTransferring(true);

            LogMessage(webSvc, portalId, MessageLevel.Information, 0, "File " + filename + " uploaded. Moving it to the user library");

            try
            {
                string docIdOrError = webSvc.MoveTempDocumentsToSpecificLibrary(libraryUserId, libraryId, false);
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
                    WriteEventLog("Error in MoveTempDocumentsToSpecificLibrary: "+ docIdOrError,EventLogEntryType.Error);
                    LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Error in MoveTempDocumentsToSpecificLibrary: "+ docIdOrError);
                    return false;
                }

                _uploadOK = true;

            }
            catch (Exception ex)
            {
                //possible timeout
                WriteEventLog("Unexpected error calling MoveTempDocumentsToUserLibrary. Message = "+ ex.Message,EventLogEntryType.Error);
                LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Unexpected error calling MoveTempDocumentsToUserLibrary. Message = "+ ex.Message);
                _uploadOK = false;
                ReportTransferring(false);
                return false;
            }

            ReportTransferring(false);

            if (!webSvc.DeleteTempUploadedFile(uploadDir))
            {
                WriteEventLog("Error deleting the Temp upload directory "+ uploadDir,EventLogEntryType.Error);
                LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Error deleting the Temp upload directory "+ uploadDir);
            }

            return true;
        }

        private static void LogMessage(SkylineWebService.SkylineWebService webSvc, Guid portalId, MessageLevel messageLevel, int eventId, string message)
        {
            var computerName = Environment.MachineName;
            if (message.Length > 2000)
            {
                message = message.Substring(0, 1996) + "...";
            }
            webSvc.SaveLogMessage(portalId,"Skyline Uploader Service",computerName,(int)messageLevel,eventId,message);
        }

        private static void LogMessage(string portalUrl, Guid portalId, MessageLevel messageLevel, int eventId, string message)
        {
            using (var webSvc = new SkylineWebService.SkylineWebService())
            {
                webSvc.Url = portalUrl + "/WebServices/SkylineWebService.asmx";

                IWebProxy proxy = WebRequest.DefaultWebProxy;
                if (proxy != null)
                {
                    webSvc.Proxy = proxy;
                }

                var computerName = Environment.MachineName;
                if (message.Length > 2000)
                {
                    message = message.Substring(0, 1996) + "...";
                }
                webSvc.SaveLogMessage(portalId,"Skyline Uploader Service",computerName,(int)messageLevel,eventId,message);
            }
            
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
            
            DirectoryInfo dinfo = new DirectoryInfo(sourceFolder);
            FileInfo[] files = dinfo.GetFiles("*.pdf");
            
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
                WriteEventLog("DoLogin portalUrl, username or password empty", EventLogEntryType.Error);
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
                    //_bwLogin.ReportProgress(0, "Looking for http://" + rawUrl);
                    Program.PricingService.Url = "http://" + rawUrl + "/webservices/PricingService.asmx";
                    portalId = Program.PricingService.GetPortalGuidFromUrl(rawUrl);
                }

                //portalId = Program.PricingService.GetPortalGuidFromUrl(rawUrl);
                if (portalId == Guid.Empty)
                {
                    WriteEventLog("Error connecting to " + rawUrl, EventLogEntryType.Error);
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
                    WriteEventLog("NameResolutionFailure connecting to " + portalUrl, EventLogEntryType.Error);
                    return false;
                }

            }
            catch (Exception e)
            {
                _portalFound = false;
                WriteEventLog("Unexpected error connecting to " + portalUrl + " error = " + e, EventLogEntryType.Error);
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

        private static void DisableProfile(Guid folderId)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;

                Folder folder = (from f in context.Folders where f.FolderId == folderId select f).FirstOrDefault();
                if (folder != null)
                {
                    folder.Enabled = false;
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
