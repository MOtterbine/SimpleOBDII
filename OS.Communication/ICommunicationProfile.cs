
namespace OS.Communication;

public interface ICommunicatioProfile
{

    IList<string> DeviceList { get; }
    string SelectedBluetoothDevice { get; set; }
    string PresetBluetoothDevice { get; }

    UInt32 SerialBaudRate { get; set; }

    void SetCommMethod(int channel = 0);
    bool StoreBluetoothDevicePreset(string device);
    /// <summary>
    /// 0 for Bluetooth, 1 for Network/WiFi
    /// </summary>
    int SelectedCommMethod { get; set; }
    string IPAddress { get; set; }
    int IPPort { get; set; }
    bool IsBluetooth { get; }
    string UserCANID { get; set; }
    bool OpenCommunicationChannel();
    String KWPInitAddress { get; set; }
    bool UseKWPWakeup { get; set; }
    UInt32 ISOBaudRate { get; set; }

}
