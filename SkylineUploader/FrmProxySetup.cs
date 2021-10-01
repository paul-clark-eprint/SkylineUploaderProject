using System;
using System.Linq;
using SkylineUploader.Classes;
using SkylineUploaderDomain.DataModel;
using SkylineUploaderDomain.DataModel.Classes;
using Telerik.WinControls;

namespace SkylineUploader
{
    public partial class FrmProxySetup : Telerik.WinControls.UI.RadForm
    {

        public FrmProxySetup()
        {
            InitializeComponent();
            LoadProxyDetails();
            CreateUi();
        }

        void CreateUi()
        {
            Text = "Proxy Setup";
            CheckBoxUseProxy.Text = "Use Proxy Server";
            LabelProxyAddress.Text = "Proxy Address";
            LabelProxyPort.Text = "Proxy Port";
            LabelUsername.Text = "Username";
            LabelPassword.Text = "Password";
            LabelDomain.Text = "Domain (Optional)";
            UxButtonSave.Text = "Save;";
            UxButtonCancel.Text = "Cancel";
            UxGroupBoxDetails.Text = "Proxy Details";
        }
        void LoadProxyDetails()
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                Proxy proxy = (from p in context.Proxy select p).FirstOrDefault();
                if (proxy != null)
                {
                    CheckBoxUseProxy.Checked = proxy.UseProxy; 
                    TextBoxProxyAddress.Text = proxy.ProxyAddress;
                    SpinPort.Value = proxy.ProxyPort;
                    TextBoxUsername.Text = proxy.ProxyUsername;
                    TextBoxPassword.Text = proxy.ProxyPassword;
                    TextBoxDomain.Text = proxy.ProxyDomain;
                }
            }
        }
        private void UxButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UxButtonSave_Click(object sender, EventArgs e)
        {
            using (UploaderDbContext context = new UploaderDbContext())
            {
                Proxy proxy = (from p in context.Proxy select p).FirstOrDefault();
                if (proxy == null)
                {
                    proxy = new Proxy();
                    context.Proxy.Add(proxy);
                }
                proxy.UseProxy = CheckBoxUseProxy.Checked;
                proxy.ProxyAddress = TextBoxProxyAddress.Text;
                proxy.ProxyPort = (int)SpinPort.Value;
                proxy.ProxyUsername = TextBoxUsername.Text;
                proxy.ProxyPassword = TextBoxPassword.Text;
                proxy.ProxyDomain = TextBoxDomain.Text;

                int saved = -1;
                saved = context.SaveChanges();
                if (saved == -1)
                {
                    RadMessageBox.Show("Error updating settings");
                }
                else
                {
                    Close();
                }
            }
        }
    }
}
