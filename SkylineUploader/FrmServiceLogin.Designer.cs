
namespace SkylineUploader
{
    partial class FrmServiceLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmServiceLogin));
            this.uxCheckBoxWindowsAuthentication = new Telerik.WinControls.UI.RadCheckBox();
            this.uxLabelTitle = new Telerik.WinControls.UI.RadLabel();
            this.uxButtonSave = new Telerik.WinControls.UI.RadButton();
            this.uxTextBoxUsername = new Telerik.WinControls.UI.RadTextBox();
            this.uxTextBoxPassword = new Telerik.WinControls.UI.RadTextBox();
            this.uxLabelUsername = new Telerik.WinControls.UI.RadLabel();
            this.uxLabelPassword = new Telerik.WinControls.UI.RadLabel();
            this.uxButtonReset = new Telerik.WinControls.UI.RadButton();
            this.uxLabelServerName = new Telerik.WinControls.UI.RadLabel();
            this.uxTextBoxServerName = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.uxCheckBoxWindowsAuthentication)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelServerName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxServerName)).BeginInit();
            this.SuspendLayout();
            // 
            // uxCheckBoxWindowsAuthentication
            // 
            this.uxCheckBoxWindowsAuthentication.Location = new System.Drawing.Point(12, 84);
            this.uxCheckBoxWindowsAuthentication.Name = "uxCheckBoxWindowsAuthentication";
            this.uxCheckBoxWindowsAuthentication.Size = new System.Drawing.Size(167, 18);
            this.uxCheckBoxWindowsAuthentication.TabIndex = 0;
            this.uxCheckBoxWindowsAuthentication.Text = "Use Windows Authentication";
            this.uxCheckBoxWindowsAuthentication.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.uxCheckBoxWindowsAuthentication_ToggleStateChanged);
            // 
            // uxLabelTitle
            // 
            this.uxLabelTitle.AutoSize = false;
            this.uxLabelTitle.Location = new System.Drawing.Point(12, 12);
            this.uxLabelTitle.Name = "uxLabelTitle";
            this.uxLabelTitle.Size = new System.Drawing.Size(310, 36);
            this.uxLabelTitle.TabIndex = 1;
            this.uxLabelTitle.Text = "Set the Database login for the Skyline Uploader App and Skyline Uploader Service " +
    "";
            // 
            // uxButtonSave
            // 
            this.uxButtonSave.Location = new System.Drawing.Point(236, 180);
            this.uxButtonSave.Name = "uxButtonSave";
            this.uxButtonSave.Size = new System.Drawing.Size(86, 24);
            this.uxButtonSave.TabIndex = 2;
            this.uxButtonSave.Text = "Save";
            this.uxButtonSave.Click += new System.EventHandler(this.uxButtonSave_Click);
            // 
            // uxTextBoxUsername
            // 
            this.uxTextBoxUsername.Location = new System.Drawing.Point(125, 108);
            this.uxTextBoxUsername.Name = "uxTextBoxUsername";
            this.uxTextBoxUsername.Size = new System.Drawing.Size(120, 24);
            this.uxTextBoxUsername.TabIndex = 3;
            // 
            // uxTextBoxPassword
            // 
            this.uxTextBoxPassword.Location = new System.Drawing.Point(125, 138);
            this.uxTextBoxPassword.Name = "uxTextBoxPassword";
            this.uxTextBoxPassword.PasswordChar = '*';
            this.uxTextBoxPassword.Size = new System.Drawing.Size(120, 24);
            this.uxTextBoxPassword.TabIndex = 4;
            // 
            // uxLabelUsername
            // 
            this.uxLabelUsername.Location = new System.Drawing.Point(35, 108);
            this.uxLabelUsername.Name = "uxLabelUsername";
            this.uxLabelUsername.Size = new System.Drawing.Size(56, 18);
            this.uxLabelUsername.TabIndex = 5;
            this.uxLabelUsername.Text = "Username";
            // 
            // uxLabelPassword
            // 
            this.uxLabelPassword.Location = new System.Drawing.Point(35, 139);
            this.uxLabelPassword.Name = "uxLabelPassword";
            this.uxLabelPassword.Size = new System.Drawing.Size(53, 18);
            this.uxLabelPassword.TabIndex = 6;
            this.uxLabelPassword.Text = "Password";
            // 
            // uxButtonReset
            // 
            this.uxButtonReset.AutoSize = true;
            this.uxButtonReset.Location = new System.Drawing.Point(12, 180);
            this.uxButtonReset.Name = "uxButtonReset";
            this.uxButtonReset.Size = new System.Drawing.Size(149, 24);
            this.uxButtonReset.TabIndex = 7;
            this.uxButtonReset.Text = "Reset Database Connection";
            this.uxButtonReset.Click += new System.EventHandler(this.uxButtonReset_Click);
            // 
            // uxLabelServerName
            // 
            this.uxLabelServerName.Location = new System.Drawing.Point(12, 55);
            this.uxLabelServerName.Name = "uxLabelServerName";
            this.uxLabelServerName.Size = new System.Drawing.Size(70, 18);
            this.uxLabelServerName.TabIndex = 8;
            this.uxLabelServerName.Text = "Server Name";
            // 
            // uxTextBoxServerName
            // 
            this.uxTextBoxServerName.Enabled = false;
            this.uxTextBoxServerName.Location = new System.Drawing.Point(125, 54);
            this.uxTextBoxServerName.Name = "uxTextBoxServerName";
            this.uxTextBoxServerName.Size = new System.Drawing.Size(197, 24);
            this.uxTextBoxServerName.TabIndex = 9;
            // 
            // FrmServiceLogin
            // 
            this.AcceptButton = this.uxButtonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 219);
            this.Controls.Add(this.uxTextBoxServerName);
            this.Controls.Add(this.uxLabelServerName);
            this.Controls.Add(this.uxButtonReset);
            this.Controls.Add(this.uxLabelPassword);
            this.Controls.Add(this.uxLabelUsername);
            this.Controls.Add(this.uxTextBoxPassword);
            this.Controls.Add(this.uxTextBoxUsername);
            this.Controls.Add(this.uxButtonSave);
            this.Controls.Add(this.uxLabelTitle);
            this.Controls.Add(this.uxCheckBoxWindowsAuthentication);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmServiceLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database Login";
            ((System.ComponentModel.ISupportInitialize)(this.uxCheckBoxWindowsAuthentication)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelServerName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxServerName)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadCheckBox uxCheckBoxWindowsAuthentication;
        private Telerik.WinControls.UI.RadLabel uxLabelTitle;
        private Telerik.WinControls.UI.RadButton uxButtonSave;
        private Telerik.WinControls.UI.RadTextBox uxTextBoxUsername;
        private Telerik.WinControls.UI.RadTextBox uxTextBoxPassword;
        private Telerik.WinControls.UI.RadLabel uxLabelUsername;
        private Telerik.WinControls.UI.RadLabel uxLabelPassword;
        private Telerik.WinControls.UI.RadButton uxButtonReset;
        private Telerik.WinControls.UI.RadLabel uxLabelServerName;
        private Telerik.WinControls.UI.RadTextBox uxTextBoxServerName;
    }
}