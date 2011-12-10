using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EU_Release_Manager.Controller
{
	public class EventArgs<T> : EventArgs
	{
		public T Value { get; set; }

		public EventArgs(T value)
		{
			Value = value;
		}
	}
}
