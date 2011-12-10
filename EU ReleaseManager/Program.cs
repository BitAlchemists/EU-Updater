using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EU_Release_Manager.GUI;

namespace EU_Release_Manager
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ReleaseManagerForm());
		}
	}
}
