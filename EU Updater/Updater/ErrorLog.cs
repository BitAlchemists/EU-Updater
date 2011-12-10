using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EU_Updater
{
	public static class ErrorLog
	{
		//path for errorlog-file
		public static string errorPath = "euu_error.log";
		public static object mutex = new object();

		//writes an error message into the log-file
		public static void Add(object sender, string error)
		{
			lock (mutex)
			{
				try
				{
					StreamWriter sw = new StreamWriter(errorPath, true);
					sw.WriteLine("[{0}] {1}: {2}",
						DateTime.Now.ToShortTimeString(), sender.GetType(), error);
					sw.Close();
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
