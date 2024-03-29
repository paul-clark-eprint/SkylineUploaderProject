﻿
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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUploader));
            this.uxGridViewFolders = new Telerik.WinControls.UI.RadGridView();
            this.uxMenuItemFile = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuItemLogging = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem1 = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem2 = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem3 = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuItemDebugOn = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuItemDebugOff = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuSetServiceLogin = new Telerik.WinControls.UI.RadMenuItem();
            this.uxMenuItemExit = new Telerik.WinControls.UI.RadMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.radStatusStrip1 = new Telerik.WinControls.UI.RadStatusStrip();
            this.uxLabelStatus = new Telerik.WinControls.UI.RadLabelElement();
            this.uxLabelSpacer = new Telerik.WinControls.UI.RadLabelElement();
            this.uxProgressBar = new Telerik.WinControls.UI.RadProgressBarElement();
            this.UxWaitingBar = new Telerik.WinControls.UI.RadWaitingBarElement();
            this.uxButtonNew1 = new Telerik.WinControls.UI.RadButtonElement();
            this.uxButtonClose1 = new Telerik.WinControls.UI.RadButtonElement();
            this.dotsLineWaitingBarIndicatorElement1 = new Telerik.WinControls.UI.DotsLineWaitingBarIndicatorElement();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
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
            this.uxGridViewFolders.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.uxGridViewFolders.Name = "uxGridViewFolders";
            this.uxGridViewFolders.Size = new System.Drawing.Size(850, 299);
            this.uxGridViewFolders.TabIndex = 0;
            this.uxGridViewFolders.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.uxGridViewFolders_CellFormatting);
            // 
            // uxMenuItemFile
            // 
            this.uxMenuItemFile.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.uxMenuItemLogging,
            this.uxMenuSetServiceLogin,
            this.uxMenuItemExit});
            this.uxMenuItemFile.Name = "uxMenuItemFile";
            this.uxMenuItemFile.Text = "File";
            // 
            // uxMenuItemLogging
            // 
            this.uxMenuItemLogging.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radMenuItem1,
            this.radMenuItem2,
            this.radMenuItem3});
            this.uxMenuItemLogging.Name = "uxMenuItemLogging";
            this.uxMenuItemLogging.Text = "Logging";
            // 
            // radMenuItem1
            // 
            this.radMenuItem1.Name = "radMenuItem1";
            this.radMenuItem1.Text = "Open Debug Log";
            this.radMenuItem1.Click += new System.EventHandler(this.uxMenuItemDebug_Click);
            // 
            // radMenuItem2
            // 
            this.radMenuItem2.Name = "radMenuItem2";
            this.radMenuItem2.Text = "Open Error Log";
            this.radMenuItem2.Click += new System.EventHandler(this.uxMenuItemError_Click);
            // 
            // radMenuItem3
            // 
            this.radMenuItem3.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.uxMenuItemDebugOn,
            this.uxMenuItemDebugOff});
            this.radMenuItem3.Name = "radMenuItem3";
            this.radMenuItem3.Text = "Skyline Uploader Service";
            // 
            // uxMenuItemDebugOn
            // 
            this.uxMenuItemDebugOn.Name = "uxMenuItemDebugOn";
            this.uxMenuItemDebugOn.Text = "Turn Debug ON";
            this.uxMenuItemDebugOn.Click += new System.EventHandler(this.uxMenuDebugOn_Click);
            // 
            // uxMenuItemDebugOff
            // 
            this.uxMenuItemDebugOff.Name = "uxMenuItemDebugOff";
            this.uxMenuItemDebugOff.Text = "Turn Debug OFF";
            this.uxMenuItemDebugOff.Click += new System.EventHandler(this.uxMenuDebugOff_Click);
            // 
            // uxMenuSetServiceLogin
            // 
            this.uxMenuSetServiceLogin.Name = "uxMenuSetServiceLogin";
            this.uxMenuSetServiceLogin.Text = "Set Database Login";
            this.uxMenuSetServiceLogin.Click += new System.EventHandler(this.uxMenuSetServiceLogin_Click);
            // 
            // uxMenuItemExit
            // 
            this.uxMenuItemExit.Name = "uxMenuItemExit";
            this.uxMenuItemExit.Text = "Exit";
            this.uxMenuItemExit.Click += new System.EventHandler(this.uxMenuItemExit_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // radStatusStrip1
            // 
            this.radStatusStrip1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.uxLabelStatus,
            this.uxLabelSpacer,
            this.uxProgressBar,
            this.UxWaitingBar,
            this.uxButtonNew1,
            this.uxButtonClose1});
            this.radStatusStrip1.Location = new System.Drawing.Point(0, 338);
            this.radStatusStrip1.Name = "radStatusStrip1";
            this.radStatusStrip1.Size = new System.Drawing.Size(874, 30);
            this.radStatusStrip1.TabIndex = 54;
            // 
            // uxLabelStatus
            // 
            this.uxLabelStatus.Name = "uxLabelStatus";
            this.radStatusStrip1.SetSpring(this.uxLabelStatus, false);
            this.uxLabelStatus.Text = "Starting up";
            this.uxLabelStatus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.uxLabelStatus.TextWrap = true;
            // 
            // uxLabelSpacer
            // 
            this.uxLabelSpacer.Name = "uxLabelSpacer";
            this.radStatusStrip1.SetSpring(this.uxLabelSpacer, true);
            this.uxLabelSpacer.Text = "";
            this.uxLabelSpacer.TextWrap = true;
            // 
            // uxProgressBar
            // 
            this.uxProgressBar.AutoSize = false;
            this.uxProgressBar.Bounds = new System.Drawing.Rectangle(0, 0, 100, 24);
            this.uxProgressBar.Name = "uxProgressBar";
            this.uxProgressBar.SeparatorColor1 = System.Drawing.Color.White;
            this.uxProgressBar.SeparatorColor2 = System.Drawing.Color.White;
            this.uxProgressBar.SeparatorColor3 = System.Drawing.Color.White;
            this.uxProgressBar.SeparatorColor4 = System.Drawing.Color.White;
            this.uxProgressBar.SeparatorGradientAngle = 0;
            this.uxProgressBar.SeparatorGradientPercentage1 = 0.4F;
            this.uxProgressBar.SeparatorGradientPercentage2 = 0.6F;
            this.uxProgressBar.SeparatorNumberOfColors = 2;
            this.radStatusStrip1.SetSpring(this.uxProgressBar, false);
            this.uxProgressBar.StepWidth = 14;
            this.uxProgressBar.SweepAngle = 90;
            this.uxProgressBar.Text = "";
            this.uxProgressBar.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            // 
            // UxWaitingBar
            // 
            this.UxWaitingBar.Name = "UxWaitingBar";
            // 
            // 
            // 
            this.UxWaitingBar.SeparatorElement.Dash = false;
            this.radStatusStrip1.SetSpring(this.UxWaitingBar, false);
            this.UxWaitingBar.Text = "";
            this.UxWaitingBar.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            ((Telerik.WinControls.UI.WaitingBarSeparatorElement)(this.UxWaitingBar.GetChildAt(0).GetChildAt(0))).Dash = false;
            // 
            // uxButtonNew1
            // 
            this.uxButtonNew1.AutoSize = false;
            this.uxButtonNew1.Bounds = new System.Drawing.Rectangle(0, 0, 100, 24);
            this.uxButtonNew1.Name = "uxButtonNew1";
            this.uxButtonNew1.ShowBorder = false;
            this.radStatusStrip1.SetSpring(this.uxButtonNew1, false);
            this.uxButtonNew1.Text = "New Profile";
            this.uxButtonNew1.Click += new System.EventHandler(this.uxButtonNew1_Click);
            // 
            // uxButtonClose1
            // 
            this.uxButtonClose1.AutoSize = false;
            this.uxButtonClose1.Bounds = new System.Drawing.Rectangle(0, 0, 100, 24);
            this.uxButtonClose1.Name = "uxButtonClose1";
            this.uxButtonClose1.ShowBorder = false;
            this.radStatusStrip1.SetSpring(this.uxButtonClose1, false);
            this.uxButtonClose1.Text = "Close";
            this.uxButtonClose1.Click += new System.EventHandler(this.uxButtonClose1_Click);
            // 
            // dotsLineWaitingBarIndicatorElement1
            // 
            this.dotsLineWaitingBarIndicatorElement1.Name = "dotsLineWaitingBarIndicatorElement1";
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
            // FrmUploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 368);
            this.Controls.Add(this.radStatusStrip1);
            this.Controls.Add(this.radMenu1);
            this.Controls.Add(this.uxGridViewFolders);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmUploader";
            this.Text = "Skyline Uploader";
            this.Load += new System.EventHandler(this.FrmUploader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxGridViewFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView uxGridViewFolders;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemFile;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemLogging;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemExit;
        private System.Windows.Forms.Timer timer1;
        private Telerik.WinControls.UI.RadStatusStrip radStatusStrip1;
        private Telerik.WinControls.UI.RadLabelElement uxLabelStatus;
        private Telerik.WinControls.UI.RadLabelElement uxLabelSpacer;
        private Telerik.WinControls.UI.RadProgressBarElement uxProgressBar;
        private Telerik.WinControls.UI.RadButtonElement uxButtonNew1;
        private Telerik.WinControls.UI.RadButtonElement uxButtonClose1;
        private Telerik.WinControls.UI.RadWaitingBarElement UxWaitingBar;
        private Telerik.WinControls.UI.DotsLineWaitingBarIndicatorElement dotsLineWaitingBarIndicatorElement1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem2;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem3;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemDebugOn;
        private Telerik.WinControls.UI.RadMenuItem uxMenuItemDebugOff;
        private Telerik.WinControls.UI.RadMenuItem uxMenuSetServiceLogin;
        private Telerik.WinControls.UI.RadMenu radMenu1;
    }
}