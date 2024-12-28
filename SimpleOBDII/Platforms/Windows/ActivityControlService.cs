using CommunityToolkit.Maui.Views;
using OS.OBDII.Interfaces;
using Microsoft.UI.Xaml.Controls;
using OS.OBDII.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Net.NetworkInformation;
using OS.OBDII.ViewModels;

namespace OS.OBDII.PartialClasses;

public partial class ActivityControlService : IPlatformAppControl
{

    public String UID { get; private set; }

    public ActivityControlService()
    {
        UID = this.GetDefaultMacAddress().ToUpper();
    }

    public string GetDefaultMacAddress()
    {
        Dictionary<string, long> macAddresses = new Dictionary<string, long>();
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus == OperationalStatus.Up)
                macAddresses[nic.GetPhysicalAddress().ToString()] = nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived;
        }
        long maxValue = 0;
        string mac = "";
        foreach (KeyValuePair<string, long> pair in macAddresses)
        {
            if (pair.Value > maxValue)
            {
                mac = pair.Key;
                maxValue = pair.Value;
            }
        }
        return mac;
    }

    public event EventHandler Paused;

    public event EventHandler Resumed;


    public void Close()
    {
#if WINDOWS
        MainThread.BeginInvokeOnMainThread(() => {
            Application.Current.Quit();
        });
#endif
    }

    public int ScreenWidth => Convert.ToInt32(DeviceDisplay.MainDisplayInfo.Width);
    public int ScreenHeight => Convert.ToInt32(DeviceDisplay.MainDisplayInfo.Height);
    public double ScreenDensity => DeviceDisplay.MainDisplayInfo.Density;

    public void RequestPermissions(string[] permissions, int requestID = 0)
    {
    }
        
    public bool CheckSelfPermission(string permission)
    {
        return true;
    }

    public void ConfigureUI()
    {

    }

    public async Task<bool> ShowLicensePopupAsync()
    {
        var popup = new LicensePopup(AppShellModel.Instance as ILicenseManager);
        var result = await Application.Current.MainPage.ShowPopupAsync(popup);
        if (result is bool boolResult) return boolResult;
        return false;
    }


    public async Task<bool> ShowPopupAsync(PopupInfo popupInfo)
    {
        var popup = new OSPopup(popupInfo);
        var result = await Application.Current.MainPage.ShowPopupAsync(popup);
        if (result is bool boolResult) return boolResult;
        return false;
    }

    void IPlatformAppControl.InvokeHapticFeedback()
    {
        try
        {
            // Perform click feedback
            //  HapticFeedback.Perform(HapticFeedbackType.Click);

            // Or use long press    
            HapticFeedback.Perform(HapticFeedbackType.LongPress);

            //this.Vibrate(40);

        }
        catch (FeatureNotSupportedException)
        {
            // Feature not supported on device
        }
        catch (Exception)
        {
            // Other error has occurred.
        }
    }


    private void Vibrate(int milliseconds)
    {
        try
        {

        }
        catch (FeatureNotSupportedException)
        {
            // Feature not supported on device
        }
        catch (Exception)
        {
            // Other error has occurred.
        }
    }

}
