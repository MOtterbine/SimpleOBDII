
using OS.Communication;
using OS.OBDII.Models;
using OS.OBDII.Views;

namespace OS.OBDII.Interfaces;
/// <summary>
/// Defines the requirements of objects passed into most view models, typically instantiated in a page's code behind constructor.
/// </summary>
public interface IOBDIICommonUI: ICommunicatioProfile 
{
 
    int PlotHeight { get; set; }
    IPid tempIPid { get; set; }


    Protocol SelectedProtocol { get; set; }
    int SelectedProtocolIndex { get; set; }
    ICommunicationDevice CommunicationService { get; }

    Task<bool> ShowPopupAsync(PopupInfo popupInfo);

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
