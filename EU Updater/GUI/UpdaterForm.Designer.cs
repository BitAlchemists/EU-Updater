namespace EU_Updater
{
    partial class UpdaterForm
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
			  this.lblUpdater = new System.Windows.Forms.Label();
			  this.cmdPlay = new System.Windows.Forms.Button();
			  this.cmdCancel = new System.Windows.Forms.Button();
			  this.wbNews = new System.Windows.Forms.WebBrowser();
			  this.lblStatus = new System.Windows.Forms.Label();
			  this.fbdGamePath = new System.Windows.Forms.FolderBrowserDialog();
			  this.ftDownload = new EU_Updater.FileTransfer();
			  this.SuspendLayout();
			  // 
			  // lblUpdater
			  // 
			  this.lblUpdater.Anchor = System.Windows.Forms.AnchorStyles.Top;
			  this.lblUpdater.AutoSize = true;
			  this.lblUpdater.BackColor = System.Drawing.Color.Transparent;
			  this.lblUpdater.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			  this.lblUpdater.ForeColor = System.Drawing.Color.White;
			  this.lblUpdater.Location = new System.Drawing.Point(211, 24);
			  this.lblUpdater.Name = "lblUpdater";
			  this.lblUpdater.Size = new System.Drawing.Size(369, 29);
			  this.lblUpdater.TabIndex = 3;
			  this.lblUpdater.Text = "Evolving Universe Updater";
			  // 
			  // cmdPlay
			  // 
			  this.cmdPlay.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			  this.cmdPlay.BackColor = System.Drawing.Color.White;
			  this.cmdPlay.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			  this.cmdPlay.ForeColor = System.Drawing.Color.Black;
			  this.cmdPlay.Location = new System.Drawing.Point(235, 521);
			  this.cmdPlay.Name = "cmdPlay";
			  this.cmdPlay.Size = new System.Drawing.Size(147, 34);
			  this.cmdPlay.TabIndex = 12;
			  this.cmdPlay.Text = "Play";
			  this.cmdPlay.UseVisualStyleBackColor = false;
			  this.cmdPlay.Click += new System.EventHandler(this.cmdPlay_Click);
			  // 
			  // cmdCancel
			  // 
			  this.cmdCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			  this.cmdCancel.BackColor = System.Drawing.Color.White;
			  this.cmdCancel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			  this.cmdCancel.ForeColor = System.Drawing.Color.Black;
			  this.cmdCancel.Location = new System.Drawing.Point(405, 521);
			  this.cmdCancel.Name = "cmdCancel";
			  this.cmdCancel.Size = new System.Drawing.Size(151, 34);
			  this.cmdCancel.TabIndex = 13;
			  this.cmdCancel.Text = "Cancel";
			  this.cmdCancel.UseVisualStyleBackColor = false;
			  this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			  // 
			  // wbNews
			  // 
			  this.wbNews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							  | System.Windows.Forms.AnchorStyles.Left)
							  | System.Windows.Forms.AnchorStyles.Right)));
			  this.wbNews.Location = new System.Drawing.Point(19, 65);
			  this.wbNews.MinimumSize = new System.Drawing.Size(20, 20);
			  this.wbNews.Name = "wbNews";
			  this.wbNews.Size = new System.Drawing.Size(752, 299);
			  this.wbNews.TabIndex = 15;
			  this.wbNews.Url = new System.Uri("http://www.evolvinguniverse.net/portal/index.php?format=feed&type=rss", System.UriKind.Absolute);
			  // 
			  // lblStatus
			  // 
			  this.lblStatus.AutoSize = true;
			  this.lblStatus.BackColor = System.Drawing.Color.Transparent;
			  this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			  this.lblStatus.ForeColor = System.Drawing.Color.White;
			  this.lblStatus.Location = new System.Drawing.Point(175, 371);
			  this.lblStatus.Name = "lblStatus";
			  this.lblStatus.Size = new System.Drawing.Size(62, 20);
			  this.lblStatus.TabIndex = 17;
			  this.lblStatus.Text = "Status";
			  // 
			  // fbdGamePath
			  // 
			  this.fbdGamePath.Description = "No game path found! Please select the directory in which your Evolving Universe g" +
					"ame is installed.";
			  this.fbdGamePath.ShowNewFolderButton = false;
			  // 
			  // ftDownload
			  // 
			  this.ftDownload.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			  this.ftDownload.BackColor = System.Drawing.Color.Transparent;
			  this.ftDownload.Enabled = false;
			  this.ftDownload.ForeColor = System.Drawing.Color.White;
			  this.ftDownload.Location = new System.Drawing.Point(162, 394);
			  this.ftDownload.Name = "ftDownload";
			  this.ftDownload.Size = new System.Drawing.Size(467, 120);
			  this.ftDownload.TabIndex = 16;
			  // 
			  // UpdaterForm
			  // 
			  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			  this.BackColor = System.Drawing.Color.Black;
			  this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			  this.ClientSize = new System.Drawing.Size(790, 570);
			  this.Controls.Add(this.lblStatus);
			  this.Controls.Add(this.ftDownload);
			  this.Controls.Add(this.wbNews);
			  this.Controls.Add(this.cmdCancel);
			  this.Controls.Add(this.cmdPlay);
			  this.Controls.Add(this.lblUpdater);
			  this.DoubleBuffered = true;
			  this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			  this.MaximizeBox = false;
			  this.MaximumSize = new System.Drawing.Size(800, 600);
			  this.MinimizeBox = false;
			  this.MinimumSize = new System.Drawing.Size(800, 600);
			  this.Name = "UpdaterForm";
			  this.Text = "Envolving Universe Updater V 0.1";
			  this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdaterForm_FormClosing);
			  this.ResumeLayout(false);
			  this.PerformLayout();

        }

        #endregion

		  private System.Windows.Forms.Label lblUpdater;
        private System.Windows.Forms.Button cmdPlay;
		  private System.Windows.Forms.Button cmdCancel;
		  private System.Windows.Forms.WebBrowser wbNews;
		  private FileTransfer ftDownload;
		  private System.Windows.Forms.Label lblStatus;
		  private System.Windows.Forms.FolderBrowserDialog fbdGamePath;
    }
}

