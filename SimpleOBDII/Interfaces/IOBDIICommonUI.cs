
using OS.OBDII.Communication;
using OS.OBDII.Models;

namespace OS.OBDII.Interfaces;

public interface IOBDIICommonUI 
{

    int PlotHeight { get; set; }
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
    ICommunicationDevice CommunicationService { get; }

    Protocol SelectedProtocol { get; set; }
    int SelectedProtocolIndex { get; set; }

    IAdService AdService { get; }

    bool UseMetric { get; set; }
    bool IsSpecialEdition { get; }
    ILogService LogService { get; }
    bool UseHeader { get; set; }
    void SendHapticFeedback();

    List<IVehicleModel> Manufacturers { get; }
    IVehicleModel SelectedManufacturer { get; set; }

    string VersionString { get; }
}
