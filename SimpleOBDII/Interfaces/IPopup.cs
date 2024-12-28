using OS.OBDII.Views;

namespace OS.OBDII.Interfaces;

public interface ICustomPopup
{
    Task<bool> ShowPopupAsync(PopupInfo popupInfo);
}
