using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylineUploader.Classes
{
    public class Global
    {
        public static string SettingsPath = @"C:\Skyline\SkylineUploader\Settings.xml";
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
    }
}
