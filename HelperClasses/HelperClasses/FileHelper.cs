using System;
using System.Diagnostics;
using System.IO;

namespace HelperClasses
{
    public class FileHelper
    {
        public static bool CreateProgramDataFolder()
        {
            string progranDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var dataPath = Path.Combine(progranDataPath, "SkylineUploader");
            if (!Directory.Exists(dataPath))
            {
                try
                {
                    Directory.CreateDirectory(dataPath);
                }
                catch (Exception )
                {
                    return false;
                }
            }

            return Directory.Exists(dataPath);
        }

        public static bool DeleteProgramDataFolder()
        {
            string progranDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var dataPath = Path.Combine(progranDataPath, "SkylineUploader");

            if (!Directory.Exists(dataPath))
            {
                return true;
            }

            try
            {
                var settingsFile = Path.Combine(dataPath, "Settings.xml");
                if (File.Exists(settingsFile))
                {
                    File.Delete(settingsFile);
                }
                Directory.Delete(dataPath,true);
            }
            catch (Exception )
            {
                return false;
            }

            return Directory.Exists(dataPath);
        }
    }
}