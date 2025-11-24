
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System.Windows.Input;
using OS.Communication;
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

    }

    public ICommand OpenWebCommand { get; }

    protected SynchronizationContext syncContext;


    public ICommand ReloadBTDevicesCommand => new Command(() => {
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

    public Tuple<string, int> SelectedCommMethod
    {
        
        get => SerialCommMethods[_appShellModel.SelectedCommMethod];
        
    
        set
        {
            
            if(value.Item2 == 0)
            {

            }
            _appShellModel.SelectedCommMethod = value.Item2; // index
            OnPropertyChanged("IsBluetooth");
            OnPropertyChanged("PresetBluetoothDevice");
            OnPropertyChanged("EditButtonRow");
        }
    }
    

    public List<Tuple<string, int>> SerialCommMethods => SerialDevices.DeviceTypes;

    public string CANHeader
    {
        get => _appShellModel.UserCANID;
        set 
        {
            _appShellModel.UserCANID = value;
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
        get => _appShellModel.UseHeader;
        set 
        {
            _appShellModel.UseHeader = value;
            OnPropertyChanged("UseHeader");
        }
    }


    public bool UseKWPWakeup
    {
        get => this._appShellModel.UseKWPWakeup;
        set
        {
            this._appShellModel.UseKWPWakeup = value;
            OnPropertyChanged("UseKWPWakeup");
        }
    }

    public String KWPInitAddress
    {
        get => this._appShellModel.KWPInitAddress;
        set
        {
            this._appShellModel.KWPInitAddress = value;
            OnPropertyChanged("KWPInitAddress");
        }
    }
    public UInt32 ISOBaudRate
    {
        get => this._appShellModel.ISOBaudRate;
        set
        {
            this._appShellModel.ISOBaudRate = value;
            OnPropertyChanged("ISOBaudRate");
        }
    }

    public List<UInt32> BaudRates => OS.Communication.BaudRates.Items;
    public List<UInt32> ISOBaudRates { get; } = OS.Communication.ISOBaudRates.Items.ToList().ConvertAll(x => x.Key);


    public UInt32 SerialBaudRate
    {
        get => _appShellModel.SerialBaudRate;
        set
        {
            _appShellModel.SerialBaudRate = value;
            OnPropertyChanged("SerialBaudRate");
        }
    }
    public bool IsBluetooth
    {
        get => _appShellModel.IsBluetooth;
    }

    public int IPPort
    {
        get => _appShellModel.IPPort;
        set { _appShellModel.IPPort = value; }
    }
    public int EditButtonRow => this.IsBluetooth ? 5 : 6;

    public string IPAddress
    {
        get => _appShellModel.IPAddress;
        set { _appShellModel.IPAddress = value; }
    }

    public int PlotHeight
    {
        get => _appShellModel.PlotHeight;
        set 
        {
            //if (value < 75) value = 75;
            //if (value > 300) value = 300;
            _appShellModel.PlotHeight = value;
            OnPropertyChanged("PlotHeight");
        }
    }

    public IList<string> DeviceList => _appShellModel.DeviceList; 
    
    //private IList<string> deviceList = new List<string>();

    private string printMessage = string.Empty;
    public string PrintMessage
    {
        get { return printMessage; }
        set { SetProperty(ref printMessage, value); }
    }

    public string SelectedBluetoothDevice
    {
        get => _appShellModel.SelectedBluetoothDevice;
        set
        { 
            _appShellModel.SelectedBluetoothDevice = value;
            OnPropertyChanged("PresetBluetoothDevice");
        }
    }

    public string PresetBluetoothDevice => _appShellModel.PresetBluetoothDevice;

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
                _appShellModel.StoreBluetoothDevicePreset(this.SelectedBluetoothDevice);
                OnPropertyChanged("PresetBluetoothDevice");

                this.ValidateSettings();
                this.IsEditing = false;
                _appShellModel.SetCommMethod();
                // Force any permissions
                //_appShellModel.CommunicationService.Open();
                //_appShellModel.CommunicationService.Close();
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
        //var tSel = _appShellModel.PresetBluetoothDevice;
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

        _appShellModel.SendHapticFeedback();

    }
    public void Stop()
    {
        this.CloseCommService();
    }
    public override void Back()
    {
        if (this.isEditing) EditSaveCommand.Execute(null);
        else NavigateHomeCommand.Execute(null);
        base.Back();
    }

}



