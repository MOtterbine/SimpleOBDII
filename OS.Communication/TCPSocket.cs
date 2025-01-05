using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OS.Communication
{
	/// <summary>
	/// Generic TCP/IP socket-based class is used across platforms
	/// </summary>
	public class TCPSocket : ICommunicationDevice
	{

		public override string ToString()
		{
			return $"{this.IPAddress}:{this.IPPort}";
		}

		public string DeviceName
		{
			get => this.ToString();
			set { }
		}



		#region Event Sourcing

		protected void FireStatusMessage(DeviceEventArgs SocketArgs)
		{
			this.FireCommunicationEvent(SocketArgs);
		}

		public event DeviceEvent CommunicationEvent;

		#endregion

		#region Properties

		public bool AllowMultipleClientConnections { get; set; }

		public ConnectMethods ConnectMethod
		{
			get
			{
				return this._ConnectMethod;
			}
			set
			{
				if (this._ConnectMethod == ConnectMethods.Undefined)
				{
					this._ConnectMethod = value;
				}
			}
		}
		protected ConnectMethods _ConnectMethod = ConnectMethods.Undefined;

		public string MessageString { get; set; }

		/// <summary>
		/// Used to keep a reference of the async callback
		/// </summary>
		private AsyncCallback _AsyncCallback = null;

		public class _InputData
		{
			public byte[] _InputBuffer;
			public int position;
			public int count;
			public _InputData()
			{
				Initialize();
			}
			public void Initialize()
			{
				_InputBuffer = new byte[1024];
				position = 0;
				count = 0;
			}
			public void Clear()
			{
				this._InputBuffer.Initialize();
				this.position = 0;
				count = 0;
			}

		};

		protected _InputData InputData = new _InputData();

		protected IAsyncResult _ConnectAsyncResult = null;

		public IAsyncResult ConnectAsyncResult
		{
			get
			{
				return this._ConnectAsyncResult;
			}
		}
		protected IAsyncResult _ReadAsyncResult = null;
		public IAsyncResult ReadAsyncResult
		{
			get
			{
				return this._ReadAsyncResult;
			}
			set
			{
				this._ReadAsyncResult = value;
			}
		}
		protected IAsyncResult _WriteAsyncResult = null;
		public IAsyncResult WriteAsyncResult
		{
			get
			{
				return this._WriteAsyncResult;
			}
		}
		protected bool _IsListening = false;
		public bool IsListening
		{
			get
			{
				if (this.mainSocket == null)
				{
					return false;
				}
				if (this.ConnectMethod == ConnectMethods.Client)
				{
					return false;
				}
				return this._IsListening;
			}
		}
		protected StateObject MainSocketState = new StateObject();
		// Points to the last socket that sent a message
		protected DateTime CurrentTime = DateTime.Now;
		protected string hostDescriptor = "";

		public string HostDescriptor
		{
			get
			{
				if (this.mainSocket.Connected == true)
				{
					return this.mainSocket.RemoteEndPoint.ToString();
				}
				else
				{
					return "No Connection";
				}
			}
		}
		/// <summary>
		/// The port number and 'key' for the hashtable containing all connected sockets
		/// </summary>
		protected int curSocketKey = -1;
		protected Hashtable connectedClients = new Hashtable(10);

		public int ListenerClientCount
		{
			get
			{
				lock (ClientLock)
				{
					return this.connectedClients.Count;
				}
			}
		}

		public Socket FirstConnectedSocket
		{
			get
			{
				Socket retSock = null;
				lock (ClientLock)
				{
					foreach (DictionaryEntry de in this.connectedClients)
					{
						retSock = ((ClientSocketObject)de.Value).socket;
					}
				}
				return retSock;

			}
		}
		protected Socket mainSocket = null;
		protected ManualResetEvent connectDone = new ManualResetEvent(false);
		protected ManualResetEvent sendDone = new ManualResetEvent(false);
		protected ManualResetEvent receiveDone = new ManualResetEvent(false);

		public ManualResetEvent allDone = new ManualResetEvent(false);
		protected string _IPAddress = "";
		//		[XmlIgnoreAttribute()]
		public string IPAddress
		{
			get
			{
				return this._IPAddress;
			}
			set
			{
				this._IPAddress = value;
			}
		}
		protected int _IPPort = 0;
		//		[XmlIgnoreAttribute()]
		public int IPPort
		{
			get
			{
				return this._IPPort;
			}
			set
			{
				this._IPPort = value;
			}
		}
		protected bool fSocketIsClosing = false;

		public bool Listen = false;
		//		[XmlIgnoreAttribute()]
		public int SendTimeout
		{
			get
			{
				if (this.mainSocket != null)
				{
					return (int)this.mainSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
				}
				return -1;
			}
			set
			{
				if (value >= 0)
				{
					if (this.mainSocket != null)
					{
						this.mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, (int)value);
					}
				}
			}
		}
		//		[XmlIgnoreAttribute()]
		public int ReceiveTimeout
		{
			get
			{
				if (this.mainSocket != null)
				{
					return (int)this.mainSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
				}
				return -1;
			}
			set
			{
				if (value >= 0)
				{
					if (this.mainSocket != null)
					{
						this.mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, (int)value);
					}
				}
			}
		}

		#endregion

		#region Methods

		// Copy Constructor
		public TCPSocket()
		{
			this.Initialize();
			// Create the TCP/IP socket.
			this.mainSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);
		}
		public TCPSocket(ConnectMethods sockMethod)
		{
			this._ConnectMethod = (sockMethod);
			// Create the TCP/IP socket.
			this.mainSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);
		}
		public TCPSocket(int HostPort)
		{

			this._ConnectMethod = ConnectMethods.Listener;
			this._IPPort = HostPort;
			if (this._IPPort == 0)
			{
				this.MessageString = "An invalid port value was passed";
				using (DeviceEventArgs evt = new DeviceEventArgs())
				{
					evt.Event = CommunicationEvents.Error;
					evt.Description = this.MessageString;
					FireStatusMessage(evt);
				}
				return;
			}
			this.mainSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);
		}
		public TCPSocket(string hostname, int port, ConnectMethods sockMethod)
		{
			this._ConnectMethod = sockMethod;
			bool initError = false;
			if ((hostname == null) || (hostname.Length == 0))
			{
				this.MessageString = "An invalid host address was passed for the client connection.";
				initError = true;
			}
			if (port < 1)
			{
				this.MessageString = "An invalid host port was passed for the client connection.";
				initError = true;
			}
			if (initError == true)
			{
				using (DeviceEventArgs evt = new DeviceEventArgs())
				{
					evt.Event = CommunicationEvents.Error;
					evt.Description = this.MessageString;
					FireStatusMessage(evt);
				}
				return;
			}
			this._IPAddress = hostname;
			this._IPPort = port;
			// Create the TCP/IP socket.
			this.mainSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			this.mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
		}
		public TCPSocket(Socket socket)
		{
			this._ConnectMethod = ConnectMethods.Client;
			//this._IPAddress = socket.RemoteEndPoint.hostname;
			//this._IPPort = port;
			// Create the TCP/IP socket.

			this.mainSocket = socket;

			// assuming that the socket exists, is connected ...so, we just want to hear about it
			if (this.MainSocket != null)
			{
				// Create the state object.
				StateObject state = new StateObject();
				state.workSocket = this.MainSocket;
				this._ReadAsyncResult = this.MainSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
					new AsyncCallback(ReceiveCallback), state);
			}

		}


		protected void FireCommunicationEvent(DeviceEventArgs e)
		{
			if (this.CommunicationEvent != null)
			{
				CommunicationEvent(this, e);
			}
		}





		public Socket GetConnectedSocket(IntPtr handle)
		{
			lock (ClientLock)
			{
				foreach (DictionaryEntry de in this.connectedClients)
				{
					if (((ClientSocketObject)de.Value).socket.Handle == handle)
					{
						return ((ClientSocketObject)de.Value).socket;
					}
				}
			}
			return null;
		}
		public Socket GetConnectedSocket(int port)
		{
			// the port number is used as the 'key' to the hashtable values
			lock (ClientLock)
			{
				if (this.connectedClients.ContainsKey(port) == false) return null;
				return ((ClientSocketObject)this.connectedClients[port]).socket;
			}
		}
		/// <summary>
		/// This function is designed to nest within an infinite loop to provide
		/// a means to exit, or timeout.
		/// </summary>
		/// <param name="counter"></param>
		/// <param name="limit"></param>
		/// <param name="sleepInterval"></param>
		/// <returns></returns>
		protected bool StepLoop(ref int counter, int limit, int sleepInterval)
		{
			System.Threading.Thread.Sleep(sleepInterval);
			//Application.DoEvents();
			if (counter++ > limit)
			{
				return false;
			}
			return true;
		}
		/// <summary>
		/// This function runs a synchronous loopback test. Usually used with serial-to-ethernet
		/// servers/clients equipped with a loopback connector.
		/// </summary>
		/// <returns></returns>
		public bool RunSyncLoopBackTest()
		{
			int tCounter = 0;
			this.ConnectMethod = ConnectMethods.Client;
			if (this.IsConnected == true)
			{
				this.Close();
				tCounter = 0;
				while (this.ReadAsyncResult == null)
				{
					if (this.StepLoop(ref tCounter, 100, 10) == false)
					{
						return false;
					}
				}
				tCounter = 0;
				while (this.ReadAsyncResult.IsCompleted != true)
				{
					if (this.StepLoop(ref tCounter, 100, 10) == false)
					{
						return false;
					}
				}
				tCounter = 0;
				while (this.IsConnected == true)
				{
					if (this.StepLoop(ref tCounter, 100, 10) == false)
					{
						return false;
					}
				}
			}
			this.Open();
			tCounter = 0;
			while (this.ConnectAsyncResult == null)
			{
				if (this.StepLoop(ref tCounter, 100, 10) == false)
				{
					return false;
				}
			}
			tCounter = 0;
			while (this.ConnectAsyncResult.IsCompleted != true)
			{
				if (this.StepLoop(ref tCounter, 100, 10) == false)
				{
					return false;
				}
			}
			if (this.mainSocket.Connected == false)
			{
				return false;
			}

			byte[] testData = new byte[256];
			byte[] rcvData = new byte[1];
			int i = 0;
			for (i = 0; i < 256; i++)
			{
				testData[i] = (byte)i;
			}


			try
			{
				for (i = 0; i < 5; i++)
				{
					this.mainSocket.Send(testData, 0, 1, SocketFlags.None);
					this.receiveDone.Reset();
					this.receiveDone.WaitOne();
					this.Read(rcvData, 0, rcvData.Length);

					//	this.mainSocket.Receive(testData,0,1,SocketFlags.None);
					if (rcvData[0] != testData[i])
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				//MessageBox.Show("Error: " + ex.Message,"Loopback Test",MessageBoxButtons.OK,
				//	MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
		private bool StartListening()
		{
			if (this._IPPort == 0)
			{
				return false;
			}
			// Data buffer for incoming data.
			byte[] bytes = new Byte[1024];
			IPEndPoint localEndPoint = new IPEndPoint(System.Net.IPAddress.Any, this._IPPort);

			// Bind the socket to the local endpoint and listen for incoming connections.
			try
			{
				this.mainSocket.Bind(localEndPoint);
				this.mainSocket.Listen(0);
				Listen = true;

				//         while (Listen == true)
				//       {
				// Set the event to nonsignaled state.
				allDone.Reset();

				// Start an asynchronous socket to listen for connections.
				using (DeviceEventArgs sk = new DeviceEventArgs())
				{

					this.mainSocket.BeginAccept(new AsyncCallback(AcceptCallback),
						this.mainSocket);
					// The socket is considered as listening at this point
					this._IsListening = true;
					sk.Event = CommunicationEvents.Listening;
					FireStatusMessage(sk);
				}

				// Wait until a connection is made before continuing.
				//       allDone.WaitOne();
				//     }
			}
			catch (SocketException ex)
			{
				this._IsListening = false;
				using (DeviceEventArgs sk = new DeviceEventArgs())
				{
					sk.Event = CommunicationEvents.Error;
					sk.Description = ex.Message;
					this.MessageString = ex.Message;
					FireStatusMessage(sk);
				}
				return false;
			}
			catch (Exception ex)
			{
				this._IsListening = false;
				this.MessageString = ex.Message;
				using (DeviceEventArgs sk = new DeviceEventArgs())
				{
					sk.Event = CommunicationEvents.Error;
					sk.Description = ex.Message;
					FireStatusMessage(sk);
				}
				return false;
			}
			return true;
		}
		public void AcceptCallback(IAsyncResult ar)
		{
			ClientSocketObject socketObject = null;

			using (DeviceEventArgs sk = new DeviceEventArgs())
			{
				if (this.fSocketIsClosing == true)
				{
					this.fSocketIsClosing = false;
					sk.Event = CommunicationEvents.Disconnected;
					FireStatusMessage(sk);
					return;
				}
				// Signal the main thread to continue.
				//				allDone.Set();
				Socket listener = null;
				int port = 0;
				try
				{
					// Get the socket that handles the client request.
					listener = (Socket)ar.AsyncState;

					// Then, get the new client socket
					socketObject = new ClientSocketObject();
					if (((Socket)listener).Handle.ToInt32() == -1)
					{
						return;
					}
					socketObject.socket = (Socket)listener.EndAccept(ar);

					// Return if only one client is intended to communicate
					if (!this.AllowMultipleClientConnections && this.connectedClients.Count > 0)
					{
						socketObject.socket.Close();
						listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
						return;
					}

					port = ((System.Net.IPEndPoint)socketObject.socket.RemoteEndPoint).Port;
					// Save the port (as key) and socket object (as value) in our hashtable
					lock (ClientLock)
					{
						this.connectedClients.Add(port, socketObject);
					}
				}

				catch (Exception ex)
				{

					this.MessageString = ex.Message;
					sk.Event = CommunicationEvents.Error;
					FireStatusMessage(sk);
					return;
				}

				try
				{
					this.curSocketKey = port;
					lock (ClientLock)
					{
						// Create the state object.
						((ClientSocketObject)connectedClients[port]).state = new StateObject();
						// StateObject state = new StateObject();
						((ClientSocketObject)connectedClients[port]).state.workSocket = ((ClientSocketObject)connectedClients[port]).socket;

						// DON'T START A BEGINRECEIVE OPERATION HERE...THIS IS THE LISTENER - TAKING NEW CLIENTS
						// LET THE HOST APP GET DATA FROM ICommunicationDevice EVENTS ACCESSIBLE IN THE EVENT BELOW
						// THE HOST APP WILL DEAL WITH THE CLIENTS - AND THEIR RESPECTIVE EVENTS
						//this._ReadAsyncResult = ((clientSocketObject)connectedClients[port]).socket.BeginReceive(((clientSocketObject)connectedClients[port]).state.buffer, 0, StateObject.BufferSize, 0,
						//    new AsyncCallback(ReceiveCallback), ((clientSocketObject)connectedClients[port]).state);
					}
					listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
					lock (ClientLock)
					{
						sk.Event = CommunicationEvents.ClientConnected;
						sk.Description = ((ClientSocketObject)connectedClients[port]).socket.RemoteEndPoint.ToString(); ;
						//sk.socketHandler = ((ClientSocketObject)connectedClients[port]).socket;
						sk.dataObject = port;
						// Offer a reference to the connected client as an 'ICommunicationDevice'
						sk.dataObject1 = new TCPSocket(((ClientSocketObject)connectedClients[port]).socket);
						FireStatusMessage(sk);
					}
				}
				catch (Exception ex)
				{
					this.MessageString = ex.Message;
					sk.Event = CommunicationEvents.Error;
					FireStatusMessage(sk);
					return;
				}

			}
		}
		public byte[] Read()
		{
			return new byte[] { };
		}

		private object ClientLock = new object();
		public void ReceiveCallback(IAsyncResult ar)
		{
			DeviceEventArgs sk = new DeviceEventArgs();
			String content = String.Empty;

			if (this.fSocketIsClosing == true)
			{
				this.fSocketIsClosing = false;
				sk.Description = "Connection Closed";
				sk.Event = CommunicationEvents.Disconnected;
				FireStatusMessage(sk);
				return;
			}
			// Retrieve the state object and the handler socket
			// from the asynchronous state object.
			StateObject state = null;
			Socket sck = null;

			state = (StateObject)ar.AsyncState;
			sck = state.workSocket;
			int portKey = 0; // serves as the socket port and the hashtable key

			if (this.ConnectMethod == ConnectMethods.Listener)
			{
				lock (this.ClientLock)
				{
					if (this.connectedClients.Count > 0)
					{
						foreach (DictionaryEntry de in this.connectedClients)
						{
							if (((ClientSocketObject)de.Value).socket.Handle == sck.Handle)
							{
								this.curSocketKey = portKey = (int)de.Key;
								break;
							}
						}
					}
				}
			}
			else
			{
				// Only do this in the case of a client where 'MainSocket' is the only socket
				this.MainSocketState = (StateObject)ar.AsyncState;
				this.MainSocketState.workSocket = state.workSocket;
			}
			int bytesRead = 0;
			try
			{

				// Read data from the client socket. 
				if (sck != null)
				{
					// Take a look at the endpoint's ip address - if its string representation is 
					// zero length then it's considered disconnected

					if ((sck.Connected == true) && (sck.Handle.ToInt32() != -1))
					{
						if (sck.Poll(50, SelectMode.SelectError) == false)
						{
							bytesRead = sck.EndReceive(ar);
							//		this._ReadAsyncResult = null;
						}
					}
				}
			}
			catch (SocketException ex)
			{
				string txt;
				if (ex.ErrorCode == 10014)
				{
					txt = "The host at address " + ((IPEndPoint)sck.RemoteEndPoint).Address.ToString()
						+ " has unexpectedly disconnected.";
				}
				else
				{
					txt = ex.Message;
				}
				txt += " - Error Code: " + Convert.ToString(ex.ErrorCode);
				this.MessageString = txt;
				sk.Event = CommunicationEvents.Error;
				sk.Description = txt;
				FireStatusMessage(sk);
			}
			catch (ObjectDisposedException ex)
			{
				this.MessageString = ex.Message;
				sk.Event = CommunicationEvents.Error;
				sk.Description = ex.Message;
				FireStatusMessage(sk);
			}
			catch (Exception ex)
			{
				this.MessageString = ex.Message;
				sk.Event = CommunicationEvents.Error;
				sk.Description = ex.Message;
				FireStatusMessage(sk);
			}
			if (bytesRead > 0)
			{
				// Move the byte data into the local input buffer
				for (int i = 0; i < bytesRead; i++)
				{
					this.InputData._InputBuffer[this.InputData.position + i] = state.buffer[i];
				}
				this.InputData.position += bytesRead;
				this.InputData.count += bytesRead;
				state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
				// Did we get all of the expected data at this point ?
				try
				{
					// *************END RECEIVE EVENT**********************
					using (DeviceEventArgs evt = new DeviceEventArgs())
					{
						evt.data = new byte[(int)this.InputData.count];
						for (int i = 0; i < this.InputData.count; i++)
						{
							evt.data[i] = this.InputData._InputBuffer[i];
						}
						this.InputData.Clear();
						evt.dataObject = sck;
						evt.Event = CommunicationEvents.ReceiveEnd;
						evt.Description = content;
						FireStatusMessage(evt);
					}
					state.sb.Remove(0, state.sb.ToString().Length);
				}
				catch (Exception ex)
				{
					state.sb.Remove(0, state.sb.ToString().Length);
					this.MessageString = "RcvCallback - " + ex.Message;
				}
				try
				{
					if (this._ConnectMethod == ConnectMethods.Listener)
					{
						lock (ClientLock)
						{
							if (this.connectedClients.Count > 0)
							{
								this._ReadAsyncResult = ((ClientSocketObject)this.connectedClients[portKey]).socket.BeginReceive(
									state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
							}
						}
					}
					else
					{
						this._ReadAsyncResult = sck.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
							new AsyncCallback(ReceiveCallback), state);
					}
				}
				catch (Exception)
				{

				}
			}
			else // No bytes - Remote Disconnect
			{
				using (DeviceEventArgs evt = new DeviceEventArgs())
				{
					sck.Close();
					evt.Event = CommunicationEvents.RemoteDisconnect;
					lock (ClientLock)
					{
						if (this.connectedClients.Count > 0)
						{
							this.connectedClients.Remove(portKey);
						}
					}
					// Keep on listening if we're not closing, and we are a server
					if ((this.fSocketIsClosing == false) && (this._ConnectMethod == ConnectMethods.Listener))
					{
						try
						{
							if (this.mainSocket != null)
							{
								this.mainSocket.BeginAccept(new AsyncCallback(AcceptCallback),
									this.mainSocket);
							}
						}
						catch (Exception e)
						{
							this.MessageString = e.Message;
						}
					}
					else
					{
						evt.Event = CommunicationEvents.Disconnected;
					}
					evt.dataObject = this.curSocketKey; // indicate which port remotely disconnected

					FireStatusMessage(evt);
				}
			}
		}
		public Socket MainSocket
		{
			get
			{
				return this.mainSocket;
			}
		}
		/// <summary>
		/// It is important that all notifications route through this function
		/// </summary>
		/// <param name="SocketArgs"></param>
		protected void SendCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				Socket client = (Socket)ar.AsyncState;

				if (client == null)
				{
					return;
				}
				// Complete sending the data to the remote device.
				int bytesSent = client.EndSend(ar);
				//	this._WriteAsyncResult = null;
				//Console.WriteLine("Sent {0} bytes.", bytesSent);

				// Signal that all bytes have been sent.
				sendDone.Set();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
		private bool StartClient()
		{
			this.ConnectTimedOut = false;
			try     // Connect to a remote device.
			{
				IPEndPoint remoteEP = new IPEndPoint(System.Net.IPAddress.Parse(this.IPAddress)
					, this._IPPort);

				// Connect to the remote endpoint.
				this._AsyncCallback = new AsyncCallback(ConnectCallback);
				this._ConnectAsyncResult = this.mainSocket.BeginConnect(remoteEP,
					this._AsyncCallback, this.mainSocket);

				connectDone.Reset();
				bool success = this._ConnectAsyncResult.AsyncWaitHandle.WaitOne(8000, true);
				if (!this.mainSocket.Connected)
				{
					if (!success)
					{
						this.mainSocket.Close();
						//this.mainSocket.Dispose();
						this.ConnectTimedOut = true;
						this.FireCommunicationEvent(new DeviceEventArgs()
						{
							Event = CommunicationEvents.Error,
							Description = "WiFi Connect Timeout",
						});
					}
				}
			}
			catch (Exception e)
			{
				using (DeviceEventArgs evt = new DeviceEventArgs())
				{
					evt.Event = CommunicationEvents.Error;
					evt.Description = e.Message;
					this.MessageString = e.Message;
					FireStatusMessage(evt);
					return false;
				}
			}

			FireCommunicationEvent(new DeviceEventArgs()
			{
				Event = CommunicationEvents.ClientConnected,
				Description = "Connected"
			});
			return true;
		}

		private bool ConnectTimedOut = false;

		protected void ConnectCallback(IAsyncResult ar)
		{

			using (DeviceEventArgs evt = new DeviceEventArgs())
			{
				Socket sck = null;
				try
				{
					if (this.ConnectTimedOut) return;

					// Retrieve the socket from the state object.
					sck = (Socket)ar.AsyncState;

					// Complete the connect action.
					sck.EndConnect(ar);

					Console.WriteLine($"Socket connected to {sck.RemoteEndPoint.ToString()}");

					// Signal that the connection action has completed.
					evt.Description = this.MainSocket.RemoteEndPoint.ToString();
					evt.Event = CommunicationEvents.ConnectedAsClient;
				}
				catch (ObjectDisposedException ex)// usually, when the user closes before connection is resolved.
				{
					evt.Event = CommunicationEvents.Disconnected;
					evt.Description = $"Error: {ex.Message}";
					FireStatusMessage(evt);
					return;
				}
				catch (Exception e)
				{
					evt.Event = CommunicationEvents.Error;
					evt.Description = e.Message;
					FireStatusMessage(evt);
					return;
				}
				finally
				{
					connectDone.Set();
				}
				if (sck != null && sck.Connected)
				{
					// Create the state object.
					StateObject state = new StateObject();
					state.workSocket = sck;
					this._ReadAsyncResult = sck.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
						new AsyncCallback(ReceiveCallback), state);
				}
				FireStatusMessage(evt);
			}
		}

		#endregion

		#region Inherited Members

		public bool IsConnected
		{
			get
			{
				if (this.mainSocket == null)
				{
					return false;
				}
				return this.mainSocket.Connected;
			}
		}
		public string Description
		{
			get
			{
				return "TCP :" + this.IPAddress + ":" + this.IPPort;
			}
			set
			{

			}
		}
		public int Read(byte[] destBuffer, int destOffset, int count)
		{
			try
			{
				int byteCount = 0;
				int totalLength = count + destOffset;
				if (totalLength == 0)
				{
					return 0;
				}
				byte[] retData = null;
				if (this.ConnectMethod == ConnectMethods.Client)
				{
					retData = new byte[totalLength];
					for (byteCount = 0; byteCount < count; byteCount++)
					{
						// access the only socket
						retData[byteCount + destOffset] = this.InputData._InputBuffer[byteCount];
					}
				}
				else
				{
					retData = new byte[totalLength];
					for (byteCount = 0; byteCount < count; byteCount++)
					{
						// access the socket for the last client to send something
						retData[byteCount + destOffset] = ((ClientSocketObject)this.connectedClients[this.curSocketKey]).state.buffer[byteCount];
					}
				}
				retData.CopyTo(destBuffer, 0);
				return byteCount;
			}
			catch (Exception)
			{
				return 0;
			}
		}
		public async Task<bool> Send(string data)
		{

			if ((data == null) || (data.Length == 0) || !this.IsConnected)
			{
				return false;
			}
			await this.Send(Encoding.ASCII.GetBytes(data), 0, data.Length);
			return false;
		}
		public async Task<bool> Send(byte[] buffer, int offset, int count)
		{
			//		int offset = 0;
			if ((buffer == null) || (buffer.Length == 0) || !this.IsConnected)
			{
				return false;
			}
			await Task.Run(() =>
			{
				if ((offset + count) > buffer.Length)
				{
					this.MessageString = "The offset and data length parameters would have " +
						"extended beyond the size of the byte array";
					return false;
				}
				if (this._ConnectMethod == ConnectMethods.Client)
				{
					try
					{
						// Begin sending the data to the remote device via the main (the only) socket.
						this._WriteAsyncResult = this.mainSocket.BeginSend(buffer, offset, count, 0,
							new AsyncCallback(SendCallback), this.mainSocket);
					}
					catch (Exception ex)
					{
						this.MessageString = ex.Message;
						return false; ;
					}
				}
				else
				{
					if (this.ListenerClientCount > 0)
					{
						if (this.curSocketKey > 0)
						{
							try
							{
								// Begin sending the data to the remote device via the main (the only) socket.
								this._WriteAsyncResult = this.GetConnectedSocket((int)this.curSocketKey).BeginSend(buffer, 0, buffer.Length, 0,
									new AsyncCallback(SendCallback), this.GetConnectedSocket((int)this.curSocketKey));
							}
							catch
							{
								if (this.GetConnectedSocket((int)this.curSocketKey).Connected == false)
								{
									lock (ClientLock)
									{
										this.connectedClients.Remove((int)this.curSocketKey);
									}
								}
							}
						}
					}

				}
				return true;
			});
			return false;
		}
		public virtual bool Reset()
		{
			if (this.IsConnected)
			{
				this.Close();
				return this.Open();
			}
			return false;
		}
		bool ICommunicationDevice.Open(string commChannel = null) 
		{
			// commChannel has no meaning in this tcp/ip class
			// IP address and port need to be set before open call
			return this.Open();
		}

        public bool Open()
        {
            if (this.mainSocket != null)
			{
				try
				{
					if (this.mainSocket.Connected == true)
					{
						this.mainSocket.Shutdown(SocketShutdown.Both);
					}
					this.mainSocket.Close();
					this.mainSocket = null;
				}
				catch (Exception ex)
				{
					this.MessageString = ex.Message;
					return false;
				}
			}
			// Create the TCP/IP socket.
			this.mainSocket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);


			if (this.ConnectMethod == ConnectMethods.Client)
			{

				return this.StartClient();
			}
			else
			{
				return this.StartListening();
			}
		}
		public virtual bool Close()
		{
			try
			{   // When this is a client
				if (this.ConnectMethod == ConnectMethods.Client)
				{
					if (this.mainSocket != null)
					{
						if (this.mainSocket.Connected == true)
						{
							this.fSocketIsClosing = true;
							this.mainSocket.Shutdown(SocketShutdown.Both);
							this.mainSocket.Disconnect(true);
						}
						//this.FireCommunicationEvent(CommunicationEvents.Disconnected, "Connection Closed");

						//	//this.mainSocket.Close();
						//this.mainSocket.Dispose();
						//this.mainSocket.EndDisconnect()
						//	this.mainSocket = null;
						//this.FireCommunicationEvent(new ChannelEventArgs() { Event = CommunicationEvents.Disconnected, Description = "Connection Closed" });
					}
				}
				else // When this is a server (listener)
				{
					if (this.mainSocket != null)
					{
						if (this.mainSocket.Connected == true)
						{
							this.fSocketIsClosing = true;
							this.mainSocket.Shutdown(SocketShutdown.Both);
						}
						this.mainSocket.Close();
						this.mainSocket = null;
						lock (ClientLock)
						{
							foreach (DictionaryEntry de in this.connectedClients)
							{
								((ClientSocketObject)de.Value).socket.Close();
							}
							this.connectedClients.Clear();
						}
					}
				}
			}
			catch (Exception ex)
			{
				this._IsListening = false;
				using (DeviceEventArgs evt = new DeviceEventArgs())
				{
					this.MessageString = ex.Message;
					evt.Event = CommunicationEvents.Error;
					evt.Description = ex.Message;
					FireStatusMessage(evt);
				}
				return false;
			}
			return true;
		}
		public virtual bool Initialize()
		{
			return true;
		}
		//public void RunEditForm()
		//{
		//	CommunicationChannelEditor editor = new CommunicationChannelEditor(typeof(TCPSocket));
		//	editor.ShowDialog();
		//}
		public bool CloseConnectedSocket(int port)
		{
			try
			{
				if (this._ConnectMethod != ConnectMethods.Listener) return false;
				Socket clientSocket = this.GetConnectedSocket(port);
				if (clientSocket != null)
				{
					clientSocket.Close();
					this.connectedClients.Remove(port);
				}
				return true;
			}
			catch (Exception ex)
			{
				using (DeviceEventArgs evt = new DeviceEventArgs())
				{
					this.MessageString = string.Format("TCPSocket.CloseConnectedSocket(...) - Error closing socket at port {0} - {1}", port, ex.Message);
					evt.Event = CommunicationEvents.Error;
					evt.Description = string.Format("Error closing socket at port {0} - {1}", port, ex.Message); ;
					FireStatusMessage(evt);
				}


			}
			return false;

		}

		#endregion
	}
}
