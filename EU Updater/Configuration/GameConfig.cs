using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace EU_Updater
{
	//configuration file in game directory
	class GameConfig
	{
		public const string DefaultVersionNumber = "0.0.0.0";

		public string VersionNumber { get; private set; }

		public GameConfig()
		{
		}

		//changes the version number and writes it into the configuration file
		public bool Save(string gameConfigurationFile, string versionNumber)
		{
			XmlDocument docConfig = new XmlDocument();
			XmlNode node;
			XmlAttribute attribute;

			try
			{
				//set new version number
				VersionNumber = versionNumber;

				//create XML-Node with Attribute
				node = docConfig.CreateElement("version");
				attribute = docConfig.CreateAttribute("name");
				attribute.Value = VersionNumber;
				node.Attributes.Append(attribute);
				docConfig.AppendChild(node);

				//save configuration file
				docConfig.Save(gameConfigurationFile);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return false;
			}

			return true;
		}

		//loads the configuration file
		public bool Load(string gameConfigurationFile)
		{
			XmlDocument docConfig = new XmlDocument();
			XmlNode node;

			try
			{
				//load configuration file if it exists
				if (File.Exists(gameConfigurationFile))
				{
					docConfig.Load(gameConfigurationFile);

					//read the version number
					node = docConfig.GetElementsByTagName("version")[0];
					VersionNumber = node.Attributes["name"].Value;

					//checks if version number is valid
					if (VersionNumber != null && VersionNumber != "" &&
						!Version.CheckVersionSyntax(VersionNumber))
						return false;
				}
				else //set version number to default
				{
					VersionNumber = DefaultVersionNumber;
					return false;
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
