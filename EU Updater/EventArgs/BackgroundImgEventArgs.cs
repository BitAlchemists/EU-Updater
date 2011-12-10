using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EU_Updater
{
	class BackgroundImgEventArgs : EventArgs
	{
		public string ImagePath { get; set; }

		public BackgroundImgEventArgs(string imagePath)
		{
			ImagePath = imagePath;
		}
	}
}
