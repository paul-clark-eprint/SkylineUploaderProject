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
    }
}