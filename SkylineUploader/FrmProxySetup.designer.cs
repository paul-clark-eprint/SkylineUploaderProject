namespace SkylineUploader
{
    partial class FrmProxySetup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UxButtonCancel = new Telerik.WinControls.UI.RadButton();
            this.UxGroupBoxDetails = new Telerik.WinControls.UI.RadGroupBox();
            this.LabelDomain = new Telerik.WinControls.UI.RadLabel();
            this.CheckBoxUseProxy = new Telerik.WinControls.UI.RadCheckBox();
            this.TextBoxDomain = new Telerik.WinControls.UI.RadTextBox();
            this.TextBoxPassword = new Telerik.WinControls.UI.RadTextBox();
            this.TextBoxUsername = new Telerik.WinControls.UI.RadTextBox();
            this.LabelPassword = new Telerik.WinControls.UI.RadLabel();
            this.SpinPort = new Telerik.WinControls.UI.RadSpinEditor();
            this.LabelUsername = new Telerik.WinControls.UI.RadLabel();
            this.LabelProxyPort = new Telerik.WinControls.UI.RadLabel();
            this.TextBoxProxyAddress = new Telerik.WinControls.UI.RadTextBox();
            this.LabelProxyAddress = new Telerik.WinControls.UI.RadLabel();
            this.UxButtonSave = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.UxButtonCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UxGroupBoxDetails)).BeginInit();
            this.UxGroupBoxDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LabelDomain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckBoxUseProxy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxDomain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpinPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelProxyPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxProxyAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelProxyAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UxButtonSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // UxButtonCancel
            // 
            this.UxButtonCancel.Location = new System.Drawing.Point(384, 172);
            this.UxButtonCancel.Name = "UxButtonCancel";
            this.UxButtonCancel.Size = new System.Drawing.Size(69, 24);
            this.UxButtonCancel.TabIndex = 0;
            this.UxButtonCancel.Text = "Cancel";
            this.UxButtonCancel.Click += new System.EventHandler(this.UxButtonCancel_Click);
            // 
            // UxGroupBoxDetails
            // 
            this.UxGroupBoxDetails.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.UxGroupBoxDetails.Controls.Add(this.LabelDomain);
            this.UxGroupBoxDetails.Controls.Add(this.CheckBoxUseProxy);
            this.UxGroupBoxDetails.Controls.Add(this.TextBoxDomain);
            this.UxGroupBoxDetails.Controls.Add(this.TextBoxPassword);
            this.UxGroupBoxDetails.Controls.Add(this.TextBoxUsername);
            this.UxGroupBoxDetails.Controls.Add(this.LabelPassword);
            this.UxGroupBoxDetails.Controls.Add(this.SpinPort);
            this.UxGroupBoxDetails.Controls.Add(this.LabelUsername);
            this.UxGroupBoxDetails.Controls.Add(this.LabelProxyPort);
            this.UxGroupBoxDetails.Controls.Add(this.TextBoxProxyAddress);
            this.UxGroupBoxDetails.Controls.Add(this.LabelProxyAddress);
            this.UxGroupBoxDetails.HeaderText = "Proxy details";
            this.UxGroupBoxDetails.Location = new System.Drawing.Point(12, 12);
            this.UxGroupBoxDetails.Name = "UxGroupBoxDetails";
            this.UxGroupBoxDetails.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);
            this.UxGroupBoxDetails.Size = new System.Drawing.Size(445, 151);
            this.UxGroupBoxDetails.TabIndex = 8;
            this.UxGroupBoxDetails.Text = "Proxy details";
            // 
            // LabelDomain
            // 
            this.LabelDomain.Location = new System.Drawing.Point(235, 94);
            this.LabelDomain.Name = "LabelDomain";
            this.LabelDomain.Size = new System.Drawing.Size(99, 18);
            this.LabelDomain.TabIndex = 66;
            this.LabelDomain.Text = "Domain (Optional)";
            // 
            // CheckBoxUseProxy
            // 
            this.CheckBoxUseProxy.Location = new System.Drawing.Point(13, 22);
            this.CheckBoxUseProxy.Name = "CheckBoxUseProxy";
            this.CheckBoxUseProxy.Size = new System.Drawing.Size(104, 18);
            this.CheckBoxUseProxy.TabIndex = 65;
            this.CheckBoxUseProxy.Text = "Use Proxy Server";
            // 
            // TextBoxDomain
            // 
            this.TextBoxDomain.Location = new System.Drawing.Point(235, 116);
            this.TextBoxDomain.Name = "TextBoxDomain";
            this.TextBoxDomain.Size = new System.Drawing.Size(136, 20);
            this.TextBoxDomain.TabIndex = 62;
            this.TextBoxDomain.TabStop = false;
            // 
            // TextBoxPassword
            // 
            this.TextBoxPassword.Location = new System.Drawing.Point(127, 116);
            this.TextBoxPassword.Name = "TextBoxPassword";
            this.TextBoxPassword.PasswordChar = '*';
            this.TextBoxPassword.Size = new System.Drawing.Size(101, 20);
            this.TextBoxPassword.TabIndex = 63;
            this.TextBoxPassword.TabStop = false;
            // 
            // TextBoxUsername
            // 
            this.TextBoxUsername.Location = new System.Drawing.Point(13, 116);
            this.TextBoxUsername.Name = "TextBoxUsername";
            this.TextBoxUsername.Size = new System.Drawing.Size(110, 20);
            this.TextBoxUsername.TabIndex = 60;
            this.TextBoxUsername.TabStop = false;
            // 
            // LabelPassword
            // 
            this.LabelPassword.Location = new System.Drawing.Point(127, 94);
            this.LabelPassword.Name = "LabelPassword";
            this.LabelPassword.Size = new System.Drawing.Size(53, 18);
            this.LabelPassword.TabIndex = 61;
            this.LabelPassword.Text = "Password";
            // 
            // SpinPort
            // 
            this.SpinPort.Location = new System.Drawing.Point(254, 67);
            this.SpinPort.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.SpinPort.Name = "SpinPort";
            // 
            // 
            // 
            this.SpinPort.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.SpinPort.Size = new System.Drawing.Size(85, 20);
            this.SpinPort.TabIndex = 59;
            this.SpinPort.TabStop = false;
            // 
            // LabelUsername
            // 
            this.LabelUsername.Location = new System.Drawing.Point(13, 94);
            this.LabelUsername.Name = "LabelUsername";
            this.LabelUsername.Size = new System.Drawing.Size(56, 18);
            this.LabelUsername.TabIndex = 57;
            this.LabelUsername.Text = "Username";
            // 
            // LabelProxyPort
            // 
            this.LabelProxyPort.Location = new System.Drawing.Point(254, 46);
            this.LabelProxyPort.Name = "LabelProxyPort";
            this.LabelProxyPort.Size = new System.Drawing.Size(58, 18);
            this.LabelProxyPort.TabIndex = 58;
            this.LabelProxyPort.Text = "Proxy port";
            // 
            // TextBoxProxyAddress
            // 
            this.TextBoxProxyAddress.Location = new System.Drawing.Point(13, 68);
            this.TextBoxProxyAddress.Name = "TextBoxProxyAddress";
            this.TextBoxProxyAddress.Size = new System.Drawing.Size(231, 20);
            this.TextBoxProxyAddress.TabIndex = 56;
            this.TextBoxProxyAddress.TabStop = false;
            // 
            // LabelProxyAddress
            // 
            this.LabelProxyAddress.Location = new System.Drawing.Point(13, 46);
            this.LabelProxyAddress.Name = "LabelProxyAddress";
            this.LabelProxyAddress.Size = new System.Drawing.Size(75, 18);
            this.LabelProxyAddress.TabIndex = 55;
            this.LabelProxyAddress.Text = "Proxy address";
            // 
            // UxButtonSave
            // 
            this.UxButtonSave.Location = new System.Drawing.Point(309, 172);
            this.UxButtonSave.Name = "UxButtonSave";
            this.UxButtonSave.Size = new System.Drawing.Size(69, 24);
            this.UxButtonSave.TabIndex = 9;
            this.UxButtonSave.Text = "Save";
            this.UxButtonSave.Click += new System.EventHandler(this.UxButtonSave_Click);
            // 
            // FrmProxySetup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(465, 208);
            this.Controls.Add(this.UxButtonSave);
            this.Controls.Add(this.UxGroupBoxDetails);
            this.Controls.Add(this.UxButtonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmProxySetup";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Proxy Setup";
            ((System.ComponentModel.ISupportInitialize)(this.UxButtonCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UxGroupBoxDetails)).EndInit();
            this.UxGroupBoxDetails.ResumeLayout(false);
            this.UxGroupBoxDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LabelDomain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckBoxUseProxy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxDomain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpinPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelProxyPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxProxyAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LabelProxyAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UxButtonSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadButton UxButtonCancel;
        private Telerik.WinControls.UI.RadGroupBox UxGroupBoxDetails;
        private Telerik.WinControls.UI.RadLabel LabelDomain;
        private Telerik.WinControls.UI.RadCheckBox CheckBoxUseProxy;
        private Telerik.WinControls.UI.RadTextBox TextBoxDomain;
        private Telerik.WinControls.UI.RadTextBox TextBoxPassword;
        private Telerik.WinControls.UI.RadTextBox TextBoxUsername;
        private Telerik.WinControls.UI.RadLabel LabelPassword;
        private Telerik.WinControls.UI.RadSpinEditor SpinPort;
        private Telerik.WinControls.UI.RadLabel LabelUsername;
        private Telerik.WinControls.UI.RadLabel LabelProxyPort;
        private Telerik.WinControls.UI.RadTextBox TextBoxProxyAddress;
        private Telerik.WinControls.UI.RadLabel LabelProxyAddress;
        private Telerik.WinControls.UI.RadButton UxButtonSave;
    }
}
