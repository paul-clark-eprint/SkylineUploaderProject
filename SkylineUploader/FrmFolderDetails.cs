using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using HelperClasses;
using SkylineUploader.Classes;
using Telerik.WinControls.UI;
using SkylineUploader.SkylineWebService;
using SkylineUploaderDomain.DataModel;
using SkylineUploaderDomain.DataModel.Classes;
using Telerik.WinControls;
using UserLibraryIds = SkylineUploader.SkylineWebService.UserLibraryIds;

namespace SkylineUploader
{
    public partial class FrmFolderDetails : RadForm
    {
        //Test of source control
        private static Guid _loginUserId = Guid.Empty;
        private static Guid _portalId;
        private static string _portalUrl;
        private static bool _validUser;
        private static bool _globalUser;
        private static bool _portalFound;
        private static string _username;
        private static string _password;
        private static bool _urlValid;
        private static string _loginError;
        private static bool _checkUrlCancelled = false;
        private static bool _uploadCancelled = false;
        private static bool _uploadOK = false;
        private static bool _waitingForXml = false;
        private static Guid _docId = Guid.Empty;
        private static string _errorMessage = string.Empty;
        private static int _totalFiles = 0;
        //private static bool _alertUser = false;

        public Guid FolderId { get; set; }

        private static List<string> _extensions;

        private static bool _useProxy = false;
        private static string _proxyDomain = string.Empty;
        private static string _proxyUserName = string.Empty;
        private static string _proxyPassword = string.Empty;
        private static string _proxyAddress = string.Empty;
        private static int _proxyPort = 0;

        private static string _userLibraryUserName;
        private static Guid _userLibraryUserId;
        private static Guid _userLibraryLibraryId;
        //private static string _userLibraryUserEmail;
        //private static string _userLibraryLibraryName;

        private static List<UserSettings> users;

        private static string _serverVersion;
        private static string _workStationVersion;
        private BackgroundWorker _bwLogin;
        BackgroundWorker _bwCheckUrl;
        BackgroundWorker _bwUpload;

        private static Webcalls.UploadParams uploadParams;

        public enum MessageLevel
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Information = 4
        }

        public FrmFolderDetails()
        {
            InitializeComponent();

        }
        private void uxButtonConnect_Click(object sender, EventArgs e)
        {
            uxListControlUsers.DataSource = null;
            uxListControlUsers.Rebind();
            uxListControlLibraries.DataSource = null;
            uxListControlLibraries.Rebind();
            uxTextBoxSelected.Text = string.Empty;
            uxTextBoxSelected.Enabled = false;
            uxTextBoxSourceFolder.Text = string.Empty;
            uxTextBoxSourceFolder.Enabled = false;
            uxTextBoxFolderName.Text = string.Empty;
            uxTextBoxFolderName.Enabled = false;
            uxTextBoxFileCount.Text = string.Empty;
            uxCheckedDropDownListFileTypes.Enabled = false;
            uxButtonSave.Enabled = false;
            uxButtonUpload.Enabled = false;
            uxLabelStatus.Visibility = ElementVisibility.Visible;
            uxLabelStatus.Text = "Connecting...";
            StopWaitingBar();
            StopProgressBar();

            DoLogin();
        }

        private void DoLogin()
        {
            if (string.IsNullOrEmpty(uxTextBoxPortalUrl.Text))
            {
                uxTextBoxPortalUrl.Focus();
                return;
            }

            if (string.IsNullOrEmpty(uxTextBoxUsername.Text))
            {
                uxTextBoxUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(uxTextBoxPassword.Text))
            {
                uxTextBoxPassword.Focus();
                return;
            }

            uxTextBoxPortalUrl.Text = uxTextBoxPortalUrl.Text.Trim();
            if (uxTextBoxPortalUrl.Text.EndsWith("/"))
                uxTextBoxPortalUrl.Text = uxTextBoxPortalUrl.Text.Substring(0, uxTextBoxPortalUrl.Text.Length - 1);

            if (!uxTextBoxPortalUrl.Text.StartsWith("https://") && !uxTextBoxPortalUrl.Text.StartsWith("http://"))
            {
                uxTextBoxPortalUrl.Text = "https://" + uxTextBoxPortalUrl.Text;
            }

            uxTextBoxPortalUrl.Enabled = false;
            uxTextBoxUsername.Enabled = false;
            uxTextBoxPassword.Enabled = false;
            uxLabelConnecting.Visible = true;
            this.uxButtonConnect.DisplayStyle = Telerik.WinControls.DisplayStyle.ImageAndText;
            uxButtonConnect.Text = "Connecting";

            _bwLogin = new BackgroundWorker();
            _bwLogin.DoWork += new DoWorkEventHandler(BwLoginDoWork);
            _bwLogin.WorkerReportsProgress = true;
            _bwLogin.WorkerSupportsCancellation = true;
            _bwLogin.ProgressChanged += BwLoginProgressChanged;
            _bwLogin.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwLoginWorkerCompleted);
            _bwLogin.RunWorkerAsync();

            _bwCheckUrl = new BackgroundWorker();
            _bwCheckUrl.WorkerSupportsCancellation = true;
            _bwCheckUrl.WorkerReportsProgress = true;
            _bwCheckUrl.ProgressChanged += BwCheckUrl_ProgressChanged;
            _bwCheckUrl.RunWorkerCompleted += BwCheckUrl_RunWorkerCompleted;
            _bwCheckUrl.DoWork += BwCheckUrl_DoWork;

            _bwUpload = new BackgroundWorker();
            _bwUpload.WorkerSupportsCancellation = true;
            _bwUpload.WorkerReportsProgress = true;
            _bwUpload.ProgressChanged += BwUpload_ProgressChanged;
            _bwUpload.DoWork += BwUpload_DoWork;
            _bwUpload.RunWorkerCompleted += BwUpload_RunWorkerCompleted;

        }

        private void BwCheckUrl_DoWork(object sender, DoWorkEventArgs e)
        {
            //_alertUser = false;
            string url = e.Argument.ToString();
            _bwCheckUrl.ReportProgress(1);
            _urlValid = Webcalls.CheckUrl(url);
            if (_urlValid)
            {
                _bwCheckUrl.ReportProgress(2);
            }
        }

        private void BwCheckUrl_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_bwCheckUrl.CancellationPending)
            {
                uxLabelStatus.Text = "Cancelling...";
                return;
            }

            int progressValue = e.ProgressPercentage;
            if (progressValue == 1)
            {
                Debug.Log("FrmFolderDetails","BwCheckUrl_ProgressChanged","Connecting to " + _portalUrl + "...");
                uxLabelStatus.Text = "Connecting to " + _portalUrl + "...";
            }

            if (progressValue == 2)
            {
                Debug.Log("FrmFolderDetails","BwCheckUrl_ProgressChanged","Logging in...");
                uxLabelStatus.Text = "Logging in...";
            }
        }

        private void BwCheckUrl_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            if (e.Cancelled || _checkUrlCancelled)
            {
                Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","Cancelled");
                StopWaitingBar();
                StopProgressBar();
                uxLabelStatus.Text = "Cancelled";
                timer1.Enabled = true;
                return;
            }

            if (e.Error != null || !_urlValid)
            {
                Debug.Error("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","_urlValid = "+ _urlValid);
                Debug.Error("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","e.Error = "+ e.Error);
                StopWaitingBar();
                StopProgressBar();
                Debug.Error("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","The upload page '" + _portalUrl + "' is not available");
                Debug.Error("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","The upload page '" + _portalUrl + "' is not available", e.Error);
                MessageBox.Show(this, "The upload website '" + _portalUrl + "' is not available.", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                uxLabelStatus.Text = "The upload website '" + _portalUrl + "' is not available.";
                timer1.Enabled = true;
                return;
            }

            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","No error and the URL has been found");
            StopWaitingBar();
            StopProgressBar();

            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","username = "+_username);
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","Password found");
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","LibraryUserId = "+_userLibraryUserId);
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","LibraryId = "+_userLibraryLibraryId);
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","UploadUrl = "+_portalUrl);
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","PdfPath = "+uxTextBoxSourceFolder.Text);
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","PortalId = "+_portalId);
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","WaitForXml = "+ uxCheckBoxWaitForXml.Checked);
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","Enabled = "+ uxCheckBoxEnabled.Checked);

            uploadParams = new Webcalls.UploadParams();
            uploadParams.username = _username;
            uploadParams.Password = _password;
            uploadParams.LibraryUserId = _userLibraryUserId;
            uploadParams.LibraryId = _userLibraryLibraryId;
            uploadParams.UploadUrl = _portalUrl;
            uploadParams.PdfPath = uxTextBoxSourceFolder.Text;
            uploadParams.PortalId = _portalId;
            uploadParams.WaitForXml = uxCheckBoxWaitForXml.Checked;

            uxLabelStatus.Text = "Connected";
            Debug.Log("FrmFolderDetails","BwCheckUrl_RunWorkerCompleted","Connected.");

            _bwUpload.RunWorkerAsync(uploadParams);

        }

        protected virtual bool FileLocked(FileInfo file)
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

        private void BwUpload_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_extensions.Count == 0)
            {
                MessageBox.Show("No File Types have been selected", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                uxLabelStatus.Text = "No File Types have been selected";
                timer1.Enabled = true;
                return;
            }

            Webcalls.UploadParams uploadParams = (Webcalls.UploadParams)e.Argument;

            foreach (var extension in _extensions)
            {
                DirectoryInfo dinfo = new DirectoryInfo(uxTextBoxSourceFolder.Text);
                FileInfo[] Files = dinfo.GetFiles(extension);

                int fileNumber = 0;
                if (Files == null || Files.Length == 0)
                {
                    continue;
                }
                //var file = Files[0];
                foreach (FileInfo file in Files)
                {
                    if (FileLocked(file))
                    {
                        Debug.Log("FrmFolderDetails","BwUpload_DoWork","File " + file.FullName + " is locked. Skipping this file");
                        //_alertUser = true;
                        continue;
                    }

                    if (uploadParams.WaitForXml)
                    {
                        var xmlFileName = Path.GetFileNameWithoutExtension(file.FullName) + ".xml";
                        var xmlFile = Path.Combine(uploadParams.PdfPath, xmlFileName);
                        Debug.Log("FrmFolderDetails","BwUpload_DoWork","File " + file.FullName + " found, waiting for XML file "+ xmlFile);
                        if (!File.Exists(xmlFile))
                        {
                            _uploadOK = false;
                            _waitingForXml = true;
                            continue;
                        }
                    }

                    _waitingForXml = false;
                    fileNumber++;
                    string filename = file.Name;
                    long filesize = file.Length;
                    uxProgressBar.Maximum = Convert.ToInt32(filesize);
                    StartProgressBar();

                    //Set status to Uploading filename...
                    string filenameTruncated = filename;
                    if (filename.Length > 32)
                    {
                        filenameTruncated = filename.Substring(0, 30) + "...";
                    }

                    _bwUpload.ReportProgress(-1, "\"" + filenameTruncated + "\"");

                    //Upload the document          
                    int Offset = 0; // starting offset.

                    //define the chunk size
                    int ChunkSize = 1048576; // 64 * 1024 kb
                    //define the buffer array according to the chunksize.
                    byte[] Buffer = new byte[ChunkSize];

                    string pdfPath = Path.Combine(uploadParams.PdfPath, filename);
                    
                    
                    string url = uploadParams.UploadUrl;
                    Guid libraryUserId = uploadParams.LibraryUserId;
                    var libraryId = uploadParams.LibraryId;
                    var portalId = uploadParams.PortalId;

                    string uploadDir = _userLibraryUserId.ToString();

                    _bwUpload.ReportProgress(0); //Set ProgressBar to 0


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

                    LogMessage(webSvc, portalId, MessageLevel.Information, 0, "Uploading file " + filename);


                    //look for XML file if the same name
                    var xmlName = Path.GetFileNameWithoutExtension(pdfPath) + ".xml";
                    var xmlPath = Path.Combine(uploadParams.PdfPath, xmlName);
                    if (File.Exists(xmlPath))
                    {
                        
                        LogMessage(webSvc, portalId, MessageLevel.Information, 0, "Found XML file " + xmlName);
                        XmlDocument doc = new XmlDocument();
                        doc.Load(xmlPath);

                        var userIdNode = doc.SelectSingleNode("/Skyline/UserId");
                        if (userIdNode != null)
                        {
                            var userIdValue = userIdNode.InnerText;
                            Guid userId = Guid.Empty;
                            var userIdFound = Guid.TryParse(userIdValue, out userId);

                            if (userIdFound)
                            {
                                var userDefaultLibraryId = webSvc.GetUserDefaultLibraryId(portalId, userId);
                                if (userDefaultLibraryId != Guid.Empty)
                                {
                                    LogMessage(webSvc, portalId, MessageLevel.Information, 0, "Found user and user's default library");

                                    bool userActive = webSvc.IsUserActive(userId);
                                    if (userActive)
                                    {
                                        uploadDir = userId.ToString();
                                        libraryUserId = userId;
                                        libraryId = userDefaultLibraryId;
                                    }
                                    else
                                    {
                                        Debug.Log("FrmFolderDetails","BwUpload_DoWork","The user with the user ID "+ userId + " is not activated. The document will be uploaded to the default library "+ uploadParams.LibraryName);
                                        LogMessage(webSvc, portalId, MessageLevel.Warning, 0, "The user with the user ID "+ userId + " is not activated. The document will be uploaded to the default library " + uploadParams.LibraryName);
                                    }
                                    
                                }
                                else
                                {
                                    Debug.Log("FrmFolderDetails","BwUpload_DoWork","Unable to get the default library for the user ID "+ userId + ". The document will be uploaded to the default library "+ uploadParams.LibraryName);
                                    LogMessage(webSvc, portalId, MessageLevel.Warning, 0, "Unable to get the default library for the user ID "+ userId + ". The document will be uploaded to the default library " + uploadParams.LibraryName);
                                }

                            }
                            else
                            {
                                userIdNode = null;
                            }
                        }

                        var emailNode = doc.SelectSingleNode("/Skyline/Email");
                        if (emailNode != null && userIdNode==null)
                        {
                            var email = emailNode.InnerText;
                            if (!string.IsNullOrEmpty(email))
                            {
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
                                        Debug.Log("FrmFolderDetails","BwUpload_DoWork","The user with the email address "+ email + " is not activated. The document will be uploaded to the default library "+ uploadParams.LibraryName);
                                        LogMessage(webSvc, portalId, MessageLevel.Warning, 0, "The user with the email address "+ email + " is not activated. The document will be uploaded to the default library " + uploadParams.LibraryName);
                                    }
                                    
                                }
                                else
                                {
                                    Debug.Log("FrmFolderDetails","BwUpload_DoWork","Unable to get the default library for the email address "+ email + ". The document will be uploaded to the default library "+ uploadParams.LibraryName);
                                    LogMessage(webSvc, portalId, MessageLevel.Warning, 0, "Unable to get the default library for the email address "+ email + ". The document will be uploaded to the default library " + uploadParams.LibraryName);
                                }
                            }
                        }
                    }

                    

                    try
                    {
                        long fileSize = new FileInfo(pdfPath).Length; // File size of file being uploaded.
                                                                      // reading the file.

                        //UxProgressBar.Maximum = Convert.ToInt32(fileSize);
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

                            if (_bwUpload.CancellationPending)
                            {
                                _uploadCancelled = true;
                                break;
                            }

                            _bwUpload.ReportProgress(Offset, fileNumber + "/" + _totalFiles);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Error("FrmFolderDetails","BwUpload_DoWork","Error uploading file " + pdfPath + " to " + url, ex);
                        fs.Close();
                        _errorMessage = "Error uploading the document:\n\n" + ex.Message;
                        _uploadOK = false;
                        timer1.Enabled = true;
                        LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Error uploading file " + pdfPath + " to " + url + " Error message: " + ex.Message);
                        return;
                    }
                    finally
                    {
                        fs.Close();
                    }

                    if (_uploadCancelled)
                    {
                        fs.Close();
                        _bwUpload.ReportProgress(-3);
                        if (webSvc.DeleteTempUploadedFile(uploadDir))
                        {
                            _bwUpload.ReportProgress(-4);
                            _uploadOK = false;
                            _errorMessage = "Upload Cancelled";
                            timer1.Enabled = true;
                            return;
                        }

                    }

                    LogMessage(webSvc, portalId, MessageLevel.Information, 0, "File " + filename + " uploaded. Moving it to the user library");

                    _bwUpload.ReportProgress(-2, "\"" + filenameTruncated + "\"");
                    try
                    {
                        string docIdOrError = webSvc.MoveTempDocumentsToSpecificLibrary(libraryUserId, libraryId,false);
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
                            Debug.Error("FrmFolderDetails","BwUpload_DoWork",docIdOrError);
                            LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Error in MoveTempDocumentsToSpecificLibrary: "+ docIdOrError);
                        }
                        else
                        {
                            _uploadOK = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //possible timeout
                        Debug.Error("FrmFolderDetails","BwUpload_DoWork","Error calling MoveTempDocumentsToUserLibrary", ex);
                        _errorMessage = "Error copying your document to your online library:\n\n" + ex.Message;
                        _uploadOK = false;
                        LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Unexpected error calling MoveTempDocumentsToUserLibrary. Message = "+ ex.Message);
                    }

                    if (_uploadOK && uxCheckBoxDeleteSource.Checked)
                    {
                        Debug.Log("FrmFolderDetails","BwUpload_DoWork","File uploaded OK. Deleting source file "+ pdfPath);
                        try
                        {
                            File.Delete(pdfPath);
                        }
                        catch (Exception exception)
                        {
                            Debug.Error("FrmFolderDetails","BwUpload_DoWork","Error deleting file "+ pdfPath,exception);
                            _uploadOK = false;
                        }
                        
                        if (File.Exists(pdfPath))
                        {
                            _uploadOK = false;
                            Debug.Error("FrmFolderDetails","BwUpload_DoWork","Unable to deleted file " + pdfPath);
                        }

                        if (File.Exists(xmlPath))
                        {
                            Debug.Log("FrmFolderDetails","BwUpload_DoWork","Deleting associated XML file "+ xmlPath);
                            try
                            {
                                File.Delete(xmlPath);
                            }
                            catch (Exception exception)
                            {
                                Debug.Error("FrmFolderDetails","BwUpload_DoWork","Error deleting file "+ xmlPath,exception);
                                _uploadOK = false;
                            }

                            if (File.Exists(xmlPath))
                            {
                                _uploadOK = false;
                                Debug.Error("FrmFolderDetails","BwUpload_DoWork","Unable to deleted file " + pdfPath);
                            }
                        }


                        Debug.Log("FrmFolderDetails","BwUpload_DoWork","Deleting Temp upload folder " + uploadDir);
                        if (!webSvc.DeleteTempUploadedFile(uploadDir))
                        {
                            Debug.Error("FrmFolderDetails","BwUpload_DoWork","Error deleting the Temp upload directory "+ uploadDir);
                            LogMessage(webSvc, portalId, MessageLevel.Error, 0, "Error deleting the Temp upload directory "+ uploadDir);
                        }

                        if (!_uploadOK)
                        {
                            Debug.Error("FrmFolderDetails","BwUpload_DoWork","Error during the upload. Skipping further uploads");
                            LogMessage(webSvc, portalId, MessageLevel.Critical, 0, "Error during the upload. Skipping further uploads from "+ uploadDir);
                            break;
                        }
                    }
                }

                if (_uploadOK)
                {
                    _bwUpload.ReportProgress(-5);
                }
                else
                {
                    if (_waitingForXml)
                    {
                        _bwUpload.ReportProgress(-6);
                    }
                    else
                    {
                        _bwUpload.ReportProgress(-3);
                    }
                }

            }
            if (_uploadOK)
            {
                _bwUpload.ReportProgress(-5);
            }
            else
            {
                if (_waitingForXml)
                {
                    _bwUpload.ReportProgress(-6);
                }
                else
                {
                    _bwUpload.ReportProgress(-3);
                }
            }
        }

        private static void LogMessage(SkylineWebService.SkylineWebService webSvc, Guid portalId, MessageLevel messageLevel, int eventId, string message)
        {
            var computerName = Environment.MachineName;
            if (message.Length > 2000)
            {
                message = message.Substring(0, 1996) + "...";
            }
            webSvc.SaveLogMessage(portalId,"Skyline Uploader App",computerName,(int)messageLevel,eventId,message);
        }

        private void BwUpload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progressValue = e.ProgressPercentage;

            if (progressValue == -1)
            {
                string filename = e.UserState as string;
                if (!string.IsNullOrEmpty(filename))
                {
                    uxLabelStatus.Text = "Uploading " + filename;
                }
                else
                {
                    uxLabelStatus.Text = "Uploading...";
                }

            }
            else if (progressValue == -2)
            {
                string filename = e.UserState as string;
                if (!string.IsNullOrEmpty(filename))
                {
                    uxLabelStatus.Text = "Moving " + filename + " to the user library";
                }
                else
                {
                    uxLabelStatus.Text = "Moving the file to the user library...";
                }
            }
            else if (progressValue == -3)
            {
                StopProgressBar();
                StartWaitingBar();
                uxLabelStatus.Text = "Cancelling upload. Please wait...";


            }
            else if (progressValue == -4)
            {
                StopWaitingBar();
                StopProgressBar();
                uxLabelStatus.Text = "Upload cancelled.";
            }

            else if (progressValue == -5)
            {
                StopWaitingBar();
                StopProgressBar();
                uxLabelStatus.Text = "Upload Complete.";
            }
            else if (progressValue == -6)
            {
                uxLabelStatus.Text = "Waiting for XML file.";
            }

            else
            {
                if (_bwUpload.CancellationPending) return;

                StartProgressBar();
                string progress = e.UserState as string;
                if (!string.IsNullOrEmpty(progress))
                {
                    uxProgressBar.Text = progress;
                }
                else
                {
                    uxProgressBar.Text = progressValue.ToString();
                }
                uxProgressBar.Value1 = progressValue;
            }

        }

        private void BwUpload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_uploadOK)
            {
                //string uploadUrl = uploadParams.UploadUrl;
                //string username = uploadParams.username;
                //string password = uploadParams.Password;


                //string siteUrl= uploadUrl+ "/ChooseDocumentType.aspx";
                //string siteUrl = uploadUrl + "/Login.aspx?ReturnUrl=%2fChooseDocumentType.aspx";                


                //Webcalls.GotoSite(siteUrl, username, password, _docId.ToString(), _numPages);

                //Close();
                Debug.Log("FrmFolderDetails","BwUpload_RunWorkerCompleted","All files uploaded OK");
            }
            else if (!_uploadCancelled)
            {
                if (_errorMessage != string.Empty)
                {
                    Debug.Error("FrmFolderDetails","BwUpload_RunWorkerCompleted",_errorMessage);
                    
                }
            }

            //if (_alertUser)
            //{
            //    DialogResult res = MessageBox.Show("There were some problems during this upload. Do you want to see the log file?", "Problems Uploading", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            //    if (res == DialogResult.Yes)
            //    {
            //        Debug.OpenLogDirectory();
            //    }
            //}
            
            _uploadCancelled = false;
            timer1.Enabled = true;
        }

        void BwLoginDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            _portalFound = false;
            string portalUrl = uxTextBoxPortalUrl.Text;

            if (Program.PricingService == null)
            {
                Program.PricingService = new PricingService.PricingService();
            }

            if (Program.SkylineService == null)
            {
                Program.SkylineService = new SkylineWebService.SkylineWebService();
            }

            if (_useProxy)
            {
                ICredentials credentials;
                if (!string.IsNullOrEmpty(_proxyDomain))
                {
                    credentials = new NetworkCredential(_proxyUserName, SettingsHelper.Decrypt(_proxyPassword), _proxyDomain);
                }
                else
                {
                    credentials = new NetworkCredential(_proxyUserName, SettingsHelper.Decrypt(_proxyPassword));
                }

                WebProxy webProxy = new WebProxy(_proxyAddress, _proxyPort);
                webProxy.Credentials = credentials;
                Program.PricingService.Proxy = webProxy;
                Program.SkylineService.Proxy = webProxy;
            }

            try
            {
                Guid portalId = Guid.Empty;

                string rawUrl = portalUrl.Replace("http://", "");
                rawUrl = rawUrl.Replace("https://", "");

                try
                {
                    Program.PricingService.Url = portalUrl + "/webservices/PricingService.asmx";
                    _bwLogin.ReportProgress(0, "Looking for https://" + rawUrl);
                    portalId = Program.PricingService.GetPortalGuidFromUrl(rawUrl);
                }
                catch (WebException)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _bwLogin.ReportProgress(0, "Looking for http://" + rawUrl);
                    Program.PricingService.Url = "http://" + rawUrl + "/webservices/PricingService.asmx";
                    portalId = Program.PricingService.GetPortalGuidFromUrl(rawUrl);
                }

                if (portalId == Guid.Empty)
                {
                    _bwLogin.ReportProgress(0, "Error connecting to " + rawUrl);
                    _portalFound = false;
                    return;
                }
                _portalId = portalId;

                bool useHttps = Program.PricingService.UseHttps(_portalId);

                string urlType = useHttps == true ? "https://" : "http://";
                _portalUrl = urlType + rawUrl;

                _bwLogin.ReportProgress(0, "Connected to " + _portalUrl);


                Program.PricingService.Url = urlType + rawUrl + "/webservices/PricingService.asmx";


                _loginUserId = Program.PricingService.ValidateUser(uxTextBoxUsername.Text, uxTextBoxPassword.Text, out _validUser, out _globalUser);

                //_portalId = Program.SkylineService.GetPortalIdForUrl(_portalUrl);

                _portalFound = true;
            }
            catch (WebException ex)
            {
                
                    _portalFound = false;
                    switch (ex.Status)
                    {
                        case  WebExceptionStatus.NameResolutionFailure:
                            _loginError = "Could not find the website name " + uxTextBoxPortalUrl.Text;
                            break;
                        case WebExceptionStatus.ProtocolError:
                            _loginError = "There was an problem connecting to " + uxTextBoxPortalUrl.Text;
                            break;
                        default:
                            _loginError = ex.Message;
                            break;
                    }

                    _bwLogin.ReportProgress(0, _loginError);
                    return;
                
            }
            catch (Exception)
            {
                _portalFound = false;
                _bwLogin.ReportProgress(0, "Error connecting to " + uxTextBoxPortalUrl.Text);
                return;
            }

            _serverVersion = Program.PricingService.GetAssemblyVersion();
            _workStationVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            try
            {
                Version ver = Version.Parse(_serverVersion);
                int major = ver.Major;
                int minor = ver.Minor;
                int revision = ver.Revision;
                if (major >= 7 && minor >= 1)
                {
                    Program.VersionOk =  true;
                }
                else
                {
                    Program.VersionOk =  false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine(e);
                Program.VersionOk =  false;
            }
            
        }

        private void BwLoginProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                string message = e.UserState.ToString();
                uxLabelConnecting.Text = message;
            }
        }

        void BwLoginWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            uxTextBoxPortalUrl.Enabled = true;
            uxTextBoxUsername.Enabled = true;
            uxTextBoxPassword.Enabled = true;
            uxButtonConnect.Enabled = true;
            //uxLabelConnecting.Visible = false;
            uxButtonConnect.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            uxButtonConnect.Text = "Connect";

            if (!_portalFound)
            {
                MessageBox.Show(_loginError, "Portal not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                uxLabelStatus.Text = "Portal not found";
                return;
            }

            if (!Program.VersionOk)
            {
                MessageBox.Show("This application is not at the correct version for this portal" + "\n\n" + "Server version: " +
                                _serverVersion + "\n" + "Your version: " + _workStationVersion, "Error");
                uxLabelStatus.Text = "This application is not at the correct version for this portal";
                return;
            }

            if (_validUser && _loginUserId != Guid.Empty)
            {
                //bool isUserAdminOrHost = Program.SkylineService.IsUserAdminOrHost(_userId, _portalId);
                uxLabelConnecting.Text = "Connected";
                uxLabelStatus.Text = "Loading settings";

                UserSettings[] userList = null;
                try
                {
                    Program.SkylineService.Url = _portalUrl+ "/WebServices/SkylineWebService.asmx";
                    userList = Program.SkylineService.GetPortalUsers(_portalId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to get the list of users. \n\n"+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (userList==null)
                {
                    MessageBox.Show("Unable to get the list of users", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    
                    users = userList.ToList();
                    uxListControlUsers.DataSource = users;
                    uxListControlUsers.DisplayMember = "UserName";
                    uxListControlUsers.ValueMember = "UserId";
                    uxListControlUsers.SelectRange(-1, -1); //clear selection
                    UxButtonBrowse.Enabled = true;
                    uxTextBoxSourceFolder.Enabled = true;
                    uxCheckedDropDownListFileTypes.Enabled = true;
                    uxTextBoxSelected.Enabled = true;
                    uxTextBoxFolderName.Enabled = true;
                    uxTextBoxPortalUrl.Text = _portalUrl;

                    uxCheckBoxEnabled.Enabled = true;
                    uxCheckBoxWaitForXml.Enabled = true;
                }
                catch (Exception)
                {
                    return;
                }
                
                

                GetDocumentFolderDetails(FolderId);

                if(!RequiredSettingsMissing()) uxButtonUpload.Enabled = true;
                uxLabelStatus.Text = "Ready";
            }

            else if (_loginUserId != Guid.Empty && !_validUser) //User OK, but not admim, host or manager
            {
                MessageBox.Show("This user is not authorised to log in", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                uxLabelConnecting.Text = "This user is not authorised to log in";

            }
            else if (_loginUserId == Guid.Empty)
            {
                MessageBox.Show("Incorrect username or password", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                uxLabelConnecting.Text = "Incorrect username or password";
            }
            else
            {
                MessageBox.Show("Error connecting to portal", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                uxLabelConnecting.Text = "Error connecting to portal";
            }

        }

        private void uxButtonClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (_workStationVersion != null && _bwLogin!= null && _bwLogin.WorkerSupportsCancellation == true)
                {
                    // Cancel the asynchronous operation.
                    _bwLogin.CancelAsync();
                }

                if (_bwCheckUrl != null && _bwCheckUrl.IsBusy)
                {
                    _bwCheckUrl.CancelAsync();
                    _checkUrlCancelled = true;
                }

                if (_bwUpload != null && _bwUpload.IsBusy)
                {
                    _bwUpload.ReportProgress(-3);
                    _bwUpload.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.Error("FrmFolderDetails","uxButtonClose_Click","Unexpected error closing the profile",ex);
                MessageBox.Show("Unexpected error closing the profile\n\n" + ex.Message, "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            
            Close();
        }

        private void uxTextBoxPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                DoLogin();
            }
        }

        private void uxPictureBoxProxy_Click(object sender, EventArgs e)
        {
            using (FrmProxySetup frm = new FrmProxySetup())
            {
                frm.ShowDialog();
            }
        }

        //private void uxButtonGetLibraries_Click(object sender, EventArgs e)
        //{
        //    string folderPath = uxTextBoxControlFolder.Text;
        //    if (uxCheckBoxGetFromXml.Checked)
        //    {
        //        uxListControlLibraries.Items.Clear();
        //        if (folderPath == string.Empty)
        //        {
        //            uxLabelErrorMessage.Text = "Please select the Folder";
        //            return;
        //        }

        //        if (!Directory.Exists(folderPath))
        //        {
        //            uxLabelErrorMessage.Text = "Folder not found";
        //            return;
        //        }

        //        string xmlFile = Path.Combine(folderPath, "UserDetails.xml");
        //        if (!File.Exists(xmlFile))
        //        {
        //            uxLabelErrorMessage.Text = "XML file UserDetails.xml not found in Folder";
        //            return;
        //        }

        //    }

        //    uxLabelErrorMessage.Text = string.Empty;

        //    GetUserLibaries();
        //    ClearSelectedLibrary();

        //}

        private void GetUserLibaries(Guid userGuid)
        {
            if (uxListControlUsers.SelectedItem != null)
            {
                _userLibraryUserName = uxListControlUsers.SelectedItem.Text;
            }
            
            
            _userLibraryUserId = userGuid;

            List<LibraryDetails> userLibraries = Program.SkylineService.GetAllUserLibraries(userGuid).ToList();
            uxListControlLibraries.DataSource = userLibraries;
            uxListControlLibraries.DisplayMember = "Name";
            uxListControlLibraries.ValueMember = "LibraryId";
            uxListControlLibraries.SelectRange(-1, -1); //clear selection
        }


        private void GetSelectedLibrary()
        {
            if (uxListControlLibraries.SelectedItem != null)
            {
                uxTextBoxSelected.Text = uxListControlLibraries.SelectedItem.Text;
                if (Guid.TryParse(uxListControlLibraries.SelectedItem.Value.ToString(), out Guid libraryId))
                {
                    _userLibraryLibraryId = libraryId;
                }

            }
        }

        private void FolderDetails_Load(object sender, EventArgs e)
        {
            Debug.Log("FrmFolderDetails","FolderDetails_Load","FolderDetails_Load");
            uxLabelErrorMessage.Text = string.Empty;
            using (UploaderDbContext context = new UploaderDbContext())
            {
               
                var logins = from l in context.Login select l;
                if (logins.Any())
                {
                    Debug.Log("FrmFolderDetails","FolderDetails_Load","logins found. Getting folder details for FolderId: "+ FolderId);
                    Login login = (from lo in logins where lo.FolderId == FolderId  select lo).FirstOrDefault();
                    if (login != null)
                    {
                        uxTextBoxPortalUrl.Text = login.PortalUrl;
                        _username = login.Username;
                        uxTextBoxUsername.Text = _username;
                        _password = SettingsHelper.Decrypt(login.Password);
                        uxTextBoxPassword.Text = _password;
                    }
                }
                else
                {
                    Debug.Log("FrmFolderDetails","FolderDetails_Load","No logins found for FolderId: "+ FolderId);
                }
                

                var proxy = (from p in context.Proxy select p).FirstOrDefault();
                if (proxy != null)
                {
                    Debug.Log("FrmFolderDetails","FolderDetails_Load","Getting Proxy settings");
                    _useProxy = proxy.UseProxy;
                    _proxyDomain = proxy.ProxyDomain;
                    _proxyUserName = proxy.ProxyUsername;
                    _proxyPassword = proxy.ProxyPassword;
                    _proxyAddress = proxy.ProxyAddress;
                    _proxyPort = proxy.ProxyPort;
                }
                else
                {
                    Debug.Log("FrmFolderDetails","FolderDetails_Load","No Proxy settings");
                }

                var folder = (from f in context.Folders where f.FolderId == FolderId select f).FirstOrDefault();
                if (folder != null)
                {
                    Debug.Log("FrmFolderDetails","FolderDetails_Load","Setting folder to EditMode");
                    folder.InEditMode = true;
                }

                Debug.Log("FrmFolderDetails","FolderDetails_Load","context.SaveChanges");
                context.SaveChanges();
            }
        }



        private void UxButtonBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(uxTextBoxSourceFolder.Text))
            {
                folderBrowserDialog1.SelectedPath = uxTextBoxSourceFolder.Text;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                uxTextBoxSourceFolder.Text = path;
                string username = FolderAlreadyConfigured(path);
                if (!string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("This Source Folder is already configured for user " + username);
                    uxButtonSave.Enabled = false;
                    return;
                }
                EnableSaveButton();
                _totalFiles = CheckSourceFolder();
                uxTextBoxFolderName.Text = path.Replace(Path.GetDirectoryName(path) + Path.DirectorySeparatorChar, "");
            }
        }

        private string FolderAlreadyConfigured(string path)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                Guid folderId = (from sf in context.SourceFolders where sf.FolderPath == path select sf.FolderId).FirstOrDefault();
                var username = (from ul in context.UserLibraries where ul.FolderId == folderId select ul.Username).FirstOrDefault();
                return username;
            }
        }


        
        private bool RequiredSettingsMissing()
        {
            if (uxTextBoxSourceFolder.Text == string.Empty)
            {
                uxLabelErrorMessage.Text = "Please select the Source Folder";
                uxLabelErrorMessage.Visible = true;
                return true;
            }

            if (uxTextBoxFolderName.Text == string.Empty)
            {
                uxLabelErrorMessage.Text = "Please enter the Folder Name";
                uxLabelErrorMessage.Visible = true;
                return true;
            }

            //if (!Directory.Exists(uxTextBoxSourceFolder.Text))
            //{
            //    uxLabelErrorMessage.Text = "The Source Folder path is not correct";
            //    uxLabelErrorMessage.Visible = true;
            //    return true;
            //}

            if (uxTextBoxSelected.Text == string.Empty)
            {
                uxLabelErrorMessage.Text = "Please select a User Library";
                uxLabelErrorMessage.Visible = true;
                return true;
            }

            int selectedTypes = uxCheckedDropDownListFileTypes.CheckedItems.Count();

            if (selectedTypes == 0)
            {
                uxLabelErrorMessage.Text = "Please select at least one File Type";
                uxLabelErrorMessage.Visible = true;
                return true;
            }

            uxLabelErrorMessage.Visible = false;
            return false;
        }

        private void uxButtonSave_Click(object sender, EventArgs e)
        {
            if (RequiredSettingsMissing())
            {
                MessageBox.Show(uxLabelErrorMessage.Text);
                return;
            }

            string folderPath = uxTextBoxSourceFolder.Text;

            if (uxCheckBoxEnabled.Checked)
            {
                if (!Directory.Exists(uxTextBoxSourceFolder.Text))
                {
                    uxLabelErrorMessage.Text = "The Source Folder path is not correct";
                    uxLabelErrorMessage.Visible = true;
                    return;
                }

                
                if (!CheckFolderPermissions(folderPath))
                {
                    MessageBox.Show(uxLabelErrorMessage.Text);
                    return;
                }
            }
            

            using (UploaderDbContext context = new UploaderDbContext())
            {
                Guid folderId = FolderId;
                Folder folder = (from f in context.Folders where f.FolderId == folderId select f).FirstOrDefault();
                if (folder == null)
                {
                    folder = new Folder();
                    context.Folders.Add(folder);
                    //folderId = Guid.NewGuid();
                    folder.FolderId = folderId;
                }

                folder.PortalId = _portalId;
                folder.FolderName = uxTextBoxFolderName.Text;
                folder.Enabled = uxCheckBoxEnabled.Checked;
                folder.SynchronizeFiles = uxCheckBoxSynchronize.Checked;
                folder.DeleteAfterUpload = uxCheckBoxDeleteSource.Checked;
                folder.HideOnOrder = uxCheckBoxRemoveDocOnOrder.Checked;
                folder.DeleteAfterDays = -1;
                folder.DeleteAfterUpload = uxCheckBoxDeleteSource.Checked;
                folder.WaitForXml = uxCheckBoxWaitForXml.Checked;
                folder.DateUpdated=DateTime.Now;
                folder.Status = "Idle";
                //folder.InEditMode = true;

                var login = (from l in context.Login where l.FolderId == folderId select l).FirstOrDefault();
                if (login == null)
                {
                    login = new Login();
                    context.Login.Add(login);
                }

                login.PortalUrl = _portalUrl;
                login.FolderId = folderId;
                login.Username = uxTextBoxUsername.Text;
                login.Password = SettingsHelper.Encrypt(uxTextBoxPassword.Text);

                var userLibrary = (from ul in context.UserLibraries where ul.FolderId == folderId select ul).FirstOrDefault();
                if (userLibrary == null)
                {
                    userLibrary = new UserLibrary();
                    context.UserLibraries.Add(userLibrary);
                }


                var user = (from u in users where u.UserId == _userLibraryUserId select u).FirstOrDefault();
                if (user == null)
                {
                    MessageBox.Show("There was a problem getting the selected users details", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    uxLabelErrorMessage.Text = "There was a problem getting the selected users details";
                    return;
                }

                userLibrary.FolderId = folderId;
                userLibrary.Username = user.UserName;
                userLibrary.UserId = user.UserId;
                userLibrary.UserEmail = SettingsHelper.Encrypt(user.Email);
                userLibrary.LibraryName = uxTextBoxSelected.Text;
                userLibrary.LibraryId = _userLibraryLibraryId;


                SourceFolder sourceFolder = (from sf in context.SourceFolders where sf.FolderId == folderId select sf).FirstOrDefault();
                if (sourceFolder == null)
                {
                    sourceFolder = new SourceFolder();
                    context.SourceFolders.Add(sourceFolder);
                }

                sourceFolder.FolderId = folderId;
                sourceFolder.FolderPath = folderPath;
                sourceFolder.Enabled = uxCheckBoxEnabled.Checked;
                sourceFolder.FileCount = 0;

                
                StringBuilder fileTypes = new StringBuilder();
                foreach (RadCheckedListDataItem item in uxCheckedDropDownListFileTypes.CheckedItems)
                {
                    fileTypes.Append(item.Text) ;
                    fileTypes.Append(",");
                }

                var supportedDocumentTypes = fileTypes.ToString();

                //delete last comma
                supportedDocumentTypes = supportedDocumentTypes.Substring(0, supportedDocumentTypes.LastIndexOf(","));
                folder.FileType = supportedDocumentTypes;

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Debug.Error("FrmFolderDetails","uxButtonSave_Click",("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State).ToString());
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.Error("FrmFolderDetails","uxButtonSave_Click",("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage).ToString());
                        }
                    }
                    return;
                }

                MessageBox.Show("Settings saved", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                uxButtonUpload.Enabled = true;
            }


        }

        private bool CheckFolderPermissions(string folderPath)
        {
            var testFile = Path.Combine(folderPath, "__test__.txt");
            try
            {
                if (File.Exists(testFile))
                {
                    File.Delete(testFile);
                }
            }
            catch (Exception e)
            {
                uxLabelErrorMessage.Text = "File "+ testFile + " already found but delete permission missing from the folder " + folderPath;
                Debug.Error("FrmFolderDetails","uxButtonSave_Click","File "+ testFile + " already found but delete permission missing from the folder " + folderPath,e);
                
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
                uxLabelErrorMessage.Text = "Unable to create files in the folder " + folderPath;
                Debug.Error("FrmFolderDetails","CheckFolderPermissions","Unable to create files in the folder " + folderPath,e);
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
                    uxLabelErrorMessage.Text = "Unable to read files in the folder " + folderPath;
                    Debug.Error("FrmFolderDetails","CheckFolderPermissions","Unable to read files in the folder " + folderPath);
                    return false;
                }
            }
            catch (Exception e)
            {
                uxLabelErrorMessage.Text = "Unable to delete files in the folder " + folderPath;
                Debug.Error("FrmFolderDetails","CheckFolderPermissions","Unable to delete files in the folder " + folderPath,e);
                return false;
            }

            uxLabelErrorMessage.Text = "Permissions check OK on folder " + folderPath;
            Debug.Log("FrmFolderDetails","CheckFolderPermissions","Permissions check OK on folder "+ folderPath);
            return true;
        }

        private void uxListControlLibraries_MouseUp(object sender, MouseEventArgs e)
        {
            GetSelectedLibrary();
            //uxTextBoxControlFolder.Text = folderBrowserDialog1.SelectedPath;
            EnableSaveButton();

        }

        private void EnableSaveButton()
        {
            if (!string.IsNullOrEmpty(uxTextBoxSourceFolder.Text) &&
                !string.IsNullOrEmpty(uxTextBoxSelected.Text))
            {
                uxButtonSave.Enabled = true;
            }
            else
            {
                uxButtonSave.Enabled = false;
            }
        }

        private void uxListControlUsers_MouseUp(object sender, MouseEventArgs e)
        {
            if (uxListControlUsers.SelectedItems.Count == 0) return;

            var userId = uxListControlUsers.SelectedItem.Value.ToString();

            Guid userGuid = Guid.Empty;

            if (Guid.TryParse(userId, out userGuid))
            {
                if (userGuid == Guid.Empty)
                {
                    return;
                }
            }

            _userLibraryUserId = userGuid;
            GetUserLibaries(userGuid);
            ClearSelectedLibrary();
        }

        private void ClearSelectedLibrary()
        {
            uxTextBoxSelected.Text = string.Empty;
            uxButtonSave.Enabled = false;
        }

        private void GetDocumentFolderDetails(Guid folderId)
        {

            using (UploaderDbContext context = new UploaderDbContext())
            {
                Folder folder = (from f in context.Folders where f.FolderId == folderId select f).FirstOrDefault();
                if (folder == null)
                {
                    //MessageBox.Show("Please select a Source Folder",
                    //    "Folder Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //_portalId = folder.PortalId;
                uxTextBoxFolderName.Text = folder.FolderName;
                uxCheckBoxEnabled.Checked = folder.Enabled;
                uxCheckBoxSynchronize.Checked = folder.SynchronizeFiles;
                uxCheckBoxRemoveDocOnOrder.Checked = folder.HideOnOrder;
                uxCheckBoxDeleteSource.Checked = folder.DeleteAfterUpload;
                uxCheckBoxWaitForXml.Checked = folder.WaitForXml;
                //folder.DeleteAfterDays = -1;

                var userLibrary = (from ul in context.UserLibraries where ul.FolderId == folderId select ul).FirstOrDefault();
                if (userLibrary == null)
                {
                    MessageBox.Show("There was a problem getting the User Library details from the database",
                        "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                var username = userLibrary.Username;

                var userId = userLibrary.UserId;
                var libraryId = userLibrary.LibraryId;
                var userEmail = SettingsHelper.Decrypt(userLibrary.UserEmail);
                uxListControlUsers.SelectedValue = userId;

                GetUserLibaries(userId);
                uxListControlLibraries.SelectedValue = libraryId;

                uxTextBoxSelected.Text = userLibrary.LibraryName;
                _userLibraryLibraryId = userLibrary.LibraryId;

                var sourceFolder = (from sf in context.SourceFolders where sf.FolderId == folderId select sf).FirstOrDefault();
                if (sourceFolder == null)
                {
                    MessageBox.Show("There was a problem getting the Source Folder details from the database",
                        "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                uxTextBoxSourceFolder.Text = sourceFolder.FolderPath;
                uxCheckBoxEnabled.Checked = sourceFolder.Enabled;

                string documentTypes = folder.FileType;
                if (!string.IsNullOrEmpty(documentTypes))
                {
                    string[] values = documentTypes.Split(',');
                    foreach (var value in values)
                    {
                    
                        foreach (RadCheckedListDataItem item in this.uxCheckedDropDownListFileTypes.Items)
                        {
                            if (item.Text == value)
                            {
                                item.Checked = true;
                            }
                        }
                    }
                }

                EnableSaveButton();

                var login = (from l in context.Login where l.FolderId == folderId select l).FirstOrDefault();
                if (login == null)
                {
                    login = new Login();
                    context.Login.Add(login);
                }

                login.PortalUrl = _portalUrl;
                login.FolderId = folderId;
                login.Username = uxTextBoxUsername.Text;
                login.Password = SettingsHelper.Encrypt(uxTextBoxPassword.Text);
            }
            _totalFiles = CheckSourceFolder();
        }

        private void uxTextBoxControlFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(uxTextBoxSourceFolder.Text) &&
                    !string.IsNullOrEmpty(uxTextBoxSelected.Text))
                {
                    uxButtonSave.Enabled = true;
                }
                else
                {
                    uxButtonSave.Enabled = false;
                }
            }
        }

        private void uxCheckBoxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            EnableSaveButton();
        }

        private void uxCheckBoxSynchronize_CheckedChanged(object sender, EventArgs e)
        {
            EnableSaveButton();
        }

        private int CheckSourceFolder()
        {
            if (string.IsNullOrEmpty(uxTextBoxSourceFolder.Text))
            {
                MessageBox.Show("Please select the Source Folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            
            int totalFiles = 0;
            _extensions = new List<string>();
            DropDownCheckedItemsCollection documentTypes = uxCheckedDropDownListFileTypes.CheckedItems;
            foreach (var documentType in documentTypes)
            {
                var type = documentType.Text;
                switch (type)
                {
                    case "PDF":
                        _extensions.Add("*.pdf");
                        break;
                    case "Word":
                        _extensions.Add("*.docx");
                        _extensions.Add("*.doc");
                        break;
                    case "Excel":
                        _extensions.Add("*.xlsx");
                        _extensions.Add("*.xls");
                        break;
                    case "PowerPoint":
                        _extensions.Add("*.pptx");
                        _extensions.Add("*.ppt");
                        break;
                    case "Image":
                        _extensions.Add("*.jpg");
                        _extensions.Add("*.jpeg");
                        _extensions.Add("*.png");
                        _extensions.Add("*.bmp");
                        _extensions.Add("*.gif");
                        //extensions.Add("*.tiff");
                        break;
                }
            }

            foreach (var extension in _extensions)
            {
                totalFiles = 0;
                DirectoryInfo dinfo = new DirectoryInfo(uxTextBoxSourceFolder.Text);
                try
                {
                    FileInfo[] Files = dinfo.GetFiles(extension);
                    totalFiles += Files.Length;
                }
                catch (Exception )
                {
                    
                }
            }

            uxTextBoxFileCount.Text = totalFiles.ToString("N0");
            return totalFiles;
        }

        private void uxButtonUpload_Click(object sender, EventArgs e)
        {
            if (RequiredSettingsMissing())
            {
                MessageBox.Show(uxLabelErrorMessage.Text);
                return;
            }

            
            _checkUrlCancelled = false;

            uxButtonUpload.Visibility = ElementVisibility.Collapsed;
            uxButtonCancel.Visibility = ElementVisibility.Visible;

            var checkUrlBusy = _bwCheckUrl.IsBusy;
            var uploadBury = _bwUpload.IsBusy;

            
            timer1.Enabled = true;
            
        }

        private void uxButtonCancel_Click(object sender, EventArgs e)
        {
            if (_bwCheckUrl.IsBusy)
            {
                _bwCheckUrl.CancelAsync();
                _checkUrlCancelled = true;
            }

            if (_bwUpload.IsBusy)
            {
                _bwUpload.ReportProgress(-3);
                _bwUpload.CancelAsync();
            }

            uxButtonCancel.Visibility = ElementVisibility.Hidden;
            //uxButtonUpload.Visibility = ElementVisibility.Visible;
            StopWaitingBar();
            StopProgressBar();

            timer1.Enabled = false;

            uxLabelStatus.Text = "Upload cancelled";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            StartWaitingBar();
            if (_bwCheckUrl.IsBusy)
            {
                MessageBox.Show("Please wait until the current upload is complete", "Upload in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                timer1.Enabled = true;
                return;
            }

            if (_bwUpload.IsBusy)
            {
                MessageBox.Show("Please wait until the current upload is complete", "Upload in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                timer1.Enabled = true;
                return;
            }

            _totalFiles = CheckSourceFolder();
            if (_totalFiles > 0)
            {
                if (!_bwCheckUrl.IsBusy) _bwCheckUrl.RunWorkerAsync(_portalUrl);
            }
            else
            {
                uxLabelStatus.Text = "Waiting for files to upload";
                timer1.Enabled = true;
            }
        }

        private void StartWaitingBar()
        {
            uxWaitingBar.Visibility = ElementVisibility.Visible;
            uxWaitingBar.StartWaiting();
        }

        private void StopWaitingBar()
        {
            uxWaitingBar.Visibility = ElementVisibility.Collapsed;
            uxWaitingBar.StopWaiting();
        }

        private void StartProgressBar()
        {
            uxProgressBar.Visibility = ElementVisibility.Visible;
        }

        private void StopProgressBar()
        {
            uxProgressBar.Visibility = ElementVisibility.Collapsed;
        }

        private void FrmFolderDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                var folder = (from f in context.Folders where f.FolderId == FolderId select f).FirstOrDefault();
                if (folder != null)
                {
                    folder.InEditMode = false;
                    folder.Status = "Idle";
                }

                context.SaveChanges();
            }
        }
    }
}
