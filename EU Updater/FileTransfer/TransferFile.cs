using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EU_Updater
{
	public struct TransferFile
	{
		public string Name;
		public long Size;
		public string Destination;

		public TransferFile(string name, long size, string destination)
		{
			this.Name = name;
			this.Size = size;
			this.Destination = destination;
		}

		public TransferFile(string name, long size)
			: this(name,size,"")
		{
		}
	}
}
