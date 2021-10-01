using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SkylineUploader
{
    public class Debug
    {
        //private static string _logDir = Settings.GetLogDir();     
        private static string LogDir
        {
            get { return Settings.GetLogDir(); }
            set { LogDir = value; }
        }

        public static void Log(string info)
        {
            if (LogDir == null) return;
            string timeStamp = DateTime.Now.ToString("F");
            try
            {
                File.AppendAllText(Path.Combine(LogDir, "Debug.txt"), timeStamp + ":\t" + info + Environment.NewLine);
            }
            catch (Exception)
            {
                //
            }
        }

        /// <summary>
        /// Writes to the Error.txt file
        /// </summary>
        /// <param name="error">Error message</param>
        /// <param name="e">Exception</param>
        public static void Error(string error, Exception e)
        {
            if (LogDir == null) return;
            string timeStamp = DateTime.Now.ToString("F");
            try
            {
                File.AppendAllText(Path.Combine(LogDir, "Error.txt"), timeStamp + ":\t" + error + Environment.NewLine);
                File.AppendAllText(Path.Combine(LogDir, "Error.txt"), timeStamp + ":\t" + "Error source: " + e.Source + Environment.NewLine);
                File.AppendAllText(Path.Combine(LogDir, "Error.txt"), e.Message + Environment.NewLine);
                File.AppendAllText(Path.Combine(LogDir, "Error.txt"), Environment.NewLine);
            }
            catch (Exception)
            {
                //
            }
        }

        public static void Error(string error)
        {
            if (LogDir == null) return;
            string timeStamp = DateTime.Now.ToString("F");
            try
            {
                File.AppendAllText(Path.Combine(LogDir, "Error.txt"), timeStamp + ":\t" + error + Environment.NewLine);
                File.AppendAllText(Path.Combine(LogDir, "Error.txt"), Environment.NewLine);
            }
            catch (Exception)
            {
                //
            }
        }

        /// <summary>
        /// Returs the EventLogType from string
        /// Valid values: error, warning 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static EventLogEntryType EventType(string type)
        {
            switch (type.ToLower())
            {
                case "error":
                    return EventLogEntryType.Error;
                case "warning":
                    return EventLogEntryType.Warning;
                default:
                    return EventLogEntryType.Information;
            }
        }

        /// <summary>
        /// Event log error, warning or information 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        public static void WriteEventLog(string message, string logType)
        {
            string sSource = "SkylineUploaded";
            string sLog = "Application";

            try
            {
                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);

                EventLog.WriteEntry(sSource, message, EventType(logType));
            }
            catch (Exception)
            {
                //
            }
            

        }

        /// <summary>
        /// Event log information 
        /// </summary>
        /// <param name="message"></param>
        public static void WriteEventLog(string message)
        {
            string sSource = "SkylineUploaded";
            string sLog = "Application";

            try
            {
                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);

                EventLog.WriteEntry(sSource, message, EventLogEntryType.Information);
            }
            catch (Exception)
            {
                //
            }
        }

        public static void CheckLogFileSizes()
        {
            if (LogDir == null) return;

            string debugLog = Path.Combine(LogDir, "Debug.txt") ;
            if (File.Exists(debugLog))
            {
                FileInfo file = new FileInfo(debugLog);
                if (file.Length > 1000000)
                {
                    string oldFile = Path.Combine(LogDir, "Debug.old");
                    try
                    {
                        if (File.Exists(oldFile)) File.Delete(oldFile);
                        File.Move(debugLog, oldFile);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteEventLog("Error copying  the directory '" + debugLog + "' to " + oldFile +" : " + ex.Message);                        
                    }
                }
            }

            string errorLog = Path.Combine(LogDir, "Error.txt");
            if (File.Exists(errorLog))
            {
                FileInfo file = new FileInfo(errorLog);
                if (file.Length > 1000000)
                {
                    string oldFile = Path.Combine(LogDir, "Error.old");
                    try
                    {
                        if (File.Exists(oldFile)) File.Delete(oldFile);
                        File.Move(errorLog, oldFile);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteEventLog("Error copying  the directory '" + errorLog + "' to " + oldFile + " : " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Show error message
        /// </summary>
        /// <param name="message"></param>
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Critical Error. Unable to continue", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
