namespace EU_Release_Manager.GUI
{
    partial class ReleaseManagerForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		  protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpDownload = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbDownloadVersions = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmdDownload = new System.Windows.Forms.Button();
            this.cmdSavePath = new System.Windows.Forms.Button();
            this.txtDownloadPath = new System.Windows.Forms.TextBox();
            this.grpUpload = new System.Windows.Forms.GroupBox();
            this.chkUpdate = new System.Windows.Forms.RadioButton();
            this.chkFullVersion = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPoint3 = new System.Windows.Forms.Label();
            this.lblPoint2 = new System.Windows.Forms.Label();
            this.txtVersion4 = new System.Windows.Forms.TextBox();
            this.txtVersion3 = new System.Windows.Forms.TextBox();
            this.txtVersion2 = new System.Windows.Forms.TextBox();
            this.lblPoint1 = new System.Windows.Forms.Label();
            this.cmdUpload = new System.Windows.Forms.Button();
            this.txtVersion1 = new System.Windows.Forms.TextBox();
            this.cmdNewVersion = new System.Windows.Forms.Button();
            this.txtUploadPath = new System.Windows.Forms.TextBox();
            this.fbdSelectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.lblStatus = new System.Windows.Forms.Label();
            this.mnuMainMenu = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.fileTransfer = new EU_Release_Manager.Network.FileTransfer();
            this.grpDownload.SuspendLayout();
            this.grpUpload.SuspendLayout();
            this.mnuMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDownload
            // 
            this.grpDownload.Controls.Add(this.label6);
            this.grpDownload.Controls.Add(this.cmbDownloadVersions);
            this.grpDownload.Controls.Add(this.label5);
            this.grpDownload.Controls.Add(this.cmdDownload);
            this.grpDownload.Controls.Add(this.cmdSavePath);
            this.grpDownload.Controls.Add(this.txtDownloadPath);
            this.grpDownload.Enabled = false;
            this.grpDownload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDownload.Location = new System.Drawing.Point(11, 185);
            this.grpDownload.Name = "grpDownload";
            this.grpDownload.Size = new System.Drawing.Size(470, 108);
            this.grpDownload.TabIndex = 1;
            this.grpDownload.TabStop = false;
            this.grpDownload.Text = "Download";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Save To:";
            // 
            // cmbDownloadVersions
            // 
            this.cmbDownloadVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDownloadVersions.FormattingEnabled = true;
            this.cmbDownloadVersions.Location = new System.Drawing.Point(100, 23);
            this.cmbDownloadVersions.Name = "cmbDownloadVersions";
            this.cmbDownloadVersions.Size = new System.Drawing.Size(121, 21);
            this.cmbDownloadVersions.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Select Version:";
            // 
            // cmdDownload
            // 
            this.cmdDownload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDownload.Location = new System.Drawing.Point(175, 76);
            this.cmdDownload.Name = "cmdDownload";
            this.cmdDownload.Size = new System.Drawing.Size(130, 23);
            this.cmdDownload.TabIndex = 3;
            this.cmdDownload.Text = "Start Download";
            this.cmdDownload.UseVisualStyleBackColor = true;
            this.cmdDownload.Click += new System.EventHandler(this.cmdDownload_Click);
            // 
            // cmdSavePath
            // 
            this.cmdSavePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSavePath.Location = new System.Drawing.Point(429, 50);
            this.cmdSavePath.Name = "cmdSavePath";
            this.cmdSavePath.Size = new System.Drawing.Size(32, 20);
            this.cmdSavePath.TabIndex = 2;
            this.cmdSavePath.Text = "...";
            this.cmdSavePath.UseVisualStyleBackColor = true;
            this.cmdSavePath.Click += new System.EventHandler(this.cmdSavePath_Click);
            // 
            // txtDownloadPath
            // 
            this.txtDownloadPath.Location = new System.Drawing.Point(100, 50);
            this.txtDownloadPath.Name = "txtDownloadPath";
            this.txtDownloadPath.Size = new System.Drawing.Size(323, 20);
            this.txtDownloadPath.TabIndex = 1;
            // 
            // grpUpload
            // 
            this.grpUpload.Controls.Add(this.chkUpdate);
            this.grpUpload.Controls.Add(this.chkFullVersion);
            this.grpUpload.Controls.Add(this.label4);
            this.grpUpload.Controls.Add(this.label3);
            this.grpUpload.Controls.Add(this.lblPoint3);
            this.grpUpload.Controls.Add(this.lblPoint2);
            this.grpUpload.Controls.Add(this.txtVersion4);
            this.grpUpload.Controls.Add(this.txtVersion3);
            this.grpUpload.Controls.Add(this.txtVersion2);
            this.grpUpload.Controls.Add(this.lblPoint1);
            this.grpUpload.Controls.Add(this.cmdUpload);
            this.grpUpload.Controls.Add(this.txtVersion1);
            this.grpUpload.Controls.Add(this.cmdNewVersion);
            this.grpUpload.Controls.Add(this.txtUploadPath);
            this.grpUpload.Enabled = false;
            this.grpUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpUpload.Location = new System.Drawing.Point(11, 52);
            this.grpUpload.Name = "grpUpload";
            this.grpUpload.Size = new System.Drawing.Size(470, 127);
            this.grpUpload.TabIndex = 0;
            this.grpUpload.TabStop = false;
            this.grpUpload.Text = "Upload";
            // 
            // chkUpdate
            // 
            this.chkUpdate.AutoSize = true;
            this.chkUpdate.Checked = true;
            this.chkUpdate.Location = new System.Drawing.Point(293, 49);
            this.chkUpdate.Name = "chkUpdate";
            this.chkUpdate.Size = new System.Drawing.Size(60, 17);
            this.chkUpdate.TabIndex = 6;
            this.chkUpdate.TabStop = true;
            this.chkUpdate.Text = "Update";
            this.chkUpdate.UseVisualStyleBackColor = true;
            this.chkUpdate.CheckedChanged += new System.EventHandler(this.chkUpdate_CheckedChanged);
            // 
            // chkFullVersion
            // 
            this.chkFullVersion.AutoSize = true;
            this.chkFullVersion.Location = new System.Drawing.Point(293, 72);
            this.chkFullVersion.Name = "chkFullVersion";
            this.chkFullVersion.Size = new System.Drawing.Size(79, 17);
            this.chkFullVersion.TabIndex = 7;
            this.chkFullVersion.Text = "Full Version";
            this.chkFullVersion.UseVisualStyleBackColor = true;
            this.chkFullVersion.CheckedChanged += new System.EventHandler(this.chkFullVersion_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Version Nr:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "New Version:";
            // 
            // lblPoint3
            // 
            this.lblPoint3.AutoSize = true;
            this.lblPoint3.Location = new System.Drawing.Point(204, 66);
            this.lblPoint3.Name = "lblPoint3";
            this.lblPoint3.Size = new System.Drawing.Size(10, 13);
            this.lblPoint3.TabIndex = 11;
            this.lblPoint3.Text = ".";
            // 
            // lblPoint2
            // 
            this.lblPoint2.AutoSize = true;
            this.lblPoint2.Location = new System.Drawing.Point(164, 66);
            this.lblPoint2.Name = "lblPoint2";
            this.lblPoint2.Size = new System.Drawing.Size(10, 13);
            this.lblPoint2.TabIndex = 10;
            this.lblPoint2.Text = ".";
            // 
            // txtVersion4
            // 
            this.txtVersion4.Location = new System.Drawing.Point(220, 59);
            this.txtVersion4.MaxLength = 2;
            this.txtVersion4.Name = "txtVersion4";
            this.txtVersion4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtVersion4.Size = new System.Drawing.Size(20, 20);
            this.txtVersion4.TabIndex = 5;
            this.txtVersion4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVersion4_KeyDown);
            this.txtVersion4.Enter += new System.EventHandler(this.txtVersion4_Enter);
            this.txtVersion4.Validating += new System.ComponentModel.CancelEventHandler(this.txtVersion4_Validating);
            // 
            // txtVersion3
            // 
            this.txtVersion3.Location = new System.Drawing.Point(180, 59);
            this.txtVersion3.MaxLength = 2;
            this.txtVersion3.Name = "txtVersion3";
            this.txtVersion3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtVersion3.Size = new System.Drawing.Size(20, 20);
            this.txtVersion3.TabIndex = 4;
            this.txtVersion3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVersion3_KeyDown);
            this.txtVersion3.Enter += new System.EventHandler(this.txtVersion3_Enter);
            this.txtVersion3.Validating += new System.ComponentModel.CancelEventHandler(this.txtVersion3_Validating);
            // 
            // txtVersion2
            // 
            this.txtVersion2.Location = new System.Drawing.Point(140, 59);
            this.txtVersion2.MaxLength = 2;
            this.txtVersion2.Name = "txtVersion2";
            this.txtVersion2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtVersion2.Size = new System.Drawing.Size(20, 20);
            this.txtVersion2.TabIndex = 3;
            this.txtVersion2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVersion2_KeyDown);
            this.txtVersion2.Enter += new System.EventHandler(this.txtVersion2_Enter);
            this.txtVersion2.Validating += new System.ComponentModel.CancelEventHandler(this.txtVersion2_Validating);
            // 
            // lblPoint1
            // 
            this.lblPoint1.AutoSize = true;
            this.lblPoint1.Location = new System.Drawing.Point(124, 66);
            this.lblPoint1.Name = "lblPoint1";
            this.lblPoint1.Size = new System.Drawing.Size(10, 13);
            this.lblPoint1.TabIndex = 6;
            this.lblPoint1.Text = ".";
            // 
            // cmdUpload
            // 
            this.cmdUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdUpload.Location = new System.Drawing.Point(175, 95);
            this.cmdUpload.Name = "cmdUpload";
            this.cmdUpload.Size = new System.Drawing.Size(130, 23);
            this.cmdUpload.TabIndex = 8;
            this.cmdUpload.Text = "Start Upload";
            this.cmdUpload.UseVisualStyleBackColor = true;
            this.cmdUpload.Click += new System.EventHandler(this.cmdUpload_Click);
            // 
            // txtVersion1
            // 
            this.txtVersion1.Location = new System.Drawing.Point(100, 59);
            this.txtVersion1.MaxLength = 2;
            this.txtVersion1.Name = "txtVersion1";
            this.txtVersion1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtVersion1.Size = new System.Drawing.Size(20, 20);
            this.txtVersion1.TabIndex = 2;
            this.txtVersion1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVersion1_KeyDown);
            this.txtVersion1.Enter += new System.EventHandler(this.txtVersion1_Enter);
            this.txtVersion1.Validating += new System.ComponentModel.CancelEventHandler(this.txtVersion1_Validating);
            // 
            // cmdNewVersion
            // 
            this.cmdNewVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdNewVersion.Location = new System.Drawing.Point(429, 22);
            this.cmdNewVersion.Name = "cmdNewVersion";
            this.cmdNewVersion.Size = new System.Drawing.Size(32, 20);
            this.cmdNewVersion.TabIndex = 1;
            this.cmdNewVersion.Text = "...";
            this.cmdNewVersion.UseVisualStyleBackColor = true;
            this.cmdNewVersion.Click += new System.EventHandler(this.cmdNewVersion_Click);
            // 
            // txtUploadPath
            // 
            this.txtUploadPath.Location = new System.Drawing.Point(100, 22);
            this.txtUploadPath.Name = "txtUploadPath";
            this.txtUploadPath.Size = new System.Drawing.Size(323, 20);
            this.txtUploadPath.TabIndex = 0;
            // 
            // fbdSelectFolder
            // 
            this.fbdSelectFolder.Description = "Please select a folder";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(115, 33);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(266, 16);
            this.lblStatus.TabIndex = 17;
            this.lblStatus.Text = "Evolving Universe Release Manager";
            this.lblStatus.SizeChanged += new System.EventHandler(this.lblStatus_SizeChanged);
            // 
            // mnuMainMenu
            // 
            this.mnuMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.editToolStripMenuItem});
            this.mnuMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuMainMenu.Name = "mnuMainMenu";
            this.mnuMainMenu.Size = new System.Drawing.Size(494, 24);
            this.mnuMainMenu.TabIndex = 20;
            this.mnuMainMenu.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRefresh,
            this.toolStripMenuItem1,
            this.mnuExit});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.dateiToolStripMenuItem.Text = "&File";
            // 
            // mnuRefresh
            // 
            this.mnuRefresh.Name = "mnuRefresh";
            this.mnuRefresh.Size = new System.Drawing.Size(123, 22);
            this.mnuRefresh.Text = "&Refresh";
            this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(123, 22);
            this.mnuExit.Text = "&Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSettings});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // mnuSettings
            // 
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(124, 22);
            this.mnuSettings.Text = "&Settings";
            this.mnuSettings.Click += new System.EventHandler(this.mnuSettings_Click);
            // 
            // fileTransfer
            // 
            this.fileTransfer.Location = new System.Drawing.Point(11, 300);
            this.fileTransfer.Name = "fileTransfer";
            this.fileTransfer.Size = new System.Drawing.Size(470, 170);
            this.fileTransfer.TabIndex = 21;
            // 
            // ReleaseManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 485);
            this.Controls.Add(this.fileTransfer);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.grpUpload);
            this.Controls.Add(this.grpDownload);
            this.Controls.Add(this.mnuMainMenu);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mnuMainMenu;
            this.MaximizeBox = false;
            this.Name = "ReleaseManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Evolving Universe Release Manager";
            this.grpDownload.ResumeLayout(false);
            this.grpDownload.PerformLayout();
            this.grpUpload.ResumeLayout(false);
            this.grpUpload.PerformLayout();
            this.mnuMainMenu.ResumeLayout(false);
            this.mnuMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDownload;
        private System.Windows.Forms.GroupBox grpUpload;
        private System.Windows.Forms.TextBox txtUploadPath;
        private System.Windows.Forms.TextBox txtVersion1;
        private System.Windows.Forms.Button cmdNewVersion;
        private System.Windows.Forms.Label lblPoint3;
        private System.Windows.Forms.Label lblPoint2;
        private System.Windows.Forms.TextBox txtVersion4;
        private System.Windows.Forms.TextBox txtVersion3;
        private System.Windows.Forms.TextBox txtVersion2;
        private System.Windows.Forms.Label lblPoint1;
        private System.Windows.Forms.Button cmdUpload;
        private System.Windows.Forms.Button cmdDownload;
        private System.Windows.Forms.Button cmdSavePath;
		  private System.Windows.Forms.TextBox txtDownloadPath;
		  private System.Windows.Forms.FolderBrowserDialog fbdSelectFolder;
		  private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton chkFullVersion;
        private System.Windows.Forms.RadioButton chkUpdate;
        private System.Windows.Forms.ComboBox cmbDownloadVersions;
        private System.Windows.Forms.Label label5;
		  private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MenuStrip mnuMainMenu;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
		  private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem mnuSettings;
		  private System.Windows.Forms.ToolStripMenuItem mnuRefresh;
		  private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		  private EU_Release_Manager.Network.FileTransfer fileTransfer;
    }
}

