
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System.Text;
using OS.OBDII;
using OS.OBDII.Views;
using Microsoft.VisualBasic;
using Constants = OS.OBDII.Constants;
using OS.OBDII;
using OS.OBDII.Interfaces;


namespace OS.OBDII.Views;

public delegate void CheckChanged(object sender, CheckedChangedEventArgs e);

public partial class HomePage : ContentPage
{
    int count = 0;
    private IViewModel viewModel = null;
    private IManufacturersManager manufacturersManager = Application.Current as IManufacturersManager;
    public string TextValue { get; set;}
    public HomePage()
	{
		InitializeComponent();
        this.BindingContext = new HomeViewModel(AppShellModel.Instance);
        viewModel = (this.BindingContext as IViewModel);
        if (viewModel != null)
        {
            viewModel.ModelEvent += this.OnViewModelEvent;
        }

        ((App)Application.Current).PermissionsReadyEvent += OnPermissionsResultReady;

        TapGestureRecognizer tapEvent = new TapGestureRecognizer();
        tapEvent.Tapped += OnControlTapped;
        DTCs.GestureRecognizers.Add(tapEvent);
        Settings.GestureRecognizers.Add(tapEvent);
        UserPids.GestureRecognizers.Add(tapEvent);
        Menu1.GestureRecognizers.Add(tapEvent);
        Menu2.GestureRecognizers.Add(tapEvent);

        // CODE-BEHIND binding (MVVM like)...
        //Binding binding = new Binding("txtValue");
        //binding.Source = this;
        //DTCButton.SetBinding(OSHapticButton.TextProperty, binding);
        //TextValue = "Whamy jjamammy";
        //OnPropertyChanged("TextValue");



        AppShellModel.Instance.LogService?.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "HomePage..ctor");
        if(this.manufacturersManager != null && !this.manufacturersManager.ManufacturersAreLoaded)
        {
            viewModel.IsBusy = true;
            this.manufacturersManager.ManufacturersLoaded += OnManufacturersLoaded;
        }

        cc = this.checkChangedCallback;
    }

    private void OnManufacturersLoaded(object sender, EventArgs e)
    {
        viewModel?.Start();

        this.manufacturersManager.ManufacturersLoaded -= OnManufacturersLoaded;
    }

    private void ContentView_ChildRemoved(object sender, ElementEventArgs e)
    {
        (e.Element as IDisposable)?.Dispose();
    }

    private void OnControlTapped(object sender, EventArgs e)
    {

        var curApp = ((App)Application.Current);

        if (sender.Equals(this.DTCs))
        {
            Application.Current.Dispatcher.Dispatch(async () =>
            {
                viewModel.IsBusy = true;
                Preferences.Set(Constants.APP_PROPERTY_KEY_INITIAL_VIEW, true);
                if (curApp.CodesPage == null) curApp.CodesPage = new CodesPage();
                await Navigation.PushAsync(curApp.CodesPage, false);
            });
            //AppShellModel.Instance.ShowPopupAsync(new PopupInfo("*** Test Version ***", $"DTCs Page"));
            return;
        }
        if (sender.Equals(this.UserPids))
        {
            Application.Current.Dispatcher.Dispatch(async () =>
            {
                viewModel.IsBusy = true;
                Preferences.Set(Constants.APP_PROPERTY_KEY_INITIAL_VIEW, true);
                if (curApp.UserPIDSPage == null) curApp.UserPIDSPage = new UserPIDSPage();
                await Navigation.PushAsync(curApp.UserPIDSPage, false);
            });
            //AppShellModel.Instance.ShowPopupAsync(new PopupInfo("*** Test Version ***", "UserPids"));
            return;
        }
        if (sender.Equals(this.Menu1))
        {
            //   AppShellModel.Instance.ShowPopupAsync(new PopupInfo("User Selection", "Menu 1"));
        }
        if (sender.Equals(this.Menu2))
        {
     //       AppShellModel.Instance.ShowPopupAsync(new PopupInfo("User Selection", "Menu 2"));
        }

        if (sender.Equals(this.Settings))
        {
            Application.Current.Dispatcher.Dispatch(async () => {
                viewModel.IsBusy = true;
                if (curApp.SettingsPage == null) curApp.SettingsPage = new SettingsPage();
                await Navigation.PushAsync(curApp.SettingsPage, false);
            });
            return;
        }
    }

    private void OnConfigurationChanged(object sender, EventArgs e)
    {
        viewModel?.Start();

    }

    private void SearchForButton(Element child)
    {
       // if(child.)
    }

    private void OnPermissionsResultReady(object sender, EventArgs e)
    {
//        CursorBehavior.SetCursor(DTCButton, CursorIcon.Hand);

        AppShellModel.Instance.LogService?.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "HomePage..ctor");
        AppShellModel.Instance.SetCommMethod();

        viewModel?.Start();
    }
    void OnPointerEntered(object sender, PointerEventArgs e)
    {
        // Handle the pointer entered event
    }

    void OnPointerExited(object sender, PointerEventArgs e)
    {
        // Handle the pointer exited event
    }

    void OnPointerMoved(object sender, PointerEventArgs e)
    {
        // Handle the pointer moved event
    }

    void OnPointerPressed(object sender, PointerEventArgs e)
    {
        
        // Handle the pointer moved event
    }


    public async Task<bool> ShowPopupAsync(PopupInfo popupInfo)
    {

        //var popup = new OSPopup(popupInfo);
        //var result = await this.ShowPopupAsync(popup);
        //if (result is bool boolResult) return boolResult;
        //return false;

        return await AppShellModel.Instance.ShowPopupAsync(popupInfo);
    }

    protected override bool OnBackButtonPressed()
    {
        StringBuilder sb = new StringBuilder($"Don't forget to recover your{Environment.NewLine}OBDII hardware from the vehicle.{Environment.NewLine}{Environment.NewLine}Continue to Exit?");
        Task<bool> answer = ShowPopupAsync(new PopupInfo("Exit Application", sb.ToString(), true, "Exit"));
        answer.ContinueWith(task =>
        {
            if (task.Result)
            {
                AppShellModel.Instance.CloseApp();
            }
        });
        return true;
    }

    bool CanExit = false;

    private void OnViewModelEvent(object sender, ViewModelEventArgs e)
    {
        switch (e.EventType)
        {
            case ViewModelEventEventTypes.NavigateTo:
                this.CanExit = false; // For displaying the 'remember your dongle' message before leaving app
                var curApp = ((App)Application.Current);

                switch ((e.dataObject as string))
                {
                    //case "SnapshotPage":
                    //    Preferences.Set(Constants.APP_PROPERTY_KEY_INITIAL_VIEW, true);
                    //    if (curApp.Snapshotpage == null) curApp.Snapshotpage = new SnapshotPage();
                    //    Navigation.PushAsync(curApp.Snapshotpage, false);
                    //    break;
                    //case "CodesPage":
                    //    AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "OnViewModelEvent(...) - navigate to CodesPage");

                    //    Preferences.Set(Constants.APP_PROPERTY_KEY_INITIAL_VIEW, true);
                    //    if (curApp.CodesPage == null) curApp.CodesPage = new CodesPage();
                    //    Navigation.PushAsync(curApp.CodesPage, false);
                    //    break;
                    //case "IMMonitorsPage":
                    //    Preferences.Set(Constants.APP_PROPERTY_KEY_INITIAL_VIEW, true);
                    //    if (curApp.IMMonitorsPage == null) curApp.IMMonitorsPage = new IMMonitorsPage();
                    //    Navigation.PushAsync(curApp.IMMonitorsPage, false);
                    //    break;
                    //case "LiveHomePage":
                    //    if (curApp.LiveHomePage == null) curApp.LiveHomePage = new LiveHomePage();
                    //    Navigation.PushAsync(curApp.LiveHomePage, false);
                    //    break;
                    //case "SettingsPage":
                    //    if (curApp.SettingsPage == null) curApp.SettingsPage = new SettingsPage();
                    //    Navigation.PushAsync(curApp.SettingsPage, false);
                    //    break;
                    //case "AboutPage":
                    //    AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "OnViewModelEvent(...) - navigate to AboutPage");
                    //    if (curApp.AboutPage == null) curApp.AboutPage = new AboutPage();
                    //    Navigation.PushAsync(curApp.AboutPage, false);
                    //    break;
                }
                break;

            case ViewModelEventEventTypes.ScrollTo:
                break;
            case ViewModelEventEventTypes.PermissionsRequest:
                // AppShellViewModel.Instance.RequestDevicePermissions();
                break;
        }
    }

    protected override void OnAppearing()
    {
        AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "HomePage.OnAppearing()");
        //if(viewModel != null)
        //{
        //    viewModel.IsBusy = false;

        //}
        ((App)Application.Current).ConfigurationChanged += OnConfigurationChanged; ;

        //  viewModel.ModelEvent -= this.OnViewModelEvent;
        if (this.manufacturersManager != null && this.manufacturersManager.ManufacturersAreLoaded)
        {
            viewModel?.Start();
        }
        //else
        //{
        //    ShowPopupAsync(new PopupInfo("No Manufacturers", "There was an error loading manufacturers"));
        //    this.IsBusy = false;
        //}

        base.OnAppearing();

        if (OS.OBDII.Constants.REQUIRE_APP_ID)
        {
            // License Dialog Popup
            Application.Current.Dispatcher.Dispatch(async () => {
                var sMgr = new SecurityManager(AppShellModel.Instance.GetAppId());
                if (!sMgr.ValidateApplication())
                {
                    if (!await AppShellModel.Instance.ShowLicensePopupAsync())
                    {
                        AppShellModel.Instance.CloseApp();
                    }
                }
            });




        }

    }
    public CheckChanged cc = null;
    private void checkChangedCallback(object sender, CheckedChangedEventArgs e)
    {
        if (sender.Equals(Menu1)) ; 
    }
    protected override void OnDisappearing()
    {
        AppShellModel.Instance.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "HomePage.OnDisappearing()");
        if (viewModel != null)
        {
            viewModel.IsBusy = true;
        }
        ((App)Application.Current).ConfigurationChanged -= OnConfigurationChanged; ;

        //     viewModel.ModelEvent -= this.OnViewModelEvent;
        viewModel?.Stop();
        base.OnDisappearing();
    }


}

