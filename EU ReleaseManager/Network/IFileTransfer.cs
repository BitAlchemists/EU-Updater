using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EU_Release_Manager.Network
{
	interface IFileTransfer
	{
		MyWebClient Client { get; }

		void CreateDirectoryAsync(string directory, Uri remotePath, FileTransferCallback callback);
		void CreateDirectory(string directory, Uri remotePath);
		void DeleteDirectoryAsync(string directory, Uri remotePath, FileTransferCallback callback);
		void DeleteDirectory(string directory, Uri remotePath);
		void DeleteFileAsync(string file, Uri remotePath, FileTransferCallback callback);
		void DeleteFile(string file, Uri remotePath);

		bool DownloadFileAsync(TransferFile file, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback);

		bool DownloadFilesAsync(List<TransferFile> files, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback);

		bool UploadFileAsync(TransferFile file, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback);

		bool UploadFilesAsync(List<TransferFile> files, bool showProgress,
			Uri remotePath, string localPath, FileTransferCallback callback);

		bool CancelTransfer();
		void ClearProgress();
	}
}
