using OS.Communication;
using OS.Localization;

namespace OS.PlatformShared;

/// <summary>
/// Facade class that combines bluetooth and usb devices into a single interface
/// </summary>
public partial class MAUI_SerialDevice : IDevicesService, ICommunicationDevice, ISerialDevice
{
    private CancellationTokenSource tokenSource = null;

    public uint BaudRate
    {
        get 
        {
            if (!(CurrentDevice is ISerialDevice)) return 9600;
            return (CurrentDevice as ISerialDevice).BaudRate;
        }
        set
        {
            if (CurrentDevice is ISerialDevice)
            {
                (CurrentDevice as ISerialDevice).BaudRate = value;
            }
        }
    }

    public bool DTR
    {
        get
        {
            if (!(CurrentDevice is ISerialDevice)) return false;
            return (CurrentDevice as ISerialDevice).DTR;
        }
        set
        {
            if (CurrentDevice is ISerialDevice)
            {
                (CurrentDevice as ISerialDevice).DTR = value;
            }
        }
    }

    public bool RTS
    {
        get 
        {
            if (!(CurrentDevice is ISerialDevice)) return false;
            return (CurrentDevice as ISerialDevice).RTS;
        }
        set
        {
            if (CurrentDevice is ISerialDevice)
            {
                (CurrentDevice as ISerialDevice).RTS = value;
            }
        }
    }

    // Supported devices
    private List<ICommunicationDevice> deviceTypesList = new List<ICommunicationDevice>()
    {
        // Generic bluetooth devices
        new AndroidBluetoothDevice(), // Wireless Bluetooth
        new CH34X(),  // Wired CH340/342
        new CP21X(),   // Wired CP21XX
        new USBSerial_FTDI()   // Wired FTDI
    };
    public ICommunicationDevice CurrentDevice { get; private set; }

    public MAUI_SerialDevice()
    {

    }

    public IList<string> GetDeviceList()
    {
        IList<string> retList = new List<string>();
        // Iterate each type of device
        foreach (IDevicesService cdev in this.deviceTypesList)
        {
            // iterate instances of this type - each device
            foreach(string s in cdev.GetDeviceList())
            {
                retList.Add(s);
            }
        }
        return retList;
    }

    public string DeviceName 
    { 

        get => this.CurrentDevice==null?null:this.CurrentDevice.DeviceName;
        set
        {
            this.SetDevice(value);
        }
    }

    private void SetDevice(string channelName)
    {

        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        foreach (IDevicesService dev in this.deviceTypesList)
        {
            if (dev.GetDeviceList().Contains(channelName))
            {
                this.CurrentDevice = dev;
                this.CurrentDevice.DeviceName = channelName;
                break;
            }
        }

    }

    public bool IsEnabled => this.CurrentDevice != null;

    public bool IsConnected => this.CurrentDevice == null ? false : this.CurrentDevice.IsConnected;

    public string Description => this.CurrentDevice == null ? "n/a" : this.CurrentDevice.DeviceName;

    public bool Open(string commChannel)
    {
        if (this.CurrentDevice == null)
        {
            if (String.IsNullOrEmpty(this.DeviceName)) FireErrorEvent((string)LocalizationResourceManager.Instance["MSG_DEVICE_NOT_SET"]);
            else FireErrorEvent((string)LocalizationResourceManager.Instance["MSG_DEVICE_NOT_CONNECTED"]);
            return false;
        }
        return this.CurrentDevice.Open(commChannel);
    }

    public bool Close()
    {
        return this.CurrentDevice == null ? false : this.CurrentDevice.Close();
    }

    private object eventLock = new object();
    private event DeviceEvent _communicationEvent;
    public event DeviceEvent CommunicationEvent
    {
        add
        {
            lock (eventLock)
            {
                this._communicationEvent += value;
                if (this.CurrentDevice == null) return;
                this.CurrentDevice.CommunicationEvent += value;
            }
        }
        remove
        {
            lock (eventLock)
            {
                this._communicationEvent -= value;
                if (this.CurrentDevice == null) return;
                this.CurrentDevice.CommunicationEvent -= value;
            }
        }
    }

    private void FireErrorEvent(string message)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.Error;
            evt.Description = message;
            if(this._communicationEvent != null) this._communicationEvent(this, evt);
        }
    }

    public override string ToString() => this.CurrentDevice.DeviceName;

    public async Task<bool> Send(string text) => await this.CurrentDevice?.Send(text);

    public async Task<bool> Send(byte[] buffer, int offset, int count) => await this.CurrentDevice?.Send(buffer, offset, count);
    
    
}
