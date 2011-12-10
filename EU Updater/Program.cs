using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace EU_Updater
{
   static class Program
   {
      [STAThread]
      static void Main(string[] arguments)
      {
			try
			{
				//waiting for the Installer application to close
				if (arguments.Length > 0)
					Process.GetProcessById(int.Parse(arguments[0])).WaitForExit();
			}
			catch (Exception e)
			{
			}

			//start the Updater form
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new UpdaterForm());
      }
   }
}
