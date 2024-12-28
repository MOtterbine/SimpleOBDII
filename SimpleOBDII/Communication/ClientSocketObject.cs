using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Communication
{
	public class ClientSocketObject
	{
		public StateObject state = null;
		public Socket socket = null;
	};
}
