using Android.Hardware.Usb;
using Android.App;
using Android.Content;
using System.Text;
using String = System.String;
using Exception = System.Exception;
using OS.Communication;

namespace OS.PlatformShared;

/// <summary>
/// AndroidUSB_Base USB to Serial Converter IC devices
/// </summary>
public class AndroidUSB_Base 
{
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


    Task listenTask = null;
    private ManualResetEvent rcvLock = new ManualResetEvent(true);
    private ManualResetEvent openEvent = new ManualResetEvent(true);
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
            FireErrorEvent($"Device Open Failure - {ex.Message}");
        }
        return false;
    }

    private async Task beginListen(string commChannel)
    {

        if (String.IsNullOrEmpty(commChannel))
        {
            FireErrorEvent(OS.Communication.Constants.COMMUNICATION_DEVICE_NOT_SETUP);
            return;
        }

        // Attempt to set the device to a user-set name (which might not be available)
        if (this.IsConnected = TryDeviceConnect(commChannel))
        {
            FireInfoEvent("Device connection established");
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
                FireErrorEvent("No USB Capability Available");
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
                FireErrorEvent("Please set device in Setup page");
                return false;
            }

            if (!this.bManager.DeviceList.ContainsKey(deviceName))
            {
                FireErrorEvent($"Device '{deviceName}' is not attached.");
                return false;
            }
            this.usbDevice = this.bManager.DeviceList[deviceName];

            if (this.usbDevice == null)
            {
                FireErrorEvent($"Cannot find Bluetooth device '{deviceName}'");
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
                FireErrorEvent("Permissions Required");
                return false;

            }

            sb.AppendFormat("get device connection{0}",  System.Environment.NewLine);

            this.deviceConnection = bManager.OpenDevice(this.usbDevice);

            if (this.deviceConnection == null)
            {
                FireErrorEvent("Invalid USB Device");
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
            this.FireErrorEvent($"ERROR: {eio.Message}{System.Environment.NewLine}{sb.ToString()}");
        }
        catch (Exception e)
        {
            //this.FireErrorEvent($"ERROR: Unable to access Bluetooth device");
            this.FireErrorEvent($"ERROR: {e.Message}{System.Environment.NewLine}{sb.ToString()}");
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

    public async Task Listen()
    {
        if (this.usbDevice == null)
        {
            this.FireErrorEvent($"ERROR: Unable to access USB device");
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

        if (this.tokenSource != null)
        {
            this.tokenSource.Cancel();
            while (!this.tokenSource.IsCancellationRequested) ;
        }
        this.rcvLock.Set();
        this.openEvent.Set();

        this.deviceConnection?.Close();
        //this.deviceConnection = null;

        this.IsConnected = false;
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
            evt.Description = "Bluetooth Response";
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
            //    this.FireErrorEvent($"Bluetooth device is not open");
            //    return false;
            //}

            await deviceConnection.BulkTransferAsync(this._endpoint_tx, buffer, count, 250);

            return true;
        }
        catch (Exception ett)
        {
            FireErrorEvent($"Bluetooth error - {ett.Message}");
        }
        return false;
    }

}