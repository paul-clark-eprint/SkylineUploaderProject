using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SkylineUploaderService
{
    public partial class SkylineUploaderService : ServiceBase
    {
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

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Skyline Uploader Service Starting");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("Skyline Uploader Service stopping");
        }
    }
}
