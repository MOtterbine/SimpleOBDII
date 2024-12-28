using Android.Content.Res;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using OS.OBDII.Interfaces;
using Microsoft.Maui.Platform;
using Android.OS;
using CommunityToolkit.Maui.Views;
using OS.OBDII.Views;
using OS.OBDII.ViewModels;
using OS.OBDII.Interfaces;
using OS.OBDII;
using static Android.Provider.Settings;

namespace OS.OBDII.PartialClasses;

public partial class ActivityControlService  : IPlatformAppControl, IDeviceId
{

    public ActivityControlService()
    {
        //(Platform.CurrentActivity as MainActivity).Paused += OnPaused;
        //(Platform.CurrentActivity as MainActivity).Resumed += OnResumed;
    }


    public String UID { get; } = Secure.GetString(Android.App.Application.Context.ContentResolver, Secure.AndroidId).ToUpper();

    private object configChangedLock = new object();
    public event OnConfigurationChanged ConfigurationChanged
    {
        add
        {
            lock (configChangedLock)
            {
                var j = (Platform.CurrentActivity as IActivity);

                j.ConfigurationChanged += value;
            }
        }
        remove
        {
            lock (configChangedLock)
            {
                (Platform.CurrentActivity as IActivity).ConfigurationChanged -= value;
            }
        }
    }


    private object pausedLock = new object();
    public event EventHandler Paused
    {
        add
        {
            lock (pausedLock)
            {
                var j = (Platform.CurrentActivity as IActivity);

                j.Paused += value;
            }
        }
        remove
        {
            lock (pausedLock)
            {
                (Platform.CurrentActivity as IActivity).Paused -= value;
            }
        }
    }

    private object resumedLock = new object();

    public event EventHandler Resumed
    {
        add
        {
            lock (resumedLock)
            {
                (Platform.CurrentActivity as IActivity).Resumed += value;
            }
        }
        remove
        {
            lock (resumedLock)
            {
                (Platform.CurrentActivity as IActivity).Resumed -= value;
            }
        }
    }

    //private void OnPaused(object sender, EventArgs e)
    //{
    //    if (this.Paused == null) return;
    //    this.Paused(this, EventArgs.Empty);
    //}

    //private void OnResumed(object sender, EventArgs e)
    //{
    //    if (this.Resumed == null) return;
    //    this.Resumed(this, EventArgs.Empty);
    //}

    void IPlatformAppControl.Close()
    {
        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.FinishAndRemoveTask();
        MainThread.BeginInvokeOnMainThread(() => {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        });

        // doesn't free all memory - use android studio profiler to verify
        //  Xamarin.Essentials.Platform.CurrentActivity.FinishAndRemoveTask();

    }

    int IPlatformAppControl.ScreenWidth => Convert.ToInt32(DeviceDisplay.Current.MainDisplayInfo.Width);
    int IPlatformAppControl.ScreenHeight => Convert.ToInt32(DeviceDisplay.Current.MainDisplayInfo.Height);
    //double IPlatformAppControl.ScreenDensity => Resources.System.DisplayMetrics.Density;
   // int IPlatformAppControl.ScreenDPI => (int)(DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density);
    double IPlatformAppControl.ScreenDensity => (DeviceDisplay.Current.MainDisplayInfo.Density);


    void IPlatformAppControl.RequestPermissions(string[] permissions, int requestID)
    {
        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
        {
            Platform.CurrentActivity.RequestPermissions(permissions, requestID);
        }
    }

    bool IPlatformAppControl.CheckSelfPermission(string permission)
    {

        var j = Platform.CurrentActivity.CheckSelfPermission(permission);
        return j == 0;
    }

    void IPlatformAppControl.ConfigureUI()
    {
        // allows for scrolling when android keyboard impinges on the screen
        // Microsoft.Maui.Controls.Application.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    void IPlatformAppControl.InvokeHapticFeedback()
    {
        try
        {
            // Perform click feedback
            //  HapticFeedback.Perform(HapticFeedbackType.Click);

            // Or use long press    
            //HapticFeedback.Perform(HapticFeedbackType.LongPress);

            this.Vibrate(40);

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
            Vibration.Default.Vibrate(milliseconds);
        }
        catch (FeatureNotSupportedException ex)
        {
            // Feature not supported on device
        }
        catch (Exception ex)
        {
            // Other error has occurred.
        }
    }

    public async Task<bool> ShowLicensePopupAsync()
    {
        var popup = new LicensePopup(AppShellModel.Instance);
        var result = await App.Current.MainPage.ShowPopupAsync(popup);// this.ShowPopupAsync(popup);
        if (result is bool boolResult) return boolResult;
        return false;
    }


    public async Task<bool> ShowPopupAsync(PopupInfo popupInfo)
    {
        var popup = new OSPopup(popupInfo);
        var result = await App.Current.MainPage.ShowPopupAsync(popup);// this.ShowPopupAsync(popup);
        if (result is bool boolResult) return boolResult;
        return false;
    }



// used for yes/no
private async Task<bool> ShowOSPopup(PopupInfo popupInfo)
    {
    //    var popup = new OSPopup(popupInfo);
        var result = await this.ShowPopupAsync(popupInfo);
        if (result is bool boolResult) return boolResult;
        return false;
    }

}