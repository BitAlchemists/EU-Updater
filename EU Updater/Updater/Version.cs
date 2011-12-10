using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EU_Updater
{
	class Version
	{
		public string VersionNumber { get; set; }
		public bool IsFullVersion { get; set; }

		public Version(string versionNumber, bool isFullVersion)
		{
			VersionNumber = versionNumber;
			IsFullVersion = isFullVersion;
		}

		//converts a version-string into a long-number
		static public long ToLong(string version)
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
				return -1;
			}
		}

		//checks if version is valid
		static public bool CheckVersionSyntax(string version)
		{
			if (version == null)
				return false;

			Regex regex = new Regex("([0-9][0-9]?[.]){3}[0-9][0-9]?");
			return regex.IsMatch(version);
		}
	}
}
