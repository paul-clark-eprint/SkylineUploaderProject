using System;
using System.Configuration;

namespace HelperClasses
{
    public class SqlServerInstance
    {
        public string ServerInstance { get; set; }
        public string Version { get; set; }
        public string TableName { get; set; }

        public static string GetConnectionString()
        {
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var section = (ConnectionStringsSection)config.GetSection("connectionStrings");
            //var connectionString = section.ConnectionStrings[0].ConnectionString;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UploaderDbContext"].ConnectionString;

            return connectionString;
        }

        public static string ModifyConnectionString(string name, string connectionString)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var section = (ConnectionStringsSection)config.GetSection("connectionStrings");
                section.ConnectionStrings[name].ConnectionString = connectionString;
                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception)
            {
                return "Unable to save the database configuration. Please run the application as Administrator";
            }

            return string.Empty;
        }
    }

    
}
