using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using SkylineUploaderDomain.DataModel;
using SkylineUploaderDomain.DataModel.Classes;

namespace SkylineUploader.Classes
{
    public class SettingsHelper
    {
        private const int Keysize = 256;
        private const int DerivationIterations = 1000;
        private const string key = "SkYlInE";

        /// <summary>
        /// Check if settings file exists and create if not
        /// </summary>
        /// <returns>bool</returns>
        public static bool CheckIfSettingsFileExists()
        {
            if (!Directory.Exists(@"C:\Skyline\SkylineUploader"))
            {
                try
                {
                    Directory.CreateDirectory(@"C:\Skyline\SkylineUploader");
                }
                catch (Exception)
                {
                    MessageBox.Show("SkylineUploader cannot create the local working directory C:\\Skyline\\SkylineUploader", "Error creating folder", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                    return false;
                }
            }

            if (!File.Exists(Global.SettingsPath))
            {
                return CreateBlankSettingsFile();
            }

            if (!CheckUseHttpsExists())
            {
                DeleteSettingsFile();
                return CreateBlankSettingsFile();
            }

            return true;
        }
        /// <summary>
        /// Get a blank settings file
        /// </summary>
        /// <returns>bool</returns>
        public static bool CreateBlankSettingsFile()
        {
            XDocument doc = new XDocument(
                new XElement("SkylineUploader",
                    new XElement("Username", string.Empty),
                    new XElement("Password", string.Empty),
                    new XElement("PortalUrl", string.Empty),
                    new XElement("UseHttps", string.Empty),
                    new XElement("RememberMe", false),
                    new XElement("UseProxy", false),
                    new XElement("ProxyAddress", string.Empty),
                    new XElement("ProxyPort", string.Empty),
                    new XElement("ProxyUsername", string.Empty),
                    new XElement("ProxyPassword", string.Empty),
                    new XElement("ProxyDomain", string.Empty)
                    )
                );
            try
            {
                doc.Save(Global.SettingsPath);
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// Delete the current Settings file
        /// </summary>
        /// <returns>bool</returns>
        public static bool DeleteSettingsFile()
        {
            if (File.Exists(Global.SettingsPath))
            {
                try
                {
                    File.Delete(Global.SettingsPath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Updates the login details in the settings file
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <param name="portalUrl">Portal URL</param>
        /// <param name="useHttps">Use HTTPS</param>
        /// <param name="rememberMe">Remember details</param>
        /// <returns>bool</returns>
        public static bool UpdateLoginDetails(string userName, string password, string portalUrl, bool rememberMe)
        {
            if (File.Exists(Global.SettingsPath))
            {
                XDocument doc = XDocument.Load(Global.SettingsPath);
                if (doc.Root != null)
                {
                    var xElement = doc.Root.Element("Username");
                    if (xElement != null)
                    {
                        xElement.Value = userName;
                    }
                    xElement = doc.Root.Element("Password");
                    if (xElement != null)
                    {
                        xElement.Value = Encrypt(password);
                    }
                    xElement = doc.Root.Element("PortalUrl");
                    if (xElement != null)
                    {
                        xElement.Value = portalUrl;
                    }
                    xElement = doc.Root.Element("RememberMe");
                    if (xElement != null)
                    {
                        xElement.Value = rememberMe.ToString();
                    }
                }
                try
                {
                    doc.Save(Global.SettingsPath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public static bool SaveLoginDetails(string userName, string password, string portalUrl, bool useHttps, bool rememberMe)
        {
            if (File.Exists(Global.SettingsPath))
            {
                XDocument doc = XDocument.Load(Global.SettingsPath);
                if (doc.Root != null)
                {
                    var xElement = doc.Root.Element("Username");
                    if (xElement != null)
                    {
                        xElement.Value = userName;
                    }
                    xElement = doc.Root.Element("Password");
                    if (xElement != null)
                    {
                        xElement.Value = Encrypt(password);
                    }
                    xElement = doc.Root.Element("PortalUrl");
                    if (xElement != null)
                    {
                        xElement.Value = portalUrl;
                    }
                    xElement = doc.Root.Element("UseHttps");
                    if (xElement != null)
                    {
                        xElement.Value = useHttps.ToString();
                    }
                    xElement = doc.Root.Element("RememberMe");
                    if (xElement != null)
                    {
                        xElement.Value = rememberMe.ToString();
                    }
                }
                try
                {
                    doc.Save(Global.SettingsPath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Update the saved details for the Proxy
        /// </summary>
        /// <param name="proxy">Proxy details</param>
        /// <returns>bool</returns>
        public static bool UpdateProxyDetails(Proxy proxy)
        {
            SettingsHelper.CheckIfSettingsFileExists();

                
            if (File.Exists(Global.SettingsPath))
            {
                XDocument doc = XDocument.Load(Global.SettingsPath);
                if (doc.Root != null)
                {
                    var xElement = doc.Root.Element("UseProxy");
                    if (xElement != null)
                    {
                        xElement.Value = proxy.UseProxy.ToString();
                    }
                    xElement = doc.Root.Element("ProxyAddress");
                    if (xElement != null)
                    {
                        xElement.Value = proxy.ProxyAddress;
                    }
                    xElement = doc.Root.Element("ProxyPort");
                    if (xElement != null)
                    {
                        xElement.Value = proxy.ProxyPort.ToString();
                    }
                    xElement = doc.Root.Element("ProxyUsername");
                    if (xElement != null)
                    {
                        xElement.Value = proxy.ProxyUsername;
                    }
                    xElement = doc.Root.Element("ProxyPassword");
                    if (xElement != null)
                    {
                        xElement.Value = Encrypt(proxy.ProxyPassword);
                    }
                    xElement = doc.Root.Element("ProxyDomain");
                    if (xElement != null)
                    {
                        xElement.Value = proxy.ProxyDomain;
                    }
                }
                try
                {
                    doc.Save(Global.SettingsPath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="plainText">String to encrypt</param>
        /// <returns>string</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="cipherText">Encrypted string</param>
        /// <returns>string</returns>
        public static string Decrypt(string cipherText)
        {
            if (cipherText.Length < 8) return string.Empty;
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        /// <summary>
        /// Gets login details from settings file
        /// </summary>
        /// <returns>Login</returns>
        public static Login GetSavedLogin()
        {
            Login login = new Login();

            if (File.Exists(Global.SettingsPath))
            {
                XDocument doc = XDocument.Load(Global.SettingsPath);
                if (doc.Root != null)
                {
                    var xElement = doc.Root.Element("Username");
                    if (xElement != null)
                    {
                        login.Username = xElement.Value;
                    }
                    xElement = doc.Root.Element("Password");
                    if (xElement != null)
                    {
                        login.Password = Decrypt(xElement.Value);
                    }
                    xElement = doc.Root.Element("PortalUrl");
                    if (xElement != null)
                    {
                        login.PortalUrl = xElement.Value;
                    }
                }
            }
            return login;
        }
        /// <summary>
        /// Gets the saved Proxy details
        /// </summary>
        /// <returns>Proxy</returns>
        public static Proxy GetProxyDetails()
        {
            Proxy proxy = new Proxy();

            if (File.Exists(Global.SettingsPath))
            {
                XDocument doc = XDocument.Load(Global.SettingsPath);
                if (doc.Root != null)
                {
                    var xElement = doc.Root.Element("UseProxy");
                    if (xElement != null)
                    {
                        bool useProxy = false;
                        bool.TryParse(xElement.Value, out useProxy);
                        proxy.UseProxy = useProxy;
                    }
                    xElement = doc.Root.Element("ProxyAddress");
                    if (xElement != null)
                    {
                        proxy.ProxyAddress = xElement.Value;
                    }
                    xElement = doc.Root.Element("ProxyPort");
                    if (xElement != null)
                    {
                        int proxyPort = 0;
                        int.TryParse(xElement.Value, out proxyPort);
                        proxy.ProxyPort = proxyPort;
                    }
                    xElement = doc.Root.Element("ProxyUsername");
                    if (xElement != null)
                    {
                        proxy.ProxyUsername = xElement.Value;
                    }
                    xElement = doc.Root.Element("ProxyPassword");
                    if (xElement != null)
                    {
                        proxy.ProxyPassword = Decrypt(xElement.Value);
                    }
                    xElement = doc.Root.Element("ProxyDomain");
                    if (xElement != null)
                    {
                        proxy.ProxyDomain = xElement.Value;
                    }
                }
            }
            return proxy;
        }

        private static bool CheckUseHttpsExists()
        {
            if (File.Exists(Global.SettingsPath))
            {
                XDocument doc = XDocument.Load(Global.SettingsPath);
                if (doc.Root != null)
                {
                    var xElement = doc.Root.Element("UseHttps");
                    return xElement != null;
                }
            }

            return false;
        }
    }
}
