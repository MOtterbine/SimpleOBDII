using CommunityToolkit.Maui.Views;
using OS.OBDII.Interfaces;


namespace OS.OBDII.Views;

public partial class LicensePopup : Popup
{
    private SecurityManager securityManager = null;

    private ILicenseManager _appLicenseManager = null;
    public LicensePopup(ILicenseManager licenseMgr)
    {
        if (licenseMgr == null) throw new ArgumentNullException("appShell");
        this._appLicenseManager = licenseMgr;


        OS.OBDII.Interfaces.IDeviceId device = new OS.OBDII.PartialClasses.DeviceIdentifier();
        this.securityManager = new SecurityManager(device.UID);
      //  PopupInfo popupInfo = new PopupInfo("Install", "do thing", true);
        InitializeComponent();

        BindingContext = this;
        // user must click button to dismiss dialog
        this.CanBeDismissedByTappingOutsideOfPopup = false;


        this.Title.Text = "App Installation";
        this.AppId.Text = securityManager.GetAppId();

    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

#if WINDOWS
        Copy.SetCustomCursor(CursorIcon.Hand, Copy.Handler?.MauiContext);
        // Exit.SetCustomCursor(CursorIcon.Hand);
        //Apply.SetCustomCursor(CursorIcon.Hand);
#endif

    }


    void CopyAppId(object sender, EventArgs args)
    {
        //this.AppId.SelectionLength = this.AppId.Text.Length;
        Clipboard.SetTextAsync(this.AppId.Text);
    }

    void ApplyLicense(object sender, EventArgs args)
    {
        string appId = this._appLicenseManager.GetAppId();
        var calcHash = SecurityManager.GetCRCStringFromString(appId);
        if(!string.IsNullOrEmpty(calcHash) 
            && !string.IsNullOrEmpty(userHash.Text) 
            && string.Compare(calcHash, userHash.Text) == 0)
        {
            this._appLicenseManager.SaveAppInstallCode(calcHash);
            //Preferences.Set(Constants.PREFS_KEY_APPLICATION_REGISTRATION_ANSWER, calcHash);
            //void SaveAppInstallCode(string code);

            Close(true);
            return;
        }
        this.userHash.Text = "";

    }

    void Button_Clicked(object sender, EventArgs args)
    {
        // 'Dismiss' returns whatever we pass back...
        Close(true);
    }

    void Button_cancel_Clicked(object sender, EventArgs args)
    {
        // 'Dismiss' returns whatever we pass back...
        Close(false);
    }

    public bool IsYesNo
    {
        get => (bool)GetValue(IsYesNoProperty);
        set => SetValue(IsYesNoProperty, value);
    }
    public static readonly BindableProperty IsYesNoProperty =
        BindableProperty.Create("IsYesNo", typeof(bool), typeof(OSPopup), false);

    public string OkText
    {
        get => (string)GetValue(OkTextProperty);
        set => SetValue(OkTextProperty, value);
    }
    public static readonly BindableProperty OkTextProperty =
        BindableProperty.Create("OkText", typeof(string), typeof(OSPopup), "Ok");

    public string CancelText
    {
        get => (string)GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }
    public static readonly BindableProperty CancelTextProperty =
        BindableProperty.Create("CancelText", typeof(string), typeof(OSPopup), "Cancel");

    protected override void OnParentSet()
    {

        base.OnParentSet();
    }
    public override View? Content 
    { 
        get => base.Content;
        set
        { 
            base.Content = value;
        }
    }

}


