using System.Text;
using Java.Util;
using Android.Bluetooth;
using OS.Communication;

namespace OS.PlatformShared;

public partial class AndroidBluetoothDevice : IDevicesService, ICommunicationDevice
{

    // ELM327 uart rx buffer is 512 bytes
    public const int BUFFER_SIZE = 2048;
    public event ConnectedDeviceEvent DeviceEvent;

    private BluetoothDevice bluetoothDevice = null;
    private BluetoothSocket bluetoothSocket = null;
    private BluetoothAdapter bluetoothAdapter = null;
    private byte[] RespBuffer = new byte[BUFFER_SIZE];
    private byte[] TmpBuffer = new byte[BUFFER_SIZE];
    private CancellationTokenSource tokenSource = null;// new CancellationTokenSource();

    public AndroidBluetoothDevice()
    {

        BluetoothManager bManager = Platform.CurrentActivity.GetSystemService(Android.Content.Context.BluetoothService) as BluetoothManager;

        this.bluetoothAdapter = bManager.Adapter;

        LoadDeviceList();
    }


    public bool IsEnabled => this.bluetoothAdapter.IsEnabled;

    public IList<string> GetDeviceList()
    {
        var btdevice = this.bluetoothAdapter?.BondedDevices.Select(d => d.Name).ToList();
        return btdevice;
    }
    private List<BluetoothDevice> devicesList = null;
    public void LoadDeviceList()
    {
        try
        {
            devicesList = this.bluetoothAdapter?.BondedDevices.ToList();
        }
        catch (Exception e)
        {
            FireErrorEvent($"Unable to get ports list. {e}");
        }
    }


    public string DeviceName { get; set; }

    public bool IsConnected => this.bluetoothSocket == null ? false : this.bluetoothSocket.IsConnected;

    public string Description => $"Bluetooth device: {this.DeviceName}";

    Task listenTask = null;

    ManualResetEvent waitConnect = new ManualResetEvent(true);

    public bool Open(string commChannel)
    {
        try
        {
            // threads wait...
            this.waitConnect.Reset();
            if (this.listenTask == null || listenTask.IsCompleted)
            {
                listenTask = new Task(async () => { await beginListen(commChannel); });
                listenTask.Start();
                // detect if we actually connected
                if (!this.waitConnect.WaitOne(2000))
                {
                    // connect timed out, cancel everthing
                    this.tokenSource?.Cancel();
                    return false;

                }
                //while (!this.IsConnected) ;
            }
            return true;
        }
        catch (Exception ex)
        {
            this.tokenSource?.Cancel();
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
        if (TryDeviceConnect(commChannel))
        {

            if (this.bluetoothSocket != null)
            {
                // Good Connect, signal it, so threads can go...
                this.waitConnect.Set();

                while (this.IsConnected)
                {
                    await this.Listen();
                }
            }
        }
        else
        {
            FireErrorEvent($"Unable to access device: {commChannel}");
        }

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
            if (this.bluetoothAdapter == null)
            {
                FireErrorEvent("No Bluetooth adapter found");
                return false;
            }

            if (this.bluetoothAdapter.State == State.Off)
            {
                FireErrorEvent("Bluetooth is not enabled on this device.");
                return false;
            }

            if (this.IsConnected)
            {
                return true;
            }

            if (String.IsNullOrEmpty(this.DeviceName))
            {
                var dd = this.bluetoothAdapter?.BondedDevices;

                foreach (var d in this.bluetoothAdapter.BondedDevices)
                {
                    var lx = d;
                }

                FireErrorEvent("Please set device in Setup page");
                return false;
            }

            this.bluetoothDevice = (from bd in this.bluetoothAdapter?.BondedDevices
                                    where bd?.Name == this.DeviceName
                                    select bd).FirstOrDefault();

            if (this.bluetoothDevice == null)
            {
                FireErrorEvent($"Cannot find Bluetooth device '{this.DeviceName}'");
                return false;
            }

            this.bluetoothSocket = this.bluetoothDevice?.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            if (this.bluetoothSocket == null)
            {
                FireErrorEvent("Invalid Bluetooth Channel");
                return false;
            }

            this.bluetoothSocket?.Connect();

            this.FireEvent(CommunicationEvents.ConnectedAsClient);

            return true;

        }
        catch (Java.IO.IOException)
        {
            this.FireErrorEvent($"ERROR: Unable to access Bluetooth device");
        }
        catch (Exception)
        {
            this.FireErrorEvent($"ERROR: Unable to access Bluetooth device");
        }
        finally
        {
            //  this._settingDevice = false;
        }
        return false; ;
    }

    private async Task Listen()
    {
        if (this.bluetoothSocket == null)
        {
            this.FireErrorEvent($"ERROR: Unable to access serial device");
            return;
        }

        if (!this.IsConnected) return;

        int bytesRead = 0;
        try
        {
            this.tokenSource = new CancellationTokenSource();

            if(this.bluetoothSocket != null)
            {

                bytesRead = await this.bluetoothSocket.InputStream.ReadAsync(TmpBuffer, 0, BUFFER_SIZE - 1, this.tokenSource.Token);
                if (bytesRead > 0)
                {
                    FireReceiveEvent(TmpBuffer.Take(bytesRead).ToArray());
                }
            } 
            else
            {
                FireDeviceEvent(new DeviceEventArgs() { Event = CommunicationEvents.Disconnected});
            }
        }
        catch (Exception ex)
        {
                FireDeviceEvent(new DeviceEventArgs() { Event = CommunicationEvents.Disconnected});
        }
        finally
        {
            // MainThread.BeginInvokeOnMainThread(this.tokenSource.Cancel);
            this.tokenSource.Cancel();
            ///this.bluetoothSocket?.Close();
            //   bluetoothSocket?.Dispose();
            //bluetoothSocket = null;
            //bluetoothDevice = null;
            // this.FireEvent(CommunicationEvents.Disconnected, "Connection Closed");

        }
        closeEvent.WaitOne();

    }

    ManualResetEvent closeEvent = new ManualResetEvent(true);

    public bool Close()
    {
        if (this.tokenSource == null) return false;
        //this.IsOpen = false;
        closeEvent.Reset();
        // stops the listen thread...

        if (this.tokenSource != null)
        {
            this.tokenSource.Cancel();
            while (!this.tokenSource.IsCancellationRequested) ;
        }

        //this.bluetoothSocket?.Close();
        //this.IsOpen = false;
        this.bluetoothSocket?.Close();
        bluetoothSocket?.Dispose();
        bluetoothSocket = null;
        bluetoothDevice = null;
        closeEvent.Set();

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

    public event DeviceEvent CommunicationEvent;

    private void FireErrorEvent(string message)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.Event = CommunicationEvents.Error;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
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

    private void FireReceiveEvent(byte[] bytes)
    {
        using (DeviceEventArgs evt = new DeviceEventArgs())
        {
            evt.data = bytes;
            evt.Event = CommunicationEvents.ReceiveEnd;
            evt.Description = "Bluetooth Response";
            this.FireDeviceEvent(evt);
        }
    }

    public bool Initialize()
    {
        return true;
        //throw new NotImplementedException();
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

    //protected IAsyncResult _WriteAsyncResult = null;

    public async Task<bool> Send(byte[] buffer, int offset, int count)
    {
        try
        {
            if (!this.IsConnected)
            {
                this.FireErrorEvent($"Bluetooth device is not open");
                return false;
            }

            await this.bluetoothSocket.OutputStream.WriteAsync(buffer, offset, count);

            return true;
        }
        catch (Exception ett)
        {
            FireErrorEvent($"Bluetooth error - {ett.Message}");
        }
        return false;
    }

}
