﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using HelperClasses;
using SkylineUploader.Classes;
using SkylineUploader.SkylineWebService;
using SkylineUploaderDomain.DataModel;

namespace ConsoleApp
{
    class Program
    {
        private static string _connectionString;
        private static System.Timers.Timer _timer;
        private static IQueryable<GridData> _folderData;

        public static SkylineUploader.PricingService.PricingService PricingService;
        public static SkylineUploader.SkylineWebService.SkylineWebService SkylineService;
        //public static Guid UserId;
        //public static Guid PortalId;
        //public static string PortalUrl;
        //public static bool ValidUser;
        //public static bool GlobalUser;
        //public static bool GlobalProducts;
        public static bool VersionOk;

        private static Guid _loginUserId = Guid.Empty;
        private static Guid _portalId;
        private static string _portalUrl;
        private static bool _validUser;
        private static bool _globalUser;
        private static bool _portalFound;
        private static string _username;
        private static string _password;
        private static bool _urlValid;
        private static bool _checkUrlCancelled = false;
        private static bool _uploadCancelled = false;
        private static bool _uploadOK = false;
        private static Guid _docId = Guid.Empty;
        private static string _errorMessage = string.Empty;
        private static int _totalFiles = 0;

        private static string _portalVersion;
        private static string _serviceVersion;

        static void Main(string[] args)
        {
            

            _connectionString = GetConnectionString();

            if (string.IsNullOrEmpty(_connectionString))
            {
                StopService();
            }

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);
                var dataSource = builder["Data Source"].ToString();
                if (string.IsNullOrEmpty(dataSource))
                {
                    StopService();
                }
            }
            catch (Exception e)
            {
                StopService();
            }

            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimerEvent);
            _timer.Interval = 5000;
            _timer.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample.");
            while(Console.Read() != 'q');

            

        }

        private static void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            using (UploaderDbContext context = new UploaderDbContext())
            {
                context.Database.Connection.ConnectionString = _connectionString;
                _folderData = from f in context.Folders
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
                        Files = 0,
                        Enabled = f.Enabled,
                        SourceFolder = sf.FolderPath,
                        InEditMode = f.InEditMode
                    };

                if (!_folderData.Any())
                {
                    _timer.Start();
                    return;
                }

                var count = _folderData.Count();
                foreach (var profile in _folderData)
                {
                    if(profile.InEditMode) continue;

                    var portalUrl = profile.PortalUrl;
                    var username = profile.AdminUsername;
                    var password = SettingsHelper.Decrypt(profile.AdminPassword);

                    if (!CheckUrl(portalUrl))
                    {
                        continue;
                    }

                    if (!DoLogin(portalUrl, username, password))
                    {
                        continue;
                    }


                }

                _timer.Start();
            }
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
                _portalUrl = urlType + rawUrl;

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
                var major = ver.Major;
                var minor = ver.Minor;
                var revision = ver.Revision;
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


        private static string GetConnectionString([CallerLineNumber] int lineNumber = 0)
        {
            string connectionString= string.Empty;
            try
            {
                Console.WriteLine("Looking for ConnectionString");
                connectionString = SettingsHelper.GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("ConnectionString empty. Stopping service",EventLogEntryType.Error,lineNumber);
                    StopService();
                    return null;
                }

                if (connectionString.Contains("*error*"))
                {
                    Console.WriteLine("Error getting the ConnectionString: "+ connectionString +". Stopping service",EventLogEntryType.Error,lineNumber);
                    StopService();
                    return null;
                }

                Console.WriteLine("ConnectionString = "+ connectionString);
                

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                var dataSource = builder["Data Source"].ToString();
                if (string.IsNullOrEmpty(dataSource))
                {
                    Console.WriteLine("Data Source not defined in ConnectionString. Shutting down",EventLogEntryType.Error,lineNumber);
                    StopService();
                }
                Console.WriteLine("Data Source: "+ dataSource);
                return connectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error in GetConnectionStringFromRegistry(). Error = "+ex.Message,EventLogEntryType.Error,lineNumber);
                Console.WriteLine("ConnectionString = "+ connectionString,EventLogEntryType.Error,lineNumber);
                StopService();
            }

            return null;
        }

        private static void StopService()
        {
            Environment.Exit(0);
        }
    }
}