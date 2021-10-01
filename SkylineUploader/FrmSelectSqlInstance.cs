using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkylineUploader.Classes;
using Telerik.WinControls.UI;

namespace SkylineUploader
{
    public partial class FrmSelectSqlInstance : Form
    {
        public List<SqlServerInstance> sqlInstances;
        public string SelectedInstance = string.Empty;
        public bool WindowsAuthentication = true;
        public string SqlUsername = string.Empty;
        public string SqlPassword = string.Empty;
        public bool DataSourceSet = false;

        public FrmSelectSqlInstance()
        {
            InitializeComponent();
        }

        private void FrmSelectSqlInstance_Load(object sender, EventArgs e)
        {
            RadListDataItem dataItem;
            foreach (var sqlInstance in sqlInstances)
            {
                dataItem = new RadListDataItem();
                dataItem.Text = sqlInstance.ServerInstance;
                this.uxDropDownList.Items.Add(dataItem);
            }

            if (!string.IsNullOrEmpty(SelectedInstance))
            {
                RadListDataItem row =  uxDropDownList.FindItemExact(SelectedInstance, false);
                if (row != null)
                {
                    var index = row.Index;
                    uxDropDownList.SelectedItem = row;
                }
                
            }
        }

        private void uxButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void uxButtonSelect_Click(object sender, EventArgs e)
        {
            if (uxDropDownList.SelectedItem == null)
            {
                MessageBox.Show("Please select a SQL Instance from the dropdown list");
                return;
            }
            string selectedInstance = uxDropDownList.SelectedItem.Text;
            if (string.IsNullOrEmpty(selectedInstance))
            {
                MessageBox.Show("Please select a SQL Instance from the dropdown list");
                return;
            }

            if (!WindowsAuthentication)
            {
                if (string.IsNullOrEmpty(uxTextBoxUsername.Text))
                {
                    MessageBox.Show("Please enter a SQL Username", "Username missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(uxLabelPassword.Text))
                {
                    MessageBox.Show("Please enter a SQL Password", "Password missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            SelectedInstance = selectedInstance;
            if (!WindowsAuthentication)
            {
                SqlUsername = uxTextBoxUsername.Text;
                SqlPassword = uxTextBoxPassword.Text;
            }


            SqlConnectionStringBuilder sqlConBuilder;

            if (WindowsAuthentication)
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "Master",
                    DataSource = SelectedInstance,
                    IntegratedSecurity = true
                };
            }
            else
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "Master",
                    DataSource = SelectedInstance,
                    UserID = SqlUsername,
                    Password = SqlPassword
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
                MessageBox.Show("Unable to connect to " + SelectedInstance + "\n\n" + ex.Message,
                   "SQl Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (WindowsAuthentication)
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "SkylineUploader",
                    DataSource = SelectedInstance,
                    IntegratedSecurity = true
                };
            }
            else
            {
                sqlConBuilder = new SqlConnectionStringBuilder()
                {
                    InitialCatalog = "SkylineUploader",
                    DataSource = SelectedInstance,
                    UserID = SqlUsername,
                    Password = SqlPassword
                };
            }

            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT IS_SRVROLEMEMBER ( 'dbcreator' )";

                    int result = ((int)cmd.ExecuteScalar());
                    connection.Close();
                    if (result != 1)
                    {
                        MessageBox.Show("The selected user does not have sufficient rights to create a database", "Insufficient User Rights",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Debug.Error("The selected user does not have sufficient rights to create a database");

                        return;
                    }
                }

                
            }

            catch (SqlException sqlExc)
            {
                var error = sqlExc.Message;
                var inner = sqlExc.InnerException;
                var errorNumber = sqlExc.Number;

                MessageBox.Show("Error creating the database. \n\n" + sqlExc.Message, "Error creating database",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Debug.Error("Error creating the database.", sqlExc);

                return;
            }
            catch (Exception ex)
            {

                string message = ex.Message;
            }

            SqlServerInstance.ModifyConnectionString("UploaderDbContext", sqlConBuilder.ConnectionString);
            DataSourceSet = true;

            Close();
        }

        

        private void radDropDownList1_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            var sqlAuthentication = radDropDownList1.SelectedIndex == 1;
            uxLabelUsername.Enabled = sqlAuthentication;
            uxLabelPassword.Enabled = sqlAuthentication;
            uxLabelInfo.Enabled = sqlAuthentication;
            uxTextBoxUsername.Enabled = sqlAuthentication;
            uxTextBoxPassword.Enabled = sqlAuthentication;

            WindowsAuthentication = !sqlAuthentication;

            if (radDropDownList1.SelectedIndex == 0)
            {
                uxTextBoxUsername.Text = string.Empty;
                uxTextBoxPassword.Text = string.Empty;
            }
        }
    }
}
