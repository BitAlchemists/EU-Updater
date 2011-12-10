using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace EU_Updater
{
	public partial class UpdaterForm : Form
	{
		private delegate void DelegateEnablePlayButton(object sender, EventArgs e);
		private delegate void DelegateCloseForm(object sender, EventArgs e);
		private delegate void DelegateSetStatus(object sender, StatusEventArgs e);
		private delegate void DelegateGetGamePath(object sender, EventArgs e);
		private bool askBeforeClosing;

		public UpdaterForm()
		{
			Thread thStartUpdater;		
			Updater updater;
			
			InitializeComponent();

			askBeforeClosing = true;
			cmdPlay.Enabled = false;
			try
			{
				updater = new Updater(ftDownload);

				//event listeners
				updater.NeedsGamePath += new Updater.GetGamePathEventHandler(GetGamePath);
				updater.OnClose += new Updater.CloseEventHandler(CloseForm);
				updater.UpdateFinished += new Updater.UpdateFinishedEventHandler(EnablePlayButton);
				updater.StatusChanged += new Updater.StatusChangedEventHandler(SetStatus);
				updater.BackgroundImgChanged += new Updater.BackgroundImgChangedEventHandler(SetBackgroundImg);

				//start the udpater in a new thread
				thStartUpdater = new Thread(new ThreadStart(updater.StartUpdater));
				thStartUpdater.IsBackground = true;
				thStartUpdater.Start();
			}
			catch (Exception e)
			{
				ErrorLog.Add(this,e.Message);
			}
		}

		//closes the form
		public void CloseForm(object sender, EventArgs e)
		{
			if (InvokeRequired)
				this.Invoke(new DelegateCloseForm(CloseForm), new object[] { sender, e });
			else
			{
				askBeforeClosing = false;
				Close();
			}
		}

		//enables the play button and disables the cancel button
		public void EnablePlayButton(object sender, EventArgs e)
		{
			if (InvokeRequired)
				cmdPlay.Invoke(new DelegateEnablePlayButton(EnablePlayButton), new object[] { sender, e});
			else
			{
				cmdPlay.Enabled = true;
				cmdCancel.Enabled = false;
			}
		}

		//shows an abort question
		public bool AbortUpdate()
		{
			DialogResult res = MessageBox.Show("Do you want to cancel the update?", "Cancel?",
						MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			return res == DialogResult.Yes;
		}

		//shows an abort question and displays the parameter string text first
		public bool AbortUpdate(string text)
		{
			DialogResult res = MessageBox.Show(text + "Do you want to cancel the update?", "Cancel?",
						MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			return res == DialogResult.Yes;
		}

		//opens a dialog for selecting the game path
		public void GetGamePath(object sender, EventArgs e)
		{
			DialogResult res;

			if (InvokeRequired)
				this.Invoke(new DelegateGetGamePath(GetGamePath),new object[] {sender, e});
			else
			{
				res = fbdGamePath.ShowDialog();
				if (res == DialogResult.Cancel)
				{
					if (AbortUpdate("The game direcotry could not be found!\n"))
					{
						askBeforeClosing = false;
						Close();
					}
					else
						GetGamePath(sender, EventArgs.Empty);
				}
				else
					((Updater)sender).SetGamePath(fbdGamePath.SelectedPath);
			}
		}

		//sets the status message
		public void SetStatus(object sender, EventArgs e)
		{
			if (lblStatus.InvokeRequired)
				lblStatus.Invoke(new DelegateSetStatus(SetStatus), new object[] { sender, e });
			else
				lblStatus.Text = ((StatusEventArgs)e).Text;
		}

		//sets the background image
		public void SetBackgroundImg(object sender, EventArgs e)
		{
			this.BackgroundImage = Image.FromFile(((BackgroundImgEventArgs)e).ImagePath);
		}

		//cancels the update
		private void cmdCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		//starts game
		private void cmdPlay_Click(object sender, EventArgs e)
		{

		}

		//asks the user if the updater shall really be closed
		private void UpdaterForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(askBeforeClosing)
				e.Cancel = !AbortUpdate();
		}
	}
}
