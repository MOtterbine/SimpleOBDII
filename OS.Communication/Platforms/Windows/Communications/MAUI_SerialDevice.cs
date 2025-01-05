using OS.Communication;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace OS.PlatformShared;

public partial class MAUI_SerialDevice : IDevicesService, ICommunicationDevice, IDisposable, ISerialDevice
{
    public const int BUFFER_SIZE = 1024;
    private byte[] TmpBuffer = new byte[BUFFER_SIZE];

    public event OS.Communication.DeviceEvent CommunicationEvent;
    private CancellationTokenSource cancellationTokenSource = null;

    #region Properties

    public string MessageString { get; set; }

    private Windows.Devices.SerialCommunication.SerialDevice SelectedSerialDevice { get; set; } = null;

    public SerialStopBitCount StopBits
    {
        get
        {
            return this.SelectedSerialDevice.StopBits;
        }
        set
        {
            this.SelectedSerialDevice.StopBits = value;
        }
    }
    protected int _StopBits = 0;
    public SerialParity Parity
    {
        get
        {
            return this.SelectedSerialDevice.Parity;
        }
        set
        {
            this.SelectedSerialDevice.Parity = value;
        }
    }
    protected ushort _DataBits = 8;
    public ushort DataBits
    {
        get
        {
            return this.SelectedSerialDevice.DataBits;
        }
        set
        {
            this.SelectedSerialDevice.DataBits = value;
        }
    }

    public uint BaudRate { get; set; } = 9600;

    public SerialHandshake Handshake
    {
        get
        {
            return this.SelectedSerialDevice.Handshake;
        }
        set
        {
            if (this.SelectedSerialDevice.Handshake != value)
            {
                this.SelectedSerialDevice.Handshake = value;
            }
        }
    }

    #endregion

    public MAUI_SerialDevice()
    {
        InitDevices();
    }

    public MAUI_SerialDevice(string chName, uint baud)
    {
        this.BaudRate = baud;
        this.DeviceName = chName;
        // Starts asychrounous methods
        InitDevices();
    }

    private void InitDevices()
    {

        Task.Factory.StartNew(LoadPortsList);

    }

    private async Task LoadPortsList()
    {
        try
        {
            devicesInfoList = await DeviceInformation.FindAllAsync(Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelector());
        }
        catch (Exception e)
        {
            FireErrorEvent($"Unable to get ports list. {e}");
        }
    }


    public bool Initialize()
    {
        return true;
    }


    private void FireReceiveEvent(byte[] bytes)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.data = bytes;
            evt.Event = CommunicationEvents.ReceiveEnd;
            evt.Description = "Serial Device Response";
            this.FireDeviceEvent(evt);
        }
    }

    protected void FireDeviceEvent(DeviceEventArgs e)
    {
            MainThread.BeginInvokeOnMainThread(() => {
                if (this.CommunicationEvent != null)
                {
                        CommunicationEvent(this, e); 
                }
            });
            
    }

    private void FireEvent(CommunicationEvents eventType, string message = null)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = eventType;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
    }

    protected void FireCommunicationEvent(DeviceEventArgs e)
    {
        if (this.CommunicationEvent != null)
        {
            CommunicationEvent(this, e);
        }
    }

    #region Inherited Properties

    public string DeviceName { get; set; }

    public string Description => "Windows serial device: " + this.SelectedSerialDevice == null ? "<none>" : SelectedSerialDevice.PortName;

    public bool IsEnabled => this.SelectedSerialDevice != null;

    public bool IsConnected { get; set; }

    protected bool _IsListening = false;

    #endregion

    #region Inherited Methods


    private DeviceInformationCollection devicesInfoList = null;

    private async Task<bool> TrySetDevice(string deviceName)
    {
        try
        {
            Windows.Devices.SerialCommunication.SerialDevice tempPort = null;

            // Find the device by name from a collection of deviceinfos
            var g = (from bd in this.devicesInfoList
                     where bd?.Name == deviceName// DeviceManager.Instance.CommChannelName
                     select bd).FirstOrDefault();

            if (g == null)
            {
                FireErrorEvent($"Device \"{deviceName}\" not found");
                return false;
            }

            if(!g.IsEnabled)
            {
                FireErrorEvent($"Device \"{deviceName}\" not paired");
                return false;
            }
            try
            {
               tempPort = await Windows.Devices.SerialCommunication.SerialDevice.FromIdAsync(g.Id);
            }
            catch (Exception) 
            { 
            }
            if (tempPort == null)
            {
                FireErrorEvent($"Device \"{deviceName}\" not Available");
                return false;
            }

            if (this.SelectedSerialDevice == null || !this.SelectedSerialDevice.Equals(tempPort))
            {
                this.SelectedSerialDevice = tempPort;
                SelectedSerialDevice.BaudRate = this.BaudRate;
                SelectedSerialDevice.BreakSignalState = false;
                SelectedSerialDevice.DataBits = 8;
                SelectedSerialDevice.Handshake = SerialHandshake.None;
                SelectedSerialDevice.IsDataTerminalReadyEnabled = true;
                SelectedSerialDevice.IsRequestToSendEnabled = true;
                SelectedSerialDevice.Parity = SerialParity.None;
                SelectedSerialDevice.StopBits = SerialStopBitCount.One;
                SelectedSerialDevice.ReadTimeout = new TimeSpan(0, 0, 0, 0, 5);
                SelectedSerialDevice.WriteTimeout = new TimeSpan(0, 0, 0, 0, 5);
            }
        }
        catch (Exception e)
        {
            FireErrorEvent($"Error: {e.Message}");
            return false;
        }
        finally
        {
            //  this._settingDevice = false;
        }
        if (this.SelectedSerialDevice == null)
        {
            return false;
        }
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.ConnectedAsClient;
            evt.Description = "Connected...";
            FireDeviceEvent(evt);
        }
        return this.SelectedSerialDevice != null;
    }

    Task listenTask = null;

    private async void beginListen(string commChannel)
    {

        if (String.IsNullOrEmpty(commChannel))
        {
            FireErrorEvent(OS.Communication.Constants.COMMUNICATION_DEVICE_NOT_SETUP);
            return;
        }

        // Attempt to set the device to a user-set name (which might not be available)
        if (await TrySetDevice(commChannel))
        {
            if (this.SelectedSerialDevice != null)
            {
                this.IsConnected = true;
                openEvent.Set();

                while (this.IsConnected)
                {
                    await this.Listen();
                }

                // Will free the port (close it) - DO NOT call SelectedSerialDevice = null
                this.SelectedSerialDevice.Dispose();
            }
        }
        else
        {
            FireErrorEvent($"Unable to access device: {commChannel}");
            openEvent.Set();
        }

        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.Disconnected;
            FireDeviceEvent(evt);
        }

    }
    private ManualResetEvent openEvent = new ManualResetEvent(true);
    public bool Open(string commChannel)
    {
        try
        {
            openEvent.Reset();
            if (this.listenTask == null || listenTask.IsCompleted)
            {
                listenTask = new Task(async ()=> { beginListen(commChannel); });
                listenTask.Start();
            }
            openEvent.WaitOne();
            return this.IsConnected;
        }
        catch (Exception ex)
        {
            FireErrorEvent($"Device Open Failure - {ex.Message}");
        }
        finally
        {
            openEvent.Set();
        }
        return false;
    }

    public bool Close()
    {
        // stops the listen thread...
        this.cancellationTokenSource?.Cancel();
        this.IsConnected = false;
        return true;
    }

    private void OnSerialPinChanged(SerialDevice sender, PinChangedEventArgs args)
    {
        switch (args.PinChange)
        {
            case Windows.Devices.SerialCommunication.SerialPinChange.BreakSignal:
                break;
            case Windows.Devices.SerialCommunication.SerialPinChange.CarrierDetect:
                break;
            case Windows.Devices.SerialCommunication.SerialPinChange.ClearToSend:
                break;
            case Windows.Devices.SerialCommunication.SerialPinChange.DataSetReady:
                break;
            case Windows.Devices.SerialCommunication.SerialPinChange.RingIndicator:
                break;
            default:
                break;
        }
    }

    private void FireErrorEvent(string message)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.Error;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
    }

    private async Task Listen()
    {

        if (this.SelectedSerialDevice == null)
        {
            this.FireErrorEvent($"ERROR: Unable to access serial device");
            return;
        }

        int bytesRead = 0;
        try
        {
            this.cancellationTokenSource = new CancellationTokenSource();

            bytesRead = await this.SelectedSerialDevice.InputStream.AsStreamForRead().ReadAsync(TmpBuffer, 0, BUFFER_SIZE - 1, this.cancellationTokenSource.Token);
            if (bytesRead > 0)
            {
                FireReceiveEvent(TmpBuffer.Take((int)this.SelectedSerialDevice.BytesReceived).ToArray());
            }
        }
        catch (Exception )
        {
        }
        finally
        {
            this.cancellationTokenSource.Cancel();
        }
    }

    public async Task<bool> Send(string text)
    {
        return await this.Send(Encoding.UTF8.GetBytes(text), 0, text.Length);
    }

    public async Task<bool> Send(byte[] buffer, int offset, int count)
    {
        if (this.SelectedSerialDevice == null)
        {
            FireErrorEvent("Device not created...");
            return false;
        }
        try
        {
            await this.SelectedSerialDevice.OutputStream.WriteAsync(buffer.AsBuffer());
            return true;
        }
        catch (Exception ex)
        {
            this.MessageString = ex.Message;
        }
        return false;
    }

    public IList<string> GetDeviceList()
    {
        return (IList<string>)this.devicesInfoList.ToList().ConvertAll(d => d.Name).Distinct().ToList();
    }

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
                this.SelectedSerialDevice?.Dispose();
            }
            // Unmanaged resources are disposed in any case

            //	CloseHandle(handle);
            //	handle = IntPtr.Zero;

            // Note disposing has been done.
            disposed = true;
        }
    }
    ~MAUI_SerialDevice()
    {
        // Do not re-create Dispose clean-up code here.
        // Calling Dispose(false) is optimal in terms of
        // readability and maintainability.
        Dispose(false);
    }

    #endregion

}
