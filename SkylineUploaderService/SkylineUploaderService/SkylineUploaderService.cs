using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using HelperClasses;
using SkylineUploader.Classes;

namespace SkylineUploaderService
{
    //https://docs.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer
    
    public partial class SkylineUploaderService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        public string _connectionString;

        public SkylineUploaderService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists("Skyline Uploader Service"))
            {
                EventLog.CreateEventSource(
                    "Skyline Uploader Service", "Skyline Uploader Service Log");
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

            _connectionString =  GetConnectionStringFromRegistry(); 
        }

        private string GetConnectionStringFromRegistry([CallerLineNumber] int lineNumber = 0)
        {
            string connectionString= string.Empty;
            try
            {
                eventLog.WriteEntry("Looking for ConnectionString");
                //connectionString = RegistryHelper.ReadRegistryKey("ConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    eventLog.WriteEntry("ConnectionString empty. Stopping service",EventLogEntryType.Error,lineNumber);
                    StopService();
                    return null;
                }

                if (connectionString.Contains("*error*"))
                {
                    eventLog.WriteEntry("Error getting the ConnectionString: "+ connectionString +". Stopping service",EventLogEntryType.Error,lineNumber);
                    StopService();
                    return null;
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
            eventLog.WriteEntry("Stopping service");
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
    }
}
