using OS.OBDII.Interfaces;
using OS.OBDII.Communication;
using OS.OBDII.Models;
using OS.OBDII.Views;
using OS.OBDII.ViewModels;
using OS.OBDII.Manufacturers;
using Constants = OS.OBDII.Constants;
using OS.OBDII;
using Newtonsoft.Json;
using System.Reflection;
using System.Resources;
using Microsoft.Maui.Storage;
using System.Formats.Tar;

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

    public IPid tempIPid = null;

    public string VersionString => OS.OBDII.VersionInfo.AppVersion;
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



    private AppShellModel()
    {
        this.LogService = new OS.OBDII.PartialClasses.LogService() as ILogService;
        this._dataService = new OS.OBDII.PartialClasses.DataService() as IDataService;

        //  Preferences.Clear();
        ActivityControl = new OS.OBDII.PartialClasses.ActivityControlService() as IPlatformAppControl;

        var t = new OS.OBDII.PartialClasses.DeviceIdentifier();

        // this._AdService = DependencyService.Get<IAdService>();
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


    private async Task<bool> ShowPopup(PopupInfo popupInfo)
    {
        var popupPage = Application.Current.MainPage as ICustomPopup;
        if(popupPage == null)
        {
            this.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Error, "Main page is not an 'ICustomPopup' and cannot support the popup request.");
            return false;
        }
        var result = await popupPage.ShowPopupAsync(popupInfo);
        //var popup = new OSPopup(popupInfo);
        //var result = await App.Current.MainPage.ShowPopupAsync(popup);
        
        if (result is bool boolResult) return boolResult;
        return false;

        // return await AppShellModel.Instance.ShowPopupAsync(popupInfo);
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
    //public bool DeviceIsInitialized { get; set; }
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

    //public List<IManufacturer> Manufacturers { get; } = new List<IManufacturer>() {
    //            new OBD2FaultCodes_generic(),
    //            new Bajaj125(),
    //            new Bajaj150(),
    //            new Benelli(),
    //            new HERO_GEN5(),
    //            new Jawa(),
    //            new Suzuki(),
    //            new Yamaha()
    //};
    public List<IVehicleModel> Manufacturers { get; set; }

    //public List<IManufacturer> Manufacturers { get; private set; } = new List<IManufacturer>();

    //private async Task LoadManufacturersData()
    //{
    //    try
    //    {

    //        List<string> modelNames = new List<string>()
    //        {
    //            "Manufacturers.Bajaj125.json",
    //            "Manufacturers.Bajaj150.json",
    //            "Manufacturers.Benelli.json",
    //            "Manufacturers.Ford.json",
    //            "Manufacturers.HERO_GEN5.json",
    //            "Manufacturers.Jawa.json",
    //            "Manufacturers.RoyalEnfield.json",
    //            "Manufacturers.Suzuki.json",
    //            "Manufacturers.US_Generic.json",
    //            "Manufacturers.Yamaha.json"
    //        };

    //        OBD2FaultCodes model = null;

    //        foreach (var m in modelNames)
    //        {
    //            var stream0 = await FileSystem.OpenAppPackageFileAsync(m);
    //            var reader0 = new StreamReader(stream0);
    //            var jsonString0 = reader0.ReadToEnd();
    //            model = JsonConvert.DeserializeObject<OBD2FaultCodes>(jsonString0);

    //            if (model != null && model.FaultCodes.Count() > 0)
    //            {
    //                this.Manufacturers.Add(model);
    //            }
    //        }

    //    }
    //    catch (Exception e)
    //    {
    //        this.LogService?.AppendLog(Microsoft.Extensions.Logging.LogLevel.Error,
    //                    $"AppShellModel.LoadManufacturersData() - {e.Message}");

    //    }
    //}
    //async Task LoadMauiAsset(string fileName)
    //{
    //    using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
    //    using var reader = new StreamReader(stream);

    //    var fucktard =  reader.ReadToEnd();
    //}


    private IDataService _dataService = null;




    private bool tabsEnabled = true;
    public bool TabsEnabled
    {
        get { return tabsEnabled; }
        set { SetProperty(ref tabsEnabled, value); }
    }



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
            // this.IsBluetooth = (value == Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR);

            this.SetCommMethod();
            //this.DeviceIsInitialized = false;
            if (this._CommunicationService == null) return;
            //this.CommunicationChannel = this._CommunicationService.DeviceName;
            //this.communicationChannel.Name = this._CommunicationService.DeviceName;
            //this.communicationChannel.BaseType = this._CommunicationService.GetType();

        }
    }



    private string selectedBluetoothDevice = Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, string.Empty);
    public string SelectedBluetoothDevice
    {
        get => selectedBluetoothDevice;
        set
        {
            //Android.Util.Log.Info(Constants.STRING_LOG_TAG, $"AppShellModel.SelectedBluetoothDevice.set({value}) ");

            SetProperty(ref selectedBluetoothDevice, value);
            //this.DeviceIsInitialized = false;
            if (this._CommunicationService != null) this._CommunicationService.DeviceName = value;
        }
    }

    public string PresetBluetoothDevice => Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, "");

    public bool StoreBluetoothDevicePreset(string device)
    {
        //if(string.IsNullOrEmpty(device)) return false;
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
            //this.DeviceIsInitialized = false;
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
            this._DevicesService = new OS.OBDII.PartialClasses.MAUI_SerialDevice() as IDevicesService;
        }

        if (this.IsBluetooth)
        {

            this._CommunicationService = this._DevicesService as ICommunicationDevice;
            this.SelectedBluetoothDevice = Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, "");
            //if(string.IsNullOrEmpty(this.SelectedBluetoothDevice))
            //{
            //    // First run...
            //    Preferences.Clear();
            //}
            this.CommunicationService.DeviceName = this.SelectedBluetoothDevice;
            var sp = (this.CommunicationService as ISerialDevice);
            if (sp != null)
            {
                sp.BaudRate = SerialBaudRate;
            }
        }
        else
        {
            //  if (this._CommunicationService is IBlueToothService)
            //  {
            this._CommunicationService = new TCPSocket(this.IPAddress, this.IPPort, ConnectMethods.Client);
            this.communicationChannel = this.CommunicationService.ToString();
            //    }
        }

        // device hardware is stateful (as is the ICommunicationService)
        //this.DeviceIsInitialized = false;

        this._CommunicationService?.Initialize();

        // Ensure communication object starts as closed
        //this.CommunicationService?.Close();

    }





}
