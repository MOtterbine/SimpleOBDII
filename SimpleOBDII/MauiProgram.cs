using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using OS.OBDII.PartialClasses;
using OS.OBDII.Controls;
using OS.OBDII.Views;
using OS.OBDII.ViewModels;
using Microsoft.Maui.Platform;
using SkiaSharp.Views.Maui.Controls.Hosting;
using OxyPlot.Maui.Skia;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Controls.PlatformConfiguration;




#if WINDOWS
    using System.Speech.Synthesis;
    using Microsoft.UI.Composition.SystemBackdrops;

    using Microsoft.UI.Xaml.Media;
    using Windows.UI.WindowManagement;
    using OS.OBDII.WinUI;
    using Microsoft.UI;
    using Microsoft.UI.Windowing;
    using Windows.Graphics;
#endif


namespace OS.OBDII;

public static class MauiProgram
{


    public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkitCore()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseOxyPlotSkia()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
			.ConfigureMauiHandlers((handlers) =>
            {
                
#if WINDOWS || ANDROID
                // handlers are in the respective platform folder
                handlers.AddHandler(typeof(OSListView), typeof(OSListViewHandler));
# endif
            })
            .ConfigureLifecycleEvents(events =>
            {

#if WINDOWS

                events.AddWindows(lifeCycleBuilder =>
                {

                    lifeCycleBuilder.OnAppInstanceActivated((sender, e) =>
                            HandleAppActions((e.Data as Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)?.Arguments))
                    .OnWindowCreated(w =>
                    {
                        
                        w.ExtendsContentIntoTitleBar = false;
                        IntPtr wHandle = WinRT.Interop.WindowNative.GetWindowHandle(w);
                        WindowId windowId = Win32Interop.GetWindowIdFromWindow(wHandle);
                        Microsoft.UI.Windowing.AppWindow mauiWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                        mauiWindow.SetPresenter(AppWindowPresenterKind.Overlapped);  // TO SET THE APP INTO FULL SCREEN

                        

                        var s = mauiWindow.Presenter as OverlappedPresenter;
                        s?.SetBorderAndTitleBar(true, true);

                        var titleBar = mauiWindow.TitleBar;
                        //titleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
                        //  titleBar.ForegroundColor = Windows.UI.Color.FromArgb(0, 255, 0, 255);

                        titleBar.ExtendsContentIntoTitleBar = true;
                        titleBar.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.ButtonBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.InactiveBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.ButtonInactiveBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();


                        titleBar.InactiveBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor(); 
                        titleBar.BackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.ForegroundColor = Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.ButtonBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.ButtonHoverBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.ButtonInactiveBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();
                        titleBar.ButtonPressedBackgroundColor = Color.FromArgb("FF505050").ToWindowsColor();

                        var dispH = DeviceDisplay.Current.MainDisplayInfo.Height;
                        var dispW = DeviceDisplay.Current.MainDisplayInfo.Width;
                        var dispD = DeviceDisplay.Current.MainDisplayInfo.Density;
                        var p = new PointInt32(Convert.ToInt32((dispW / dispD - Constants.MIN_WINDOW_WIDTH_WINDOWS) / 2), Convert.ToInt32((dispH / dispD - Constants.MIN_WINDOW_HEIGHT_WINDOWS) / 2));

                        var wndRect = new RectInt32(p.X, p.Y, Constants.MIN_WINDOW_WIDTH_WINDOWS, Constants.MIN_WINDOW_HEIGHT_WINDOWS);
                      //  titleBar.SetDragRectangles([new RectInt32(0, 0, WindowWidth, WindowHeight)]);
   
                        
                        // CENTER AND RESIZE THE APP
                        mauiWindow.MoveAndResize(wndRect);


                    });
                });
#endif
            });


        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("EntryHandlerCustomization", (handler, view) =>
        {

#if ANDROID
          //  handler.PlatformView.SetPadding(10,10,10,10);  
#elif WINDOWS

#endif

        });

        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("PickerHandlerCustomization", (handler, view) =>
        {

#if ANDROID
          //  handler.PlatformView.SetPadding(10,10,10,10);  
#elif WINDOWS

#endif

        });


        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS

            /// Resize and Position The App
            var mauiWindow = handler.VirtualView;

            var nativeWindow = handler.PlatformView;
            nativeWindow.AppWindow.TitleBar.PreferredHeightOption = 0;
            nativeWindow.ExtendsContentIntoTitleBar = false;

            nativeWindow.Activate();

            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new SizeInt32(Constants.MIN_WINDOW_WIDTH_WINDOWS, Constants.MIN_WINDOW_HEIGHT_WINDOWS));

#endif

        });




         Microsoft.Maui.Handlers.CheckBoxHandler.Mapper.AppendToMapping("CheckBoxHandler", (handler, view) =>
        {
            if (view is CheckBox)
            {
#if ANDROID

#elif IOS || MACCATALYST

#elif WINDOWS
                // Something changed after this point - suspect is VS 2022 build.
                if (System.Environment.Version.Build > 8)
                {
                    handler.PlatformView.Margin = new Microsoft.UI.Xaml.Thickness(35, 0, -100, 0);
                }
#endif
            }
        });






#if DEBUG
        //    Thread.Sleep(5000);
        builder.Logging.AddDebug();
#endif
        builder.Services.AddScoped<SettingsPage>();
        var build =  builder.Build();

        return build;
	}

#if WINDOWS

    // this event will run for old app instances
    private static void HandleAppActions(string? arguments) =>
        HandleAppAction(AppActionsHelper.GetAppActionId(arguments));

    // this event will run for new app instances
    private static void HandleAppActions(AppAction appAction) =>
        HandleAppAction(appAction.Id);

    private static void HandleAppAction(string? appActionId)
    {
        var wasAppAction = appActionId == "appicon";

        if (Application.Current is not App app)
            return;

        foreach (var window in app.Windows)
        {
            if (window.Page is not Page page)
                continue;

            if (page is Shell shell)
                page = shell.CurrentPage;

            page.Dispatcher.DispatchAsync(async () =>
            {
                await Task.Delay(100);

                await AppShellModel.Instance.ShowPopupAsync(new PopupInfo("OS OBDII", "App is already open"));

                //await page.DisplayAlert(
                //    "Single Instance",
                //    wasAppAction ? "This was shown from the App Actions." : "This was shown from a new launch.",
                //    "OK");
            });
        }
    }

#endif

}
