using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OS.OBDII.WinUI;
[XamlCompilation(XamlCompilationOptions.Skip)]

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{


    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
	{

            var singleInstance = AppInstance.FindOrRegisterForKey("OS.OBDII");

#if WINDOWS

        // Ensuring only one instance is open at a time
        if (!singleInstance.IsCurrent)
        {

            // 1. activate the first instance
            var currentInstance = AppInstance.GetCurrent();
            var args = currentInstance.GetActivatedEventArgs();
            singleInstance.RedirectActivationToAsync(args).GetAwaiter().GetResult();

            // 2. close this instance
            Process.GetCurrentProcess().Kill();
            return;
        }

#endif

        // 1. register for future activation
        singleInstance.Activated += OnAppInstanceActivated; ;


        this.InitializeComponent();

		
	}

    private void OnAppInstanceActivated(object sender, AppActivationArguments e)
    {
        Services.GetRequiredService<ILifecycleEventService>().OnAppInstanceActivated(sender, e);
    }

    protected override MauiApp CreateMauiApp()
	{
		Console.WriteLine("CreateMauiApp()...");
		
		var ma = MauiProgram.CreateMauiApp();
		return ma;
	}


}

