using System;
using System.IO;
using System.Management;
using System.Xml;
using SkylineUploader.Classes;


namespace SkylineUploader
{
    class Settings
    {
        //reference
        //http://csharp.net-tutorials.com/xml/writing-xml-with-the-xmldocument-class/

        private static string GetUserDataPath()
        {
            string userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".SkylineUploader");
            if (!Directory.Exists(userDataPath))
            {
                try
                {
                    Directory.CreateDirectory(userDataPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteEventLogError("Unable to create the directory '" + userDataPath + "': " + ex.Message);
                    Debug.ShowErrorMessage("Unable to create the directory '" + userDataPath + "': \n\n" + ex.Message);
                    Environment.Exit(0);
                    throw;
                }
                
            }
            return userDataPath;
        }
        
        public static string GetLogDir()
        {
            string settingsDir = Path.GetDirectoryName(Global.GetSettingsPath());
            string logDir = Path.Combine(settingsDir + @"\Log");

            if (!Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (Exception ex)
                {
                    Debug.WriteEventLogError("Unable to create the directory '" + logDir + "': " + ex.Message);
                    Debug.ShowErrorMessage("Unable to create the directory '" + logDir + "': " + ex.Message);
                    logDir = null;
                }
            }
            return logDir;
        }


    }
}
