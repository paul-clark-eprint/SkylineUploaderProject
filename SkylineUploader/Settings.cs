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
                    Debug.WriteEventLog("Unable to create the directory '" + userDataPath + "': " + ex.Message);
                    Debug.ShowErrorMessage("Unable to create the directory '" + userDataPath + "': \n\n" + ex.Message);
                    Environment.Exit(0);
                    throw;
                }
                
            }
            return userDataPath;
        }

        private static string _progDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ePrint Direct\Skyline\Driver");

        private static string ProgDataPath
        {
            get { return _progDataPath; }
            set { _progDataPath = value; }
        }

        

        /// <summary>
        /// Returns the debug directory C:\Users\username\.SkylineDriver\Log
        /// or exits the application if it does not exist and cannot be created
        /// </summary>
        /// <returns></returns>
        public static string GetLogDir()
        {
            string logDir = Path.Combine(GetUserDataPath() + @"\Log");

            if (!Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (Exception ex)
                {
                    Debug.WriteEventLog("Unable to create the directory '" + logDir + "': " + ex.Message);
                    Debug.ShowErrorMessage("Unable to create the directory '" + logDir + "': " + ex.Message);
                    logDir = null;
                }
            }
            return logDir;
        }

        

        

        

        

        
        
        
    }
}
