using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EU_Release_Manager.Controller
{
	public static class Strings
	{
		// MessageBox Text
		public static string FillOutAllFields = "Please fill out all fields";
		public static string PleaseSelectValidDirectory = "Please select a valid Directory";
		public static string VersionTooLow = "Version nr. must be higher than the latest version";
		public static string CurrentlyLocked = "Upload is currently locked, please try again later";
		public static string UnableToLock = "Unable to lock the fileserver";
		public static string UnableToUnlock = "Unable to unlock the fileserver, please unlock manually";
		public static string UnableToCreateRemoteDirectory = "Unable to create remote directory";
		public static string UnableToCreateLocalDirectory = "Unable to create local directory";
		public static string UnableToAnalyzeNewVersion = "Unable to analyze new version";
		public static string UnableToDownloadConfigurationFiles = "Unable to download configuration files";
		public static string UnableToBuildConfiguration = "Unable to build configuration";
		public static string UnableToBuildDownloadList = "Unable to build download List";
		public static string UnableToCleanUpRemoteDirectory = "Unable to clean up remote directory";
		public static string UploadFailed = "Upload failed";
		public static string UploadCompleted = "Upload completed";
		public static string DownloadFailed = "Download failed";
		public static string DownloadCompleted = "Download completed";

		// Status Text
		public static string NoSettingsFound = "No Settings Found";
		public static string Timeout = "Timeout, Please Check Your Settings";
		public static string InvalidDirectory = "Invalid Directory, Please Check Your Settings";
		public static string InvalidLogin = "Invalid Login, Pease Check Your Settings";
		public static string Connecting = "Connecting...";
		public static string LatestVersion = "Latest Version: ";
		public static string NoVersionsFound = "No Versions Found";
		public static string AnalysingNewVersion = "Analysing New Version...";
		public static string DownloadingConfigurationFiles = "Downloading Configuration Files...";
		public static string BuildingConfiguration = "Building Configuration...";
		public static string BuildingDownloadList = "Building Download List...";
		public static string UploadingFiles = "Uploading Files...";
		public static string DownloadingFiles = "Downloading Files...";
		public static string CleaningUp = "Cleaning Up...";

		// Other Text
		public static string Settings = "Settings";
		public static string ReleaseManager = "Release Manager";
	}
}
