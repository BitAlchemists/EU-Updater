using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EU_Release_Manager.Network;

namespace EU_Release_Manager.Controller.XML
{
	public class XmlUpdate
	{
		public List<TransferFile> NewFiles { get; set; }
		public List<TransferFile> DeletedFiles { get; set; }

		private string version;

		public XmlUpdate(List<TransferFile> newFiles, List<TransferFile> deletedFiles)
		{
			NewFiles = (newFiles != null) ? newFiles : new List<TransferFile>();
			DeletedFiles = (deletedFiles != null) ? deletedFiles : new List<TransferFile>();
		}

		public XmlUpdate()
		{
			NewFiles = new List<TransferFile>();
			DeletedFiles = new List<TransferFile>();
		}

		// loads update config
		public bool Load(string path, string version)
		{
			this.version = version;

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(path);

				LoadFiles(doc, "newFiles", NewFiles, true);
				LoadFiles(doc, "deletedFiles", DeletedFiles, false);

				return true;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				NewFiles.Clear();
				DeletedFiles.Clear();
			}

			return false;
		}

		// loads all files of a type (new files or deleted files)
		private void LoadFiles(XmlDocument doc, string parentNode, List<TransferFile> files, bool details)
		{
			long size = 0;
			string hash = null;

			files.Clear();
			foreach (XmlNode node in doc.SelectNodes("//" + parentNode + "/file"))
			{
				string name = node.Attributes["path"].Value;

				if (details)
				{
					size = long.Parse(node.SelectSingleNode("size").InnerText);
					hash = node.SelectSingleNode("hash").InnerText;
				}

				files.Add(new TransferFile(name, size, hash, version));
			}
		}

		// writes update config
		public bool Save(string path)
		{
			try
			{
				XmlDocument doc = new XmlDocument();

				XmlNode root = doc.CreateElement("update");

				root.AppendChild(SaveFiles(doc, "newFiles", NewFiles, true));
				root.AppendChild(SaveFiles(doc, "deletedFiles", DeletedFiles, false));

				doc.AppendChild(root);
				doc.Save(path);

				return true;
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
			}

			return false;
		}

		// saves all files of a type (new files or deleted files)
		private XmlNode SaveFiles(XmlDocument doc, string parentNode, List<TransferFile> files, bool details)
		{
			XmlNode node = doc.CreateElement(parentNode);

			foreach (TransferFile f in files)
			{
				XmlAttribute name = doc.CreateAttribute("path");
				name.Value = f.name;

				XmlNode file = doc.CreateElement("file");
				file.Attributes.Append(name);

				if (details)
				{
					XmlNode size = doc.CreateElement("size");
					size.InnerText = f.size.ToString();

					XmlNode hash = doc.CreateElement("hash");
					hash.InnerText = f.hash;

					file.AppendChild(size);
					file.AppendChild(hash);
				}

				node.AppendChild(file);
			}

			return node;
		}

		// returns list of the names of all deleted files
		public List<string> GetDeletedFileNames()
		{
			return (from f in DeletedFiles select f.name).ToList<string>();
		}

		// returns list of the names of all new files
		public List<string> GetNewFileNames()
		{
			return (from f in NewFiles select f.name).ToList<string>();
		}
	}
}
