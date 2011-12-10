using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EU_Release_Manager.Network;

namespace EU_Release_Manager.Controller.XML
{
	public class XmlFullVersion
	{
		public List<TransferFile> Files { get; set; }

		public XmlFullVersion(List<TransferFile> files)
		{
			Files = (files != null) ? files : new List<TransferFile>();
		}

		public XmlFullVersion()
		{
			Files = new List<TransferFile>();
		}

		// loads full version config
		public bool Load(string path, string version)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(path);

				Files.Clear();
				foreach (XmlNode node in doc.SelectNodes("//file"))
				{
					string name = node.Attributes["path"].Value;
					long size = long.Parse(node.SelectSingleNode("size").InnerText);
					string hash = node.SelectSingleNode("hash").InnerText;

					Files.Add(new TransferFile(name, size, hash, version));
				}

				return true;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				Files.Clear();
			}

			return false;
		}

		// writes full version config
		public bool Save(string path)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				XmlNode files = doc.CreateElement("files");

				foreach (TransferFile f in Files)
				{
					XmlAttribute name = doc.CreateAttribute("path");
					name.Value = f.name;

					XmlNode size = doc.CreateElement("size");
					size.InnerText = f.size.ToString();

					XmlNode hash = doc.CreateElement("hash");
					hash.InnerText = f.hash;

					XmlNode file = doc.CreateElement("file");
					file.Attributes.Append(name);
					file.AppendChild(size);
					file.AppendChild(hash);

					files.AppendChild(file);
				}

				doc.AppendChild(files);
				doc.Save(path);

				return true;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
			}

			return false;
		}

		// returns list of all filenames from this full version
		public List<string> GetFileNames()
		{
			return (from f in Files select f.name).ToList<string>();
		}
	}
}
