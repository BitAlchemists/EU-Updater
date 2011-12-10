using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EU_Updater
{
	class StatusEventArgs : EventArgs
	{
		public string Text { get; set; }

		public StatusEventArgs(string text)
		{
			Text = text;			
		}
	}
}
