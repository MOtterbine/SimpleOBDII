
namespace OS.Communication;

public interface ICommunicatioProfile
{

    IList<string> DeviceList { get; }
    string SelectedBluetoothDevice { get; set; }
    string PresetBluetoothDevice { get; }

    UInt32 SerialBaudRate { get; set; }

    void SetCommMethod();
    bool StoreBluetoothDevicePreset(string device);
    string SelectedCommMethod { get; set; }
    string IPAddress { get; set; }
    int IPPort { get; set; }
    bool IsBluetooth { get; }
    string UserCANID { get; set; }
    bool OpenCommunicationChannel();
    String KWPInitAddress { get; set; }
    bool UseKWPWakeup { get; set; }
    UInt32 ISOBaudRate { get; set; }

}
