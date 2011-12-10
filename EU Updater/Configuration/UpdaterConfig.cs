using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EU_Updater
{
	class UpdaterConfig
	{
		//server path
		public string ServerPath { get; private set; }
		//installation path of the game
		public string GamePath { get; private set; }
		//name of the configuration file of the game
		public string GameConfigurationFile { get; private set; }
		//name of the configuration file in which all versions are administrated
		public string VersionsConfigurationFile { get; private set; }
		//name of the configuration file of a update
		public string UpdateConfigurationFile { get; set; }
		//name of the configuration file of a full version
		public string FullVersionConfigurationFile { get; set; }
		//name of the temporary download folder
		public string TempFolder { get; private set; }
		//version number of the Updater
		public string VersionNumber { get; private set; }
		//relative server path of the Updater
		public string RelUpdaterPathOnServer { get; private set; }
		//absolute server path of the Updater
		public string AbsUpdaterPathOnServer { get; private set; }
		//name of the .exe file of the Updater
		public string UpdaterExe { get; private set; }
		//name of the .exe file of the Installer
		public string InstallerExe { get; private set; }
		//number of the last background image in the Updater
		public int LastBackgroundImgNumber { get; private set; }
		//number of the next background image in the Updater
		public int NextBackgroundImgNumber { get; private set; }
		//sorted list with all numbers and paths of the background images
		public SortedList<int, string> BackgroundImgList { get; private set; }
		//flag if the background images should be updated
		public bool UpdatePictures { get; private set; }

		public UpdaterConfig()
		{
			BackgroundImgList = new SortedList<int, string>();
		}

		//loads the configuration file
		public bool Load(string configurationFile)
		{
			XmlDocument docConfig = new XmlDocument();
			XmlNode xmlNode;

			try
			{
				docConfig.Load(configurationFile);

				//read server path
				xmlNode = docConfig.GetElementsByTagName("server")[0];
				ServerPath = xmlNode.Attributes["path"].Value;
				if (ServerPath != "" && !ServerPath.EndsWith("/"))
					ServerPath += "/";

				//read game path
				xmlNode = docConfig.GetElementsByTagName("game")[0];
				GamePath = xmlNode.Attributes["path"].Value;
				if (GamePath != "" && !GamePath.EndsWith("/"))
					GamePath += "/";
				GameConfigurationFile = xmlNode.Attributes["configFile"].Value;

				//read relative server path of the Updater
				xmlNode = docConfig.GetElementsByTagName("updater")[0];
				RelUpdaterPathOnServer = xmlNode.Attributes["serverpath"].Value;
				if (RelUpdaterPathOnServer != "" && !RelUpdaterPathOnServer.EndsWith("/"))
					RelUpdaterPathOnServer += "/";

				//set absolute server path of the Updater
				AbsUpdaterPathOnServer = ServerPath + RelUpdaterPathOnServer;

				//read name of the administrated versions configuration file
				xmlNode = docConfig.GetElementsByTagName("versionsFile")[0];
				VersionsConfigurationFile = xmlNode.Attributes["name"].Value;

				//read name of a update configuration file
				xmlNode = docConfig.GetElementsByTagName("updateConfig")[0];
				UpdateConfigurationFile = xmlNode.Attributes["name"].Value;

				//read name of a full version configuration file
				xmlNode = docConfig.GetElementsByTagName("fullVersionConfig")[0];
				FullVersionConfigurationFile = xmlNode.Attributes["name"].Value;

				//read name of the temporary download folder
				xmlNode = docConfig.GetElementsByTagName("temp")[0];
				TempFolder = xmlNode.Attributes["path"].Value;

				//read name of the Udater executeable
				xmlNode = docConfig.GetElementsByTagName("updaterExe")[0];
				UpdaterExe = xmlNode.Attributes["name"].Value;

				//read name of the Installer executeable
				xmlNode = docConfig.GetElementsByTagName("installerExe")[0];
				InstallerExe = xmlNode.Attributes["name"].Value;

				//read version of the Updater
				xmlNode = docConfig.GetElementsByTagName("version")[0];
				VersionNumber = xmlNode.Attributes["name"].Value;

				//check version syntax
				if (!Version.CheckVersionSyntax(VersionNumber))
					return false;
				
				//read number of last background image and image-update flag
				xmlNode = docConfig.GetElementsByTagName("pictures")[0];
				LastBackgroundImgNumber = int.Parse(xmlNode.Attributes["lastNumber"].Value);
				UpdatePictures = bool.Parse(xmlNode.Attributes["update"].Value);

				//read all background images
				foreach (XmlNode pictureNode in xmlNode.ChildNodes)
					BackgroundImgList.Add(int.Parse(pictureNode.Attributes["number"].Value),
						pictureNode.Attributes["path"].Value);

				//set next background image number
				NextBackgroundImgNumber = GetNextBackgroundImgNumber(LastBackgroundImgNumber);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		//determines the next background image number
		private int GetNextBackgroundImgNumber(int lastBackgroundImgNumber)
		{
			int nextBackgroundImgNumber;

			nextBackgroundImgNumber = LastBackgroundImgNumber + 1;
			if (!BackgroundImgList.ContainsKey(nextBackgroundImgNumber))
				nextBackgroundImgNumber = BackgroundImgList.First().Key;

			return nextBackgroundImgNumber;
		}

		//changes the game path and writes it into the configuration file
		public bool SaveGamePath(string configurationFile, string gamePath)
		{
			XmlDocument docConfig = new XmlDocument();
			XmlNode gameNode;

			try
			{
				docConfig.Load(configurationFile);

				gameNode = docConfig.GetElementsByTagName("game")[0];
				gameNode.Attributes["path"].Value = gamePath;

				docConfig.Save(configurationFile);

				GamePath = gamePath;
				if (GamePath != "" && !GamePath.EndsWith("/"))
					GamePath += "/";
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		//changes the number of the last background image and writes it into the configuration file
		public bool SaveLastBackgroundImg(string configurationFile, int lastBackgroundImgNumber)
		{
			XmlDocument docConfig = new XmlDocument();
			XmlNode picturesNode;

			try
			{
				docConfig.Load(configurationFile);

				picturesNode = docConfig.GetElementsByTagName("pictures")[0];
				picturesNode.Attributes["lastNumber"].Value = lastBackgroundImgNumber.ToString();

				docConfig.Save(configurationFile);

				LastBackgroundImgNumber = lastBackgroundImgNumber;
				NextBackgroundImgNumber = GetNextBackgroundImgNumber(lastBackgroundImgNumber);
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
