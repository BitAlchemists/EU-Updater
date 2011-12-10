namespace EU_Release_Manager.Network
{
	partial class FileTransfer
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

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.grpProgress = new System.Windows.Forms.GroupBox();
			this.lblSpeed = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblIdleTime = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lblElapsedTime = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.pbOverall = new System.Windows.Forms.ProgressBar();
			this.pbCurrent = new System.Windows.Forms.ProgressBar();
			this.lblCurrentFile = new System.Windows.Forms.Label();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.grpProgress.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpProgress
			// 
			this.grpProgress.Controls.Add(this.lblSpeed);
			this.grpProgress.Controls.Add(this.label1);
			this.grpProgress.Controls.Add(this.lblIdleTime);
			this.grpProgress.Controls.Add(this.label9);
			this.grpProgress.Controls.Add(this.lblElapsedTime);
			this.grpProgress.Controls.Add(this.label8);
			this.grpProgress.Controls.Add(this.label7);
			this.grpProgress.Controls.Add(this.pbOverall);
			this.grpProgress.Controls.Add(this.pbCurrent);
			this.grpProgress.Controls.Add(this.lblCurrentFile);
			this.grpProgress.Controls.Add(this.cmdCancel);
			this.grpProgress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpProgress.Enabled = false;
			this.grpProgress.Location = new System.Drawing.Point(0, 0);
			this.grpProgress.Margin = new System.Windows.Forms.Padding(0);
			this.grpProgress.Name = "grpProgress";
			this.grpProgress.Size = new System.Drawing.Size(470, 170);
			this.grpProgress.TabIndex = 3;
			this.grpProgress.TabStop = false;
			this.grpProgress.Text = "Progress";
			// 
			// lblSpeed
			// 
			this.lblSpeed.AutoSize = true;
			this.lblSpeed.Location = new System.Drawing.Point(386, 27);
			this.lblSpeed.Name = "lblSpeed";
			this.lblSpeed.Size = new System.Drawing.Size(38, 13);
			this.lblSpeed.TabIndex = 22;
			this.lblSpeed.Text = "0 kb/s";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(339, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 21;
			this.label1.Text = "Speed:";
			// 
			// lblIdleTime
			// 
			this.lblIdleTime.AutoSize = true;
			this.lblIdleTime.Location = new System.Drawing.Point(272, 27);
			this.lblIdleTime.Name = "lblIdleTime";
			this.lblIdleTime.Size = new System.Drawing.Size(49, 13);
			this.lblIdleTime.TabIndex = 20;
			this.lblIdleTime.Text = "00:00:00";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(164, 27);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(102, 13);
			this.label9.TabIndex = 19;
			this.label9.Text = "Estimated Idle Time:";
			// 
			// lblElapsedTime
			// 
			this.lblElapsedTime.AutoSize = true;
			this.lblElapsedTime.Location = new System.Drawing.Point(96, 27);
			this.lblElapsedTime.Name = "lblElapsedTime";
			this.lblElapsedTime.Size = new System.Drawing.Size(49, 13);
			this.lblElapsedTime.TabIndex = 18;
			this.lblElapsedTime.Text = "00:00:00";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(16, 27);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(74, 13);
			this.label8.TabIndex = 17;
			this.label8.Text = "Elapsed Time:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 54);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(63, 13);
			this.label7.TabIndex = 16;
			this.label7.Text = "Current File:";
			// 
			// pbOverall
			// 
			this.pbOverall.Location = new System.Drawing.Point(19, 100);
			this.pbOverall.Name = "pbOverall";
			this.pbOverall.Size = new System.Drawing.Size(430, 24);
			this.pbOverall.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbOverall.TabIndex = 5;
			// 
			// pbCurrent
			// 
			this.pbCurrent.Location = new System.Drawing.Point(19, 70);
			this.pbCurrent.Name = "pbCurrent";
			this.pbCurrent.Size = new System.Drawing.Size(430, 24);
			this.pbCurrent.Step = 1;
			this.pbCurrent.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbCurrent.TabIndex = 6;
			// 
			// lblCurrentFile
			// 
			this.lblCurrentFile.AutoSize = true;
			this.lblCurrentFile.BackColor = System.Drawing.Color.Transparent;
			this.lblCurrentFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCurrentFile.ForeColor = System.Drawing.Color.Black;
			this.lblCurrentFile.Location = new System.Drawing.Point(85, 54);
			this.lblCurrentFile.Name = "lblCurrentFile";
			this.lblCurrentFile.Size = new System.Drawing.Size(0, 13);
			this.lblCurrentFile.TabIndex = 13;
			// 
			// cmdCancel
			// 
			this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCancel.Location = new System.Drawing.Point(175, 130);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(130, 23);
			this.cmdCancel.TabIndex = 0;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// FileTransfer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grpProgress);
			this.DoubleBuffered = true;
			this.Name = "FileTransfer";
			this.Size = new System.Drawing.Size(470, 170);
			this.grpProgress.ResumeLayout(false);
			this.grpProgress.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox grpProgress;
		private System.Windows.Forms.Label lblSpeed;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblIdleTime;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lblElapsedTime;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ProgressBar pbOverall;
		private System.Windows.Forms.ProgressBar pbCurrent;
		private System.Windows.Forms.Label lblCurrentFile;
		private System.Windows.Forms.Button cmdCancel;

	}
}
