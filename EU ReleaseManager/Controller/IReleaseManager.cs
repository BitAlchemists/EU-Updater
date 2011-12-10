using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EU_Release_Manager.Controller
{
	interface IReleaseManager
	{
		Settings Settings { get; set; }
		bool Valid { get; set; }

		event EventHandler<EventArgs<string>> Status;
		event EventHandler<EventArgs<string>> Error;
		event EventHandler<EventArgs<string>> Warning;
		event EventHandler<EventArgs<string>> Information;
		event EventHandler<EventArgs> Exit;
		event EventHandler<EventArgs> MainMenu;
		event EventHandler<EventArgs<string>> UploadVersion;
		event EventHandler<EventArgs<List<string>>> DownloadVersions;
		event EventHandler<EventArgs<bool>> EnableUpload;
		event EventHandler<EventArgs> EnableDownload;
		event EventHandler<EventArgs> DisableUpload;
		event EventHandler<EventArgs> DisableDownload;

		void Start();
		void Close(CancelEventArgs e);
		void Upload(string path, string version, bool FullVersion);
		void Download(string path, string version);
		bool CheckNewVersionNr(string version);
	}
}
