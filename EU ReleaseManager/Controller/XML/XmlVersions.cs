using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace EU_Release_Manager.Controller.XML
{
	public class XmlVersions
	{
		public string LatestUpdate { get; set; }
		public string LatestFullVersion { get; set; }
		public List<Version> Versions { get; private set; }

		public string LatestVersion
		{
			get { return (LatestUpdate != null) ? LatestUpdate : LatestFullVersion; }
		}

		public struct Version
		{
			public string name;
			public string fullVersion;

			public Version(string name, string fullVersion)
			{
				this.name = name;
				this.fullVersion = fullVersion;
			}
		}

		public XmlVersions()
		{
			Versions = new List<Version>();
		}

		// loads global config
		public bool Load(string path)
		{
			try
			{
				XmlDocument config = new XmlDocument();
				config.Load(path);

				LatestUpdate = config.SelectSingleNode("//latestUpdate").Attributes["name"].Value;
				LatestFullVersion = config.SelectSingleNode("//latestFullVersion").Attributes["name"].Value;

				if (LatestFullVersion != null && CheckVersionSyntax(LatestFullVersion))
				{
					if (LatestUpdate == null || !CheckVersionSyntax(LatestUpdate))
						LatestUpdate = LatestFullVersion;

					Versions.Clear();
					foreach (XmlNode node in config.SelectNodes("//version"))
					{
						string version = node.Attributes["name"].Value;
						bool fullVersion = (node.Attributes["fullVersion"].Value == "true");

						Add(version, fullVersion);
					}

					return true;
				}
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
			}

			Clear();
			return false;
		}

		// writes global config
		public bool Save(string path)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				XmlNode config = doc.CreateElement("config");

				XmlNode latestUpdate = doc.CreateElement("latestUpdate");
				XmlAttribute luName = doc.CreateAttribute("name");
				luName.Value = LatestUpdate;
				latestUpdate.Attributes.Append(luName);

				XmlNode latestFullVersion = doc.CreateElement("latestFullVersion");
				XmlAttribute lfvName = doc.CreateAttribute("name");
				lfvName.Value = LatestFullVersion;
				latestFullVersion.Attributes.Append(lfvName);

				XmlNode versions = doc.CreateElement("versions");
				foreach (Version v in Versions)
				{
					XmlAttribute name = doc.CreateAttribute("name");
					name.Value = v.name;

					XmlAttribute fullVersion = doc.CreateAttribute("fullVersion");
					fullVersion.Value = v.fullVersion;

					XmlNode version = doc.CreateElement("version");
					version.Attributes.Append(name);
					version.Attributes.Append(fullVersion);

					versions.AppendChild(version);
				}

				config.AppendChild(latestUpdate);
				config.AppendChild(latestFullVersion);
				config.AppendChild(versions);
				doc.AppendChild(config);
				doc.Save(path);

				return true;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
			}

			return false;
		}

		// adds a version to the list of versions
		public bool Add(string version, bool fullVersion)
		{
			if (!CheckVersionSyntax(version))
				return false;

			string fv = (fullVersion) ? "true" : "false";
			Versions.Add(new Version(version, fv));
			return true;
		}

		// clears the stored information
		public void Clear()
		{
			Versions.Clear();
			LatestUpdate = null;
			LatestFullVersion = null;
		}

		// checks the syntax of a version number
		private bool CheckVersionSyntax(string version)
		{
			if (version == null)
				return false;

			Regex regex = new Regex("([0-9][0-9]?[.]){3}[0-9][0-9]?");
			return regex.IsMatch(version);
		}

		// checks a new version number
		public bool CheckNewVersionNr(string version)
		{
			if (!CheckVersionSyntax(version))
				return false;

			if (LatestVersion == null)
				return true;

			return VersionToLong(version) > VersionToLong(LatestVersion);
		}

		// converts version number string into a number (for sorting)
		private long VersionToLong(string version)
		{
			if (!CheckVersionSyntax(version))
				return 0;

			string[] tokens = version.Split('.');
			string temp = "";

			for (int i = 0; i < tokens.Length; i++)
			{
				if (tokens[i].Length == 1)
					tokens[i] = "0" + tokens[i];

				temp += tokens[i];
			}

			try
			{
				return long.Parse(temp);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				return 0;
			}
		}

		// returns a list of the numbers of all versions
		public List<string> GetVersionNames()
		{
			return (from v in Versions select v.name).ToList<string>();
		}

		// returns the latest full version before a declared version
		public string GetFullVersion(string version)
		{
			long lVersion = VersionToLong(version);

			List<string> fullVersions = (from v in Versions
												  where v.fullVersion == "true" &&
												  VersionToLong(v.name) <= lVersion
												  orderby VersionToLong(v.name) descending
												  select v.name).ToList<string>();

			if (fullVersions.Count > 0)
				return fullVersions[0];
			else
				return null;
		}

		// returns a list of the numbers of all updates between fullVersion and version
		public List<string> GetUpdates(string version, string fullVersion)
		{
			long lVersion = VersionToLong(version);
			long lFullVersion = VersionToLong(fullVersion);

			return (from v in Versions
					  where v.fullVersion == "false" &&
					  VersionToLong(v.name) <= lVersion &&
					  VersionToLong(v.name) > lFullVersion
					  select v.name).ToList<string>();
		}

		// returns a list of numbers of all updates since latest full version
		public List<string> GetLatestUpdates()
		{
			long lLatestFullVersion = VersionToLong(LatestFullVersion);

			return (from v in Versions 
					  where v.fullVersion == "false" && VersionToLong(v.name) > lLatestFullVersion 
					  select v.name).ToList<string>();
		}

		// returns an ordered list of version numbers
		public List<string> OrderVersions(List<string> versions, bool desc)
		{
			List<string> temp;

			if (desc)
				temp = (from v in versions orderby VersionToLong(v) descending select v).ToList<string>();
			else
				temp = (from v in versions orderby VersionToLong(v) select v).ToList<string>();

			return temp;
		}
	}
}
