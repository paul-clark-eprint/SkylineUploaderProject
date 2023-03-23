using System;
using System.Configuration;

namespace SkylineUploader.Classes
{
    public class SqlHelper
    {
        public string ServerInstance { get; set; }

        public static string GetConnectionString(string contextName)
        {
            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //ConnectionStringsSection section = (ConnectionStringsSection)config.GetSection("connectionStrings");
            //var connectionString = section.ConnectionStrings[contextName].ConnectionString;
            
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings[contextName].ConnectionString;
                return connectionString;
            }
            catch (Exception e)
            {
                string errorMessage = "*error* " + e.Message;
                return errorMessage;
            }
            
        }

        public static string ModifyConnectionString(string contextName, string connectionString)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection section = (ConnectionStringsSection)config.GetSection("connectionStrings");
                section.ConnectionStrings[contextName].ConnectionString = connectionString;
                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception)
            {
                return "Unable to save the database configuration. Please run the application for the first time as Administrator";
            }

            return string.Empty;
        }
    }

    
}
