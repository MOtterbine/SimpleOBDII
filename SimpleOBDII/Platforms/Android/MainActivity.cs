using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Media.Metrics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Activity;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using Java.Lang;
using Kotlin.Reflect;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Platform;
using OS.OBDII.Controls;
using OS.OBDII.Interfaces;
using OS.OBDII.PartialClasses;


namespace OS.OBDII;

[Activity(Theme = "@style/OSTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, IActivity
{
    private class BackPress : OnBackPressedCallback
    {
        private readonly Activity activity;
        private long backPressed;

        public BackPress(Activity activity) : base(true)
        {
            this.activity = activity;
        }

        public override void HandleOnBackPressed()
        {
            var p = Microsoft.Maui.Controls.Application.Current?.Windows.Last().Page;// Navigation.NavigationStack.Last();
            if (p == null)
            {
                p = Microsoft.Maui.Controls.Application.Current?.MainPage;
            }

            ((AppShell)p).InvokeBackPressed();
        }
    };

#if ANDROID21_0_OR_GREATER

    private IOnApplyWindowInsetsListener _insetsListener = null;

    public AndroidX.Core.View.WindowInsetsCompat? EatShit(Android.Views.View view, WindowInsets insets0)
    {
        if (_insetsListener == null)
        {
            return null;
        }
        var insets = AndroidX.Core.View.WindowInsetsCompat.ToWindowInsetsCompat(Window.DecorView.RootWindowInsets);
        var newInsets = _insetsListener.OnApplyWindowInsets(Window.DecorView, insets);
        return newInsets;
    }
    public WindowInsets OnapplyWindowInsets(Android.Views.View view, WindowInsets insets)
    {
        var statusBarInsets = insets.GetInsets(WindowInsets.Type.StatusBars());
        view.SetBackgroundColor(Android.Graphics.Color.Black);
        // Adjust padding to avoid overlap
        view.SetPadding(0, statusBarInsets.Top, 0, 0);
        return insets;
    }

    #endif

    public event OnConfigurationChanged ConfigurationChanged =  null;

    public void FirePause()
    {
        if (this.Paused == null) return;
        this.Paused(this, EventArgs.Empty);
    }
    public event EventHandler Paused;

    public void FireResume()
    {
        if (this.Resumed == null) return;
        this.Resumed(this, EventArgs.Empty);
    }
    public event EventHandler Resumed;





    // public 
    public MainActivity()
    {
        // helps with text keyboard which won't operate correctly in MAUI
        pApp.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    public override void OnConfigurationChanged(Configuration newConfig)
    {
        base.OnConfigurationChanged(newConfig);
        pApp.NotifyConfigurationChanged();
        if (this.ConfigurationChanged == null) return;
        this.ConfigurationChanged(newConfig);
    }

    protected override void OnResume()
    {
        base.OnResume();
        this.FireResume();
    }

    protected override void OnPause()
    {
        this.FirePause();
        base.OnPause();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        OnBackPressedDispatcher.AddCallback(this, new BackPress(this));


#if ANDROID21_0_OR_GREATER
        Window.AddFlags(WindowManagerFlags.TranslucentStatus);
        Window.SetFlags(WindowManagerFlags.TranslucentStatus, WindowManagerFlags.TranslucentStatus);
        Window.SetFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds, Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
#if ANDROID35_0_OR_GREATER
        // Android 15+
        Window.DecorView.ApplyWindowInsets =
        (Android.Views.View view, WindowInsets insets) =>
        {
            var statusBarInsets = insets.GetInsets(WindowInsets.Type.StatusBars());
            // background colors for nav and status are set in the platform's styles.xml file
            //view.SetBackgroundColor(Android.Graphics.Color.Red);
            // Adjust padding to avoid overlap
            view.SetPadding(0, statusBarInsets.Top, 0, 0);
            return insets;
        };
#else
        Window.SetNavigationBarColor(Android.Graphics.Color.Black);
        Window.SetStatusBarColor(Android.Graphics.Color.Black);
#endif
#endif

        base.OnCreate(savedInstanceState);

        SetupCustomControlMapping();

        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
        {
            // Start permissions check...
            RequestPermissions(new string[] { "android.permission.BLUETOOTH_CONNECT" }, 15001); // the value 15001 is arbitrary and random

            //StartBluetoothPermissionCheck();
        }
        else
        {
            pApp.HasPermissions = true;
            pApp.FirePermissionsReadyEvent();
        }


        //if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
        //{
        //    Window.SetNavigationBarColor(Android.Graphics.Color.Black);
        //    Window.SetStatusBarColor(Android.Graphics.Color.Black);


        //    Window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
        //    Window.SetFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds, Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);

        //    Window.SetNavigationBarColor(Android.Graphics.Color.Argb(0xFF, 0x00, 0x00, 0x00));
        //    Window.SetStatusBarColor(Android.Graphics.Color.Argb(0xFF, 0x00, 0x00, 0x00));


        //    //DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;
        //}
        //base.OnCreate(savedInstanceState);

        //SetupCustomControlMapping();


        //    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
        //    {

        //        // Start permissions check...
        //        RequestPermissions(new string[] { "android.permission.BLUETOOTH_CONNECT" }, 15001); // the value 15001 is arbitrary and random

        //        //StartBluetoothPermissionCheck();
        //    }
        //    else
        //    {
        //        pApp.HasPermissions = true;
        //        pApp.FirePermissionsReadyEvent();
        //    }

    }


    bool cancelHaptic = false;

    private void SetupCustomControlMapping()
    {
        // IT'S PREFERRED TO TO CREATE AND USE A HANDLER CLASS TO DO THIS.
        // THIS IS HERE JUST FOR DEMO

        Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("ClustomButtonEntry", (handler, view) =>
        {
            if (false)//view is ICustomButton)
            {
                handler.PlatformView.Touch += (s, e) =>
                {
                    switch(e.Event.Action)
                    {
                        case MotionEventActions.Down:
                            cancelHaptic = false;
                            //if ((view as ICustomButtonController).SendTouched())
                            //{
                            //    Vibration.Vibrate(15);
                            //}
                            break;
                        case MotionEventActions.Up:
                            if (cancelHaptic)
                            {
                                break;
                            }
                           // Vibration.Vibrate(15);
                            (view as ICustomButtonController).SendClicked();
                            break;
                        case MotionEventActions.Cancel:
                            cancelHaptic = true;
                            break;
                    }
                };
            }
        });
    }


    App pApp = ((App)App.Current);

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    {

      //  Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        switch (requestCode)
        {
            case 15001:
                // Check for Android sdk 31, or higher bluetooth permissions
                if (grantResults.Length > 0)
                {
                    if (grantResults[0] == 0) // good permission - this is a sdk 31, or higher, device
                    {
                        pApp.HasPermissions = true;
                        pApp.FirePermissionsReadyEvent();
                        return;
                    }
                }
                pApp.HasPermissions = false; // No device permissions at all...
                pApp.FirePermissionsReadyEvent();
                break;
        }

    }

    private async Task StartBluetoothPermissionCheck()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<OSBluetoothPermissions>();

        switch (status)
        {
            case PermissionStatus.Granted:
                pApp.HasPermissions = true;
                pApp.FirePermissionsReadyEvent();
                break;
            default :
                pApp.HasPermissions = false; // No device permissions at all...
                pApp.FirePermissionsReadyEvent();
                break;
        }
    }
}

public class OSBluetoothPermissions : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            //(global::Android.Manifest.Permission.Bluetooth, true), // pre android 12
            //(global::Android.Manifest.Permission.BluetoothAdmin, true), // pre android 12
            (global::Android.Manifest.Permission.BluetoothConnect, true) // only needed to be checked
        }.ToArray();
}


