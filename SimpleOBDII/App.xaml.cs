using OS.OBDII.Models;
using OS.OBDII;
using OS.OBDII.ViewModels;
using OS.OBDII.Views;
using OS.OBDII.Interfaces;
using OS.OBDII.Views.Styles;
using System.Text;
using OS.OBDII.Manufacturers;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using OS.OBDII.Security;
using Microsoft.Maui.Platform;
using CommunityToolkit.Maui.Core.Primitives;


#if WINDOWS
using static OS.OBDII.WinUI.App;
//using System.Windows.Interop;
using System.Runtime.InteropServices;
#endif

namespace OS.OBDII;


public delegate void PermissionsResultReady(object sender, EventArgs e);
public partial class App : Microsoft.Maui.Controls.Application, IAppSizing, IPermissions, ICustomPopup, IManufacturersManager, IAppConfiguration
{

    public bool HasPermissions { get; set; } = false;
    private int statusBarHeight = 0;
    public int StatusBarHeight
    {
        get => this.statusBarHeight;
        set
        {
            this.statusBarHeight = value;

            OnPropertyChanged("StatusBarHeight");
            this.HeaderPadding = new Thickness(0, this.StatusBarHeight, 0, 0);
        }
    }

    public async Task<bool> ShowPopupAsync(PopupInfo popupInfo)
    {
        return await AppShellModel.Instance.ShowPopupAsync(popupInfo);
    }

    public int AdHeight
    {
        get { return (int)GetValue(AdHeightProperty); }
        set { SetValue(AdHeightProperty, value); }
    }
    public static readonly BindableProperty AdHeightProperty =
    BindableProperty.Create("AdHeight", typeof(int), typeof(App), 0);



    public WifiInfoRecord SelectedDeviceWifiType
    {
        get
        {
            return WifiTypes.WifiTypesList.Where(w=>w.WifiType == currentWiFiTypeTypeId).FirstOrDefault();
        }
        set
        {
            this.currentWiFiTypeTypeId = value.WifiType;
            Preferences.Default.Set(Constants.PREFS_KEY_DEVICE_WIFI_MODE, (int)value.WifiType);
        }
    }
    private WifiTypeDescriptions currentWiFiTypeTypeId = (WifiTypeDescriptions)Preferences.Default.Get(Constants.PREFS_KEY_DEVICE_WIFI_MODE, (int)WifiTypes.WifiTypesList[0].WifiType);


    public Thickness HeaderPadding
    {
        get { return (Thickness)GetValue(HeaderPaddingProperty); }
        set { SetValue(HeaderPaddingProperty, value); }
    }

    public static readonly BindableProperty HeaderPaddingProperty =
        BindableProperty.Create("HeaderPadding", typeof(Thickness), typeof(App), new Thickness(0));

    public SettingsPage SettingsPage { get; set; } = null;
    public CodesPage CodesPage { get; set; } = null;
    public FreezeFramePage FreezeFramePage { get; set; } = null;
    public UserPIDSPage UserPIDSPage { get; set; } = null;
    public PIDDetailsPage PIDDetailsPage { get; set; } = null;
    public VehicleInfoPage VehicleInfoPage { get; set; } = null;


    #region IManufacturersManager

    public List<IVehicleModel> VehicleModels { get; set; } = new List<IVehicleModel>();
    public event EventHandler ManufacturersLoaded;
    public void FireManufacturersLoadedEvent()
    {
        if (this.ManufacturersLoaded != null)
        {
            this.ManufacturersLoaded(null, EventArgs.Empty);
        }
    }

    public IVehicleModel SelectedManufacturer
    {
        get { return OBD2Device.SystemReport.SelectedManufacturer; }
        set
        {
            OBD2Device.SystemReport.SelectedManufacturer = value;
            if (value == null) return;
            Preferences.Default.Set(Constants.PREFS_KEY_MANUFACTURER, value.Name);
        }
    }

    private async Task LoadManufacturersData()
    {
        try
        {

            // vehicle models models
            List<string> modelNames = new List<string>()
            {
                "manufacturers.custom.json"
            };
			
            AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug,
                                    $"App.LoadManufacturersData() - {modelNames.Count} manufacturers to load");
			

            IVehicleModel model = null;

            string jsonString = String.Empty;
            String encodedString = string.Empty;
            foreach (var m in modelNames)
            {
                using (var stream = await FileSystem.OpenAppPackageFileAsync(m))
                {
                    using (var reader = new StreamReader(stream))
                    {
#if ANDROID
                        int len = 1000000;
                        byte[] bytes = new byte[len];
                        int numBytesToRead = len;
                        int numBytesRead = 0;

#elif WINDOWS
                        byte[] bytes = new byte[stream.Length];
                        int numBytesToRead = (int)stream.Length;
                        int numBytesRead = 0;
#endif

#if WINDOWS || ANDROID
                        while (numBytesToRead > 0)
                        {
                            // Read may return anything from 0 to numBytesToRead.
                            int n = stream.Read(bytes, numBytesRead, numBytesToRead);
                            //int n = stream.Read(bytes, numBytesRead, numBytesToRead);

                            // Break when the end of the file is reached.
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }
                        numBytesToRead = bytes.Length;

                        byte[] temp = new byte[numBytesRead];
                        Array.Copy(bytes, temp, numBytesRead);
                        jsonString = Encoding.ASCII.GetString(temp);
#endif  
                    }
                }
                // Add all the models
                model = JsonConvert.DeserializeObject<VehicleModel>(jsonString);

                if (model != null && model.FaultCodes.Count() > 0)
                {
                    this.VehicleModels.Add(model);
                }
            }
            ManufacturersAreLoaded = true;
        }
        catch (Exception e)
        {
            AppShellModel.Instance.LogService?.AppendLog(Microsoft.Extensions.Logging.LogLevel.Error,
                        $"AppShellModel.LoadManufacturersData() - {e.Message}");
        }
    }


    public bool ManufacturersAreLoaded { get; private set; } = false;

#endregion IManufacturersManager

    public App()
	{

        //Preferences.Default.Clear();

        // Memory holder for current vehicle data
        OBD2Device.SystemReport = new SystemReport(new OS.OBDII.PartialClasses.DataService() as IPIDManager);

        // Load Up the Manufacturers
       this.LoadManufacturersData().ContinueWith((result) =>
        {
            if(this.VehicleModels == null || this.VehicleModels.Count < 1)
            {
                AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Error,
                           $"App..ctor - no manufactures");
                throw new InvalidDataException("No manufacturers could be loaded");
            }
            var mfString = Preferences.Default.Get(Constants.PREFS_KEY_MANUFACTURER, this.VehicleModels[0].Name); ;
            SelectedManufacturer = this.VehicleModels.Where(m => m.Name == mfString).FirstOrDefault();

            if(SelectedManufacturer == null)
            {
                SelectedManufacturer = VehicleModels[0];
            }

            this.FireManufacturersLoadedEvent();
        });


#if WINDOWS
            this.HasPermissions = true;
#endif

            InitializeComponent();


            AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "About to create MainPage");

            LoadStylesAndTemplates();
            MainPage = new NavigationPage(new HomePage());

    }


    private void OnActivityPaused(object sender, EventArgs e)
    {
    }
    private void OnActivityResumed(object sender, EventArgs e)
    {
    }

    private ResourceDictionary customStyles = null;
    void LoadStylesAndTemplates()
    {
        // Come up with some way to decide which additional styles file to use (sm vs lg)
        bool isSmall = AppShellModel.Instance.DeviceScreenDPI < Constants.STYLES_BREAKPOINT_0;
        customStyles = new ResourceDictionary();

        customStyles.Add(new Styles_C());
        if (isSmall)
        {
            customStyles.Add(new Styles_sm());
            AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "App.AssignStyles() - apply small styles");
        }
        else
        {
            customStyles.Add(new Styles_lg());
            AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "App.AssignStyles() - apply larger styles");
        }

        // templates
        customStyles.Add(new OS.OBDII.Templates());

        Resources.Add(customStyles);

    }

    protected override Window CreateWindow(IActivationState activationState)
    {

        var window = base.CreateWindow(activationState);

#if WINDOWS

            window.MinimumWidth = Constants.MIN_WINDOW_WIDTH_WINDOWS;
            window.MinimumHeight = Constants.MIN_WINDOW_HEIGHT_WINDOWS;
            AppShellModel.Instance.SetCommMethod();

#endif

            return window;

    }

    #region IAppConfiguration

    public event EventHandler ConfigurationChanged;
    public void FireConfigurationChangedEvent()
    {
        if (this.ConfigurationChanged != null)
        {
            this.ConfigurationChanged(null, EventArgs.Empty);
        }
    }

    public void NotifyConfigurationChanged()
    {
        FireConfigurationChangedEvent();
    }

    #endregion IAppConfiguration


    public event EventHandler Resumed;
    public void FireResumedEvent()
    {
        if (this.Resumed != null)
        {
            this.Resumed(null, EventArgs.Empty);
        }
    }

    public event PermissionsResultReady PermissionsReadyEvent;

    public void FirePermissionsReadyEvent()
    {

        if (this.PermissionsReadyEvent != null)
        {
            this.PermissionsReadyEvent(null, EventArgs.Empty);
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        FireResumedEvent();
    }

}


