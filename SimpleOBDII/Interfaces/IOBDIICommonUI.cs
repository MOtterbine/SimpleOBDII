
using OS.Communication;
using OS.OBDII.Models;
using OS.OBDII.Views;

namespace OS.OBDII.Interfaces;

public interface IOBDIICommonUI 
{
    // Yes, this interface is way too large - it's a 'catch all' .
    // It's main function is to provide a reference to a centralized class instance which holds persistent and runtime data
    // ...for xaml view models, passed in the view model's constructor typically instantiated in the code-behind constructor.
 
    int PlotHeight { get; set; }
    IList<string> DeviceList { get; }
    string SelectedBluetoothDevice { get; set; }
    string PresetBluetoothDevice { get; }

    UInt32 SerialBaudRate { get; set; }
    IPid tempIPid { get; set; }

    void SetCommMethod();
    bool StoreBluetoothDevicePreset(string device);
    string SelectedCommMethod { get; set; }
    string IPAddress { get; set; }
    int IPPort { get; set; }
    bool IsBluetooth { get; }
    string UserCANID { get; set; }
    ICommunicationDevice CommunicationService { get; }

    Task<bool> ShowPopupAsync(PopupInfo popupInfo);
    bool OpenCommunicationChannel();

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
