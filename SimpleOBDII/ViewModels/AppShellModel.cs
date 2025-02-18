using OS.OBDII.Interfaces;
using OS.Communication;
using OS.OBDII.Models;
using OS.OBDII.Views;

namespace OS.OBDII.ViewModels;


public sealed partial class AppShellModel : BaseViewModel, IOBDIICommonUI, ILicenseManager
{
    private static readonly object padlock = new object();
    private IPlatformAppControl ActivityControl { get; } = DependencyService.Get<IPlatformAppControl>();
    private IAdService _AdService = null;
    private ICommunicationDevice _CommunicationService = null;
    private IDevicesService _DevicesService = null;
    private IWiFiService _WiFiService = null;
    public ILogService LogService { get; }

    public IPid tempIPid { get; set; } = null;

    public string VersionString => OS.OBDII.VersionInfo.AssemblyVersion;
    public IVehicleModel SelectedManufacturer
    {
        get => OBD2Device.SystemReport.SelectedManufacturer;
        set
        {
            OBD2Device.SystemReport.SelectedManufacturer = value;
            OnPropertyChanged("SelectedManufacturer");
        }
    }

    #region ILicenseManager

    public void SaveAppInstallCode(string code)
    {
        Preferences.Set(Constants.PREFS_KEY_APPLICATION_REGISTRATION_ANSWER, code);
    }

    public string GetAppId() => this.ActivityControl.UID;

    #endregion ILicenseManager

    // inherited interface 
    bool ICommunicatioProfile.OpenCommunicationChannel()
    {
        if (this.CommunicationService == null) return false;
        return this.CommunicationService.Open(this.CommunicationChannel);
    }

    private AppShellModel()
    {
        this.LogService = new OS.OBDII.PartialClasses.LogService() as ILogService;
        this._dataService = new OS.OBDII.PartialClasses.DataService() as IDataService;

        //  Preferences.Clear();
        ActivityControl = new OS.OBDII.PartialClasses.ActivityControlService() as IPlatformAppControl;

        var t = new OS.OBDII.PartialClasses.DeviceIdentifier();

        this._AdService = new OS.OBDII.PartialClasses.AdService() as IAdService;

        this.LogService?.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "**** APP START **** (AppShellModel..ctor)");

        var app = (Application.Current as IAppSizing);
        if (app != null)
        {
            this.HeaderPadding = app.StatusBarHeight;
        }

    }


    public bool IsSpecialEdition => OS.OBDII.Constants.SPECIAL_EDITION;

    public Task<bool> ShowPopupAsync(PopupInfo popupInfo)
    {
        return this.ActivityControl.ShowPopupAsync(popupInfo);
    }

    public Task<bool> ShowLicensePopupAsync()
    {
        return this.ActivityControl.ShowLicensePopupAsync();
    }

    public void CloseApp()
    {
        this.ActivityControl.Close();
    }

    public void RequestDevicePermissions(string[] permissions, int requestId = 0)
    {
        this.ActivityControl.RequestPermissions(permissions, requestId);
    }

    public int DeviceScreenWidth => this.ActivityControl.ScreenWidth;
    public int DeviceScreenHeight => this.ActivityControl.ScreenHeight;
    public double DeviceScreenDPI => this.ActivityControl.ScreenDensity;

    public void InitPlatformUI()
    {
        this.ActivityControl.ConfigureUI();
    }

    public void SetupAnimation(Image img)
    {
        if (img == null) return;
        parentAnimation = null;
        fadeOutAnimation = null;
        fadeInAnimation = null;
        //if (parentAnimation != null)
        //{
        //    return;
        //    //foreach(var ani in parentAnimation)
        //    //{
        //    //    ((Animation)ani).em
        //    //}
        //}

        parentAnimation = new Animation();
        fadeOutAnimation = new Animation(d => img.Opacity = d, 1, 0, Easing.Linear);
        fadeInAnimation = new Animation(d => img.Opacity = d, 0, 1, Easing.Linear);
        parentAnimation.Add(0, 0.5, fadeInAnimation);
        parentAnimation.Add(0.5, 1, fadeOutAnimation);
        parentAnimation.Commit(img, Constants.STRING_COMM_BLINKING_VISUAL_ELEMENT, 1, 200, repeat: () => true);
    }

    private Animation parentAnimation = null;
    private Animation fadeOutAnimation = null;
    private Animation fadeInAnimation = null;

    public void RemoveAnimation()
    {
        parentAnimation = null;
        fadeOutAnimation = null;
        fadeInAnimation = null;
    }

    public int HeaderPadding { get; private set; } = 0;
    public static AppShellModel Instance
    {
        get
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AppShellModel();
                    }
                }
            }
            return instance;
        }
    }
    private static AppShellModel instance = null;

    public IAdService AdService => this._AdService;
    public ICommunicationDevice CommunicationService => this._CommunicationService;

    public void SendHapticFeedback()
    {
        this.ActivityControl.InvokeHapticFeedback();
    }
    public bool CheckSelfPermission(string permission)
    {
        return this.ActivityControl.CheckSelfPermission(permission);
    }

    public List<IVehicleModel> Manufacturers { get; set; }

    private IDataService _dataService = null;


    private bool tabsEnabled = true;
    public bool TabsEnabled
    {
        get { return tabsEnabled; }
        set { SetProperty(ref tabsEnabled, value); }
    }





    private UInt32 iSOBaudRate = Convert.ToUInt32(Preferences.Get(Constants.PREFS_ISO_BAUD_RATE, (uint)10400));
    public UInt32 ISOBaudRate
    {
        get { return iSOBaudRate; }
        set
        {
            SetProperty(ref iSOBaudRate, value);
            Preferences.Set(Constants.PREFS_ISO_BAUD_RATE, value);

        }
    }

    public bool UseKWPWakeup
    {
        get { return useKWPWakeup; }
        set
        {
            SetProperty(ref useKWPWakeup, value);
            Preferences.Set(Constants.PREFS_KWP_WAKEUP_ON, value);
        }
    }
    private bool useKWPWakeup = Preferences.Get(Constants.PREFS_KWP_WAKEUP_ON, true);

    public String KWPInitAddress
    {
        get { return kWPInitAddress; }
        set
        {
            SetProperty(ref kWPInitAddress, value);
            Preferences.Set(Constants.PREFS_KWP_INIT_ADDRESS, value);
        }
    }
    private String kWPInitAddress = Preferences.Get(Constants.PREFS_KWP_INIT_ADDRESS, "33");

    public int SelectedProtocolIndex
    {
        get { return selectedProtocolIndex; }
        set
        {
            SetProperty(ref selectedProtocolIndex, value);
            Preferences.Set(Constants.PREFS_KEY_SELECTED_PROTOCOL, value);
        }
    }
    private int selectedProtocolIndex = Preferences.Get(Constants.PREFS_KEY_SELECTED_PROTOCOL, 0);


    public Protocol SelectedProtocol { get; set; } = null;

    public int IPPort
    {
        get { return ipPort; }
        set
        {
            SetProperty(ref ipPort, value);
            Preferences.Set(Constants.PREFS_KEY_IP_PORT, value);
            this.SetCommMethod();
        }
    }
    private int ipPort = Preferences.Get(Constants.PREFS_KEY_IP_PORT, Constants.DEFAULTS_WIFI_PORT);


    private string ipAddress = Preferences.Get(Constants.PREFS_KEY_IP_ADDRESS, Constants.DEFAULTS_WIFI_IPADDRESS);
    public string IPAddress
    {
        get { return ipAddress; }
        set
        {
            SetProperty(ref ipAddress, value);
            Preferences.Set(Constants.PREFS_KEY_IP_ADDRESS, value);

        }
    }


    private int plotHeight = Preferences.Get(Constants.PREFS_KEY_PLOT_HEIGHT, Constants.DEFAULTS_PLOT_HEIGHT);
    public int PlotHeight
    {
        get { return plotHeight; }
        set
        {
            SetProperty(ref plotHeight, value);
            Preferences.Set(Constants.PREFS_KEY_PLOT_HEIGHT, value);

        }
    }


    public List<UInt32> BaudRates { get; } = new List<UInt32>()
    { 1200, 2400, 4800, 9600, 19200, 28800, 38400, 57600, 76800, 115200, 230400 };

    private UInt32 serialBaudRate = Convert.ToUInt32(Preferences.Get(Constants.PREFS_SERIAL_BAUD_RATE, (uint)9600));
    public UInt32 SerialBaudRate
    {
        get { return serialBaudRate; }
        set
        {
            SetProperty(ref serialBaudRate, value);
            Preferences.Set(Constants.PREFS_SERIAL_BAUD_RATE, value);

        }
    }

    public bool UseHeader
    {
        get { return useHeader; }
        set
        {
            SetProperty(ref useHeader, value);
            Preferences.Set(Constants.PREFS_USE_HEADER, value);
        }
    }
    private bool useHeader = Preferences.Get(Constants.PREFS_USE_HEADER, false);

    private string userCANID = Preferences.Get(Constants.PREFS_KEY_CANID, Constants.DEFAULT_DIAG_FUNCADDR_CAN_ID_11.ToString("X"));
    public string UserCANID
    {
        get => this.userCANID;
        set
        {
            SetProperty(ref userCANID, value);
            Preferences.Set(Constants.PREFS_KEY_CANID, value);

        }
    }
    private string lastValidVIN = Preferences.Get(Constants.PREFS_KEY_LAST_VALID_VIN, string.Empty);
    public string LastValidVIN
    {
        get => this.lastValidVIN;
        set
        {
            SetProperty(ref lastValidVIN, value);
            Preferences.Set(Constants.PREFS_KEY_LAST_VALID_VIN, value);

        }
    }

    public bool UseMetric
    {
        get { return useMetric; }
        set
        {
            SetProperty(ref useMetric, value);
            Preferences.Set(Constants.PREFS_USE_METRIC, value);
            OBD2Device.UseMetricUnits = value;

        }
    }
    private bool useMetric = Preferences.Get(Constants.PREFS_USE_METRIC, false);

    public bool IsBluetooth => selectedCommMethod == null ? true : String.Compare(selectedCommMethod, Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR) == 0;

    private string selectedCommMethod = Preferences.Get(Constants.PREFS_KEY_DEVICE_COMM_TYPE, Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR);
    public string SelectedCommMethod
    {
        get => selectedCommMethod;
        set
        {
            SetProperty(ref selectedCommMethod, value);
            Preferences.Set(Constants.PREFS_KEY_DEVICE_COMM_TYPE, value);
            this.SetCommMethod();
            if (this._CommunicationService == null) return;
        }
    }


    private string selectedBluetoothDevice = Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, string.Empty);
    public string SelectedBluetoothDevice
    {
        get => selectedBluetoothDevice;
        set
        {
            SetProperty(ref selectedBluetoothDevice, value);
            if (this._CommunicationService != null) this._CommunicationService.DeviceName = value;
        }
    }

    public string PresetBluetoothDevice => Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, "");

    public bool StoreBluetoothDevicePreset(string device)
    {
        Preferences.Set(Constants.PREFS_KEY_BLUETOOTH_DEVICE, device);
        return true;
    }
    private string communicationChannel = string.Empty;
    public string CommunicationChannel
    {
        get
        {
            return String.IsNullOrEmpty(this._CommunicationService.DeviceName) ? string.Empty : this._CommunicationService.DeviceName;
        }
        set
        {
            SetProperty(ref communicationChannel, value);
            this._CommunicationService.DeviceName = value;
        }
    }

    public IList<string> DeviceList => this._DevicesService.GetDeviceList();


    public void SetCommMethod()
    {

        if (!Preferences.ContainsKey("AppInitialized"))
        {
            Preferences.Clear();
        }
        Preferences.Set("AppInitialized", true);


        if (this._DevicesService == null)
        {
            this._DevicesService = new OS.PlatformShared.MAUI_SerialDevice() as IDevicesService;
        }

        if (this.IsBluetooth)
        {

            this._CommunicationService = this._DevicesService as ICommunicationDevice;
            this.SelectedBluetoothDevice = Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, "");
            this.CommunicationService.DeviceName = this.SelectedBluetoothDevice;
            var sp = (this.CommunicationService as ISerialDevice);
            if (sp != null)
            {
                sp.BaudRate = SerialBaudRate;
            }
        }
        else
        {
            this._CommunicationService = new TCPSocket(this.IPAddress, this.IPPort, ConnectMethods.Client);
            this.communicationChannel = this.CommunicationService.ToString();
        }

    }

}
