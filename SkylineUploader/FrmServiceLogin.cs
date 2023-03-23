using HelperClasses;
using SkylineUploader.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkylineUploaderDomain.DataModel.Classes;

namespace SkylineUploader
{
    public partial class FrmServiceLogin : Form
    {
        public FrmServiceLogin()
        {
            InitializeComponent();
            GetConnectionString();
        }

        private void GetConnectionString()
        {
            string connectionString = SqlHelper.GetConnectionString("UploaderDbContext");
            if (connectionString.Contains("*error*"))
            {
                Debug.WriteEventLogError("ConnectionString missing from app.config. Closing application. " + connectionString);
                MessageBox.Show("ConnectionString missing from app.config. Closing application\n\n" + connectionString, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FileHelper.DeleteProgramDataFolder();
                Environment.Exit(0);
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            var dataSource = builder["Data Source"].ToString();

            try
            {
                string userId = builder["User ID"].ToString();
                if (!string.IsNullOrEmpty(userId))
                {
                    uxTextBoxUsername.Text = userId;
                }

                string password = builder["Password"].ToString();
                if (!string.IsNullOrEmpty(password))
                {
                    uxTextBoxPassword.Text = password;
                }

                string integratedSecurity = builder["Integrated Security"].ToString();
                if (!string.IsNullOrEmpty(integratedSecurity))
                {
                    uxCheckBoxWindowsAuthentication.Checked = integratedSecurity.ToLower() == "true";
                }

            }
            catch (Exception)
            {
                
            }

            uxTextBoxServerName.Text = dataSource;
        }

        private void uxCheckBoxWindowsAuthentication_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if(uxCheckBoxWindowsAuthentication.Checked)
            {
                uxTextBoxUsername.Text = null;
                uxTextBoxUsername.Enabled = false;

                uxTextBoxPassword.Text = null;
                uxTextBoxPassword.Enabled = false;
            }
            else
            {
                uxTextBoxUsername.Enabled = true;
                uxTextBoxPassword.Enabled = true;
            }
        }

        private void uxButtonReset_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Do you want reset the connection to the database?", "Reset Database Connection", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (res == DialogResult.OK)
            {
                //reset the app.config ConnectionString back to blank Data Source
                SqlConnectionStringBuilder sqlConBuilder = new SqlConnectionStringBuilder();
                sqlConBuilder.ConnectionString = "Data Source=NotSet;Initial Catalog=SkylineUploader;Integrated Security=SSPI;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                string errorMessage = SqlHelper.ModifyConnectionString("UploaderDbContext", sqlConBuilder.ConnectionString);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Debug.Error("FrmServiceLogin","uxButtonReset_Click", "Error resetting the ConnectionString. Error: " + errorMessage);
                    MessageBox.Show("Error resetting the ConnectionString. Error: \n\n" + errorMessage, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                var deletedOk = SettingsHelper.DeleteSettingsFile();
                if (deletedOk)
                {
                    MessageBox.Show("Database connection reset. Please restart the Skyline Uploader App",
                        "Database Connection Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to delete the configuration file " + Global.SettingsPath +
                        "\n\nTry to delete it manually", "Error deleting the configuration file", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void uxButtonSave_Click(object sender, EventArgs e)
        {
            bool WindowsAuthentication = uxCheckBoxWindowsAuthentication.Checked;
            string selectedInstance = uxTextBoxServerName.Text;
            string sqlUsername = string.Empty;
            string sqlPassword = string.Empty;
            if (!WindowsAuthentication)
            {
                sqlUsername = uxTextBoxUsername.Text;
                sqlPassword = uxTextBoxPassword.Text;
            }

            SqlConnectionStringBuilder sqlConBuilder;

            if (WindowsAuthentication)
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "SkylineUploader",
                    DataSource = selectedInstance,
                    IntegratedSecurity = true
                };
            }
            else
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "SkylineUploader",
                    DataSource = selectedInstance,
                    UserID = sqlUsername,
                    Password = sqlPassword
                };
            }

            string connectionString = sqlConBuilder.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to " + selectedInstance + "\n\n" + ex.Message,
                    "SQL Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (WindowsAuthentication)
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "SkylineUploader",
                    DataSource = selectedInstance,
                    IntegratedSecurity = true,
                    ConnectTimeout = 30,
                    Encrypt = false,
                    TrustServerCertificate = false,
                    ApplicationIntent = ApplicationIntent.ReadWrite,
                    MultiSubnetFailover = false
                };
            }
            else
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "SkylineUploader",
                    DataSource = selectedInstance,
                    UserID = sqlUsername,
                    Password = sqlPassword,
                    ConnectTimeout = 30,
                    Encrypt = false,
                    TrustServerCertificate = false,
                    ApplicationIntent = ApplicationIntent.ReadWrite,
                    MultiSubnetFailover = false
                };
            }

            bool dataSourceSet;

            string hidePassword = sqlConBuilder.ConnectionString.Replace(sqlPassword, "*******");

            Debug.Log("FrmServiceLogin","uxButtonSave_Click", "Setting the connectionString to: '" + hidePassword + "'");
            string errorMessage = SqlHelper.ModifyConnectionString("UploaderDbContext", sqlConBuilder.ConnectionString);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Debug.Error("FrmServiceLogin","uxButtonSave_Click", errorMessage);

                MessageBox.Show(errorMessage, "Run As Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataSourceSet = false;
            }
            else
            {
                try
                {
                    Debug.Log("FrmServiceLogin","uxButtonSave_Click", "ConnectionString string saved to app.config");

                    if (!SettingsHelper.CreateSettingsFile())
                    {
                        Debug.Error("FrmServiceLogin","uxButtonSave_Click", "Unable to create the settings file");
                        dataSourceSet = false;
                        return;
                    }
                    Debug.Log("FrmServiceLogin","uxButtonSave_Click", "Settings file found. Saving ConnectionString");

                    dataSourceSet = SettingsHelper.SaveConnectionString(sqlConBuilder.ConnectionString);
                    
                }
                catch (Exception ex)
                {
                    Debug.Error("FrmServiceLogin","uxButtonSave_Click", ex.Message, ex);
                    MessageBox.Show("Unexptected error saving the ConnectionString setting\n\n" + ex.Message,
                        "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dataSourceSet = false;
                }
            }

            if (dataSourceSet)
            {
                MessageBox.Show("Database Login changed. Please restart the application", "Restart Application",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();

            }
        }

        private void uxButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }


}
