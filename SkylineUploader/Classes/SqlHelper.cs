using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkylineUploader.Classes
{
    public class SqlServerInstance
    {
        public string ServerInstance { get; set; }
        public string Version { get; set; }
        public string TableName { get; set; }

        public static string GetConnectionString()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = (ConnectionStringsSection)config.GetSection("connectionStrings");
            var connectionString = section.ConnectionStrings[0].ConnectionString;

            return connectionString;
        }

        public static void ModifyConnectionString(string name, string connectionString)
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
                MessageBox.Show("Unable to save the database configuration. Please run the appication as Administrator",
                    "Run As Administrator required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

        }
    }

    
}
