using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.IO;
using System.Timers;

namespace EU_Updater
{
	public partial class FileTransfer : UserControl
	{
		private const int bufferSize = 10;
		private const int refreshInterval = 1000;

		public WebClient Client { get; private set; }

		private List<TransferFile> files;
		private List<TransferFile>.Enumerator fileEnumerator;
		private long totalSize;
		private long totalTransferred;
		private long currentSize;
		private long currentTransferred;
		private long lastTransferred;
		private bool showProgress;
		private bool download;
		private DateTime startTime;
		private Queue<long> buffer;
		private System.Timers.Timer progressTimer;
		private Uri remotePath;
		private string localPath;
		private FileTransferCallback callback;
		private bool busy;
		private List<string> directories;

		private delegate void DelegateString(string text);
		private delegate void DelegateInt(int value);
		private delegate void DelegateTimeSpan(TimeSpan timeSpan);
		private delegate void DelegateVoid();
		private delegate void DelegateRequest(string directory, Uri remotePath);

		public FileTransfer()
		{
			InitializeComponent();

			progressTimer = new System.Timers.Timer();
			progressTimer.Interval = refreshInterval;
			progressTimer.Elapsed += new ElapsedEventHandler(progressTimer_Elapsed);

			buffer = new Queue<long>();
			Client = new WebClient();
			directories = new List<string>();

			Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
			Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
			Client.UploadFileCompleted += new UploadFileCompletedEventHandler(UploadFileCompleted);
			Client.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgressChanged);

			pbOverall.Maximum = pbOverall.Width;
			pbCurrent.Maximum = pbCurrent.Width;
		}

		// creates a remote directory (async)
		public void CreateDirectoryAsync(string directory, Uri remotePath, FileTransferCallback callback)
		{
			SendRequestAsync(directory, remotePath, callback, new DelegateRequest(CreateDirectory));
		}

		// creates a remote directory
		public void CreateDirectory(string directory, Uri remotePath)
		{
			string[] temp = directory.Split('/');
			string uriString = remotePath.AbsoluteUri;

			for (int i = 0; i < temp.Length; i++)
			{
				uriString += temp[i];
				if (!DirectoryExists(uriString))
					SendRequest(uriString, WebRequestMethods.Ftp.MakeDirectory);
				uriString += "/";
			}
		}

		// deletes a remote directory (async)
		public void DeleteDirectoryAsync(string directory, Uri remotePath, FileTransferCallback callback)
		{
			SendRequestAsync(directory, remotePath, callback, new DelegateRequest(DeleteDirectory));
		}

		// deletes a remote directory (recursive)
		public void DeleteDirectory(string directory, Uri remotePath)
		{
			Uri path = remotePath;
			string name = directory;

			if (name == "" && path.AbsoluteUri.EndsWith("/"))
				path = new Uri(path.AbsoluteUri.Substring(0, path.AbsoluteUri.Length - 1));
			else if (name != "" && !path.AbsoluteUri.EndsWith("/"))
				path = new Uri(path.AbsoluteUri + "/");

			if (name.EndsWith("/"))
				name = name.Substring(0, name.Length - 1);

			// get files and directories of the current directory
			string[] list = GetDirectoryList(path + name);

			if (list == null || list.Length == 0 || list[0] == "")
				// current directory is empty
				SendRequest(path + name, WebRequestMethods.Ftp.RemoveDirectory);
			else if (list[0] == name)
				// current directory is a file
				SendRequest(path + name, WebRequestMethods.Ftp.DeleteFile);
			else
			{
				// current directory is neighter empty nor a file
				path = new Uri(path + name + "/");

				// start recursion for every directory/file in the current directory
				for (int i = 0; i < list.Length; i++)
					DeleteDirectory(list[i].Split('/')[1], path);

				// delete current directory
				SendRequest(path.AbsoluteUri, WebRequestMethods.Ftp.RemoveDirectory);
			}
		}

		// deletes a remote file (async)
		public void DeleteFileAsync(string file, Uri remotePath, FileTransferCallback callback)
		{
			SendRequestAsync(file, remotePath, callback, new DelegateRequest(DeleteFile));
		}

		// deletes a remote file
		public void DeleteFile(string file, Uri remotePath)
		{
			SendRequest(remotePath + file, WebRequestMethods.Ftp.DeleteFile);
		}

		// checks if remote directory exists
		private bool DirectoryExists(string uriString)
		{
			try
			{
				SendRequest(uriString, WebRequestMethods.Ftp.PrintWorkingDirectory);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		// returns all files and directories in the declared remote directory
		private string[] GetDirectoryList(string uriString)
		{
			string[] list = null;

			try
			{
				// get directory list
				FtpWebResponse response = (FtpWebResponse)SendRequest(uriString, WebRequestMethods.Ftp.ListDirectory);

				Stream stream = response.GetResponseStream();
				StreamReader reader = new StreamReader(stream, Encoding.UTF8);

				// format directory list
				list = reader.ReadToEnd().Trim().Replace("\r", "").Split('\n');
				response.Close();
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
			}

			return list;
		}

		// sends an request to the remote server (async)
		private void SendRequestAsync(string name, Uri remotePath, FileTransferCallback callback, DelegateRequest caller)
		{
			try
			{
				caller.BeginInvoke(name, remotePath,
					delegate(IAsyncResult iar)
					{
						var c = (DelegateRequest)iar.AsyncState;
						c.EndInvoke(iar);
						callback.Invoke(FileTransferResult.Success);
					}, caller);
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				callback.Invoke(FileTransferResult.Failure);
			}
		}

		// sends an request to the remote server
		private WebResponse SendRequest(string uriString, string method)
		{
			FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(uriString);
			request.Credentials = Client.Credentials;
			request.Method = method;
			return request.GetResponse();
		}

		// downloads a remote file (async)
		public bool DownloadFileAsync(TransferFile file, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback)
		{
			var files = new List<TransferFile>() { file };
			return TransferFilesAsync(files, showProgress, remotePath, localPath, callback, true);
		}

		// downloads remote files (async)
		public bool DownloadFilesAsync(List<TransferFile> files, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback)
		{
			return TransferFilesAsync(files, showProgress, remotePath, localPath, callback, true);
		}

		// uploads a file (async)
		public bool UploadFileAsync(TransferFile file, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback)
		{
			var files = new List<TransferFile>() { file };
			return TransferFilesAsync(files, showProgress, remotePath, localPath, callback, false);
		}
		
		// uploads files (async)
		public bool UploadFilesAsync(List<TransferFile> files, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback)
		{
			return TransferFilesAsync(files, showProgress, remotePath, localPath, callback, false);
		}

		// up- or downloads files (async)
		private bool TransferFilesAsync(List<TransferFile> files, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback, bool download)
		{
			// set busy flag
			lock (this)
			{
				if (busy)
					return false;

				if (files.Count == 0)
				{
					callback.Invoke(FileTransferResult.Success);
					return true;
				}

				busy = true;
			}

			totalTransferred = 0;
			totalSize = 0;

			// calculate total size
			foreach (TransferFile file in files)
				totalSize += file.Size;

			this.files = files;
			this.remotePath = remotePath;
			this.localPath = localPath;
			this.showProgress = showProgress;
			this.callback = callback;
			this.download = download;

			// select next file
			fileEnumerator = files.GetEnumerator();
			fileEnumerator.MoveNext();

			// check if progress should be displayed
			if (showProgress)
			{
				EnableProgress();
				buffer.Clear();
				startTime = DateTime.Now;
				progressTimer.Start();
			}

			// up- or download next file
			TransferNextFile();
			return true;
		}

		// up- or downloads next file from file list
		private void TransferNextFile()
		{
			string fileName = fileEnumerator.Current.Name;
			string destination = fileEnumerator.Current.Destination;

			// check if progress should be displayed
			if (showProgress)
			{
				lastTransferred = 0;
				SetCurrentFile(fileName);
				SetCurrentProgress(0);
			}

			try
			{
				string dir = "";

				if (destination.IndexOf('/') > 0)
					dir = destination.Substring(0, destination.LastIndexOf('/'));

				// check if file should be up- or downloaded
				if (download)
				{
					// create directory if doesn't exist
					if (!Directory.Exists(localPath + dir))
						Directory.CreateDirectory(localPath + dir);

					// download file
					Client.DownloadFileAsync(new Uri(remotePath + fileName), localPath + destination);
				}
				else
				{
					// check if directory was created before
					if (!directories.Contains(remotePath + dir))
					{
						// check if directory exists
						if (!DirectoryExists(remotePath + dir))
							CreateDirectory(dir, remotePath);
						directories.Add(remotePath + dir);
					}
					
					// upload file
					Client.UploadFileAsync(new Uri(remotePath + destination), localPath + fileName);
				}
			}
			catch (Exception e)
			{
				ErrorLog.Add(this, e.Message);
				CleanUp();
				callback.Invoke(FileTransferResult.Failure);
			}
		}

		// handles the progress changed event (while downloading)
		private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			ProgressChanged(e.BytesReceived, e.TotalBytesToReceive);
		}

		// handels the progress changed event (while uploading)
		private void UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
		{
			ProgressChanged(e.BytesSent, e.TotalBytesToSend);
		}

		// handels the progress changed event (while down- or uploading)
		private void ProgressChanged(long bytesTransferred, long totalBytesToTransfer)
		{
			// check if progress should be displayed
			if (showProgress)
			{
				// calculate how much data has been transferred so far
				currentSize = totalBytesToTransfer;
				currentTransferred = bytesTransferred;
				totalTransferred += bytesTransferred - lastTransferred;
				totalSize = totalSize - fileEnumerator.Current.Size + currentSize;
				lastTransferred = bytesTransferred;

				if (totalTransferred > totalSize)
					totalTransferred = totalSize;

				long maxOverall = GetOverallProgressMaximum();
				long maxCurrent = GetCurrentProgressMaximum();

				// set current progress bar
				if (currentSize > 0)
					SetCurrentProgress((int)(maxCurrent * ((double)currentTransferred / (double)currentSize)));

				// set overall progress bar
				if (totalSize > 0)
					SetOverallProgress((int)(maxOverall * ((double)totalTransferred / (double)totalSize)));
			}
		}

		// handels the file completed event (while downloading)
		private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			FileCompleted(e.Error, e.Cancelled);
		}

		// handels the file completed event (while uploading)
		private void UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
		{
			FileCompleted(e.Error, e.Cancelled);
		}

		// handels the file completed event (while down- or uploading)
		private void FileCompleted(Exception error, bool cancelled)
		{
			// check if there has been an error
			if (error != null)
			{
				CleanUp();
				callback.Invoke(FileTransferResult.Failure);
			}
			// check if there are files left to transfer
			else if (!fileEnumerator.MoveNext())
			{
				// check if progress should be displayed
				if (showProgress)
				{
					// set current and overall progress to 100%
					SetCurrentProgress(GetCurrentProgressMaximum());
					SetOverallProgress(GetOverallProgressMaximum());
				}

				CleanUp();
				callback.Invoke(FileTransferResult.Success);
			}
			else
				TransferNextFile();
		}

		// cleans up
		private void CleanUp()
		{
			// unset busy flag
			busy = false;

			// clear list of created directories
			directories.Clear();

			// check if progress should be displayed
			if (showProgress)
			{
				// stop display timer
				progressTimer.Stop();
				DisableProgress();
				showProgress = false;
			}
		}

		// cancels an active file transfer (up- or download)
		public bool CancelTransfer()
		{
			// check if there is an active transfer
			if (Client != null && Client.IsBusy)
			{
				// cancel the transfer
				Client.CancelAsync();
				return true;
			}
			return false;
		}

		// handels the elapsed event of the timer (for updating GUI)
		private void progressTimer_Elapsed(object sender, EventArgs e)
		{
			// buffer is a ringbuffer which saves size of the total transferred data every interval
			if (buffer.Count == 0)
				buffer.Enqueue(0);

			buffer.Enqueue(totalTransferred);
			if (buffer.Count > bufferSize)
				buffer.Dequeue();

			// calculate average download speed
			long kilobytes = (long)((double)(buffer.Last() - buffer.First()) / 1024.0);
			int seconds = (int)((buffer.Count - 1) * ((double)progressTimer.Interval / 1000.0));
			int avgSpeed = (int)(kilobytes / seconds);

			if (avgSpeed > 0)
			{
				// calculate idle time
				long kilobytesToTransfer = (long)((double)(totalSize - totalTransferred) / 1024.0);
				TimeSpan idleTime = TimeSpan.FromSeconds(kilobytesToTransfer / avgSpeed);
				SetIdleTime(idleTime);
			}

			SetSpeed(avgSpeed);
			SetElapsedTime(DateTime.Now - startTime);
		}

		// resets the GUI
		public void ClearProgress()
		{
			SetElapsedTime(TimeSpan.FromSeconds(0));
			SetIdleTime(TimeSpan.FromSeconds(0));
			SetSpeed(0);
			SetOverallProgress(0);
			SetCurrentProgress(0);
			SetCurrentFile(null);
		}

		// returns maximum value of the current progress bar
		private int GetCurrentProgressMaximum()
		{
			return pbCurrent.Maximum;
		}

		// returns maximum value of the overall progress bar
		private int GetOverallProgressMaximum()
		{
			return pbOverall.Maximum;
		}

		private void EnableProgress()
		{
			if (this.InvokeRequired)
				this.Invoke(new DelegateVoid(EnableProgress));
			else
				this.Enabled = true;
		}

		private void DisableProgress()
		{
			if (this.InvokeRequired)
				this.Invoke(new DelegateVoid(DisableProgress));
			else
				this.Enabled = false;
		}

		// sets current progress bar (GUI)
		private void SetCurrentProgress(int value)
		{
			if (pbCurrent.InvokeRequired)
				pbCurrent.Invoke(new DelegateInt(SetCurrentProgress), new object[] { value });
			else
				pbCurrent.Value = value;
		}

		// sets overall progress bar (GUI)
		private void SetOverallProgress(int value)
		{
			if (pbOverall.InvokeRequired)
				pbOverall.Invoke(new DelegateInt(SetOverallProgress), new object[] { value });
			else
				pbOverall.Value = value;
		}

		// sets current file label (GUI)
		private void SetCurrentFile(string fileName)
		{
			if (lblCurrentFile.InvokeRequired)
				lblCurrentFile.Invoke(new DelegateString(SetCurrentFile), new object[] { fileName });
			else
				lblCurrentFile.Text = fileName;
		}

		// sets speed label (GUI)
		private void SetSpeed(int speed)
		{
			if (lblSpeed.InvokeRequired)
				lblSpeed.Invoke(new DelegateInt(SetSpeed), new object[] { speed });
			else
				lblSpeed.Text = speed.ToString() + " kb/s";
		}

		// sets elapsed time label (GUI)
		private void SetElapsedTime(TimeSpan elapsedTime)
		{
			if (lblElapsedTime.InvokeRequired)
				lblElapsedTime.Invoke(new DelegateTimeSpan(SetElapsedTime), new object[] { elapsedTime });
			else
				lblElapsedTime.Text = GetTimeString(elapsedTime);
		}

		// sets idle time label (GUI)
		private void SetIdleTime(TimeSpan idleTime)
		{
			if (lblIdleTime.InvokeRequired)
				lblIdleTime.Invoke(new DelegateTimeSpan(SetIdleTime), new object[] { idleTime });
			else
				lblIdleTime.Text = GetTimeString(idleTime);
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			CancelTransfer();
		}

		// returns time string from a declared time span
		private string GetTimeString(TimeSpan ts)
		{
			return ts.Hours.ToString("D2") + ":" + ts.Minutes.ToString("D2") + ":" + ts.Seconds.ToString("D2");
		}
	}
}