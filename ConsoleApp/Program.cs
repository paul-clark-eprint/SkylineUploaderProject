using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
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

        static void Main(string[] args)
        {
            

            _connectionString = GetConnectionString();
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

                    if (CheckUrl(portalUrl))
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
