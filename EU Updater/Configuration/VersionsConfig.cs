using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace EU_Updater
{
	class VersionsConfig
	{
		//sorted list with all versions needed for the update
		public SortedList<long, Version> VersionsForUpdate { get; private set; }
		//version number of the latest update
		public string LatestUpdate { get; private set; }
		//version number of the latest full version
		public string LatestFullVersion { get; private set; }

		public VersionsConfig()
		{
			VersionsForUpdate = new SortedList<long, Version>();
		}

		/* loads the configuration file
		 * parameter needsFullVersion determines if a full version or only an update is needed */
		public bool LoadNewVersions(string configurationFile, string installedVersionNumber,
			bool needsFullVersion)
		{
			XmlDocument docConfig = new XmlDocument();
			XmlNode node;
			var allVersions = new SortedList<long, Version>();
			long version;
			string name;
			bool isFullVersion;

			try
			{
				docConfig.Load(configurationFile);

				//read the version number of the latest update
				node = docConfig.GetElementsByTagName("latestUpdate")[0];
				LatestUpdate = node.Attributes["name"].Value;

				//read the version number of the latest full version
				node = docConfig.GetElementsByTagName("latestFullVersion")[0];
				LatestFullVersion = node.Attributes["name"].Value;

				//load all existing versions into a list sorted by the version numbers
				node = docConfig.GetElementsByTagName("versions")[0];
				foreach (XmlNode versionNode in node.ChildNodes)
				{
					name = versionNode.Attributes["name"].Value;
					isFullVersion = bool.Parse(versionNode.Attributes["fullVersion"].Value);
					allVersions.Add(Version.ToLong(name), new Version(name, isFullVersion));
				}

				//iterate through the versions list from the end to the beginning
				for (int i = allVersions.Keys.Count - 1; i >= 0; i--)
				{
					version = allVersions.Keys[i];
					//check if the version is needed
					if (allVersions[version].VersionNumber != installedVersionNumber)
						VersionsForUpdate.Add(version, allVersions[version]);
					else
						return true;

					//if a full version was needed and found then the search can be finished
					if (needsFullVersion && allVersions[version].IsFullVersion)
						return true;
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
