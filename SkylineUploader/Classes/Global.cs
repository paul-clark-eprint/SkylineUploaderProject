using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkylineUploader.Classes
{
    public class Global
    {
        public static string SettingsPath = GetSettingsPath();
        public static string PreferencesPath;
        public static string PortalName;
        public static bool? UseHttps = null;
        public static bool Connected;
        public static string ErrorMessage = string.Empty;
        public static bool UseProxy;
        public static string ProxyServer;
        public static string ProxyDomain;
        public static string ProxyUsername;
        public static string ProxyPassword;
        public static int ProxyPort;

        public static string DocumentOptionTitle;

        private static string GetSettingsPath()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var programDataSkylineUploader = Path.Combine(programData, "SkylineUploader");
            if (!Directory.Exists(programDataSkylineUploader))
            {
                try
                {
                    Directory.CreateDirectory(programDataSkylineUploader);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "SkylineUploader cannot create the local working directory" + programDataSkylineUploader,
                        "Error creating folder", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return "*error*";
                }
            }

            return Path.Combine(programDataSkylineUploader, "Settings.xml");
        }
    }
}
