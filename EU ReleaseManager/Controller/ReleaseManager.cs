using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;
using EU_Release_Manager.Network;
using EU_Release_Manager.Controller.XML;

namespace EU_Release_Manager.Controller
{
	public class ReleaseManager : IReleaseManager
	{
		// configuration
		private const string localPath = "euu_temp/";
		private const string protocol = "ftp://";
		private const string globalConfig = "versions.xml";
		private const string fullVersionConfig = "euu_FullVersion.xml";
		private const string updateConfig = "euu_Update.xml";
		private const string lockFile = "lock";

		// class members
		private IFileTransfer fileTransfer;
		private Uri remotePath;
		private XmlVersions versions;
		private bool serverLock;
		private bool exit;
		private bool connected;

		// upload
		private string ulLocalPath;
		private Dictionary<string, TransferFile> ulLocalFiles;
		private string ulVersion;
		private bool ulFullVersion;
		private UploadStatus ulStatus;
		private List<TransferFile> ulFiles;
		private List<TransferFile> ulNewFiles;
		private List<TransferFile> ulDeletedFiles;

		// download
		private List<TransferFile> dlFiles;
		private string dlVersion;
		private List<string> dlUpdates;
		private string dlFullVersion;
		private string dlLocalPath;
		private DownloadStatus dlStatus;

		// callback delegates
		private delegate void DelegateTransfer(bool value);
		private delegate TestSettingsResult TestSettingsAsync();
		private delegate Dictionary<string, TransferFile> GetLocalFilesAsync(string path);
		private delegate void BuildTransferListAsync();

		// states while uploading
		private enum UploadStatus
		{
			CheckLock = 0,
			LockServer = 1,
			DownloadGlobalConfig = 2,
			AnalyseNewVersion = 3,
			CreateDirectory = 4,
			DownloadVersionConfigs = 5,
			BuildUploadList = 6,
			UploadFiles = 7,
			UploadVersionConfigs = 8,
			UploadGlobalConfig = 9,
			Start = 10,
			DeleteDirectory = 11,
		}

		// states while downloading
		private enum DownloadStatus
		{
			Start = 0,
			DownloadVersionConfigs = 2,
			BuildDownloadList = 3,
			DownloadFiles = 4
		}

		// properties
		public Settings Settings { get; set; }
		public bool Valid { get; set; }

		// events
		public event EventHandler<EventArgs<string>> Status;
		public event EventHandler<EventArgs<string>> Error;
		public event EventHandler<EventArgs<string>> Warning;
		public event EventHandler<EventArgs<string>> Information;
		public event EventHandler<EventArgs> Exit;
		public event EventHandler<EventArgs> MainMenu;
		public event EventHandler<EventArgs<string>> UploadVersion;
		public event EventHandler<EventArgs<List<string>>> DownloadVersions;
		public event EventHandler<EventArgs<bool>> EnableUpload;
		public event EventHandler<EventArgs> EnableDownload;
		public event EventHandler<EventArgs> DisableUpload;
		public event EventHandler<EventArgs> DisableDownload;

		public ReleaseManager(FileTransfer fileTransfer)
		{
			this.fileTransfer = fileTransfer;

			// setting object loads settings from file or displays form
			Settings = new Settings();

			versions = new XmlVersions();

			try
			{
				// create temp folder (if not exists)
				if (!Directory.Exists(localPath))
					Directory.CreateDirectory(localPath);

				Valid = true;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);

				// fire error event
				FireError(Strings.UnableToCreateLocalDirectory);
			}
		}

		// tests settings (serverpath & login)
		public void Start()
		{
			connected = false;
			versions.Clear();
			
			// try to get the settings from settings object
			if (!Settings.Loaded && !Settings.LoadSettings())
				FireStatus(Strings.NoSettingsFound);
			else
			{
				FireStatus(Strings.Connecting);

				// test settings async
				var caller = new TestSettingsAsync(Settings.TestSettings);
				caller.BeginInvoke(new AsyncCallback(TestSettingsCallback), caller);
			}
		}

		// callback of async settings test
		private void TestSettingsCallback(IAsyncResult ar)
		{
			TestSettingsAsync caller = (TestSettingsAsync)ar.AsyncState;
			TestSettingsResult result = caller.EndInvoke(ar);

			// check the result of the settings test
			if (result != TestSettingsResult.Success)
			{
				if (result == TestSettingsResult.Timeout)
					FireStatus(Strings.Timeout);
				else if (result == TestSettingsResult.InvalidLogin)
					FireStatus(Strings.InvalidLogin);
				else if (result == TestSettingsResult.InvalidDirectory)
					FireStatus(Strings.InvalidDirectory);

				// fire some events
				ResetInterface();
			}
			else
				Init();
		}

		// downloads and handles the global config file from the ftp server
		private void Init()
		{
			connected = true;

			// set remote path (e.g. "ftp://www.example.com/myFolder")
			remotePath = new Uri(protocol + Settings.Path);

			// set credentials (username, password, passive mode)
			fileTransfer.Client.Credentials = new NetworkCredential(Settings.User, Settings.Password);
			fileTransfer.Client.UsePassive = Settings.Passive;

			// download global config async
			TransferFile file = new TransferFile(globalConfig);
			fileTransfer.DownloadFileAsync(file, false, remotePath, localPath,
				// callback of the download
				delegate(FileTransferResult result)
				{
                    if (exit)
                        FireExit();
                    else
                    {
                        // versions object handels global config file if download has been successful
                        if (result == FileTransferResult.Success)
                            versions.Load(localPath + globalConfig);

                        // fire some events
                        ResetInterface();
                    }
				});	
		}

		// creates a new update (or fullversion) and uploads it
		public void Upload(string path, string version, bool FullVersion)
		{
			ulLocalPath = (path.EndsWith("\\")) ? path : path + "\\";
			ulVersion = version;
			ulFullVersion = FullVersion;
			ulStatus = UploadStatus.Start;
			ulNewFiles = new List<TransferFile>();
			ulDeletedFiles = new List<TransferFile>();
			ulFiles = new List<TransferFile>();

			Upload(true);
		}

		// main upload method
		private void Upload(bool continueUpload)
		{
			// this method is called after every step of the upload progress
			var callback = new DelegateTransfer(Upload);

			// check if last step has been finished successfully
			if (continueUpload)
			{
				// check status of the upload progress
				switch (ulStatus)
				{
					case UploadStatus.Start:
						// step 1: check if server is locked
						FireStatus(Strings.Connecting);
						ulStatus = UploadStatus.CheckLock;
						CheckLock(callback); break;
					case UploadStatus.CheckLock:
						// step 2: lock the server
						ulStatus = UploadStatus.LockServer;
						LockServer(callback); break;
					case UploadStatus.LockServer:
						// step 3: download global config file
						ulStatus = UploadStatus.DownloadGlobalConfig;
						DownloadGlobalConfig(callback); break;
					case UploadStatus.DownloadGlobalConfig:
						// step 4: analyse local version (create local file list)
						FireStatus(Strings.AnalysingNewVersion);
						ulStatus = UploadStatus.AnalyseNewVersion;
						AnalyzeNewVersion(callback); break;
					case UploadStatus.AnalyseNewVersion:
						// step 5: create remote directory
						ulStatus = UploadStatus.CreateDirectory;
						CreateDirectory(callback);
						break;	
					case UploadStatus.CreateDirectory:
						// step 6: check if there are versions on the server
						if (versions.LatestVersion != null)
						{
							// step 7a: download all version configs
							FireStatus(Strings.DownloadingConfigurationFiles);
							ulStatus = UploadStatus.DownloadVersionConfigs;
							DownloadVersionConfigs(callback);
						}
						else
						{
							// step 7b.1: upload local files
							FireStatus(Strings.UploadingFiles);
							ulStatus = UploadStatus.UploadFiles;
							ulFiles = ulLocalFiles.Values.ToList<TransferFile>();
							UploadFiles(callback);
						}
						break;
					case UploadStatus.DownloadVersionConfigs:
						// step 7b.2: build lists of files to upload and files to delete
						FireStatus(Strings.BuildingConfiguration);
						ulStatus = UploadStatus.BuildUploadList;
						BuildUploadListAsync(callback); break;
					case UploadStatus.BuildUploadList:
						// step 7b.3: upload local files
						FireStatus(Strings.UploadingFiles);
						ulStatus = UploadStatus.UploadFiles;
						UploadFiles(callback); break;
					case UploadStatus.UploadFiles:
						// step 8: upload new version config
						ulStatus = UploadStatus.UploadVersionConfigs;
						UploadVersionConfigs(callback); break;
					case UploadStatus.UploadVersionConfigs:
						// step 9: upload global config
						ulStatus = UploadStatus.UploadGlobalConfig;
						UploadGlobalConfig(callback); break;
					case UploadStatus.UploadGlobalConfig:
						// step 10: unlock server and inform the gui
						FireInformation(Strings.UploadCompleted);
						fileTransfer.ClearProgress();
						UnlockServer();
						ResetInterface(); break;
				}
			}
			// a step hasn't been finished successfully: upload aborted
			else
			{
				// fire an event to inform the GUI about the error
				switch (ulStatus)
				{
					case UploadStatus.CheckLock:
						FireWarning(Strings.CurrentlyLocked); 
						break;
					case UploadStatus.LockServer:
						FireError(Strings.UnableToLock); 
						break;
					case UploadStatus.DownloadGlobalConfig:
						FireWarning(Strings.VersionTooLow); 
						break;
					case UploadStatus.AnalyseNewVersion:
						FireError(Strings.UnableToAnalyzeNewVersion); 
						break;
					case UploadStatus.CreateDirectory:
						FireError(Strings.UnableToCreateRemoteDirectory); 
						break;
					case UploadStatus.DownloadVersionConfigs:
						FireError(Strings.UnableToDownloadConfigurationFiles);
						break;
					case UploadStatus.BuildUploadList:
						FireError(Strings.UnableToBuildConfiguration);
						break;
					case UploadStatus.UploadFiles:
					case UploadStatus.UploadVersionConfigs:
					case UploadStatus.UploadGlobalConfig:
						FireWarning(Strings.UploadFailed);
						fileTransfer.ClearProgress(); 
						break;
				}

				// if remote directory was created, delete it
				switch (ulStatus)
				{
					case UploadStatus.DownloadVersionConfigs:
					case UploadStatus.BuildUploadList:
					case UploadStatus.UploadFiles:
					case UploadStatus.UploadVersionConfigs:
					case UploadStatus.UploadGlobalConfig:
						FireStatus(Strings.CleaningUp);
						DeleteRemoteDirectory();
						break;
				}

				// unlock the server
				UnlockServer();

				if (exit)
					// if program should quit, fire exit event
					FireExit();
				else
					// otherwise restart (test settings & download global config)
					Start();
			}
		}

		// downloads a given version from the server
		public void Download(string path, string version)
		{
			dlLocalPath = (path.EndsWith("\\") ? path : path + "\\");
			dlVersion = version;
			dlFullVersion = versions.GetFullVersion(version);
			dlUpdates = versions.OrderVersions(versions.GetUpdates(version, dlFullVersion), true);
			dlStatus = DownloadStatus.Start;
			dlFiles = new List<TransferFile>();

			Download(true);
		}

		// main download method
		private void Download(bool continueDownload)
		{
			// this method is called after every step of the download progress
			var callback = new DelegateTransfer(Download);

			// check if last step has been finished successfully
			if (continueDownload)
			{
				switch (dlStatus)
				{
					case DownloadStatus.Start:
						// step 1: download version configs
						FireStatus(Strings.DownloadingConfigurationFiles);
						dlStatus = DownloadStatus.DownloadVersionConfigs;
						DownloadVersionConfigs(callback, dlUpdates, dlFullVersion);
						break;
					case DownloadStatus.DownloadVersionConfigs:
						// step 2: build list of files to download
						FireStatus(Strings.BuildingDownloadList);
						dlStatus = DownloadStatus.BuildDownloadList;
						BuildDownloadListAsync(callback);
						break;
					case DownloadStatus.BuildDownloadList:
						// step 3: download remote files
						FireStatus(Strings.DownloadingFiles);
						dlStatus = DownloadStatus.DownloadFiles;
						DownloadFiles(callback);
						break;
					case DownloadStatus.DownloadFiles:
						// step 4: inform the gui
						FireInformation(Strings.DownloadCompleted);
						fileTransfer.ClearProgress();
						ResetInterface();
						break;
				}
			}
			// a step hasn't been finished successfully: download aborted
			else
			{
				// fire an event to inform the GUI about the error
				switch (dlStatus)
				{
					case DownloadStatus.DownloadVersionConfigs:
						FireWarning(Strings.UnableToDownloadConfigurationFiles);
						break;
					case DownloadStatus.BuildDownloadList:
						FireWarning(Strings.UnableToBuildDownloadList);
						break;
					case DownloadStatus.DownloadFiles:
						FireWarning(Strings.DownloadFailed);
						fileTransfer.ClearProgress();
						break;
				}

				if (exit)
					// if program should quit, fire exit event
					FireExit();
				else
					// othewise restart (test settings & download global config)
					Start();
			}
		}

		// checks if new version number is valid
		public bool CheckNewVersionNr(string version)
		{
			return versions.CheckNewVersionNr(version);
		}

		// checks if server is locked (async)
		private void CheckLock(DelegateTransfer callback)
		{
			TransferFile file = new TransferFile(lockFile);

			// try to download lock file
			fileTransfer.DownloadFileAsync(file, false, remotePath, localPath,
				delegate(FileTransferResult result)
				{
                    if (!exit)
                        callback(result == FileTransferResult.Failure);
                    else
                        FireExit();
				});
		}

		// locks the server (async)
		private void LockServer(DelegateTransfer callback)
		{
			try
			{
				// create local lockfile
				if (!File.Exists(localPath + lockFile))
					File.Create(localPath + lockFile);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				callback(false);
				return;
			}

			// try to upload the lockfile
			TransferFile file = new TransferFile(lockFile);
			fileTransfer.UploadFileAsync(file, false, remotePath, localPath,
				delegate(FileTransferResult result)
				{
					serverLock = (result == FileTransferResult.Success);
					callback(serverLock);
				});
		}

		// downloads global config file and checks new version (async)
		private void DownloadGlobalConfig(DelegateTransfer callback)
		{
			TransferFile file = new TransferFile(globalConfig);

			// try to download global config file
			fileTransfer.DownloadFileAsync(file, false, remotePath, localPath,
				delegate(FileTransferResult result)
				{
					if (result == FileTransferResult.Success)
						versions.Load(localPath + globalConfig);

					// check new version number
					callback(versions.CheckNewVersionNr(ulVersion));
				});	
		}

		// creates a list of all local files (async)
		private void AnalyzeNewVersion(DelegateTransfer callback)
		{
			try
			{
				// call GetLocalFiles method async
				var caller = new GetLocalFilesAsync(GetLocalFiles);
				caller.BeginInvoke(ulLocalPath,
					delegate(IAsyncResult iar)
					{
						var c = (GetLocalFilesAsync)iar.AsyncState;
						ulLocalFiles = caller.EndInvoke(iar);
						callback(ulLocalFiles != null);
					}, caller);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				callback(false);
			}
		}

		// creates a remote directory for the new version (async)
		private void CreateDirectory(DelegateTransfer callback)
		{
			// try to create remote directory async
			fileTransfer.CreateDirectoryAsync(ulVersion, remotePath,
				delegate(FileTransferResult result)
				{
					callback(result == FileTransferResult.Success);
				});
		}

		// downloads version configs of wanted versions (async)
		private void DownloadVersionConfigs(DelegateTransfer callback, List<string> updates, string fullVersion)
		{
			var files = new List<TransferFile>();

			// create list with paths of version configs
			foreach (string update in updates)
				files.Add(new TransferFile(update + "/" + updateConfig));
			files.Add(new TransferFile(fullVersion + "/" + fullVersionConfig));

			// try to download the version configs async
			fileTransfer.DownloadFilesAsync(files, false, remotePath, localPath,
				delegate(FileTransferResult result)
				{
					callback(result == FileTransferResult.Success);
				});
		}

		// downloads configs of latest updates and latest full version (async)
		private void DownloadVersionConfigs(DelegateTransfer callback)
		{
			List<string> updates = versions.GetLatestUpdates();
			string fullVersion = versions.LatestFullVersion;
			DownloadVersionConfigs(callback, updates, fullVersion);
		}

		// builds the list of files to download (async)
		private void BuildDownloadListAsync(DelegateTransfer callback)
		{
			var caller = new BuildTransferListAsync(BuildDownloadList);
			BuildTransferList(callback, caller);
		}

		// builds the list of files do upload (async)
		private void BuildUploadListAsync(DelegateTransfer callback)
		{
			var caller = new BuildTransferListAsync(BuildUploadList);
			BuildTransferList(callback, caller);
		}

		// builds the list of files to transfer (async)
		private void BuildTransferList(DelegateTransfer callback, BuildTransferListAsync caller)
		{
			try
			{
				// calls wether BuildUploadList or BuildDownloadList async
				caller.BeginInvoke(
					delegate(IAsyncResult iar)
					{
						var c = (BuildTransferListAsync)iar.AsyncState;
						c.EndInvoke(iar);
						callback(true);
					}, caller);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				callback(false);
			}
		}

		// uploads files (async)
		private void UploadFiles(DelegateTransfer callback)
		{
			Uri address = new Uri(remotePath + ulVersion + "/");

			// try to upload files async
			fileTransfer.UploadFilesAsync(ulFiles, true, address, ulLocalPath,
				delegate(FileTransferResult result)
				{
					callback(result == FileTransferResult.Success);
				});
		}

		// downloads files (async)
		private void DownloadFiles(DelegateTransfer callback)
		{
			// try to download files async
			fileTransfer.DownloadFilesAsync(dlFiles, true, remotePath, dlLocalPath,
				delegate(FileTransferResult result)
				{
					callback(result == FileTransferResult.Success);
				});
		}

		// creates version config and uploads it (async)
		private void UploadVersionConfigs(DelegateTransfer callback)
		{
			var files = new List<TransferFile>();

			// check if version should be a full version
			if (ulFullVersion)
			{
				// create a new full version config and save it to temp folder
				var xmlFullVersion = new XmlFullVersion(ulFiles);
				xmlFullVersion.Save(localPath + fullVersionConfig);
				files.Add(new TransferFile(fullVersionConfig));
			}

			// create a new update config and save it to temp folder
			var xmlUpdate = new XmlUpdate(ulNewFiles, ulDeletedFiles);
			xmlUpdate.Save(localPath + updateConfig);			
			files.Add(new TransferFile(updateConfig));
			
			Uri address = new Uri(remotePath + ulVersion + "/");

			// try to upload configs async
			fileTransfer.UploadFilesAsync(files, false, address, localPath,
				delegate(FileTransferResult result)
				{
					callback(result == FileTransferResult.Success);
				});
		}

		// creates global config and uploads it (async)
		private void UploadGlobalConfig(DelegateTransfer callback)
		{
			// latest update = uploaded version
			versions.LatestUpdate = ulVersion;

			// if uploaded version is a full version 
			if (ulFullVersion)
				// latest version = uploaded version
				versions.LatestFullVersion = ulVersion;

			// add uploaded version to version list
			versions.Add(ulVersion, ulFullVersion);

			// save global config to local temp folder
			versions.Save(localPath + globalConfig);

			var file = new TransferFile(globalConfig);

			// try to upload new global config
			fileTransfer.UploadFileAsync(file, false, remotePath, localPath,
				delegate(FileTransferResult result)
				{
					callback(result == FileTransferResult.Success);
				});
		}

		// deletes a remote directory
		private void DeleteRemoteDirectory()
		{
			try
			{
				fileTransfer.DeleteDirectory(ulVersion, remotePath);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				FireWarning(Strings.UnableToCleanUpRemoteDirectory);
			}
		}

		// builds the list of files to upload
		private void BuildUploadList()
		{
			List<string> updates = versions.GetLatestUpdates();
			string fullVersion = versions.LatestFullVersion;

			// create list with all files of the latest version
			Dictionary<string, TransferFile> files = GetVersionFiles(fullVersion, updates);

			// iterate through all files of the local version
			foreach (TransferFile file in ulLocalFiles.Values)
			{
				// check if new version should be a full version
				if (ulFullVersion)
					// add file to list of files to upload
					ulFiles.Add(file);

				// check if local file is part of the latest version
				if (!files.ContainsKey(file.name) ||
					files[file.name].size != file.size ||
					files[file.name].hash != file.hash)
					// if not, add local file to list of new files
					ulNewFiles.Add(file);
			}

			// if new version shouldn't be a full version
			if (!ulFullVersion)
				// only new files are going to be uploaded
				ulFiles = ulNewFiles;

			// iterate through all filles of the latest version
			foreach (TransferFile file in files.Values)
				// if file is not part of the local file list
				if (!ulLocalFiles.ContainsKey(file.name))
					// add file to list of files which should be deleted
					ulDeletedFiles.Add(file);
		}

		// builds the list of files to download
		private void BuildDownloadList()
		{
			// create list with all files of the wanted version
			Dictionary<string, TransferFile> files = GetVersionFiles(dlFullVersion, dlUpdates);
			TransferFile dlFile;

			// add every file to the list of files which should be downloaded
			foreach (TransferFile file in files.Values)
			{
				dlFile = file;
				dlFile.destination = file.name;
				dlFile.name = file.version + "/" + file.name;
				dlFiles.Add(dlFile);
			}
		}

		// returns file list of a specific version
		private Dictionary<string, TransferFile> GetVersionFiles(string fullVersion, List<string> updates)
		{
			var newFiles = new Dictionary<string, TransferFile>();
			var deletedFiles = new Dictionary<string, TransferFile>();
			var files = new Dictionary<string, TransferFile>();

			XmlUpdate xmlUpdate = new XmlUpdate();
			XmlFullVersion xmlFullVersion = new XmlFullVersion();

			// order the update configs descending (start with the last update)
			updates = versions.OrderVersions(updates, true);
			foreach (string update in updates)
			{
				// load the update config
				xmlUpdate.Load(localPath + update + "/" + updateConfig, update);

				// add every new file to the list of new files (if it hasn't been added before)
				foreach (TransferFile file in xmlUpdate.NewFiles)
					if (!newFiles.ContainsKey(file.name)
						&& !deletedFiles.ContainsKey(file.name))
						newFiles.Add(file.name, file);

				// add every deleted file to the list of files to delete (if it hasn't been added before)
				foreach (TransferFile file in xmlUpdate.DeletedFiles)
					if (!newFiles.ContainsKey(file.name)
						&& !deletedFiles.ContainsKey(file.name))
						deletedFiles.Add(file.name, file);
			}

			// load the full version config
			xmlFullVersion.Load(localPath + fullVersion + "/" + fullVersionConfig, fullVersion);

			// add every new file to the list of version files
			foreach (TransferFile file in newFiles.Values)
				files.Add(file.name, file);

			// add every full version file to the list of version files (if it hasn't been added before)
			foreach (TransferFile file in xmlFullVersion.Files)
				if (!files.ContainsKey(file.name) &&
					!deletedFiles.ContainsKey(file.name))
					files.Add(file.name, file);

			return files;
		}

		// returns file list of the local version
		private Dictionary<string, TransferFile> GetLocalFiles(string path)
		{
			var files = new Dictionary<string, TransferFile>();

			try
			{
				// create list of local files recursive
				InitLocalFiles(path, files);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				files = null;
			}

			return files;
		}

		// creates file list of the local version recursive
		private void InitLocalFiles(string path, Dictionary<string, TransferFile> files)
		{
			// iterate through all files in current folder
			foreach (string file in Directory.GetFiles(path))
			{
				// get file name, file size and create a hash
				string name = file.Substring(ulLocalPath.Length).Replace('\\', '/');
				string hash = GetMD5Hash(file);
				FileInfo info = new FileInfo(file);
				
				// add file to file list
				files.Add(name, new TransferFile(name, info.Length, hash));
			}

			// iterate through all folders in current folder
			foreach (string dir in Directory.GetDirectories(path))
				// start recursion
				InitLocalFiles(dir, files);
		}

		// returns MD5 hash of a local file
		private string GetMD5Hash(string fileName)
		{
			MD5 md5 = MD5.Create();
			FileInfo info = new FileInfo(fileName);
			FileStream stream = File.OpenRead(fileName);

			var list = new List<byte>();
			byte[] megabyte = new byte[1024 * 1024];

			// divide the file into pieces of 1 mb
			for (int i = 0; i < info.Length; i += megabyte.Length)
			{
				// create hash for file part and save into list
				for (int j = 0; j < megabyte.Length && (i + j) < info.Length; j++)
					megabyte[j] = (byte)stream.ReadByte();
				foreach (byte b in md5.ComputeHash(megabyte))
					list.Add(b);
			}

			// create hash over all hashes
			string hash = "";
			foreach (byte b in md5.ComputeHash(list.ToArray()))
				hash += b.ToString("x2").ToLower();				

			return hash;
		}

		// deletes a local directory recursive
		private void DeleteLocalDirectory(string path)
		{
			try
			{
				// start recursion for all directories in current directory
				foreach (string name in Directory.GetDirectories(path))
					DeleteLocalDirectory(name);

				// delete all files in current directory
				foreach (string name in Directory.GetFiles(path))
					File.Delete(name);

				// delete current directory
				Directory.Delete(path);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
			}
		}

		// fires events to set the user interface
		private void ResetInterface()
		{
			// fire event to enable the main menu
			FireMainMenu();

			// check if settings are loaded and global config has been downloaded
			if (Settings.Loaded && connected)
			{
				// fire event to inform GUI about the number of the latest version
				FireUploadVersion(versions.LatestVersion);

				// if there is at least 1 version on the server
				if (versions.LatestVersion != null)
				{
					// fire event to set status
					FireStatus(Strings.LatestVersion + versions.LatestVersion);

					// fire events to enable up- and download
					FireEnableUpload();
					FireEnableDownload();

					// fire event to inform GUI about versions which can be downloaded
					FireDownloadVersions(versions.GetVersionNames());
				}
				else
				{
					// fire event to set status
					FireStatus(Strings.NoVersionsFound);

					// fire event to enable upload (full version only)
					FireEnableUpload(true);
				}
			}
			else
			{
				// fire events to disable up- and download
				FireDisableDownload();
				FireDisableUpload();
			}
		}

		// unlocks the server
		private void UnlockServer()
		{
			try
			{
				// if server is locked, delete lockfile on the server
				if (serverLock)
					fileTransfer.DeleteFile(lockFile, remotePath);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				FireWarning(Strings.UnableToUnlock);
			}
		}

		// cleans up
		public void Close(CancelEventArgs e)
		{
			// set global exit
			exit = true;

			// if there was an active file transfer
			if (fileTransfer.CancelTransfer())
				// cancel closing of the GUI
				e.Cancel = true;
			else
				// delete local temp folder
				DeleteLocalDirectory(localPath);
		}

		// fires status event
		private void FireStatus(string s)
		{
			if (Status != null)
				Status(this, new EventArgs<string>(s));
		}

		// fires error event
		private void FireError(string s)
		{
			if (Error != null)
				Error(this, new EventArgs<string>(s));
		}

		// fires warning event
		private void FireWarning(string s)
		{
			if (Warning != null)
				Warning(this, new EventArgs<string>(s));
		}

		// fires information event
		private void FireInformation(string s)
		{
			if (Information != null)
				Information(this, new EventArgs<string>(s));
		}

		// fires exit event
		private void FireExit()
		{
			if (Exit != null)
				Exit(this, new EventArgs());
		}

		// fires main menu event
		private void FireMainMenu()
		{
			if (MainMenu != null)
				MainMenu(this, new EventArgs());
		}

		// fires upload version event
		private void FireUploadVersion(string s)
		{
			if (UploadVersion != null)
				UploadVersion(this, new EventArgs<string>(s));
		}

		// fires enable upload event
		private void FireEnableUpload(bool fullVersionOnly)
		{
			if (EnableUpload != null)
				EnableUpload(this, new EventArgs<bool>(fullVersionOnly));
		}

		// fires enable upload event
		private void FireEnableUpload()
		{
			if (EnableUpload != null)
				EnableUpload(this, new EventArgs<bool>(false));
		}

		// fires enable download event
		private void FireEnableDownload()
		{
			if (EnableDownload != null)
				EnableDownload(this, new EventArgs());
		}

		// fires download versions event
		private void FireDownloadVersions(List<string> versions)
		{
			if (DownloadVersions != null)
				DownloadVersions(this, new EventArgs<List<string>>(versions));
		}

		// fires disable download event
		private void FireDisableDownload()
		{
			if (DisableDownload != null)
				DisableDownload(this, new EventArgs());
		}

		// fires disable upload event
		private void FireDisableUpload()
		{
			if (DisableUpload != null)
				DisableUpload(this, new EventArgs());
		}
	}
}