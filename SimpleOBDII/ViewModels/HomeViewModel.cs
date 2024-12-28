using System;
//using System.Windows.Input;
using OS.OBDII.ViewModels;
using OS.OBDII;
using OS.OBDII.Models;
using System.Windows.Input;
using OS.OBDII.Interfaces;
using OS.OBDII;
using Constants = OS.OBDII.Constants;

namespace OS.OBDII.ViewModels;

public class HomeViewModel : BaseViewModel_AdSupport, IViewModel
{



    public event ViewModelEvent ModelEvent;
    public event RequestPopup NeedYesNoPopup;

    IOBDIICommonUI _appShellModel = null;
    public HomeViewModel(IOBDIICommonUI appShell) : base(appShell)
    {
        if (appShell == null) throw new Exception("HomeViewModel..ctor - appShell cannot be null");

        this._appShellModel = appShell;

        Preferences.Default.Set(Constants.APP_PROPERTY_KEY_INITIAL_VIEW, true);

        Title = "Simple OBDII";
        Description = "Simple OBDII";

        OpenSnapshotPageCommand = new Command(async () => {
            this.IsBusy = true;
            await Task.Run(() => {
                Dispatcher.GetForCurrentThread().DispatchAsync(() => {
                    if (this.ModelEvent != null)
                    {
                        this.IsBusy = true;
                        using (ViewModelEventArgs evt = new ViewModelEventArgs())
                        {
                            evt.EventType = ViewModelEventEventTypes.NavigateTo;
                            evt.dataObject = "SnapshotPage";
                            this.ModelEvent(this, evt);
                        }
                    }
                });
            });
        });

        OpenDTCDataPageCommand = new Command(async () => {
            this.IsBusy = true;
            await Task.Run(() => {

                Dispatcher.GetForCurrentThread().DispatchAsync(() => {
                    if (this.ModelEvent != null)
                    {
                        this.IsBusy = true;
                        using (ViewModelEventArgs evt = new ViewModelEventArgs())
                        {
                            evt.EventType = ViewModelEventEventTypes.NavigateTo;
                            evt.dataObject = "CodesPage";
                            this.ModelEvent(this, evt);
                        }
                    }
                });
            });
        });
        OpenReadinessPageCommand = new Command(async () => {
            this.IsBusy = true;
            await Task.Run(() => {
                Dispatcher.GetForCurrentThread().DispatchAsync(() => {
                    if (this.ModelEvent != null)
                    {
                        this.IsBusy = true;
                        using (ViewModelEventArgs evt = new ViewModelEventArgs())
                        {
                            evt.EventType = ViewModelEventEventTypes.NavigateTo;
                            evt.dataObject = "IMMonitorsPage";
                            this.ModelEvent(this, evt);
                        }
                    }
                });
            });
        });
        OpenLiveDataPageCommand = new Command(async () => {
            this.IsBusy = true;
            await Task.Run(() => {
                Dispatcher.GetForCurrentThread().DispatchAsync(() => {
                    if (this.ModelEvent != null)
                    {
                        this.IsBusy = true;
                        using (ViewModelEventArgs evt = new ViewModelEventArgs())
                        {
                            evt.EventType = ViewModelEventEventTypes.NavigateTo;
                            evt.dataObject = "LiveHomePage";
                            this.ModelEvent(this, evt);
                        }
                    }
                });
            });
        });

        OpenSetupPageCommand = new Command(async () => {
            this.IsBusy = true;
            await Task.Run(() => {
                Dispatcher.GetForCurrentThread().DispatchAsync(() => {
                    if (this.ModelEvent != null)
                    {
                        this.IsBusy = true;
                        using (ViewModelEventArgs evt = new ViewModelEventArgs())
                        {
                            evt.EventType = ViewModelEventEventTypes.NavigateTo;
                            evt.dataObject = "SettingsPage";
                            this.ModelEvent(this, evt);
                        }
                    }
                });
            });
        }, () => CanSetDevices);
        OpenAboutPageCommand = new Command(async () => {
            this.IsBusy = true;
            await Task.Run(() => {
                Dispatcher.GetForCurrentThread().DispatchAsync(() => {
                    if (this.ModelEvent != null)
                    {
                        this.IsBusy = true;
                        using (ViewModelEventArgs evt = new ViewModelEventArgs())
                        {
                            evt.EventType = ViewModelEventEventTypes.NavigateTo;
                            evt.dataObject = "AboutPage";
                            this.ModelEvent(this, evt);
                        }
                    }
                });
            });
        });

        RequestPermissionsCommand = new Command(async () => {
            this.IsBusy = true;
            await Task.Run(() => {
                Dispatcher.GetForCurrentThread().DispatchAsync(() => {
                    using (var evt = new ViewModelEventArgs())
                    {
                        evt.EventType = ViewModelEventEventTypes.PermissionsRequest;

                        this.FireModelEvent(evt);
                    }
                });
            });
        });

        View1Command = new Command(async () => {
            await  AppShellModel.Instance.ShowPopupAsync(new Views.PopupInfo("Menu 1 Title", "Some message about menu 1"));
        });

        View2Command = new Command(async () => {
            await  AppShellModel.Instance.ShowPopupAsync(new Views.PopupInfo("Menu 2 Title","Some message about menu 2"));
        });


        OnPropertyChanged("CurrentView");

        OnPropertyChanged("CanSetDevices");
        OnPropertyChanged("SelectedManufacturer");
        OnPropertyChanged("SystemProtocolDescription");
        OnPropertyChanged("TargetCANID");

    }


    public List<IVehicleModel> Manufacturers => _appShellModel.Manufacturers;


    public IVehicleModel SelectedManufacturer
    {
        get => _appShellModel.SelectedManufacturer;
        set
        {
            _appShellModel.SelectedManufacturer = value;
            OnPropertyChanged("SelectedManufacturer");
        }
    }



    private void FireModelEvent(ViewModelEventArgs evt)
    {
        if (this.ModelEvent != null)
        {
            this.ModelEvent(this, evt);
        }
    }



    public bool CanSetDevices
    {
        get => ((App)Application.Current).HasPermissions;
    }



    string description = string.Empty;


    public void Start()
    {
        OnPropertyChanged("SelectedManufacturer");
        if (Preferences.Default.ContainsKey(Constants.APP_PROPERTY_KEY_INITIAL_VIEW))
        {

            // Clear the flag
            Preferences.Default.Remove(Constants.APP_PROPERTY_KEY_INITIAL_VIEW);
        }
        //DemoPrompt();

        OnPropertyChanged("CanSetDevices");
        OnPropertyChanged("TargetCANID");
        OnPropertyChanged("ProtocolDescription");

        //    OnPropertyChanged("AdContent");
        //    OnPropertyChanged("AdContent");
        //   var b = AdContent;
        this.IsBusy = false;
    }
    public void Initialize()
    {
        this.IsBusy = false;

    }
    public void CloseCommService()
    {
        AppShellModel.Instance.SendHapticFeedback();
    }
    public void Stop()
    {
        OnPropertyChanged("CanSetDevices");
    }



    public string Description
    {
        get { return description; }
        set { SetProperty(ref description, value); }
    }

    // public string ModelName => ((App)Application.Current).SelectedManufacturer.Name;
    //public string ModelName => OBD2Device.SystemReport.ModelName;


    string appVersion = string.Empty;
    public ICommand ViewCommand { get; }
    public ICommand View1Command { get; }
    public ICommand View2Command { get; }
    public ICommand OpenSnapshotPageCommand { get; }
    public ICommand OpenDTCDataPageCommand { get; }
    public ICommand OpenReadinessPageCommand { get; }
    public ICommand OpenLiveDataPageCommand { get; }
    public ICommand OpenSetupPageCommand { get; }
    public ICommand OpenAboutPageCommand { get; }
    public ICommand RequestPermissionsCommand { get; }

}
