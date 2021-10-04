using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace HelperClasses
{
    public class RegistryHelper
    {
        public static string CreateRegistryKeys()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE",true);
                key.CreateSubKey("SkylineUploader");
                key = key.OpenSubKey("SkylineUploader", true);
                key.Close();
                return string.Empty;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string SaveRegistryKey(string valueName, string valueData)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software",true);
                key = key.OpenSubKey("SkylineUploader", true);
                key.SetValue(valueName, valueData,RegistryValueKind.String);
                key.Close();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string ReadRegistryKey(string valueName)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software",true);
                key = key.OpenSubKey("SkylineUploader", true);
                if (key == null)
                {
                    return "*error* Unable to find the Value Name " + valueName + " in the registry HKEY_LOCAL_MACHINE\\SOFTWARE\\SkylineUploader";
                }

                return key.GetValue(valueName).ToString();
            }
            catch (Exception e)
            {
                return "*error* Unexpected error trying the read the Value Name " + valueName + " in the registry HKEY_LOCAL_MACHINE\\SOFTWARE\\SkylineUploader";
                
            }
        }
    }
}
