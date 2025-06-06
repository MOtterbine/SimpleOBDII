using Android.Hardware.Usb;
using Android.App;
using Android.Content;
using System.Text;
using String = System.String;
using Exception = System.Exception;
using OS.Communication;
using OS.Localization;

namespace OS.PlatformShared;

    public enum FlowControl
{
    NONE, RTS_CTS, DTR_DSR,  XON_XOFF, XON_XOFF_INLINE
}

/// <summary>
/// AndroidUSB_Base USB to Serial Converter IC devices
/// </summary>
public class AndroidUSB_Base 
{
    /** XON character used with flow control XON/XOFF */
    protected char CHAR_XON = (char)17;
    /** XOFF character used with flow control XON/XOFF */
    protected char CHAR_XOFF = (char)19;
    protected FlowControl mFlowControl = FlowControl.NONE;

    protected virtual uint VENDOR_ID => 0x0000; 
    public virtual int BUFFER_SIZE => 1024;

    public uint BaudRate { get; set; } = 38400;

    public event DeviceEvent DeviceEvent;

    protected UsbManager bManager = null;
    protected UsbEndpoint _endpoint_rx = null;
    protected UsbEndpoint _endpoint_tx = null;
    protected UsbDevice usbDevice = null;
    protected UsbDeviceConnection deviceConnection = null;
    protected byte[] RespBuffer =null;
    protected byte[] TmpBuffer = null;
    protected CancellationTokenSource tokenSource = null;// new CancellationTokenSource();
    protected Action tokenCancelOperation = null;
    protected System.Text.StringBuilder sb = new System.Text.StringBuilder();

    public AndroidUSB_Base()
    {
        RespBuffer = new byte[BUFFER_SIZE];
        TmpBuffer = new byte[BUFFER_SIZE];
        bManager = Platform.CurrentActivity.GetSystemService(Android.Content.Context.UsbService) as UsbManager;
    }

    public virtual IList<string> GetDeviceList()
    {
        return bManager == null ? null : bManager.DeviceList.Where(dl=>dl.Value.VendorId == VENDOR_ID).ToList().ConvertAll(p=>p.Key);
    }

    public string DeviceName { get; set; }

    public bool IsEnabled => this.deviceConnection != null;
    public virtual string Description => $"USB device: {this.DeviceName}";

    public bool IsConnected { get; set; }
    //public bool IsConnected => this.usbDevice == null ? false : this.usbDevice.InterfaceCount > 1;// cur.deviceConnection.ibluetoothSocket.IsConnected;


    Task listenTask = null;
    protected ManualResetEvent rcvLock = new ManualResetEvent(true);
    protected ManualResetEvent openEvent = new ManualResetEvent(true);
    public event DeviceEvent CommunicationEvent;


    public virtual bool Open(string commChannel)
    {
        try
        {
            openEvent.Reset();
            if (this.listenTask == null || listenTask.IsCompleted)
            {
                listenTask = new Task(async ()=> { await beginListen(commChannel); });
                listenTask.Start();
            }
            openEvent.WaitOne(); // wait until the port is actually opened and ready to communicate
            return this.IsConnected;
        }
        catch (Exception ex)
        {
            this.tokenSource?.Cancel();
            this.rcvLock.Set();
            FireErrorEvent($"{(string)LocalizationResourceManager.Instance["MSG_DEVICE_OPEN_FAILURE"]} - {ex.Message}");
        }
        return false;
    }

    private async Task beginListen(string commChannel)
    {

        if (String.IsNullOrEmpty(commChannel))
        {
            FireErrorEvent((string)LocalizationResourceManager.Instance["MSG_DEVICE_NOT_SET"]);
            return;
        }

        // Attempt to set the device to a user-set name (which might not be available)
        if (this.IsConnected = TryDeviceConnect(commChannel))
        {
            FireInfoEvent((string)LocalizationResourceManager.Instance["MSG_CONNECTED"]);
            while (this.IsConnected)
            {
                await this.Listen();
            }
        }
        this.openEvent.Set(); // ensure the device open even is resolved

        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.Disconnected;
            FireDeviceEvent(evt);
        }
    }

    private bool TryDeviceConnect(string deviceName)
    {
        try
        {
            sb.Clear();
            if (this.bManager == null)
            {
                
                FireErrorEvent((string)LocalizationResourceManager.Instance["NO_USB_AVAILABLE"]);
                return false;
            }

            if (this.IsConnected)
            {
               // this.Close();
                //AppShellViewModel.Instance.CommunicationService.Close();
                //return true;
            }

            if (String.IsNullOrEmpty(deviceName))
            {
                FireErrorEvent((string)LocalizationResourceManager.Instance["MSG_DEVICE_NOT_SET"]);
                return false;
            }

            if (!this.bManager.DeviceList.ContainsKey(deviceName))
            {
                FireErrorEvent($"{(string)LocalizationResourceManager.Instance["MSG_DEVICE_NOT_CONNECTED"]}: '{deviceName}'");
                return false;
            }
            this.usbDevice = this.bManager.DeviceList[deviceName];

            if (this.usbDevice == null)
            {
                FireErrorEvent($"{(string)LocalizationResourceManager.Instance["MSG_INVALID_USB"]}: '{deviceName}'");
                return false;
            }

            var datInterface = this.usbDevice.GetInterface(this.usbDevice.InterfaceCount - 1);

            for(int i = 0; i< datInterface.EndpointCount;i++)// UsbEndpoint ep in datInterface.)
            {
                if(datInterface.GetEndpoint(i).Direction == UsbAddressing.Out && this._endpoint_tx == null)
                {
                    this._endpoint_tx = datInterface.GetEndpoint(i);
                }
                if (datInterface.GetEndpoint(i).Direction == UsbAddressing.In && this._endpoint_rx == null)
                {
                    this._endpoint_rx = datInterface.GetEndpoint(i);
                }
            }

            //         sb.AppendFormat("usb device found: {0}{1}", deviceName, Environment.NewLine);

            string ACTION_USB_PERMISSION = "android.permission.MANAGE_USB";

            PendingIntent mPermissionIntent = PendingIntent.GetBroadcast(Platform.CurrentActivity, 0, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.Immutable);
            IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);

            //    sb.AppendFormat("permission intent defined{0}",  Environment.NewLine);

            if (bManager.HasPermission(usbDevice) == false)
            {
                bManager.RequestPermission(usbDevice, mPermissionIntent);
                Close();
                FireErrorEvent((string)LocalizationResourceManager.Instance["MSG_PERMISSIONS_REQUIRED"]);
                return false;

            }

            sb.AppendFormat("get device connection{0}",  System.Environment.NewLine);

            this.deviceConnection = bManager.OpenDevice(this.usbDevice);

            if (this.deviceConnection == null)
            {
                FireErrorEvent((string)LocalizationResourceManager.Instance["MSG_INVALID_USB"]);
                return false;
            }

            UsbInterface tmpIface;
            for (int i = 0; i < usbDevice.InterfaceCount; i++)
            {
                tmpIface = usbDevice.GetInterface(i);
                if (!deviceConnection.ClaimInterface(tmpIface, true))
                {
                    throw new Java.IO.IOException($"Could not claim data interface, idx: {i}");
                }
            }

            sb.AppendFormat("Device connection Established{0}", System.Environment.NewLine);

            FireInfoEvent(sb.ToString());
            sb.Clear();

            InitUSBSerial(); // throws an exception upon failure

            this.FireEvent(CommunicationEvents.ConnectedAsClient);
            this.DeviceName = deviceName;
            return true;

        }
        catch (Java.IO.IOException eio)
        {
            //this.FireErrorEvent($"ERROR: Unable to access Bluetooth device");
            this.FireErrorEvent($"{(string)LocalizationResourceManager.Instance["ERROR"]}: {eio.Message}{System.Environment.NewLine}{sb.ToString()}");
        }
        catch (Exception e)
        {
            //this.FireErrorEvent($"ERROR: Unable to access Bluetooth device");
            this.FireErrorEvent($"{(string)LocalizationResourceManager.Instance["ERROR"]}: {e.Message}{System.Environment.NewLine}{sb.ToString()}");
        }
        finally
        {
            //  this._settingDevice = false;
        }
        return false; ;
    }

    /// <summary>
    /// Intended for specific implementations called from the base
    /// </summary>
    protected virtual void InitUSBSerial() {   }

    public virtual async Task Listen()
    {
        if (this.usbDevice == null)
        {
            this.FireErrorEvent($"{(string)LocalizationResourceManager.Instance["ERROR"]}: {(string)LocalizationResourceManager.Instance["MSG_DEVICE_ACCESS_FAILURE"]}");
            return;
        }

        if (!this.IsConnected) return;
        int bytesRead = 0;
        try
        {
            this.tokenSource = new CancellationTokenSource();

            if (this._endpoint_rx != null)
            {
                this.openEvent.Set(); // the device open even is resolved

                while (!this.tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        rcvLock.WaitOne();
                        for (int i = 0; i < 40; i++)
                        {
                            TmpBuffer[i] = 0x00;
                        }
                        rcvLock.Reset();
                        bytesRead = await deviceConnection.BulkTransferAsync(_endpoint_rx, TmpBuffer, BUFFER_SIZE, 370);
                        if (bytesRead > 0)
                        {
                            FireReceiveEvent(TmpBuffer.Take(bytesRead).ToArray());
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {
                        rcvLock.Set();
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }
        finally
        {
            this.tokenSource.Cancel();
            this.rcvLock.Set();
        }

    }

    public bool Close()
    {
        // must be set before call to tokenSource.Cancel()
        this.IsConnected = false;
        if (this.tokenSource != null)
        {
            this.tokenSource.Cancel();
            while (!this.tokenSource.IsCancellationRequested) ;
        }
        this.rcvLock.Set();
        this.openEvent.Set();

        this.deviceConnection?.Close();

        this.FireEvent(CommunicationEvents.Disconnected, "Connection Closed");

        return true;
    }

    protected void FireDeviceEvent(DeviceEventArgs e)
    {
        if (this.CommunicationEvent != null)
        {
            CommunicationEvent(this, e);
        }
    }


    protected void FireInfoEvent(string message)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.Information;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
    }

    protected void FireErrorEvent(string message)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.Error;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
    }

    protected void FireEvent(CommunicationEvents eventType, string message = null)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = eventType;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
    }

    protected void FireReceiveEvent(byte[] bytes)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.data = bytes;
            evt.Event = CommunicationEvents.ReceiveEnd;
            evt.Description = "USB Response";
            this.FireDeviceEvent(evt);
        }
    }

    public virtual bool Initialize()
    {
        return true;
    }

    public override string ToString()
    {
        return this.DeviceName;
    }

    public async Task<bool> Send(string text)
    {
        await this.Send(Encoding.UTF8.GetBytes(text), 0, text.Length);
        return false;
    }

    public async Task<bool> Send(byte[] buffer, int offset, int count)
    {
        try
        {
            //if (!this.IsConnected)
            //{
            //    this.FireErrorEvent($"USB device is not open");
            //    return false;
            //}

            await deviceConnection.BulkTransferAsync(this._endpoint_tx, buffer, count, 250);

            return true;
        }
        catch (Exception ett)
        {
            FireErrorEvent($"{(string)LocalizationResourceManager.Instance["USB"]} {(string)LocalizationResourceManager.Instance["ERROR"]} - {ett.Message}");
        }
        return false;
    }

}