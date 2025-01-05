
namespace OS.Communication;

/// <summary>
/// Client connection events
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void ConnectedDeviceEvent(object sender, ConnectedDeviceEventArgs e);

public enum DeviceEventTypes
{
	AbortAll,
	Abort,
	Start,
	Update,
	Receive,
	Connect,
	Disconnect,
	StateChange,
	Error
};

/// <summary>
/// This class is wrapped by the more general cref:"ICommunicationDevice" implementation
/// </summary>
public class ConnectedDeviceEventArgs : EventArgs, IDisposable
{

	#region Enums


	#endregion

	#region Properties

	public DeviceEventTypes EventType;
	public byte[] Data = null;
	public Type iType = null;
	public string Description;
	public ConnectedDeviceEventArgs()
	{
	}
	public ConnectedDeviceEventArgs(byte[] data)
	{
		this.Data = new byte[data.Length];
		data.CopyTo(this.Data, 0);

	}
	public object dataObject = null;

	#endregion

	#region Methods


	#endregion

	#region IDisposable

	// Implement IDisposable.
	// Do not make this method virtual.
	// A derived class should not be able to override this method.

	// Track whether Dispose has been called.
	private bool disposed = false;
	public void Dispose()
	{
		Dispose(true);
		// This object will be cleaned up by the Dispose method.
		// Therefore, you should call GC.SupressFinalize to
		// take this object off the finalization queue
		// and prevent finalization code for this object
		// from executing a second time.
		GC.SuppressFinalize(this);
	}
	// Dispose(bool disposing) executes in two distinct scenarios.
	// 1) If disposing equals true, the method has been called directly
	// or indirectly by a user's code. Managed and unmanaged resources
	// can be disposed.
	// 2) If disposing equals false, the method has been called by the
	// runtime from inside the finalizer and you should not reference
	// other objects. Only unmanaged resources can be disposed.
	private void Dispose(bool disposing)
	{
		// Check to see if Dispose has already been called.
		if (this.disposed == false)
		{
			// If disposing equals true managed resources can be disposed
			if (disposing == true)
			{
				// Dispose managed resources.
				//	component.Dispose();
			}
			// Unmanaged resources are disposed in any case

			//	CloseHandle(handle);
			//	handle = IntPtr.Zero;

			// Note disposing has been done.
			disposed = true;
		}
	}
	~ConnectedDeviceEventArgs()
	{
		// Do not re-create Dispose clean-up code here.
		// Calling Dispose(false) is optimal in terms of
		// readability and maintainability.
		Dispose(false);
	}

	#endregion

}
