
using OS.OBDII.Views;

namespace OS.OBDII.Interfaces
{
    // For basic control of the platform
    public interface IPlatformAppControl
    {
        string UID { get; }
        void Close();
        int ScreenWidth { get; }
        int ScreenHeight { get; }
        double ScreenDensity { get; }
        void RequestPermissions(string[] permissions, int requestID = 0);
        bool CheckSelfPermission(string permission);
        void ConfigureUI();
        void InvokeHapticFeedback();

        event EventHandler Paused;
        event EventHandler Resumed;

        Task<bool> ShowPopupAsync(PopupInfo popupInfo);
        Task<bool> ShowLicensePopupAsync();
    }
}
