using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace EU_Release_Manager.Network
{
	public class MyWebClient : WebClient
	{
		public bool UsePassive { get; set; }
        public int Timeout { get; set; }

		public MyWebClient()
			: base()
		{
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);

			if (address.AbsoluteUri.StartsWith("ftp://"))
				((FtpWebRequest)request).UsePassive = UsePassive;

            if (Timeout > 0)
                request.Timeout = Timeout;

			return request;
		}
	}
}
