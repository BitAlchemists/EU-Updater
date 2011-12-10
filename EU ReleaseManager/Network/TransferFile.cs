using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EU_Release_Manager.Network
{
	public struct TransferFile
	{
		public string name;
		public string destination;
		public long size;
		public string hash;
		public string version;

		public TransferFile(string name, long size, string hash, string version, string destination)
		{
			this.name = name;
			this.size = size;
			this.hash = hash;
			this.version = version;
			this.destination = (destination != null) ? destination : name;
		}

		public TransferFile(string name, long size, string hash, string version)
			: this(name, size, hash, version, null)
		{
		}

		public TransferFile(string name, long size, string hash)
			: this(name, size, hash, null)
		{
		}

		public TransferFile(string name, long size)
			: this(name, size, null)
		{
		}

		public TransferFile(string name)
			: this(name, 0)
		{
		}
	}
}
