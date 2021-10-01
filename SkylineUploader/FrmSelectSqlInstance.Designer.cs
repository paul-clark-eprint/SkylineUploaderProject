
namespace SkylineUploader
{
    partial class FrmSelectSqlInstance
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
            Telerik.WinControls.UI.RadListDataItem radListDataItem1 = new Telerik.WinControls.UI.RadListDataItem();
            Telerik.WinControls.UI.RadListDataItem radListDataItem2 = new Telerik.WinControls.UI.RadListDataItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectSqlInstance));
            this.uxDropDownList = new Telerik.WinControls.UI.RadDropDownList();
            this.uxButtonClose = new Telerik.WinControls.UI.RadButton();
            this.uxButtonSelect = new Telerik.WinControls.UI.RadButton();
            this.uxLabelTitle = new Telerik.WinControls.UI.RadLabel();
            this.uxLabelInstance = new Telerik.WinControls.UI.RadLabel();
            this.uxLabelInfo = new Telerik.WinControls.UI.RadLabel();
            this.uxTextBoxUsername = new Telerik.WinControls.UI.RadTextBoxControl();
            this.uxTextBoxPassword = new Telerik.WinControls.UI.RadTextBoxControl();
            this.uxLabelUsername = new Telerik.WinControls.UI.RadLabel();
            this.radDropDownList1 = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.uxLabelPassword = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.uxDropDownList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelInstance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radDropDownList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // uxDropDownList
            // 
            this.uxDropDownList.DropDownAnimationEnabled = true;
            this.uxDropDownList.Location = new System.Drawing.Point(118, 54);
            this.uxDropDownList.Name = "uxDropDownList";
            this.uxDropDownList.Size = new System.Drawing.Size(204, 24);
            this.uxDropDownList.TabIndex = 0;
            // 
            // uxButtonClose
            // 
            this.uxButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxButtonClose.Location = new System.Drawing.Point(275, 194);
            this.uxButtonClose.Name = "uxButtonClose";
            this.uxButtonClose.Size = new System.Drawing.Size(100, 26);
            this.uxButtonClose.TabIndex = 1;
            this.uxButtonClose.Text = "Close";
            this.uxButtonClose.Click += new System.EventHandler(this.uxButtonClose_Click);
            // 
            // uxButtonSelect
            // 
            this.uxButtonSelect.Location = new System.Drawing.Point(169, 194);
            this.uxButtonSelect.Name = "uxButtonSelect";
            this.uxButtonSelect.Size = new System.Drawing.Size(100, 26);
            this.uxButtonSelect.TabIndex = 2;
            this.uxButtonSelect.Text = "Connect";
            this.uxButtonSelect.Click += new System.EventHandler(this.uxButtonSelect_Click);
            // 
            // uxLabelTitle
            // 
            this.uxLabelTitle.AutoSize = false;
            this.uxLabelTitle.Location = new System.Drawing.Point(12, 12);
            this.uxLabelTitle.Name = "uxLabelTitle";
            this.uxLabelTitle.Size = new System.Drawing.Size(363, 35);
            this.uxLabelTitle.TabIndex = 4;
            this.uxLabelTitle.Text = "Please select the SQL Instance that you want to use for the Skyline Uploader data" +
    "base";
            // 
            // uxLabelInstance
            // 
            this.uxLabelInstance.Location = new System.Drawing.Point(12, 53);
            this.uxLabelInstance.Name = "uxLabelInstance";
            this.uxLabelInstance.Size = new System.Drawing.Size(75, 18);
            this.uxLabelInstance.TabIndex = 5;
            this.uxLabelInstance.Text = "SQL Instances";
            // 
            // uxLabelInfo
            // 
            this.uxLabelInfo.Enabled = false;
            this.uxLabelInfo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.uxLabelInfo.Location = new System.Drawing.Point(7, 166);
            this.uxLabelInfo.Name = "uxLabelInfo";
            this.uxLabelInfo.Size = new System.Drawing.Size(382, 18);
            this.uxLabelInfo.TabIndex = 6;
            this.uxLabelInfo.Text = "*The user must had rights to create a database in the selected Instance";
            // 
            // uxTextBoxUsername
            // 
            this.uxTextBoxUsername.Enabled = false;
            this.uxTextBoxUsername.Location = new System.Drawing.Point(118, 114);
            this.uxTextBoxUsername.Name = "uxTextBoxUsername";
            this.uxTextBoxUsername.Size = new System.Drawing.Size(125, 22);
            this.uxTextBoxUsername.TabIndex = 7;
            // 
            // uxTextBoxPassword
            // 
            this.uxTextBoxPassword.Enabled = false;
            this.uxTextBoxPassword.Location = new System.Drawing.Point(118, 142);
            this.uxTextBoxPassword.Name = "uxTextBoxPassword";
            this.uxTextBoxPassword.Size = new System.Drawing.Size(125, 22);
            this.uxTextBoxPassword.TabIndex = 8;
            this.uxTextBoxPassword.UseSystemPasswordChar = true;
            // 
            // uxLabelUsername
            // 
            this.uxLabelUsername.Enabled = false;
            this.uxLabelUsername.Location = new System.Drawing.Point(12, 114);
            this.uxLabelUsername.Name = "uxLabelUsername";
            this.uxLabelUsername.Size = new System.Drawing.Size(64, 18);
            this.uxLabelUsername.TabIndex = 9;
            this.uxLabelUsername.Text = "Username *";
            // 
            // radDropDownList1
            // 
            this.radDropDownList1.DropDownAnimationEnabled = true;
            radListDataItem1.Selected = true;
            radListDataItem1.Text = "Windows Authentication";
            radListDataItem2.Text = "SQL Server Authentication";
            this.radDropDownList1.Items.Add(radListDataItem1);
            this.radDropDownList1.Items.Add(radListDataItem2);
            this.radDropDownList1.Location = new System.Drawing.Point(118, 84);
            this.radDropDownList1.Name = "radDropDownList1";
            this.radDropDownList1.Size = new System.Drawing.Size(204, 24);
            this.radDropDownList1.TabIndex = 10;
            this.radDropDownList1.Text = "Windows Authentication";
            this.radDropDownList1.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.radDropDownList1_SelectedIndexChanged);
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(12, 84);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(80, 18);
            this.radLabel2.TabIndex = 11;
            this.radLabel2.Text = "Authentication";
            // 
            // uxLabelPassword
            // 
            this.uxLabelPassword.Enabled = false;
            this.uxLabelPassword.Location = new System.Drawing.Point(12, 142);
            this.uxLabelPassword.Name = "uxLabelPassword";
            this.uxLabelPassword.Size = new System.Drawing.Size(53, 18);
            this.uxLabelPassword.TabIndex = 12;
            this.uxLabelPassword.Text = "Password";
            // 
            // FrmSelectSqlInstance
            // 
            this.AcceptButton = this.uxButtonSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxButtonClose;
            this.ClientSize = new System.Drawing.Size(395, 231);
            this.Controls.Add(this.uxLabelPassword);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radDropDownList1);
            this.Controls.Add(this.uxLabelUsername);
            this.Controls.Add(this.uxTextBoxPassword);
            this.Controls.Add(this.uxTextBoxUsername);
            this.Controls.Add(this.uxLabelInfo);
            this.Controls.Add(this.uxLabelInstance);
            this.Controls.Add(this.uxLabelTitle);
            this.Controls.Add(this.uxButtonSelect);
            this.Controls.Add(this.uxButtonClose);
            this.Controls.Add(this.uxDropDownList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSelectSqlInstance";
            this.Text = "Select SQL Instance";
            this.Load += new System.EventHandler(this.FrmSelectSqlInstance_Load);
            ((System.ComponentModel.ISupportInitialize)(this.uxDropDownList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelInstance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxTextBoxPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radDropDownList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadDropDownList uxDropDownList;
        private Telerik.WinControls.UI.RadButton uxButtonClose;
        private Telerik.WinControls.UI.RadButton uxButtonSelect;
        private Telerik.WinControls.UI.RadLabel uxLabelTitle;
        private Telerik.WinControls.UI.RadLabel uxLabelInstance;
        private Telerik.WinControls.UI.RadLabel uxLabelInfo;
        private Telerik.WinControls.UI.RadTextBoxControl uxTextBoxUsername;
        private Telerik.WinControls.UI.RadTextBoxControl uxTextBoxPassword;
        private Telerik.WinControls.UI.RadLabel uxLabelUsername;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadDropDownList radDropDownList1;
        private Telerik.WinControls.UI.RadLabel uxLabelPassword;
    }
}