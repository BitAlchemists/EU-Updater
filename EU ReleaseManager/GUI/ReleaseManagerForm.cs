using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EU_Release_Manager.Controller;
using EU_Release_Manager.Network;

namespace EU_Release_Manager.GUI
{
	public partial class ReleaseManagerForm : Form
	{
		private IReleaseManager manager;
        delegate void DelegateClose();

		public ReleaseManagerForm()
		{
			InitializeComponent();
			
			// create controller object
			manager = new ReleaseManager(fileTransfer);

			// create event listeners
			manager.Status += new EventHandler<EventArgs<string>>(SetStatus);
			manager.Warning += new EventHandler<EventArgs<string>>(ShowWarning);
			manager.Error += new EventHandler<EventArgs<string>>(ShowError);
			manager.Information += new EventHandler<EventArgs<string>>(ShowInformation);
			manager.MainMenu += new EventHandler<EventArgs>(EnableMainMenu);
			manager.Status += new EventHandler<EventArgs<string>>(SetStatus);
			manager.DisableDownload += new EventHandler<EventArgs>(DisableDownload);
			manager.DisableUpload += new EventHandler<EventArgs>(DisableUpload);
			manager.EnableDownload += new EventHandler<EventArgs>(EnableDownload);
			manager.EnableUpload += new EventHandler<EventArgs<bool>>(EnableUpload);
			manager.Exit += new EventHandler<EventArgs>(Exit);
			manager.UploadVersion += new EventHandler<EventArgs<string>>(SetUploadVersion);
			manager.DownloadVersions += new EventHandler<EventArgs<List<string>>>(SetDownloadVersions);

			// if controller object hasn't produced an error
			if (manager.Valid)
				// start (test settings & download global config)
				manager.Start();
		}

		private void Exit(object sender, EventArgs e)
		{
            Invoke(new DelegateClose(Close));
		}

		private void ShowInformation(object sender, EventArgs<string> e)
		{
			MessageBox.Show(e.Value, Strings.ReleaseManager, 
				MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void ShowWarning(object sender, EventArgs<string> e)
		{
			MessageBox.Show(e.Value, Strings.ReleaseManager,
				MessageBoxButtons.OK, MessageBoxIcon.Warning);			
		}

		private void ShowError(object sender, EventArgs<string> e)
		{
			MessageBox.Show(e.Value, Strings.ReleaseManager,
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void EnableMainMenu(object sender, EventArgs e)
		{
			SetMainMenu(sender, new EventArgs<bool>(true));
		}

		private void DisableMainMenu(object sender, EventArgs e)
		{
			SetMainMenu(sender, new EventArgs<bool>(false));
		}

		private void SetMainMenu(object sender, EventArgs<bool> e)
		{
			if (mnuMainMenu.InvokeRequired)
			{
				var caller = new EventHandler<EventArgs<bool>>(SetMainMenu);
				mnuMainMenu.Invoke(caller, sender, e);
			}
			else
				mnuMainMenu.Enabled = e.Value;
		}

		private void EnableUpload(object sender, EventArgs e)
		{
			EnableUpload(sender, new EventArgs<bool>(false));
		}

		private void EnableUpload(object sender, EventArgs<bool> e)
		{
			SetUpload(sender, new EventArgs<bool>(true));
			bool FullVersionOnly = e.Value;

			if (FullVersionOnly)
			{
				SetUpdate(sender, new EventArgs<bool>(false));
				SelectFullVersion(sender, new EventArgs());
			}
			else
			{
				SetUpdate(sender, new EventArgs<bool>(true));
				SelectUpdate(sender, new EventArgs());
			}
		}

		private void DisableUpload(object sender, EventArgs e)
		{
			SetUpload(sender, new EventArgs<bool>(false));
		}

		private void SetUpload(object sender, EventArgs<bool> e)
		{
			if (lblStatus.InvokeRequired)
			{
				var caller = new EventHandler<EventArgs<bool>>(SetUpload);
				lblStatus.Invoke(caller, sender, e);
			}
			else
				grpUpload.Enabled = e.Value;
		}

		private void SetUpdate(object sender, EventArgs<bool> e)
		{
			if (chkUpdate.InvokeRequired)
			{
				var caller = new EventHandler<EventArgs<bool>>(SetUpdate);
				chkUpdate.Invoke(caller, sender, e);
			}
			else
				chkUpdate.Enabled = e.Value;
		}

		private void SelectFullVersion(object sender, EventArgs e)
		{
			if (chkFullVersion.InvokeRequired)
				chkFullVersion.Invoke(new EventHandler<EventArgs>(SelectFullVersion), sender, e);
			else
				chkFullVersion.Checked = true;
		}

		private void SelectUpdate(object sender, EventArgs e)
		{
			if (chkUpdate.InvokeRequired)
				chkUpdate.Invoke(new EventHandler<EventArgs>(SelectUpdate), sender, e);
			else
				chkUpdate.Checked = true;
		}

		private void EnableDownload(object sender, EventArgs e)
		{
			SetDownload(sender, new EventArgs<bool>(true));
		}

		private void DisableDownload(object sender, EventArgs e)
		{
			SetDownload(sender, new EventArgs<bool>(false));
		}

		private void SetDownload(object sender, EventArgs<bool> e)
		{
			if (lblStatus.InvokeRequired)
			{
				var caller = new EventHandler<EventArgs<bool>>(SetDownload);
				lblStatus.Invoke(caller, sender, e);
			}
			else
				grpDownload.Enabled = e.Value;
		}	

		private void SetStatus(object sender, EventArgs<string> e)
		{
			if (lblStatus.InvokeRequired)
			{
				var caller = new EventHandler<EventArgs<string>>(SetStatus);
				lblStatus.Invoke(caller, sender, e);
			}
			else
				lblStatus.Text = e.Value;
		}

		private void SetUploadVersion(object sender, EventArgs<string> e)
		{
			string[] v = new string[] { "0", "0", "0", "0" };
			string version = e.Value;

			if (txtVersion1.InvokeRequired)
			{
				var caller = new EventHandler<EventArgs<string>>(SetUploadVersion);
				txtVersion1.Invoke(caller, sender, e);
			}
			else
			{
				if (version != null)
				{
					string[] tokens = version.Split('.');

					for (int i = 0; tokens.Length == v.Length && i < v.Length; i++)
						v[i] = tokens[i];
				}

				try
				{
					v[3] = (int.Parse(v[3]) + 1).ToString();
				}
				catch (Exception ex)
				{
					ErrorLog.Add(this, ex.Message);
				}

				txtVersion1.Text = v[0];
				txtVersion2.Text = v[1];
				txtVersion3.Text = v[2];
				txtVersion4.Text = v[3];
			}
		}

		private void SetDownloadVersions(object sender, EventArgs<List<string>> e)
		{
			if (cmbDownloadVersions.InvokeRequired)
			{
				var caller = new EventHandler<EventArgs<List<string>>>(SetDownloadVersions);
				cmbDownloadVersions.Invoke(caller, sender, e);
			}
			else
			{
				cmbDownloadVersions.Items.Clear();
				foreach (string version in e.Value)
					cmbDownloadVersions.Items.Add(version);

				if (cmbDownloadVersions.Items.Count > 0)
					cmbDownloadVersions.SelectedIndex = cmbDownloadVersions.Items.Count - 1;
			}
		}

		private void DisableForm()
		{
			DisableDownload(this, new EventArgs());
			DisableUpload(this, new EventArgs());
			DisableMainMenu(this, new EventArgs());
		}

		private void cmdNewVersion_Click(object sender, EventArgs e)
		{
			fbdSelectFolder.ShowDialog();
			txtUploadPath.Text = fbdSelectFolder.SelectedPath;
		}

		private void cmdSavePath_Click(object sender, EventArgs e)
		{
			fbdSelectFolder.ShowDialog();
			txtDownloadPath.Text = fbdSelectFolder.SelectedPath;
		}

		private void cmdUpload_Click(object sender, EventArgs e)
		{
			string version = txtVersion1.Text;
			version += "." + txtVersion2.Text;
			version += "." + txtVersion3.Text;
			version += "." + txtVersion4.Text;

			if (!Directory.Exists(txtUploadPath.Text))
				ShowWarning(this, new EventArgs<string>(Strings.PleaseSelectValidDirectory));
			else if (!manager.CheckNewVersionNr(version))
				ShowWarning(this, new EventArgs<string>(Strings.VersionTooLow));
			else
			{
				DisableForm();
				manager.Upload(txtUploadPath.Text, version, chkFullVersion.Checked);
			}
		}

		private void cmdDownload_Click(object sender, EventArgs e)
		{
			if (txtDownloadPath.Text == "")
			{
				ShowWarning(this, new EventArgs<string>(Strings.PleaseSelectValidDirectory));
				return;
			}

			try
			{
				if (!Directory.Exists(txtDownloadPath.Text))
					Directory.CreateDirectory(txtDownloadPath.Text);
			}
			catch (Exception ex)
			{
				ErrorLog.Add(this, ex.Message);
				ShowWarning(this, new EventArgs<string>(Strings.UnableToCreateLocalDirectory));
				return;
			}

			DisableForm();

			string version = (string)cmbDownloadVersions.Items[cmbDownloadVersions.SelectedIndex];
			manager.Download(txtDownloadPath.Text, version);
		}

		private void chkFullVersion_CheckedChanged(object sender, EventArgs e)
		{
			if (chkFullVersion.Checked)
				chkUpdate.Checked = false;
		}

		private void chkUpdate_CheckedChanged(object sender, EventArgs e)
		{
			if (chkUpdate.Checked)
				chkFullVersion.Checked = false;
		}

		private void mnuExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void mnuSettings_Click(object sender, EventArgs e)
		{
			DialogResult result = manager.Settings.ShowDialog();
			if (result == DialogResult.OK)
			{
				DisableForm();
				manager.Start();
			}
		}

		private void lblStatus_SizeChanged(object sender, EventArgs e)
		{
			int x = this.Width / 2 - lblStatus.Width / 2;
			int y = lblStatus.Location.Y;

			lblStatus.Location = new System.Drawing.Point(x, y);
		}

		private void mnuRefresh_Click(object sender, EventArgs e)
		{
			DisableForm();
			manager.Start();
		}

		private void txtVersion1_KeyDown(object sender, KeyEventArgs e)
		{
			VersionKeyDown(e);
		}

		private void txtVersion2_KeyDown(object sender, KeyEventArgs e)
		{
			VersionKeyDown(e);
		}

		private void txtVersion3_KeyDown(object sender, KeyEventArgs e)
		{
			VersionKeyDown(e);
		}

		private void txtVersion4_KeyDown(object sender, KeyEventArgs e)
		{
			VersionKeyDown(e);
		}

		private void VersionKeyDown(KeyEventArgs e)
		{
			if ("1234567890\b".IndexOf((char)e.KeyCode) < 0)
				e.SuppressKeyPress = true;
		}

		private void txtVersion1_Validating(object sender, CancelEventArgs e)
		{
			VersionCausesValidation(txtVersion1);
		}

		private void txtVersion2_Validating(object sender, CancelEventArgs e)
		{
			VersionCausesValidation(txtVersion2);
		}

		private void txtVersion3_Validating(object sender, CancelEventArgs e)
		{
			VersionCausesValidation(txtVersion3);
		}

		private void txtVersion4_Validating(object sender, CancelEventArgs e)
		{
			VersionCausesValidation(txtVersion4);
		}

		private void VersionCausesValidation(TextBox txtVersion)
		{
			try
			{ 
				int.Parse(txtVersion.Text); 
			}
			catch (Exception) 
			{ 
				txtVersion.Text = "0"; 
			}

			if (txtVersion.TextLength == 2 && txtVersion.Text[0] == '0')
				txtVersion.Text = txtVersion.Text[1].ToString();

			if (txtVersion.Text == "")
				txtVersion.Text = "0";

			if (AllVersionsZero())
				txtVersion4.Text = "1";
		}

		private bool AllVersionsZero()
		{
			return txtVersion1.Text == "0" && txtVersion2.Text == "0" &&
				txtVersion3.Text == "0" && txtVersion4.Text == "0";
		}

		private void txtVersion1_Enter(object sender, EventArgs e)
		{
			txtVersion1.SelectAll();
		}

		private void txtVersion2_Enter(object sender, EventArgs e)
		{
			txtVersion2.SelectAll();
		}

		private void txtVersion3_Enter(object sender, EventArgs e)
		{
			txtVersion3.SelectAll();
		}

		private void txtVersion4_Enter(object sender, EventArgs e)
		{
			txtVersion4.SelectAll();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (manager != null)
				manager.Close(e);
		}
	}
}