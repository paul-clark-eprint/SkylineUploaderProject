
namespace SkylineUploader
{
    partial class FrmUploader
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
            this.components = new System.ComponentModel.Container();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition2 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUploader));
            this.uxGridViewFolders = new Telerik.WinControls.UI.RadGridView();
            this.uxButtonNew = new Telerik.WinControls.UI.RadButton();
            this.uxButtonClose = new Telerik.WinControls.UI.RadButton();
            this.uxWaitingBar = new Telerik.WinControls.UI.RadWaitingBar();
            this.dotsLineWaitingBarIndicatorElement1 = new Telerik.WinControls.UI.DotsLineWaitingBarIndicatorElement();
            this.uxMenuItemFile = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuItemDebug = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuItemError = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuItemExit = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.uxLabelStatus = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonNew)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxWaitingBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // uxGridViewFolders
            // 
            this.uxGridViewFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxGridViewFolders.Location = new System.Drawing.Point(12, 31);
            // 
            // 
            // 
            this.uxGridViewFolders.MasterTemplate.AllowAddNewRow = false;
            this.uxGridViewFolders.MasterTemplate.AllowColumnReorder = false;
            this.uxGridViewFolders.MasterTemplate.AllowDragToGroup = false;
            this.uxGridViewFolders.MasterTemplate.AllowEditRow = false;
            this.uxGridViewFolders.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            this.uxGridViewFolders.MasterTemplate.ViewDefinition = tableViewDefinition2;
            this.uxGridViewFolders.Name = "uxGridViewFolders";
            this.uxGridViewFolders.Size = new System.Drawing.Size(850, 226);
            this.uxGridViewFolders.TabIndex = 0;
            this.uxGridViewFolders.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.uxGridViewFolders_CellFormatting);
            // 
            // uxButtonNew
            // 
            this.uxButtonNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonNew.Enabled = false;
            this.uxButtonNew.Location = new System.Drawing.Point(658, 263);
            this.uxButtonNew.Name = "uxButtonNew";
            this.uxButtonNew.Size = new System.Drawing.Size(100, 26);
            this.uxButtonNew.TabIndex = 1;
            this.uxButtonNew.Text = "New Profile";
            this.uxButtonNew.Click += new System.EventHandler(this.uxButtonNew_Click);
            // 
            // uxButtonClose
            // 
            this.uxButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxButtonClose.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            this.uxButtonClose.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.uxButtonClose.Location = new System.Drawing.Point(764, 263);
            this.uxButtonClose.Name = "uxButtonClose";
            this.uxButtonClose.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.uxButtonClose.Size = new System.Drawing.Size(100, 26);
            this.uxButtonClose.TabIndex = 49;
            this.uxButtonClose.Text = "Close";
            this.uxButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.uxButtonClose.Click += new System.EventHandler(this.uxButtonClose_Click);
            // 
            // uxWaitingBar
            // 
            this.uxWaitingBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxWaitingBar.Location = new System.Drawing.Point(522, 265);
            this.uxWaitingBar.Name = "uxWaitingBar";
            this.uxWaitingBar.Size = new System.Drawing.Size(130, 24);
            this.uxWaitingBar.TabIndex = 51;
            this.uxWaitingBar.Visible = false;
            this.uxWaitingBar.WaitingIndicators.Add(this.dotsLineWaitingBarIndicatorElement1);
            this.uxWaitingBar.WaitingIndicatorSize = new System.Drawing.Size(100, 14);
            this.uxWaitingBar.WaitingSpeed = 80;
            this.uxWaitingBar.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.DotsLine;
            ((Telerik.WinControls.UI.RadWaitingBarElement)(this.uxWaitingBar.GetChildAt(0))).WaitingIndicatorSize = new System.Drawing.Size(100, 14);
            ((Telerik.WinControls.UI.RadWaitingBarElement)(this.uxWaitingBar.GetChildAt(0))).WaitingSpeed = 80;
            ((Telerik.WinControls.UI.WaitingBarSeparatorElement)(this.uxWaitingBar.GetChildAt(0).GetChildAt(0).GetChildAt(0))).Dash = false;
            // 
            // dotsLineWaitingBarIndicatorElement1
            // 
            this.dotsLineWaitingBarIndicatorElement1.Name = "dotsLineWaitingBarIndicatorElement1";
            // 
            // uxMenuItemFile
            // 
            this.uxMenuItemFile.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.uxMenuItemDebug,
            this.uxMenuItemError,
            this.uxMenuItemExit});
            this.uxMenuItemFile.Name = "uxMenuItemFile";
            this.uxMenuItemFile.Text = "File";
            // 
            // uxMenuItemDebug
            // 
            this.uxMenuItemDebug.Name = "uxMenuItemDebug";
            this.uxMenuItemDebug.Text = "Open Debug Log";
            this.uxMenuItemDebug.Click += new System.EventHandler(this.uxMenuItemDebug_Click);
            // 
            // uxMenuItemError
            // 
            this.uxMenuItemError.Name = "uxMenuItemError";
            this.uxMenuItemError.Text = "Open Error Log";
            this.uxMenuItemError.Click += new System.EventHandler(this.uxMenuItemError_Click);
            // 
            // uxMenuItemExit
            // 
            this.uxMenuItemExit.Name = "uxMenuItemExit";
            this.uxMenuItemExit.Text = "Exit";
            this.uxMenuItemExit.Click += new System.EventHandler(this.uxMenuItemExit_Click);
            // 
            // radMenu1
            // 
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.uxMenuItemFile});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.Size = new System.Drawing.Size(874, 25);
            this.radMenu1.TabIndex = 52;
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // uxLabelStatus
            // 
            this.uxLabelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uxLabelStatus.Location = new System.Drawing.Point(12, 275);
            this.uxLabelStatus.Name = "uxLabelStatus";
            this.uxLabelStatus.Size = new System.Drawing.Size(37, 18);
            this.uxLabelStatus.TabIndex = 50;
            this.uxLabelStatus.Text = "Status";
            this.uxLabelStatus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.uxLabelStatus.Visible = false;
            // 
            // FrmUploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 301);
            this.Controls.Add(this.radMenu1);
            this.Controls.Add(this.uxWaitingBar);
            this.Controls.Add(this.uxLabelStatus);
            this.Controls.Add(this.uxButtonClose);
            this.Controls.Add(this.uxButtonNew);
            this.Controls.Add(this.uxGridViewFolders);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmUploader";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Skyline Uploader";
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonNew)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxButtonClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxWaitingBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxLabelStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView uxGridViewFolders;
        private Telerik.WinControls.UI.RadButton uxButtonNew;
        private Telerik.WinControls.UI.RadButton uxButtonClose;
        private Telerik.WinControls.UI.RadLabel uxLabelStatus;
        private Telerik.WinControls.UI.RadWaitingBar uxWaitingBar;
        private Telerik.WinControls.UI.DotsLineWaitingBarIndicatorElement dotsLineWaitingBarIndicatorElement1;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemFile;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemDebug;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemError;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemExit;
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private System.Windows.Forms.Timer timer1;
    }
}