using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net.Sockets;

namespace EU_Release_Manager.Controller
{
	public enum TestSettingsResult { Success = 0, Timeout = 1, InvalidLogin = 2, InvalidDirectory = 3 };

	public partial class Settings : Form
	{
		// local config file
		private const string configPath = "euu_config.xml";
		
		private bool cancelClosing;

		public string Path { get; set; }
		public string User { get; set; }
		public string Password { get; set; }
		public bool Passive { get; set; }
		public bool Loaded { get; set; }

		public Settings()
		{
			InitializeComponent();
		}

		// tests settings (server-address, username, password, remote-directory)
		public TestSettingsResult TestSettings()
		{
			string hostname = Path.Substring(0, Path.IndexOf('/'));
			string directory = Path.Substring(Path.IndexOf('/'));
			TcpClient client = new TcpClient();
			client.ReceiveTimeout = 5000;

			try
			{
				// connect to the server
				client.Connect(hostname, 21);
				
				NetworkStream stream = client.GetStream();
				StreamReader reader = new StreamReader(stream);
				StreamWriter writer = new StreamWriter(stream);
				writer.AutoFlush = true;

				// receive welcome message
				if (!reader.ReadLine().StartsWith("220"))
				{
					stream.Close();
					return TestSettingsResult.Timeout;
				}
				
				// send USER command
				writer.WriteLine("USER " + User);
				// receive answer
				if (!reader.ReadLine().StartsWith("331"))
				{
					stream.Close();
					return TestSettingsResult.InvalidLogin;
				}

				// send PASS command
				writer.WriteLine("PASS " + Password);
				// receive answer
				if (!reader.ReadLine().StartsWith("230"))
				{
					stream.Close();
					return TestSettingsResult.InvalidLogin;
				}
				
				// send CWD command
				writer.WriteLine("CWD " + directory);
				// receive answer
				if (!reader.ReadLine().StartsWith("250"))
				{
					stream.Close();
					return TestSettingsResult.InvalidDirectory;
				}
				
				// send QUIT command
				writer.WriteLine("QUIT");
				stream.Close();
				return TestSettingsResult.Success;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return TestSettingsResult.Timeout;
			}
		}

		// loads settings from config file
		public bool LoadSettings()
		{
			XmlDocument config = new XmlDocument();
			XmlNode nodePath, nodeUser, nodePassword, nodePassive;
			bool valid = false;

            if (File.Exists(configPath))
            {
			    try
			    {
				    // load config file
				    config.Load(configPath);

				    // read xml nodes
				    nodePath = config.SelectSingleNode("//path");
				    nodeUser = config.SelectSingleNode("//user");
				    nodePassword = config.SelectSingleNode("//password");
				    nodePassive = config.SelectSingleNode("//passive");

				    Path = (nodePath != null) ? nodePath.InnerText : null;
				    User = (nodeUser != null) ? nodeUser.InnerText : null;
				    Password = (nodePassword != null) ? nodePassword.InnerText : null;
				    Passive = (nodePassive != null) ? (nodePassive.InnerText == "true") : true;

				    // set textboxes
				    txtPath.Text = Path;
				    txtUser.Text = User;
				    txtPassword.Text = Password;
				    chkPassive.Checked = Passive;

				    valid = (Path != null) && (User != null) && (Password != null);
			    }
			    catch (Exception e)
			    {
				    ErrorLog.Add(this, e.Message);
			    }
            }

			// if config is invalid, show settings dialog
			if (!valid)
			{
				this.ShowDialog();
				valid = (this.DialogResult == DialogResult.OK);
			}

			Loaded = valid;

			return valid;
		}

		// writes settings to config file
		private bool WriteSettings()
		{
			XmlDocument config = new XmlDocument();
			XmlNode root, nodePath, nodeUser, nodePassword, nodePassive;

			nodePath = config.CreateElement("path");
			nodePath.InnerText = Path;

			nodeUser = config.CreateElement("user");
			nodeUser.InnerText = User;

			nodePassword = config.CreateElement("password");
			nodePassword.InnerText = Password;

			nodePassive = config.CreateElement("passive");
			nodePassive.InnerText = (Passive) ? "true" : "false";

			root = config.CreateElement("config");
			root.AppendChild(nodePath);
			root.AppendChild(nodeUser);
			root.AppendChild(nodePassword);
			root.AppendChild(nodePassive);

			config.AppendChild(root);

			try
			{
				config.Save(configPath);
				return true;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			// check textboxes
			if (txtPath.Text != "" && txtUser.Text != "" && txtPassword.Text != "")
			{
				// set properties (settings)
				Path = txtPath.Text;
				User = txtUser.Text;
				Password = txtPassword.Text;
				Passive = chkPassive.Checked;

				if (!Path.EndsWith("/"))
					Path += "/";

				// write settings to config file
				WriteSettings();
			}
			else
			{
				MessageBox.Show(Strings.FillOutAllFields, Strings.Settings, 
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				cancelClosing = true;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

            if (!cancelClosing)
            {
                txtPath.Text = Path;
                txtUser.Text = User;
                txtPassword.Text = Password;
                chkPassive.Checked = Passive;
            }

			e.Cancel = cancelClosing;
			cancelClosing = false;
		}
	}
}
