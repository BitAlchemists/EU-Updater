using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace EU_Updater
{
	class Updater
	{
		//name of the configuration file of the Updater
		private const string updaterConfigurationFile = "config.xml";

		//events
		public delegate void GetGamePathEventHandler(object sender, EventArgs e);
		public event GetGamePathEventHandler NeedsGamePath;
		public delegate void CloseEventHandler(object sender, EventArgs e);
		public event CloseEventHandler OnClose;
		public delegate void UpdateFinishedEventHandler(object sender, EventArgs e);
		public event UpdateFinishedEventHandler UpdateFinished;
		public delegate void StatusChangedEventHandler(object sender, StatusEventArgs e);
		public event StatusChangedEventHandler StatusChanged;
		public delegate void BackgroundImgChangedEventHandler(object sender, BackgroundImgEventArgs e);
		public event BackgroundImgChangedEventHandler BackgroundImgChanged;

		//sorted list with old files which have to be deleted
		private SortedList<string, string> filesToDelete;
		//sorted list with new files which have to be downloaded and installed
		private SortedList<string, TransferFile> filesToDownload;
		//the network object for the file transfer
		private FileTransfer fileTransfer;
		//the Updater configuration file
		private UpdaterConfig updaterConfig;

		public Updater(FileTransfer fileTransfer)
		{
			this.fileTransfer = fileTransfer;
			filesToDelete = new SortedList<string, string>();
			filesToDownload = new SortedList<string,TransferFile>();
			updaterConfig = new UpdaterConfig();
		}

		//starts the update process
		public void StartUpdater()
		{
			NextAction("initialize"); //first initialize
		}

		//starts the next action
		private void NextAction(string action)
		{
			switch (action)
			{
				case "initialize":
					Initialize();
					break;
				case "getNewUpdater":
					GetNewUpdater();
					break;
				case "getNewGame":
					GetNewGame();
					break;
				default:
					break;
			}
		}

		//initializes the Updater
		private void Initialize()
		{
			StatusChanged(this,new StatusEventArgs("Loading Updater configuration file..."));

			//load Updater configuration file
			if(!updaterConfig.Load(updaterConfigurationFile))
			{
				StatusChanged(this, new StatusEventArgs("Error! Bad Updater configuration file."));
				MessageBox.Show("Error! Bad Updater configuration file.", "Error!", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//delete temporary download folder
			DeleteTempFolder();

			//set background image
			StatusChanged(this,new StatusEventArgs("Loading background..."));
			SetBackgroundImg();

			//set number of the last background image to the next number
			StatusChanged(this,new StatusEventArgs("Writing updater configuration file..."));
			if (!updaterConfig.SaveLastBackgroundImg(updaterConfigurationFile, updaterConfig.NextBackgroundImgNumber))
			{
				StatusChanged(this, new StatusEventArgs("Error! Can't write to Updater configuration file."));
				MessageBox.Show("Error! Can't write to Updater configuration file.", "Error!", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//next action --> get new Updater
			NextAction("getNewUpdater");
		}

		//checks if a new version of the Updater is existing and downloads it
		private void GetNewUpdater()
		{
			//configuration file of the new Updater
			UpdaterConfig newUpdaterConfig = new UpdaterConfig();

			//load configuration file of the new Updater
			StatusChanged(this,new StatusEventArgs("Loading new Updater configuration file..."));
			if (!newUpdaterConfig.Load(updaterConfig.AbsUpdaterPathOnServer + 
				updaterConfigurationFile))
			{
				StatusChanged(this,new StatusEventArgs("Error! Bad Updater configuration file on server."));
				MessageBox.Show("Error! Bad Updater configuration file on server.", "Error!", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//check if a new version of the Updater is existing
			StatusChanged(this,new StatusEventArgs("Checking version..."));
			if (Version.ToLong(newUpdaterConfig.VersionNumber) > Version.ToLong(updaterConfig.VersionNumber))
			{
				//load files into download list
				StatusChanged(this,new StatusEventArgs("Creating file list..."));
				if (!LoadUpdaterFilesIntoDownloadList(newUpdaterConfig))
				{
					StatusChanged(this,new StatusEventArgs("Error on creating file list for self-update"));
					MessageBox.Show("Error on creating file list for self-update.", "Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					OnClose(this, EventArgs.Empty);
					return;
				}

				//create the temporary download folder
				StatusChanged(this,new StatusEventArgs("Creating temporary download directory..."));
				if (!CreateTempFolder())
				{
					StatusChanged(this,new StatusEventArgs("Error on creating temporary download directory..."));
					MessageBox.Show("Error on creating temporary download directory...", "Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					OnClose(this, EventArgs.Empty);
					return;
				}

				//start download
				StatusChanged(this,new StatusEventArgs("Downloading..."));
				fileTransfer.DownloadFilesAsync(filesToDownload.Values.ToList(), true,
					new Uri(updaterConfig.AbsUpdaterPathOnServer), updaterConfig.TempFolder + "/",
					delegate(FileTransferResult result) //callback function after download is completed
					{
						filesToDownload.Clear();

						//download successful
						if (result == FileTransferResult.Success)
						{
							InstallUpdater();
							NextAction("getNewGame");
						}
						//download unsuccessful
						else
						{
							MessageBox.Show("Download Failure! Please try again.",
								"Failure!",MessageBoxButtons.OK,MessageBoxIcon.Error);
							OnClose(this, EventArgs.Empty);
						}
					}
				);
			}
			else //if no new version of the Updater was found
				NextAction("getNewGame");
		}

		//loads files for the new Updater version into the download list
		private bool LoadUpdaterFilesIntoDownloadList(UpdaterConfig newUpdaterConfig)
		{
			long size;
			string serverPath;
			string file;

			try
			{
				serverPath = updaterConfig.AbsUpdaterPathOnServer;

				//if the background images shall be updated too
				if (newUpdaterConfig.UpdatePictures)
				{
					//get all background images
					foreach (string imagePath in newUpdaterConfig.BackgroundImgList.Values)
					{
						size = GetFileSize(serverPath + imagePath);
						filesToDownload.Add(imagePath, new TransferFile(imagePath, size, imagePath));
					}
				}

				//get executeable of the Updater
				file = newUpdaterConfig.UpdaterExe;
				size = GetFileSize(serverPath + file);
				filesToDownload.Add(file, new TransferFile(file, size, file));

				//get the configuration file of the Updater
				file = updaterConfigurationFile;
				size = GetFileSize(serverPath + file);
				filesToDownload.Add(file, new TransferFile(file, size, file));

				//get the executeable of the Installer
				file = newUpdaterConfig.InstallerExe;
				size = GetFileSize(serverPath + file);
				filesToDownload.Add(file, new TransferFile(file, size, file));
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		//determines the size of a file from the server through http
		private long GetFileSize(string uri)
		{
			HttpWebResponse response;
			HttpWebRequest request;
			long size = -1;

			try
			{
				request = (HttpWebRequest)WebRequest.Create(uri);
				request.Method = WebRequestMethods.Http.Head;
				response = (HttpWebResponse)request.GetResponse();
				size = response.ContentLength;
				response.Close();
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return -1;
			}

			return size;
		}

		//fires the event BackgroundImgChanged
		private void SetBackgroundImg()
		{
			string file;

			file = updaterConfig.BackgroundImgList[updaterConfig.NextBackgroundImgNumber];

			if (File.Exists(file))
				BackgroundImgChanged(this, new BackgroundImgEventArgs(file));
		}

		//deletes the temporary download directory
		private bool DeleteTempFolder()
		{
			try
			{
				if (Directory.Exists(updaterConfig.TempFolder))
					Directory.Delete(updaterConfig.TempFolder, true);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		//creates the temporary download directory
		private bool CreateTempFolder()
		{
			try
			{
				DeleteTempFolder();
				Directory.CreateDirectory(updaterConfig.TempFolder);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		/* set the game path and checks if it is existing
		 * if not the event NeedsGamePath is fired */
		public void SetGamePath(string gamePath)
		{
			Thread th;

			try
			{
				if (gamePath != null && Directory.Exists(gamePath))
				{
					updaterConfig.SaveGamePath(updaterConfigurationFile, gamePath);
					th = new Thread(new ThreadStart(GetNewGame));
					th.IsBackground = true;
					th.Start();
				}
				else
					NeedsGamePath(this, EventArgs.Empty);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
			}
		}

		//checks if a new game version is existing and downloads it
		private void GetNewGame()
		{
			bool needsFullVersion;
			GameConfig gameConfig = new GameConfig();
			VersionsConfig versionsConfig = new VersionsConfig();
			DialogResult res;

			//check if there is a game path directory
			StatusChanged(this,new StatusEventArgs("Checking game path..."));
			if (!Directory.Exists(updaterConfig.GamePath))
			{
				NeedsGamePath(this, EventArgs.Empty);
				return;
			}

			//load the configuration file of the game
			StatusChanged(this,new StatusEventArgs("Loading game configuration file..."));
			//if there is NO configuration file a full version is needed
			if (!gameConfig.Load(updaterConfig.GamePath + updaterConfig.GameConfigurationFile))
			{
				StatusChanged(this, new StatusEventArgs("No game found in selected directory!"));
				res = MessageBox.Show("In the chosen folder was no game found. Shall it be installed into " +
					"the selected directory?", "Install?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.No)
				{
					NeedsGamePath(this, EventArgs.Empty);
					return;
				}
				needsFullVersion = true;
			}
			//if there is A configuration file an update is needed
			else
				needsFullVersion = false;

			//load the new versions available on the server
			StatusChanged(this,new StatusEventArgs("Checking new game versions..."));
			versionsConfig.LoadNewVersions(updaterConfig.ServerPath + updaterConfig.VersionsConfigurationFile,
				gameConfig.VersionNumber, needsFullVersion);

			//if there are new versions
			if (versionsConfig.VersionsForUpdate.Count == 0)
			{
				if (gameConfig.VersionNumber == GameConfig.DefaultVersionNumber)
				{
					StatusChanged(this, new StatusEventArgs("Unable to install. No version found on server."));
					MessageBox.Show("Unable to install. No version found on server.", "No version found",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					OnClose(this, EventArgs.Empty);
				}
				else
				{
					StatusChanged(this, new StatusEventArgs("No new Versions found!"));
					UpdateFinished(this, EventArgs.Empty);
				}
				return;
			}

			//load files for download into list
			StatusChanged(this,new StatusEventArgs("Creating file lists..."));
			if (!LoadGameFilesIntoDownloadList(versionsConfig))
			{
				StatusChanged(this,new StatusEventArgs("Error on creating file lists for the game!"));
				MessageBox.Show("Error on creating file lists for the game!", "Error!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//create temporary download directory
			StatusChanged(this,new StatusEventArgs("Creating temporary download directory..."));
			if (!CreateTempFolder())
			{
				StatusChanged(this,new StatusEventArgs("Error on creating temporary download directory!"));
				MessageBox.Show("Error on creating temporary download directory!", "Error!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//start the download
			StatusChanged(this,new StatusEventArgs("Downloading..."));
			fileTransfer.DownloadFilesAsync(filesToDownload.Values.ToList(), true,
				new Uri(updaterConfig.ServerPath), updaterConfig.TempFolder + "/",
				delegate(FileTransferResult result) //callback function after download is completed
				{
					filesToDownload.Clear();
					//download successful
					if (result == FileTransferResult.Success)
						InstallGame(versionsConfig.LatestUpdate);
					//download unsuccessful
					else
					{
						MessageBox.Show("Download Failure! Please Try Again",
							"Failure!",MessageBoxButtons.OK, MessageBoxIcon.Error);
						OnClose(this, EventArgs.Empty);
					}
				}
			);
		}

		//loads files for the new game version into the download list
		private bool LoadGameFilesIntoDownloadList(VersionsConfig versionsConfig)
		{
			XmlDocument docUpdate = new XmlDocument();
			XmlNode newFilesNode;
			XmlNode deletedFilesNode;
			long fileSize, version;
			string filePath, versionNumber;

			try
			{
				//iterate through the new version from the end to the beginning
				for (int i = versionsConfig.VersionsForUpdate.Keys.Count - 1; i >= 0; i--)
				{
					//set the key
					version = versionsConfig.VersionsForUpdate.Keys[i];
					//set the version number
					versionNumber = versionsConfig.VersionsForUpdate[version].VersionNumber;

					//if the version is a full version
					if (versionsConfig.VersionsForUpdate[version].IsFullVersion)
					{
						//load the configuration file of the full version
						docUpdate.Load(updaterConfig.ServerPath + versionNumber + "/" + 
							updaterConfig.FullVersionConfigurationFile);
						newFilesNode = docUpdate.SelectSingleNode("/files");
					}
					//if the version is an update
					else
					{
						//load the configuration file of the update
						docUpdate.Load(updaterConfig.ServerPath + versionNumber + "/" + 
							updaterConfig.UpdateConfigurationFile);
						newFilesNode = docUpdate.SelectSingleNode("/update/newFiles");
					}

					//if new files are existing
					if (newFilesNode != null)
					{
						//iterate through the new files and add them to the installation list if needed
						foreach (XmlNode file in newFilesNode.ChildNodes)
						{
							filePath = file.Attributes["path"].Value;
							fileSize = long.Parse(file.SelectSingleNode("size").InnerText);
							if (!filesToDownload.ContainsKey(filePath) && !filesToDelete.ContainsKey(filePath))
								filesToDownload.Add(filePath,
									new TransferFile(versionNumber + "/" + filePath, fileSize, filePath));
						}
					}

					//if old files are existing
					deletedFilesNode = docUpdate.SelectSingleNode("/update/deletedFiles");
					if (deletedFilesNode != null)
					{
						//iterate through the old files and add them to the delete list if not needed anymore
						foreach (XmlNode file in deletedFilesNode.ChildNodes)
						{
							filePath = file.Attributes["path"].Value;
							if (!filesToDownload.ContainsKey(filePath) && !filesToDelete.ContainsKey(filePath))
								filesToDelete.Add(filePath, versionNumber + "/" + filePath);
						}
					}
				}
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		//starts the Installer application for the self-update of the Updater
		private void InstallUpdater()
		{
			StatusChanged(this,new StatusEventArgs("Starting installation application..."));
			OnClose(this, EventArgs.Empty);
			Process.Start(Environment.CurrentDirectory + "\\" + updaterConfig.TempFolder + "\\" + 
				updaterConfig.InstallerExe,
				Process.GetCurrentProcess().Id.ToString() + " " + updaterConfig.TempFolder);
		}

		//installs the new game version
		private void InstallGame(string latestUpdate)
		{
			GameConfig gameConfig = new GameConfig();

			//move new files to game direcotry
			StatusChanged(this,new StatusEventArgs("Installing new files..."));
			if (!MoveFilesToGameFolder(updaterConfig.TempFolder))
			{
				StatusChanged(this,
					new StatusEventArgs("Error on moving files to game directory! Installation not completed"));
				MessageBox.Show("Error on moving files to game directory! Installation not completed.", "Error!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//delete old files from game directory
			StatusChanged(this,new StatusEventArgs("Deleting old files..."));
			if (!DeleteOldFiles())
			{
				StatusChanged(this,new StatusEventArgs("Error on deleting old files! Installation not completed."));
				MessageBox.Show("Error on deleting old files! Installation not completed.", "Error!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//save the new game configuration file
			StatusChanged(this,new StatusEventArgs("Writing game configuration file..."));
			if (!gameConfig.Save(updaterConfig.GamePath + updaterConfig.GameConfigurationFile, latestUpdate))
			{
				StatusChanged(this,
					new StatusEventArgs("Error writing game configuration file! Installation not completed."));
				MessageBox.Show("Error writing game configuration file! Installation not completed.", "Error!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				OnClose(this, EventArgs.Empty);
				return;
			}

			//delete the temporary download directory
			if (!DeleteTempFolder())
				StatusChanged(this,
					new StatusEventArgs("Error on deleting temporary download directory! But installation completed."));
			else
				StatusChanged(this,new StatusEventArgs("Update completed!"));

			//fire event UpdateFinished
			UpdateFinished(this, EventArgs.Empty);
		}

		//moves the downloaded files recursive to the game directory
		private bool MoveFilesToGameFolder(string source)
		{
			string subPath, destPath;
			int index = -1;

			try
			{
				//creates the directory if it is not existing
				index = source.IndexOf('\\');
				if (index != -1)
				{
					subPath = source.Substring(index + 1);
					if (!Directory.Exists(updaterConfig.GamePath + subPath))
						Directory.CreateDirectory(updaterConfig.GamePath + subPath);
				}

				//iterate through the directories
				foreach (string dir in Directory.GetDirectories(source))
					MoveFilesToGameFolder(dir);

				//iterate through the new files and install them
				foreach (string file in Directory.GetFiles(source))
				{
					destPath = updaterConfig.GamePath + file.Substring(file.IndexOf('\\') + 1);
					if (File.Exists(destPath))
						File.Delete(destPath);
					File.Move(file, destPath);
				}
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		//deletes the old files from the game directory
		private bool DeleteOldFiles()
		{
			try
			{
				//iterate through the old files and delete them
				foreach (string file in filesToDelete.Keys)
				{
					if (File.Exists(updaterConfig.GamePath + file))
						File.Delete(updaterConfig.GamePath + file);
				}
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}
	}
}