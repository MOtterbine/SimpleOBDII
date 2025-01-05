using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using OS.OBDII.ViewModels;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace OS.OBDII.ViewModels;

public class BaseViewModel_AdSupport : OS.OBDII.ViewModels.BaseViewModel
{
    public virtual string VersionString => VersionInfo.AssemblyVersion;

    protected Queue<Task> ActionQueue = new Queue<Task>();
    protected IAdService adService => this._appShellModel.AdService;
    public ContentView AdContent => this._appShellModel.AdService.BannerAd;
    public virtual bool AdsAreActive { get; }
    private TimerCallback _CommTimeoutHandler = null;

    protected Timer _CommTimer = null;

    protected object dLock = new object();

    public void SendHapticFeedback()
    {
        this._appShellModel.SendHapticFeedback();
    }

    public string EmptyGridMessage
    {
        get { return emptyGridMessage; }
        set { SetProperty(ref emptyGridMessage, value); }
    }
    protected string emptyGridMessage = Constants.STRING_NO_DATA;

    public string StatusMessage
    {
        get { return statusMessage; }
        set { SetProperty(ref statusMessage, value); }
    }
    protected string statusMessage = string.Empty;


    public virtual bool UseMetric
    {
        get => this._appShellModel.UseMetric;
        set
        {
            this._appShellModel.UseMetric = value;
            OnPropertyChanged("UseMetric");
            OnPropertyChanged("UnitTypeDescriptor");
        }
    }

    public string UnitTypeDescriptor => this.UseMetric ? "Metric" : "English";


    public virtual void Back()
    {
        
    }

    protected IOBDIICommonUI _appShellModel = null;

    public BaseViewModel_AdSupport(IOBDIICommonUI appShell)
    {
        if (appShell == null) throw new NullReferenceException("BaseViewModel_AdSupport..ctor - appShell is null");

        this._appShellModel = appShell;

        var protId = Preferences.Get(Constants.PREFS_KEY_SELECTED_PROTOCOL, (int)0);
        this.SelectedProtocol = this.Protocols.Where(p => p.Id == protId).FirstOrDefault();

        //var l = this._appShellModel.AdService;
        this.AdsAreActive = this.adService.AdsAreActive;
        this.IsBusy = false;

        this._CommTimeoutHandler = this.OnCommTimeout;
        _CommTimer = new System.Threading.Timer(_CommTimeoutHandler,null,Timeout.Infinite, Timeout.Infinite);// new TimerCallback  ///(_CommTimer(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT) { AutoReset = false };

    }

    protected virtual void OnCommTimeout(object sender)
    {

        this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);

    }


    protected virtual void AdClosed(object sender, EventArgs e)
    {

        if (this.adService.PostAdTask != null)
        {
            this.adService.PostAdTask.Start();
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.IsCommunicating = false;
        });

        this.adService.PopupClosed -= this.AdClosed;
        this.adService.PostAdTask = null;

    }

    public List<Protocol> Protocols => OBD2Device.Protocols;

    public bool IsSpecialEdition => this._appShellModel.IsSpecialEdition;// OS.OBDII.UI.Constants.SPECIAL_EDITION;


    public Protocol SelectedProtocol
    {
        get => this._appShellModel.SelectedProtocol;
        set
        {
            this._appShellModel.SelectedProtocol = value;
            this._appShellModel.SelectedProtocolIndex = value.Id;
            OnPropertyChanged("SelectedProtocol");
        }
    }

    public bool IsBusy
    {
        get => this.isBusy;
        set => SetProperty(ref this.isBusy, value);
    }
    private bool isBusy = false;
    /// <summary>
    /// True from beginning-to-end of connection including attempt
    /// to connect.
    /// </summary>

    protected bool IsConnecting = false;

    //       public virtual bool IsCommunicating { get; set; } = false;

    public virtual bool IsCommunicating
    {
        get { return isCommunicating; }
        set
        {
            SetProperty(ref isCommunicating, value);
        }
    }
    private bool isCommunicating = false;

    protected void LEDOn()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            this.DataIsTransmitting = true;
        });
    }

    protected void ActivityLEDOff()
    {
        MainThread.BeginInvokeOnMainThread(() => {
            this.DataIsTransmitting = false;
        });
    }

    /// <summary>
    /// True between send and receive, or timeout during connections
    /// </summary>
    public bool DataIsTransmitting //{ get; set; }
    {
        get => dataIsTransmitting;
        set => SetProperty(ref dataIsTransmitting, value);
    }
    protected bool dataIsTransmitting = false;

    public string Title
    {
        get { return title; }
        set { SetProperty(ref title, value); }
    }
    string title = string.Empty;

    public string LogText
    {
        get { return logText; }
        set { SetProperty(ref logText, value); }
    }
    string logText = string.Empty;
    private object sendLock = new object();
    protected virtual async Task SendRequest(string data)
    {
        LEDOn();
        lock (sendLock)
        {
            this._LastSentCommand = data;
            this._appShellModel.CommunicationService.Send(data);
        }

    }
    protected string _LastSentCommand = string.Empty;


}
