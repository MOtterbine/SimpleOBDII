using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using OS.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers;

namespace OS.OBDII.ViewModels
{
    public class VehicleInfoViewModel : CommViewModel, IViewModel
    {

        public event ViewModelEvent ModelEvent;
        public event RequestPopup NeedYesNoPopup;
        public override bool IsCommunicating 
        {
            get => base.IsCommunicating;
            set
            {
                base.IsCommunicating = value;
            }
        }
        public bool ShowMenu => !IsCommunicating;


        private string vin = string.Empty;
        public string VIN
        {
            get { return vin; }
            set { SetProperty(ref vin, value); }
        }
        public bool IsVinValid
        {
            get { return isVinValid; }
            set { SetProperty(ref isVinValid, value); }
        }
        private bool isVinValid = false;
        public bool IMTestsComplete
        {
            get { return iMTestsComplete; }
            set { SetProperty(ref iMTestsComplete, value); }
        }
        private bool iMTestsComplete = false;

        public string SystemProtocolDescription
        {
            get { return systemProtocolDescription; }
            set { SetProperty(ref systemProtocolDescription, value); }
        }
        private string systemProtocolDescription = string.Empty;

        public string SystemDescription
        {
            get { return systemDescription; }
            set { SetProperty(ref systemDescription, value); }
        }
        private string systemDescription = string.Empty;

        private int dtcCount = 0;
        public int DTCCount
        {
            get { return dtcCount; }
            set { SetProperty(ref dtcCount, value); }
        }

        private string distSinceReset = Constants.STRING_BLANK_LINE;
        public string DistSinceReset
        {
            get { return distSinceReset; }
            set { SetProperty(ref distSinceReset, value); }
        }

        private string distWithDTC = Constants.STRING_BLANK_LINE;
        public string DistWithDTC
        {
            get { return distWithDTC; }
            set { SetProperty(ref distWithDTC, value); }
        }

        private bool showOnlyNotReady = true;
        public bool ShowOnlyNotReady
        {
            get { return showOnlyNotReady; }
            set { SetProperty(ref showOnlyNotReady, value); }
        }


        // VERY IMPORTANT FOR UPDATING A COLLECTION PROPERTY WHEN THE COLLECTION IS MODIFIED
        private ObservableCollection<ReadinessMonitor> readinessMonitors = new ObservableCollection<ReadinessMonitor>();
        public ObservableCollection<ReadinessMonitor> ReadinessMonitors
        {
            get { return readinessMonitors; }
            set { SetProperty(ref readinessMonitors, value); }
        }
        public CollectionView SelectedReadinessMonitors { get; set; } = null;

        IOBDIICommonUI _appShellModel = null;

        public VehicleInfoViewModel(IOBDIICommonUI appShell) : base(appShell)
        {
            if (appShell == null) throw new NullReferenceException("VehicleInfoViewModel..ctor - appShell cannot be null");
            this._appShellModel = appShell;

            Title = "Vehicle Information";

            // Watches the collection for changes/modifications
            this.SelectedReadinessMonitors = new CollectionView
            {
                ItemsSource = this.ReadinessMonitors
            };

            //this.CommunicationService.CommunicationEvent += OnCommunicationEvent;
            this.ResetFields();

            this.ReadinessMonitors = new ObservableCollection<ReadinessMonitor>();
            //{
            //    new ReadinessMonitor() { Description = "Coolaid in the house.", IsCompleted = true, Code = 1 },
            //    new ReadinessMonitor() { Description = "Let's Go Brandon!", IsCompleted = true, Code = 2 },
            //    new ReadinessMonitor() { Description = "Test 1", IsCompleted = true, Code = 3 },
            //    new ReadinessMonitor() { Description = "Airplane Steps", IsCompleted = false, Code = 4 },
            //    new ReadinessMonitor() { Description = "Atilla The Bun", IsCompleted = true, Code = 5 },
            //    new ReadinessMonitor() { Description = "Some Test Never heard of", IsCompleted = false, Code = 6 },
            //    new ReadinessMonitor() { Description = "From each everything to each what you might find", IsCompleted = true, Code = 7 },
            //    new ReadinessMonitor() { Description = "ALfred Newman", IsCompleted = true, Code = 7 },
            //    new ReadinessMonitor() { Description = "Elvis Is Alive", IsCompleted = true, Code = 7 }
            //};

        }

        
        

        public override void CloseCommService()
        {
            this.OpenCloseButtonText = "Open";
            base.CloseCommService();
            OnPropertyChanged("IsCommunicating");
        }

        protected override void OnCommTimeout(object sender)
        {
            base.OnCommTimeout(sender);
            ActivityLEDOff();

            // Retrying
            if (this._RetryCounter++ < Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT)
            {
                ResetCommTimout(false);
                // Resend the last command
                //if (AppShellModel.Instance.CommunicationService.IsConnected)
                //{
                //    //Task.Run(SendRequest($"{base._LastSentCommand}"));
                //    SendRequest($"{base._LastSentCommand}").Start();
                //    prepareGetVehicleStatus().RunSynchronously();
                //}

                //this.prepareGetVehicleStatus().RunSynchronously();

                return;
            }

            // Timed out
            this._RetryCounter = 0;
            this.ErrorExists = true;
                this.CloseCommService();
            this.EmptyGridMessage = this.StatusMessage = Constants.MSG_NO_RESPONSE_VEHICLE;
        }

        StringBuilder sb = new StringBuilder();
        protected override async Task OnCommunicationEvent(object sender, DeviceEventArgs e)
        {
            try
            {
                string nextRequest;
                switch (e.Event)
                {
                    case CommunicationEvents.Information:
                        await Application.Current.Dispatcher.DispatchAsync(() =>  
                        //MainThread.BeginInvokeOnMainThread(() =>
                        {
                            this.StatusMessage = e.Description;
                        });
                        return;
                    case CommunicationEvents.Receive:
                    case CommunicationEvents.ReceiveEnd:
                        ActivityLEDOff();
                        this.rawStringData.Append(Encoding.UTF8.GetString(e.data));

                        // from elm327 datasheet '>' is end of message
                        if (e.data[e.data.Length - 1] != '>')
                        {
                            return;
                        }

                        if (OBD2Device.CheckForDongleMessages(this.rawStringData.ToString(), out nextRequest, true))
                        {
                            this.EmptyGridMessage = this.StatusMessage = nextRequest;
                            this.rawStringData.Clear();
                            this.ErrorExists = true;
                            this.OBD2Adapter.ClearQueue();
                            this.CloseCommService();
                            return;
                        }

                        // Reset RX timeout timer
                        //this._CommTimer.Change(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT, Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT);
                        this.ResetCommTimout();


                        switch (this.OBD2Adapter.CurrentQueueSet)
                        {
                            case QueueSets.GeneralSnapshot:
                                this.OBD2Adapter.ParseResponse(this.rawStringData.ToString());
                                this.rawStringData.Clear();

                                nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                                if (nextRequest == null)
                                {
                                     await Application.Current.Dispatcher.DispatchAsync(() => 
                                    //await MainThread.InvokeOnMainThreadAsync(() =>
                                    {
                                        CancelCommTimout();
                                        this.CloseCommService();
                                    });
                                }
                                else
                                {
                                    await this.SendRequest(nextRequest);
                                    return;
                                }
                                break;
                            case QueueSets.DeviceReset:
                                nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                                if (nextRequest != null)
                                {
                                    this.rawStringData.Clear();
                                    await this.SendRequest(nextRequest);
                                    return;
                                }
                                else
                                {
                                    this.rawStringData.Clear();
                                    if (this.ActionQueue.Count < 1)
                                    {
                                        //this.StatusMessage = "Closing...";
                                        this.CloseCommService();
                                    }
                                    else
                                    {
                                        this.ActionQueue.Dequeue().Start();
                                    }
                                    // Declare device has been through a basic inititialization
                                    //AppShellModel.Instance.DeviceIsInitialized = true;
                                }
                                break;
                        }
                        break;
                    case CommunicationEvents.ConnectedAsClient:
                        this.IsConnecting = false;
                        this.EmptyGridMessage = this.StatusMessage = "Connected";
                        //CancelCommTimout();
                        if (this.ActionQueue.Count > 0)
                        {
                            this.ActionQueue.Dequeue().Start();
                        }


                        break;
                    case CommunicationEvents.Disconnected:
                        CancelCommTimout();
                        this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
                        AppShellModel.Instance.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
                           
                        this.IsCommunicating = false;
                        this.IsBusy = false;
                        this.OpenCloseButtonText = "Open";
                        if (!this.ErrorExists)
                        {
                            this.StatusMessage = string.Empty;
                            //this.EmptyGridMessage = Constants.NO_DATA_STRING;
                            SendHapticFeedback();
                        }
                        ActivityLEDOff();

                        break;
                    case CommunicationEvents.Error:
                        CloseCommService();
                        lock (dLock)
                        {
                            if (!string.IsNullOrEmpty(AppShellModel.Instance.CommunicationService.DeviceName) &&
                                    this.IsConnecting && this.ConnectTimeoutCount++ < Constants.DEFAULT_COMM_CONNECT_RETRY_COUNT)
                            {
                                this.EmptyGridMessage = "Retrying...";
                                this.prepareGetVehicleStatus(false).Start();
                                return;
                            }
                        }
                        ActivityLEDOff();

                        this.ErrorExists = true;
                        this.StatusMessage = e.Description;
                        this.EmptyGridMessage = e.Description;

                        SendHapticFeedback();


                        break;
                    default:
                        break;
                }

                await base.OnCommunicationEvent(sender, e);
            }
            catch(Exception e3)
            {
                await Application.Current.Dispatcher.DispatchAsync(() =>
                //await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.ErrorExists = true;
                    this.ActionQueue.Clear();
                    this.OBD2Adapter.ClearQueue();
                    this.CloseCommService();
                    this.IsBusy = false;
                    this.OpenCloseButtonText = "Open";
                    if (!string.IsNullOrEmpty(e3.Message))
                    {
                        this.StatusMessage = e.Description;
                        this.EmptyGridMessage = e.Description;
                    }
                });
            }
        }





        private Readiness tempReadinessObject = new Readiness();
        private List<int> CodesToRemove = new List<int>();

        protected override void OnAdapterEvent(object sender, OBD2AdapterEventArgs e)
        {
            try 
            {
                switch (e.EventType)
                {
                    case OBD2AdapterEventTypes.Update:
                        try
                        {
                            System.Reflection.PropertyInfo propInf = this.GetType().GetProperty(e.propertyName);
                            if (propInf == null) return;
                            propInf.SetValue(this, e.dataObject);
                            switch (e.propertyName)
                            {
                                case "VIN":
                                        this.IsVinValid = this.VIN.Length == 17;
                                    break;
                            }

                        }
                        catch (Exception)
                        {

                        }

                        break;
                    case OBD2AdapterEventTypes.ReadinessMonitors:

                        try
                        {
                            ReadinessMonitor pidCat = null;
                            //Task.Run(() =>
                            //{
                                // Get the new readiness tests
                                this.tempReadinessObject = (Readiness)e.dataObject;

                                // Go through each new readiness test
                                this.tempReadinessObject.MonitorTestList.ToList().ForEach(newTest =>
                                {
                                    // If the current test code is not in the list, add it
                                    if ((pidCat = this.ReadinessMonitors.Where(oldTest => oldTest.Code == newTest.Code).FirstOrDefault()) == null)
                                    {
                                        // ADDING NEW TEST CODES
                                        if (this.ShowOnlyNotReady)
                                        {
                                            if (!newTest.IsCompleted)
                                            {
                                                Application.Current.Dispatcher.DispatchAsync(() => {
                                                    //MainThread.InvokeOnMainThreadAsync(() => {
                                                    // Show only failing, or 'not ready' tests..
                                                    this.ReadinessMonitors.Add(newTest);
                                                    // keep the code that was just handled as new
                                                    this.CodesToRemove.Remove(newTest.Code);
                                                });
                                            }
                                        }
                                        else
                                        {
                                            // AVAILABLE TESTS - SHOW ONLY WHERE THER IS NO "NOT READY" SIBLING
                                            if (!this.ReadinessMonitors.Any(mt => mt.SiblingCode == newTest.Code && !mt.IsCompleted))
                                            {
                                                if (!this.tempReadinessObject.MonitorTestList.Any(mt => mt.SiblingCode == newTest.Code && !mt.IsCompleted))
                                                {
                                                    Application.Current.Dispatcher.DispatchAsync(()=> { 
                                                    //MainThread.InvokeOnMainThreadAsync(() => {
                                                        // Show all on the list
                                                        this.ReadinessMonitors.Add(newTest);
                                                        // Keep the code that was just handled as new
                                                        this.CodesToRemove.Remove(newTest.Code);
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    else // NEW TEST CODE ALREADY EXISTS THE LIST
                                    {
                                        // UPDATE EXISTING TEST CODES
                                        if (this.ShowOnlyNotReady)
                                        {
                                            // NOT READY TESTS
                                            if (!newTest.IsCompleted)
                                            {
                                                Application.Current.Dispatcher.DispatchAsync(() => {
                                                    //MainThread.InvokeOnMainThreadAsync(() => {
                                                    // keep the code that was just handled as updated
                                                    this.CodesToRemove.Remove(newTest.Code);
                                                });
                                            }
                                        }
                                        else
                                        {
                                            // AVAILABLE TESTS - SHOW ONLY WHERE THER IS NO "NOT READY" SIBLING
                                            if(!this.ReadinessMonitors.Any(mt=>mt.SiblingCode == newTest.Code && !mt.IsCompleted))
                                            {
                                                Application.Current.Dispatcher.DispatchAsync(() => {
                                                    //MainThread.InvokeOnMainThreadAsync(() => {
                                                    // keep the code that was just handled as updated
                                                    this.CodesToRemove.Remove(newTest.Code);
                                                });
                                            }
                                        }
                                    }
                                });

                                this.SystemDescription = this.tempReadinessObject.ToString();

                            //Application.Current.Dispatcher.DispatchAsync(() =>
                            //{
                            //    // PURGE< REMOVE REMAINING CODES THAT WERE NOT UPDATED OR ADDED SINCE LAST CALL(No Information to report)
                            //    this.CodesToRemove.ForEach(code =>
                            //        {
                            //            this.ReadinessMonitors.Remove(this.ReadinessMonitors.Where(rm => rm.Code == code).FirstOrDefault());
                            //        });
                            //});
                            base.InvokeUpdate("ReadinessMonitors");
                                // How many errors is the car reporting...
                                this.DTCCount = this.tempReadinessObject.DTCCount;

                                // remember the codes we have, are now 'old' codes that will be purged next time around if there isn't a new update...
                                this.CodesToRemove = this.ReadinessMonitors.ToList().ConvertAll(mon => mon.Code);

                                this.IMTestsComplete = this.CodesToRemove.Count == 0;

                                this.EmptyGridMessage = "Readiness Tests Complete";

                           // });
                        }
                        catch (Exception ex)
                        {

                        }
                        break;

                    case OBD2AdapterEventTypes.Error:
                        if (this.IsConnecting)
                        {
                           // this.ApplyConnectRetry();
                            return;
                        }
                        this.ActionQueue.Clear();
                        this.OBD2Adapter.ClearQueue();
                        this.CloseCommService();
                        this.ErrorExists = true;
                        this.StatusMessage = this.EmptyGridMessage = e.Description;

                        break;
                }
            }
            catch (Exception e3)
            {
                this.StatusMessage = this.EmptyGridMessage = e3.Message;
            }

        }


        private void ResetFields()
        {
            IsVinValid = false;
            this.VIN = Constants.STRING_BLANK_LINE;
            this.SystemDescription = Constants.STRING_BLANK_LINE;
            this.SystemProtocolDescription = Constants.STRING_BLANK_LINE;
            this.DTCCount = 0;
            this.DistWithDTC = Constants.STRING_BLANK_LINE; 
            this.DistSinceReset = Constants.STRING_BLANK_LINE;
           // MainThread.InvokeOnMainThreadAsync(this.ReadinessMonitors.Clear);
            //this.EmptyGridMessage = String.Empty;
        }

        protected SynchronizationContext syncContext;

        protected override OBD2DeviceAdapter OBD2Adapter { get; } = new OBD2DeviceAdapter();

        public string SelectedCommMethod
        {
            get => AppShellModel.Instance.SelectedCommMethod;
            set { AppShellModel.Instance.SelectedCommMethod = value; }
        }

        public bool IsBluetooth
        {
            get => AppShellModel.Instance.IsBluetooth;
            //set { AppShellModel.Instance.IsBluetooth = value; }
        }

        public int IPPort
        {
            get => AppShellModel.Instance.IPPort;
            set { AppShellModel.Instance.IPPort = value; }
        }

        public string IPAddress
        {
            get => AppShellModel.Instance.IPAddress;
            set { AppShellModel.Instance.IPAddress = value; }
        }

        private string printMessage = string.Empty;
        public string PrintMessage
        {
            get { return printMessage; }
            set { SetProperty(ref printMessage, value); }
        }

        private string runButtonText = "Get Status";
        public string RunButtonText
        {
            get { return runButtonText; }
            set { SetProperty(ref runButtonText, value); }
        }


        private string openCloseButtonText = "Open";
        public string OpenCloseButtonText
        {
            get { return openCloseButtonText; }
            set { SetProperty(ref openCloseButtonText, value); }
        }

        private string startStopButtonText = "Get Data";
        public string StartStopButtonText
        {
            get { return startStopButtonText; }
            set { SetProperty(ref startStopButtonText, value); }
        }

        private string communicationChannel = "<None>";
        public string CommunicationChannel
        {
            get { return communicationChannel; }
            set { SetProperty(ref communicationChannel, value); }
        }

        #region Device Reset

        private Task ResetDevice()
        {
            return new Task(async () => {
                this.IsCommunicating = true;

                this.EmptyGridMessage = this.StatusMessage = "Resetting Device...";
                this.OBD2Adapter.CreateQueue(QueueSets.DeviceReset);
                var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                if (nextRequest != null)
                {
                    // Start 'No Response' timer...
                    this.ResetCommTimout();
                    await this.SendRequest(nextRequest);
                }
            });
        }

        #endregion Device Reset


        #region InitializeDeviceCommand


        private Task InitializeDevice()
        {
            return new Task(async () => {

                // Start 'No Response' timer...
                this.ResetCommTimout();
                this.IsCommunicating = true;
                Application.Current.Dispatcher.Dispatch(this.ReadinessMonitors.Clear);
                //    await MainThread.InvokeOnMainThreadAsync(this.ReadinessMonitors.Clear);

                base.OnPropertyChanged("ReadinessMonitors");

                this.EmptyGridMessage = StatusMessage = "Initializing...";
                this.OBD2Adapter.CreateQueue(QueueSets.Initialize); ;
                var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                if (nextRequest != null)
                {
                    // Start 'No Response' timer...
                    this.ResetCommTimout();
                    await this.SendRequest(nextRequest);
                }
            });
        }

        public ICommand GetVehicleStatusCommand => new Command(async ()=> {

            this.IMTestsComplete = false;
            this.EmptyGridMessage = this.StatusMessage = Constants.STRING_CONNECTING;
            //if (this.adService.DoAdPopup())
            //{

            //    // assign a method to call after the ad
            //    this.adService.PostAdTask = this.prepareGetVehicleStatus();
            //    // the post-ad callback where PostAdAction will be called.
            //    this.adService.PopupClosed += AdClosed;
            //    //await Task.Run(this.prepareGetVehicleStatus);
            //    return;
            //}

            this.IsCommunicating = true;

            //await Task.Factory.StartNew(StartInitDevice);

            this.prepareGetVehicleStatus().Start();
           
        });

        private Task prepareGetVehicleStatus(bool resetRetries = true)
        {
            return new Task(()=>InitStatusCall(resetRetries));

            return new Task(()=>{
                this.IsCommunicating = true;
                Application.Current.Dispatcher.Dispatch(this.ReadinessMonitors.Clear);
              //  MainThread.InvokeOnMainThreadAsync(this.ReadinessMonitors.Clear).Wait();

                base.OnPropertyChanged("ReadinessMonitors");

                //this.EmptyGridMessage = Constants.STRING_NO_DATA;
                //this.EmptyGridMessage = this.StatusMessage = Constants.STRING_CONNECTING;
                this.ErrorExists = false;
                this.IsVinValid = false;

                this.ActionQueue.Clear();
                //this.ActionQueue.Enqueue(ResetDevice());
                this.ActionQueue.Enqueue(InitializeDevice());
                this.ActionQueue.Enqueue(GetVehicleStatus());
                this.ResetFields();
                if (resetRetries)
                {
                    ConnectTimeoutCount = 0;
                }
         
                this.Open();
            });
        }


        private Task GetVehicleStatus()
        {
            return new Task(async () => {
                // Start 'No Response' timer...
                this.IsCommunicating = true;

                this.EmptyGridMessage = this.StatusMessage = "Reading Status...";

                this.OBD2Adapter.CreateQueue(QueueSets.GeneralSnapshot);
                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                if (nextRequest != null)
                {
                    this.ResetCommTimout();
                    await this.SendRequest(nextRequest);
                } 
            });
        }

        #endregion GetVehicleStatusCommand


        #region NavigateHomeCommand

        public ICommand NavigateHomeCommand => new Command(async () => {

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



        #region LookupVINCommand

        public ICommand LookupVINCommand => new Command(async () => {

            //if (this.VIN.Length == 17)
            //{
            //    await Browser.OpenAsync($"https://www.thoughtpill.com/VinLookup/{this.VIN}");
            //}

            if (this.ModelEvent != null)
            {
                this.IsBusy = true;
                using (ViewModelEventArgs evt = new ViewModelEventArgs())
                {
                    evt.EventType = ViewModelEventEventTypes.NavigateTo;
                    evt.dataObject = "VINLookupPage";
                    evt.Data = Encoding.ASCII.GetBytes(this.VIN);
                    this.ModelEvent(this, evt);
                }
            }








        });

        //private async Task LookupVin(string VIN)
        //{
        //    if (this.VIN.Length == 17)
        //    {
        //        await Browser.OpenAsync($"https://www.thoughtpill.com/VinLookup/{this.VIN}");
        //    }
        //}

        #endregion LookupVINCommand


        public void Stop()
        {
            //this.adService.PopupClosed -= AdClosed;
            //this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
            //AppShellModel.Instance.CommunicationService.CommunicationEvent -= OnCommunicationEvent;

            this.CloseCommService();
        }

        public virtual void Start()
        {
            this.DataIsTransmitting = false;
            this.ErrorExists = false;

            this.EmptyGridMessage = Constants.STRING_NO_DATA;
            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_INITIAL_VIEW))
            {
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_INITIAL_VIEW);

                this.Initialize();
            }

            this.IsBusy = false;
        }

        public void Initialize()
        {
            Task.Factory.StartNew(StartInitDevice);
        }

        private void InitStatusCall(bool resetRetries = true)
        {
            Application.Current.Dispatcher.Dispatch(this.ReadinessMonitors.Clear);

            OnPropertyChanged("ReadinessMonitors");

            this.IsCommunicating = true;
            //this.EmptyGridMessage = Constants.STRING_NO_DATA;
            this.EmptyGridMessage = this.StatusMessage = $"Connecting to {AppShellModel.Instance.CommunicationService.DeviceName}";
            this.ErrorExists = false;
            this.IsVinValid = false;
            this.ActionQueue.Clear();

            //this.ActionQueue.Enqueue(ResetDevice());
            this.ActionQueue.Enqueue(InitializeDevice());
            this.ActionQueue.Enqueue(GetVehicleStatus());

            Application.Current.Dispatcher.Dispatch(this.ResetFields);
            if (resetRetries)
            {
                ConnectTimeoutCount = 0;
            }
            this.Open();
        }

        private async Task StartInitDevice()
        {
            InitStatusCall();
        }








        protected StringBuilder rawStringData = new StringBuilder();

        protected void SetStatusText(string text = null)
        {
            if (!String.IsNullOrEmpty(text))
            {
                this.StatusMessage = text;
                return;
            }
            if (AppShellModel.Instance.CommunicationService.IsConnected)
            {
                this.StatusMessage = "** Connected **";
                this.OpenCloseButtonText = "Close";
            }
            else
            {
                this.StatusMessage = "Disconnected";
                this.OpenCloseButtonText = "Open";
            }
        }

    }
}