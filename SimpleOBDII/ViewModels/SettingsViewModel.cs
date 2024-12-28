
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System.Windows.Input;
using OS.OBDII.Communication;
using Constants = OS.OBDII.Constants;
using OS.OBDII.Interfaces;


namespace OS.OBDII.ViewModels;


public class SettingsViewModel : BaseViewModel_AdSupport, IViewModel
{

    public event ViewModelEvent ModelEvent;
    public event RequestPopup NeedYesNoPopup;

    public SettingsViewModel(IOBDIICommonUI appShell) : base(appShell)
    {

        Title = "Hardware Setup";
        OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.thoughtpill.com/ODB2AppDownload"));

        //Refresh();

    }

    public ICommand OpenWebCommand { get; }

    protected SynchronizationContext syncContext;


    public ICommand ReloadBTDevicesCommand => new Command(()=> {
       // this.RefreshBluetoothDevices();
    
    });


    #region NavigateHomeCommand

    public ICommand NavigateHomeCommand => new Command(() => {

        if (this.ModelEvent != null)
        {
            this.IsBusy = true;
            using (ViewModelEventArgs evt = new ViewModelEventArgs())
            {
                evt.EventType = ViewModelEventEventTypes.NavigateTo;
                evt.dataObject = "HomePage";
                this.ModelEvent(this, evt);
            }
        }
    });

    #endregion NavigateHomeCommand

    public string SelectedCommMethod
    {
        get => AppShellModel.Instance.SelectedCommMethod;
        set {
            AppShellModel.Instance.SelectedCommMethod = value;
            OnPropertyChanged("IsBluetooth");
            OnPropertyChanged("PresetBluetoothDevice");
            OnPropertyChanged("EditButtonRow");
        }
    
    }

    public List<string> SerialCommMethods => SerialDevices.DeviceTypes;

    public string CANHeader
    {
        get => AppShellModel.Instance.UserCANID;
        set 
        {
            AppShellModel.Instance.UserCANID = value;
            OnPropertyChanged("CANHeader");
        }
    }

    /// <summary>
    /// Based on the protocol, which needs to be read first.
    /// </summary>
    public int CANAddressLength
    {
        get => OBD2Device.DataPositionOffset-2;
    }

    public bool UseHeader
    {
        get => AppShellModel.Instance.UseHeader;
        set 
        {
            AppShellModel.Instance.UseHeader = value;
            OnPropertyChanged("UseHeader");
        }
    }


    public List<UInt32> BaudRates => AppShellModel.Instance.BaudRates;


    public UInt32 SerialBaudRate
    {
        get => AppShellModel.Instance.SerialBaudRate;
        set
        {
            AppShellModel.Instance.SerialBaudRate = value;
            OnPropertyChanged("SerialBaudRate");
        }
    }
    public bool IsBluetooth
    {
        get => AppShellModel.Instance.IsBluetooth;
    }

    public int IPPort
    {
        get => AppShellModel.Instance.IPPort;
        set { AppShellModel.Instance.IPPort = value; }
    }
    public int EditButtonRow => this.IsBluetooth ? 5 : 6;

    public string IPAddress
    {
        get => AppShellModel.Instance.IPAddress;
        set { AppShellModel.Instance.IPAddress = value; }
    }

    public int PlotHeight
    {
        get => AppShellModel.Instance.PlotHeight;
        set 
        {
            //if (value < 75) value = 75;
            //if (value > 300) value = 300;
            AppShellModel.Instance.PlotHeight = value;
            OnPropertyChanged("PlotHeight");
        }
    }

    public IList<string> DeviceList => AppShellModel.Instance.DeviceList; 
    
    //private IList<string> deviceList = new List<string>();

    private string printMessage = string.Empty;
    public string PrintMessage
    {
        get { return printMessage; }
        set { SetProperty(ref printMessage, value); }
    }

    public string SelectedBluetoothDevice
    {
        get => AppShellModel.Instance.SelectedBluetoothDevice;
        set
        { 
            AppShellModel.Instance.SelectedBluetoothDevice = value;
            OnPropertyChanged("PresetBluetoothDevice");
        }
    }

    public string PresetBluetoothDevice => AppShellModel.Instance.PresetBluetoothDevice;

    private string saveCancelButtonText = "Edit";
    public string EditSaveButtonText
    {
        get { return saveCancelButtonText; }
        set { SetProperty(ref saveCancelButtonText, value); }
    }

    private bool isEditing = false;
    public bool IsEditing
    {
        get { return isEditing; }
        set 
        { 
            SetProperty(ref isEditing, value);
            this.EditSaveButtonText = value?"Done":"Edit";
          //  if (!isEditing) return;
            OnPropertyChanged("SelectedCommMethod");
            OnPropertyChanged("SelectedBluetoothDevice");
            OnPropertyChanged("IPAddress");
            OnPropertyChanged("IPPort");
            OnPropertyChanged("EditButtonRow");


        }
    }


    private bool canSend = false;
    public bool CanSend
    {
        get { return canSend; }
        set { SetProperty(ref canSend, value); }
    }


    /// <summary>
    /// This method will call the first queued command which should result in a bluetooth device event
    /// where the next queued command (if one exists) can be called
    /// </summary>

    public ICommand EditSaveCommand => new Command(async () => {
        OnPropertyChanged("DeviceList");

        if (this.IsEditing)
        {
            await Task.Run(() =>
            {
                AppShellModel.Instance.StoreBluetoothDevicePreset(this.SelectedBluetoothDevice);
                OnPropertyChanged("PresetBluetoothDevice");

                this.ValidateSettings();
                this.IsEditing = false;
                AppShellModel.Instance.SetCommMethod();
                // Force any permissions
                //AppShellModel.Instance.CommunicationService.Open();
                //AppShellModel.Instance.CommunicationService.Close();
            });
        }
        else
        {
            await Task.Run(() =>
            {
                this.IsEditing = true;
            });
        }
    });


    private bool ValidateSettings()
    {
        if (this.PlotHeight > Constants.MAX_PLOT_HEIGHT) this.PlotHeight = Constants.MAX_PLOT_HEIGHT;
        if (this.PlotHeight < Constants.MIN_PLOT_HEIGHT) this.PlotHeight = Constants.MIN_PLOT_HEIGHT;

        return true;
    }

    public void Start()
    {
        OnPropertyChanged("UseMetric");
        if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_INITIAL_VIEW))
        {
            // Clear the flag
            Preferences.Remove(Constants.APP_PROPERTY_KEY_INITIAL_VIEW);
        }

        this.IsEditing = false;
        //var tSel = AppShellModel.Instance.PresetBluetoothDevice;
        //var tSel = this.SelectedBluetoothDevice;
        OnPropertyChanged("DeviceList");
       // this.SelectedBluetoothDevice = tSel;
        OnPropertyChanged("PresetBluetoothDevice");
        if (DeviceList.Contains(this.PresetBluetoothDevice))
        {
            this.SelectedBluetoothDevice = this.PresetBluetoothDevice;
        } else
        {
            OnPropertyChanged("SelectedBluetoothDevice");
        }

        this.IsBusy = false;
    }
    public void Initialize()
    {
        this.IsBusy = false;
    }

    // to satisfy IViewModel
    public void CloseCommService()
    {

        AppShellModel.Instance.SendHapticFeedback();

    }
    public void Stop()
    {
        this.CloseCommService();
    }

}



