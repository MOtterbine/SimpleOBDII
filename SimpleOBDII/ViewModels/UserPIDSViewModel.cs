using OS.OBDII.Interfaces;
using OS.Communication;
using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Constants = OS.OBDII.Constants;

namespace OS.OBDII.ViewModels
{


    [QueryProperty(nameof(SelectedCode), "code")]
    [QueryProperty(nameof(NewItem), "new")]
    public class UserPIDSViewModel : CommViewModel, IEditableViewModel
    {
        ManualResetEvent plotArraySync = new ManualResetEvent(true);
        private object _PlotsLock = new object();

        private int CANIDLength = 3;

        private readonly DataPlotModel[] PlotArray = new DataPlotModel[4];

        public event ViewModelEvent ModelEvent;
        public event RequestPopup NeedYesNoPopup;

        protected StringBuilder rawStringData = new StringBuilder();
        protected override OBD2DeviceAdapter OBD2Adapter { get; } = new OBD2DeviceAdapter();

        public int PlotHeight => _appShellModel.PlotHeight;

        void IEditableViewModel.Edit(object editObject)
        {
            var x = editObject as UserPID;
            if (x == null) return;
            // avoid triggering INotify
            pIDToEdit = UserPIDs.FirstOrDefault(p=>p.Code == x.Code);
            if(PIDToEdit == null) return;
            this.ViewPIDDetailsCommand.Execute(null);
        }

        public string SelectedCode
        {
            get => this.selectedCode; 
            set
            {
                SetProperty(ref selectedCode, value);
            }
        }
        private string selectedCode;
        public bool NewItem
        {
            get => this.newItem; 
            set
            {
                    SetProperty(ref newItem, value);
                //MainThread.BeginInvokeOnMainThread(() => {
                //});

                if (this.newItem)
                {
            //        LoadUserPids();
                    if (!string.IsNullOrEmpty(this.selectedCode))
                    {
                        this.PIDToEdit = this.UserPIDs.FirstOrDefault(up => up.Code == uint.Parse(this.selectedCode));
                        if (PIDToEdit == null) return;
                    }
                    // scroll to end of list
                    using (var evt = new ViewModelEventArgs(new byte[] { 1 }) { EventType = ViewModelEventEventTypes.ScrollTo, dataObject = PIDToEdit })
                    {
                        FireModelEvent(evt); // SCROLL TO END
                    }
                }
            }
        }
        private bool newItem;


    //    public bool CanExitPage => !this.IsCommunicating;// && !this.IsPaused;


        public override bool IsCommunicating
        {
            get => base.IsCommunicating;
            set 
            { 
                base.IsCommunicating = value;
                OnPropertyChanged("CanExitPage");
                OnPropertyChanged("CanEditPID");
                OnPropertyChanged("CanAddUserPID");
                OnPropertyChanged("CanRefreshSupportedPIDS");
            }
        }

        protected override void OnAdapterEvent(object sender, OBD2AdapterEventArgs e)
        {

        }

        public ObservableCollection<DataPlotModel> PlotData
        {
            get { return plotData; }
            set { SetProperty(ref plotData, value); }
        }
        private ObservableCollection<DataPlotModel> plotData = null;


        public bool PlottablePidsSelected
        {
            get { return plottablePidsSelected; }
            set { SetProperty(ref plottablePidsSelected, value); }
        }
        private bool plottablePidsSelected = false;


        public bool UsePlots
        {
            get { return usePlots; }
            set 
            {
                SetProperty(ref usePlots, value);
                if (usePlots) this.ResetPlots();
            }
        }
        private bool usePlots = false;

        private void ResetPlots()
        {
            
            this.plotArraySync.Reset();
            lock (_PlotsLock)
            {
                for (int i = 0; i < this.PlotArray.Length; i++)
                {
                    try
                    {
                        this.PlotArray[i].ResetData();
                    }
                    catch (Exception)
                    {
                        //break;
                    }
                }
            }
            this.plotArraySync.Set();
        }

        public bool IsLive
        {
            get { return isLive; }
            set 
            { 
                SetProperty(ref isLive, value);
            }
        }
        private bool isLive = false;

        public string AddDeleteButtonText
        {
            get { return addDeleteButtonText; }
            set { SetProperty(ref addDeleteButtonText, value); }
        }
        private string addDeleteButtonText = "Detail";

        public int DTCCount
        {
            get { return dtcCount; }
            set { SetProperty(ref dtcCount, value); }
        }
        private int dtcCount = 0;

        private bool StopAll = false;
        public ObservableCollection<IPid> UserPIDs
        {
            get => OBD2Device.SystemReport.UserPIDs;
            set 
            { 
                OBD2Device.SystemReport.UserPIDs = value;
                OnPropertyChanged("UserPIDs");
            }
        }

        public ObservableCollection<IPid> SelectedPIDS
        {
            get { return selectedPIDS; }
            set { SetProperty(ref selectedPIDS, value); }
        }
        private ObservableCollection<IPid> selectedPIDS = null;
                                                               
        private object tmpObject = null;
        private static bool PidsHaveBeenInitialized = false;


        private void AdvanceToNextRequest()
        {
            if (this.selectedPIDS.Count < 1) return;
            plotArraySync.WaitOne();

            if (usePlots)
            {
                var done = false;

                do
                {
                    if (++this.LivePIDRequestIndex > this.SelectedPIDS.Count - 1)
                    {
                        this.LivePIDRequestIndex = 0;
                        if (PlotData.Count > 0) this.InvalidatePlots();
                    }
                    if (this.LivePIDRequestIndex < Constants.PLOT_MAX_COUNT) done = true;
                }
                while (!done);
                return;
            }

            do
            {
                if (++this.LivePIDRequestIndex > this.SelectedPIDS.Count - 1)
                {
                    this.LivePIDRequestIndex = 0;
                }
            }
            // Skip while...
            while ((this.selectedPIDS[this.LivePIDRequestIndex].Code & Constants.EXPANDED_CODE_MASK) > 0);


        }

        public UserPIDSViewModel(IOBDIICommonUI appShell) : base(appShell)
        {

            //Preferences.Remove(Constants.APP_PROPERTY_KEY_ADD);
            Preferences.Remove(Constants.USER_PIDS_VM_CODE);

            Title = "User PIDs";
            this.ResetFields();

            var l = new OBD2PIDS();
            this.SelectedPIDS = new ObservableCollection<IPid>();
            this.PlotData = new ObservableCollection<DataPlotModel>();

            this.IsPaused = false;
            this.IsLive = false;
            this.CanStartStop = false;
            this.IsBusy = false;

            for (int i = 0; i < Constants.PLOT_MAX_COUNT; i++)
            {
                this.PlotArray[i] = new DataPlotModel();
            }

        }



        private void LoadUserPids()
        {
            try 
            {

                // limits on free version, if applicable       
                OBD2Device.SystemReport.LoadUserPIDs(0, Constants.FORCE_LIMITS ? Constants.APP_LIMIT_MAX_USER_PID_ROWS : 0);

            }
            catch (Exception)
            {

            }
                

            // If no user pids exist, create the persistent user preference with the default set of pids and save it
            if (OBD2Device.SystemReport.UserPIDs.Count < 1 && !PidsHaveBeenInitialized)
            {
                var sPids = new List<UserPID>()
                {
                    // property 'Code' is used as an index with USER PIDs
                    new UserPID("OBDII Engine Coolant", "°F", 1, Encoding.ASCII.GetBytes("0105"), "((A-40)*1.8)+32")
                        { CANID="7DF", Code=0, ResponseByteCount=1, UnitDescriptor = "°F" },
                    new UserPID("OBDII Engine Coolant (ISO 9141)", "°F", 1, Encoding.ASCII.GetBytes("0105"), "((A-40)*1.8)+32")
                        { CANID="686AF1", Code=0, ResponseByteCount=1, UnitDescriptor = "°F" },
                    new UserPID("OBDII Engine Coolant (ISO 14230)", "°F", 1, Encoding.ASCII.GetBytes("0105"), "((A-40)*1.8)+32")
                        { CANID="C133F1", Code=0, ResponseByteCount=1, UnitDescriptor = "°F" },
                    new UserPID("7E0 Engine RPM", string.Empty, 0, Encoding.ASCII.GetBytes("010C"), "((A*256)+B)/4")
                        { CANID="7E0", Code=0, ResponseByteCount=2, UnitDescriptor = string.Empty  },
                    new UserPID("GM Trans Temp", "°F", 1, Encoding.ASCII.GetBytes("221940"), "((A-40)*1.8)+32")
                        { CANID="7E2", Code=0, ResponseByteCount=1, UnitDescriptor = "°F"  },
                    new UserPID("Powertrain Voltage", "V", 2, Encoding.ASCII.GetBytes("0142"), "((A*256)+B)*.001")
                        { CANID="7E0", Code=0, ResponseByteCount=2, UnitDescriptor = "V"  },
                    new UserPID("GM Trans Voltage", "V", 2, Encoding.ASCII.GetBytes("0142"), "((A*256)+B)*.001")
                        { CANID="7E2", Code=4, ResponseByteCount=2, UnitDescriptor = "V"  },
                    new UserPID("OBDII Throttle Position", "%", 1, Encoding.ASCII.GetBytes("0111"), "(A*100)/256")
                        { CANID="7DF", Code=5, ResponseByteCount=1, UnitDescriptor = "%"  },
                    new UserPID("GM Oil Life", "%", 1, Encoding.ASCII.GetBytes("1A6D"), "A*100/256")
                        {CANID="7E0", Code=7, ResponseByteCount=1, UnitDescriptor = "%"  }
                };
                OBD2Device.SystemReport.SaveUserPIDs(sPids);
                OBD2Device.SystemReport.LoadUserPIDs(0, Constants.FORCE_LIMITS?Constants.APP_LIMIT_MAX_USER_PID_ROWS:0);

            }


            ///TEST******************
           // this.UserPIDs.Clear();

            foreach (UserPID pid in this.UserPIDs)
            {
                pid.PropertyChanged += OnSelectedPIDsChanged;
            }

            this.EmptyGridMessage = "No User PIDs";

        }


        protected override void OnCommTimeout(object sender)
        {
            this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);

            base.OnCommTimeout(sender);
            ActivityLEDOff();
            // Paused
            if (this.IsPaused)
            {
                this._RetryCounter = 0;
                return;
            }

            if (this._RetryCounter++ < Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT)
            {
                this._CommTimer.Change(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT, Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT);
                // Resend the last command
                if (_appShellModel.CommunicationService.IsConnected)
                {
                    switch (this.OBD2Adapter.CurrentQueueSet)
                    {
                        case QueueSets.Live:
                            // if (++this.LivePIDRequestIndex > this.UserPIDs.Count - 1) this.LivePIDRequestIndex = 0;
                            //this.SendLivePIDRequest();
                            return;
                        default:
                            // Task.Run(SendRequest($"{base._LastSentCommand}").Start);
                           // SendRequest($"{base._LastSentCommand}");
                            return;
                    }

                }

                return;
            }


            // Timed out
            this._RetryCounter = 0;
            this.LivePIDRequestIndex = 0;
            this.ErrorExists = true;
            this.StopAll = true;
            this.CloseCommService();
            this.EmptyGridMessage = this.StatusMessage = Constants.MSG_NO_RESPONSE_VEHICLE;
            OnSelectedPIDsChanged(this, null);
            this.RunButtonText = Constants.STRING_START;

        }

        private void StopLive()
        {
            // ensure we are not blocking anything
            this.plotArraySync.Set();

            this.StopAll = true;
            if (this.IsPaused) // done like to avoid triggering the binding
            {
                this.IsPaused = false;
            }
            this.CloseCommService();
            OnSelectedPIDsChanged(null, null);
        }


        public virtual void Start()
        {
            //this.IsBusy = false;
            //return;
            uint selCode = 0;
            OnPropertyChanged("PlotHeight");
            OnPropertyChanged("PlotData");
            this.ErrorExists = false;
            this.DataIsTransmitting = false;
            //if (Preferences.ContainsKey(Constants.USER_PIDS_VM_CODE))
            //{
            //    // must be set before NewItem
            //    selCode = (uint)Preferences.Get(Constants.USER_PIDS_VM_CODE, (int)0);
            //    this.selectedCode = selCode.ToString();
            //    // Clear the flag
            //    Preferences.Remove(Constants.USER_PIDS_VM_CODE);
            //}

            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_INITIAL_VIEW))
            {
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_INITIAL_VIEW);
                if (this.UserPIDs.Count < 1)
                {
                    this.PlottablePidsSelected = false;
                    this.UsePlots = false;
                    this.Initialize();
                    this.IsBusy = false;
                    return;
                }
            }



            //this.UserPIDs.Clear();
            OBD2Device.SystemReport.LoadUserPIDs(0, Constants.FORCE_LIMITS ? Constants.APP_LIMIT_MAX_USER_PID_ROWS : 0);
            // subscribe to changes on list items - listen for check box changes
            foreach (IPid pid in this.UserPIDs)
            {
                pid.PropertyChanged += this.OnSelectedPIDsChanged;
            }

            this.IsBusy = false;


            OnSelectedPIDsChanged(null, null);


            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_ADD))
            {
                this.NewItem = (bool)Preferences.Get(Constants.APP_PROPERTY_KEY_ADD, false);
                using (var evt = new ViewModelEventArgs(new byte[] { 1 }) { EventType = ViewModelEventEventTypes.ScrollTo, dataObject = UserPIDs.Last() })
                {
                    FireModelEvent(evt); // SCROLL TO END
                }
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_ADD);
            }

        }

        public void Stop()
        {
            if (IsCommunicating && !IsLive)
            {
                return;
            }
            if(!IsLive)
            {
                // subscribe to changes on list items - listen for check box changes
                foreach (IPid pid in this.UserPIDs)
                {
                    pid.PropertyChanged -= this.OnSelectedPIDsChanged;
                }
                this.UserPIDs.Clear();

            }
            this.Back();
        }


        #region BackCommand

        public ICommand BackCommand => new Command(Stop);

        #endregion BackCommand



        public override void Back()
        {
            CancelCommTimout();
            this.CanStartStop = false;
            if (this.IsLive)
            {
                this.RunButtonText = Constants.STRING_START;
                this.StopLive();

#if WINDOWS
                this.UsePlots = false;
                // this is covering a bug in windows where the plot won't run properly
                // after the initial start if UsePlots = true at the time listening is started
#endif
                return;
            }

            this.CloseCommService();

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

        }



        public void Initialize()
        {
          //  CloseCommService();
            this.LoadUserPids();
        }

        public override void CloseCommService()
        {
            base.CloseCommService();

            this.RunButtonText = Constants.STRING_START;
            this.Negotiating = false;
            this.IsPaused = false;
            this.CanStop = false;
            this.IsLive = false;
            StatusMessage = string.Empty;
            GC.Collect();
        }


        private void FireModelEvent(ViewModelEventArgs evt)
        {
            if(this.ModelEvent != null)
            {
                this.ModelEvent(this, evt);
            }
        }

        private void OnSelectedPIDsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.PlottablePidsSelected = this.UserPIDs.Any(p => p.IsSelected && p.CanPlot);
            this.CanStartStop = this.UserPIDs.Any(p=>p.IsSelected);
            OnPropertyChanged("CanRefreshSupportedPIDS");
        }
        private StringBuilder sndBuilder = new StringBuilder();
        private string currentECUAddress = Constants.DEFAULT_DIAG_FUNCADDR_CAN_ID_11.ToString("X");
        int plotx = Constants.PLOT_POINT_WIDTH-1;
        //private DataPlotModel currentDataPlotModel = null;
        private StringBuilder sbu = new StringBuilder();
        private async Task SendLivePIDRequest()
        {
            try {
                if (this.LivePIDRequestIndex > this.selectedPIDS.Count - 1)
                {
                    return;
                }

                sbu.Clear();// = new StringBuilder();
                rawStringData.Clear();
                //bool lastPIDWasBroadcast = tmpPid == null ? false : tmpPid.IsBroadcast;

                // reassign tmpPid with the next pid
                tmpPid = SelectedPIDS[LivePIDRequestIndex];

                // do we need to change the ecu address?
                // If so, then send a set header command and return
                if (this.currentECUAddress.CompareTo(tmpPid.CANID) != 0)
                {

                    // reassign tmpPid with the next pid
               //     tmpPid = this.SelectedPIDS[this.LivePIDRequestIndex];

                    OBD2Adapter.CurrentRequest = DeviceRequestType.SetHeader;
                    this.currentECUAddress = tmpPid.CANID;//.Substring(2);

                    if (this.currentECUAddress.Length > 6)
                    {
                        var data1 = tmpPid.CANID.Substring(this.currentECUAddress.Length - 6).ToString();
                        sbu.Append($"ATSH{data1}{Constants.CARRIAGE_RETURN}");
                    }
                    else
                    {
                        var data1 = tmpPid.CANID.PadLeft(6, '0').ToString();
                        sbu.Append($"ATSH{data1}{Constants.CARRIAGE_RETURN}");
                    }

                    await this.SendRequest(sbu.ToString());

                    return;
                }
            }
                catch( Exception e)
            {
            }
            this.OBD2Adapter.CurrentLivePIDRequest = (int)tmpPid.Code;
            if(tmpPid.QueryBytes == null || tmpPid.QueryBytes.Length < 2)
            {
                return;
            }

            // put together the next PID request
            tmpPid.QueryBytes.ToList().ForEach(qb=> {
                sbu.Append((char)qb);
            });
            await this.SendRequest($"{sbu.ToString()}{tmpPid.ResponseByteCount}{Constants.CARRIAGE_RETURN}");
         //   sbu.Append($"{tmpPid.ResponseByteCount}{Constants.CARRIAGE_RETURN}");
        //    var jack = sbu.ToString();
          //  await this.SendRequest(sbu.ToString());
        }

        private async Task SendECURequest()
        {

            tmpPid = this.SelectedPIDS[this.LivePIDRequestIndex];
            this.OBD2Adapter.CurrentLivePIDRequest = (int)tmpPid.Code;
            sndBuilder.Clear();
            if (tmpPid.QueryBytes == null || tmpPid.QueryBytes.Length < 2)
            {
                //this.StatusMessage = "Can't send an empty or invalid command";
                //this.CloseCommService();
                //this.ErrorExists = true;
                return;
            }
            // put together the next PID request
            tmpPid.QueryBytes.ToList().ForEach(qb => {
                sndBuilder.Append((char)qb);
            });
            sndBuilder.Append($"{tmpPid.ResponseByteCount}{Constants.CARRIAGE_RETURN}");
            await this.SendRequest(sndBuilder.ToString());
        }

        private bool liveModeInitialized = false;

        private char[] dataEndTrimChars = new char[] { '\n', '\r', '>' };

        private int LivePIDRequestIndex = 0;
        private IPid tmpPid = null; // for live data
        object tempVal = null;
        private string tempString = string.Empty;
        IPid p = null;
        char[] tmpChArray = null;
        string[] inputStringArray = null;
        StringBuilder sb = new StringBuilder();
        protected override async Task OnCommunicationEvent(object sender, DeviceEventArgs e)
        {
            tmpPid = null; // for live data
            tempVal = null;
            tmpChArray = null;
            inputStringArray = null;

            try 
            { 
                string nextRequest;
                switch (e.Event)
                {
                    case CommunicationEvents.Receive:
                    case CommunicationEvents.ReceiveEnd:
                        ActivityLEDOff();

                        if (this.StopAll)
                        {
                            CancelCommTimout();
                            //LEDOff();
                            this.OBD2Adapter.ClearQueue();
                            this.IsPaused = false;
                            this.CanStop = false;
                            this.CanStartStop = true;
                            this.CloseCommService();
                            return;
                        }

                        this.rawStringData.Append(Encoding.UTF8.GetString(e.data));
                        if (OBD2Adapter.CurrentRequest == DeviceRequestType.MonitorAll ||
                            OBD2Adapter.CurrentRequest == DeviceRequestType.SetHeader ||
                            OBD2Adapter.CurrentRequest == DeviceRequestType.SetCANRxAddress)
                        {
                            if (OBD2Adapter.CurrentRequest == DeviceRequestType.SetHeader ||
                            OBD2Adapter.CurrentRequest == DeviceRequestType.SetCANRxAddress)
                            {
                                if (e.data[e.data.Length - 1] != '>')
                                {
                                    return;
                                }
                            }
                        }

                        // from elm327 datasheet '>' is end of message
                        else
                        {
                            if (e.data[e.data.Length - 1] != '>')
                            {
                                return;
                            }
                        }
                       // ResetCommTimout();

                        switch (this.OBD2Adapter.CurrentQueueSet)
                        {
                            case QueueSets.Live:
                              //  ResetCommTimout();
                                if (this.IsPaused)
                                {
                                    CancelCommTimout();
                                    this.LivePIDRequestIndex = 0;
                                    this.rawStringData.Clear();
                                    // timeout callback interrogates IsPaused   
                                    return;
                                }

                                switch (OBD2Adapter.CurrentRequest)
                                {

                                    case DeviceRequestType.SetHeader: // Setting up for which CPU address to send to

                                        // Reset RX timeout timer
                                        this.ResetCommTimout();
                                        sb.Clear();

                                        // special case for uppermost 5 bits of 29-bit CAN id
                                        if (this.currentECUAddress.Length > 6)
                                        {
                                            sb.Append($"ATCP{this.currentECUAddress.Substring(0, 2).PadLeft(2, '0')}");
                                            sb.Append(Constants.CARRIAGE_RETURN);
                                            OBD2Adapter.CurrentRequest = DeviceRequestType.SetCANPriorityBits;
                                            await this.SendRequest(this.sb.ToString());
                                            return;
                                        }
                                        //LEDOff();

                                        OBD2Adapter.CurrentRequest = DeviceRequestType.None;

                                        this.SendLivePIDRequest();
                                        return;

                                    case DeviceRequestType.SetCANRxAddress:
                                        this.sb.Clear();
                                        this.currentECUAddress = tmpPid.CANID;
                                        // monitor command
                                        sndBuilder.Clear();
                                        OBD2Adapter.CurrentRequest = DeviceRequestType.MonitorAll;
                                        // Monitor All
                                        sndBuilder.Append($"ATMA{Constants.CARRIAGE_RETURN}");
                                        await this.SendRequest(sndBuilder.ToString());
                                        // Thread.Sleep(500);
                                        break;
                                    case DeviceRequestType.SetCANPriorityBits:

                                        // Reset RX timeout timer
                                        this.ResetCommTimout();
                                        OBD2Adapter.CurrentRequest = DeviceRequestType.None;

                                        break;
                                    default:
                                        inputStringArray = System.Text.RegularExpressions.Regex.Replace(rawStringData.ToString(), @"([^']{,10}|\r|\n|>)", "\r").Split('\r');

                                        foreach (string inputString in inputStringArray)
                                        {
                                            try
                                            {
                                                // ELM327 Headers should be off - should know the CAN id we are talking to
                                                //if (inputString.Length < 11) continue;
                                                if (inputString.Length < 6) continue; // no, or bad, data - 
                                                if (inputString.CompareTo("STOPPED") == 0) continue; // no, or bad, data - 
                                                if (inputString.CompareTo("SEARCHING...") == 0) continue; // no, or bad, data - 
                                                this.tmpChArray = inputString.ToCharArray();

                                                p = this.SelectedPIDS[this.LivePIDRequestIndex];

                                                tempVal = 0x00;

                                                p.Parse(inputString.Substring(inputString.Length - (p.ResponseByteCount * 2)));

                                                // Parse the data from end of the string - based on the expected response count
                                                if (this.UsePlots && p.CanPlot)// && this.LivePIDRequestIndex< Constants.PLOT_MAX_COUNT)
                                                {
                                                    plotArraySync.WaitOne();
                                                    //lock (_PlotsLock)
                                                    {
                                                        plotArraySync.Reset();
                                                        for(int i=0;i<PlotData.Count;i++)
                                                        {
                                                            if(PlotData[i].Id == p.Code)
                                                            {
                                                                PlotData[i].AddDataPoint(Convert.ToDouble(p.Value));
                                                                PlotData[i].Title = p.OutputString;
                                                                break;
                                                            }
                                                        }
                                                        plotArraySync.Set();
                                                    }
                                                }

                                                // Reset RX timeout timer
                                                this.ResetCommTimout();


                                                if (!IsLive)
                                                {
                                                    //Console.WriteLine("IsLive ........");
                                                    this.IsLive = true;
                                                    this.StatusMessage = "Live...";
                                                    this.MonitorGridTitle = "Live";
                                                    this.CanStartStop = true;
                                                }

                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                p.Value = "----";
                                                //this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = null });
                                                //var kk = ex.Message;
                                                //return;
                                            }
                                        }

                                        this.AdvanceToNextRequest();

                                        break;
                                }

                                this.rawStringData.Clear();


                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    this.IsBusy = false;
                                    this.Negotiating = false;
                                });

                                await this.SendLivePIDRequest();

                                return;
                            case QueueSets.DeviceReset:
                                // Reset RX timeout timer
                                this.ResetCommTimout();

                                break;
                            case QueueSets.Initialize:
                            case QueueSets.InitializeForUserPIDS:
                                switch (this.OBD2Adapter.CurrentRequest)
                                {
                                    case DeviceRequestType.SetCANPriorityBits:
                                        StartListening().Start();
                                        return;
                                    case DeviceRequestType.SetHeader:

                                        if (_appShellModel.UseHeader)
                                        {
                                            // Set the header...
                                            await this.SendRequest($"ATSH{_appShellModel.UserCANID}{Constants.CARRIAGE_RETURN}");
                                            break;
                                        }
                                        else
                                        {
                                            //    await this.SendRequest($"{nextRequest}{this.currentECUAddress}{Constants.CARRIAGE_RETURN}");
                                            //    break;
                                        }
                                        StartListening().Start();
                                        return;
                                    case DeviceRequestType.ForgetEvents:
                                        break;

                                }

                                nextRequest = this.OBD2Adapter.GetNextQueuedCommand(true, false);
                                this.rawStringData.Clear();
                                if (nextRequest != null)
                                {

                                    switch (this.OBD2Adapter.CurrentRequest)
                                    {
                                        case DeviceRequestType.SetProtocol:
                                            // Set the selected protocol...
                                            await this.SendRequest($"{nextRequest}{this.SelectedProtocol.Id:X}{Constants.CARRIAGE_RETURN}");
                                            break;
                                        case DeviceRequestType.SetHeader: // MUST BE LAST COMMAND....


                                            // Reset RX timeout timer
                                            //this.ResetCommTimout();

                                            // special case for uppermost 5 bits of 29-bit CAN id
                                            if (this.currentECUAddress.Length > 6)
                                            {
                                                sb.Clear();
                                                sb.Append($"ATCP{this.currentECUAddress.Substring(0, 2).PadLeft(2, '0')}");
                                                sb.Append(Constants.CARRIAGE_RETURN);
                                                OBD2Adapter.CurrentRequest = DeviceRequestType.SetCANPriorityBits;
                                                await this.SendRequest(this.sb.ToString());
                                                return;
                                            }
                                            //if (_appShellModel.UserCANID.Length > 6)
                                            //{
                                            //    var data1 = _appShellModel.UserCANID.Substring(_appShellModel.UserCANID.Length - 6).ToString();
                                            //    await this.SendRequest($"{nextRequest}{data1}{Constants.CARRIAGE_RETURN}");
                                            //}
                                            //else
                                            //{
                                            //    var data1 = _appShellModel.UserCANID.PadLeft(6, '0').ToString();
                                            //    await this.SendRequest($"{nextRequest}{data1}{Constants.CARRIAGE_RETURN}");
                                            //}
                                            //LEDOff();

                                            // Once the header has been set, send the data
                                            this.sb.Clear();

                                            OBD2Adapter.CurrentRequest = DeviceRequestType.None;



                                            break;
                                        case DeviceRequestType.SET_Timeout:
                                            await this.SendRequest($"{nextRequest}80{Constants.CARRIAGE_RETURN}");
                                            break;
                                        default:
                                            //queueIndexed = true;
                                            await this.SendRequest($"{nextRequest}{Constants.CARRIAGE_RETURN}");
                                            break;
                                    }
                                    return;
                                }
                                StartListening().Start();

                                break;
                            }
                            break;
                        case CommunicationEvents.ConnectedAsClient:
                            this.CanStop = this.IsCommunicating = true;
                            this.StatusMessage = "Connected...";
                            this.EmptyGridMessage = "Getting Data...";
                            if(this.ActionQueue.Count > 0)
                            {
                                this.ActionQueue.Dequeue().Start();
                            }
                            else
                            {
                                CancelCommTimout();
                                this.RunButtonText = Constants.STRING_START;
                            }
                            break;
                        case CommunicationEvents.Disconnected:
                            // Reset RX timeout timer
                            CancelCommTimout();
                            _appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
                           // MainThread.BeginInvokeOnMainThread(() =>
                          //  {
                                this.RunButtonText = Constants.STRING_START;
                                this.IsBusy = false;
                                this.Negotiating = false;
                                this.IsCommunicating = false;
                                ActivityLEDOff();
                                this.IsPaused = false;
                                this.CanStop = false;
                                //this.CanStartStop = false;
                                this.IsLive = false;

                                if (!this.ErrorExists)
                                {
                                    this.StatusMessage = "Ready";
                                    SendHapticFeedback();
                                }

                        OnSelectedPIDsChanged(null, null);

                            break;
                        case CommunicationEvents.Error:
                            CancelCommTimout();
                            CloseCommService();
                            lock (dLock)
                            {
                                if (this.IsConnecting && this.ConnectTimeoutCount++ < Constants.DEFAULT_COMM_CONNECT_RETRY_COUNT)
                                {
                                    this.EmptyGridMessage = "Retrying...";
                                    this.IsCommunicating = true;
                                    ResetCommTimout();
                                    Task.Factory.StartNew(() => this.Open()).ConfigureAwait(false);
                                    return;
                                }
                            }
                            this.Negotiating = false;
                            this.ErrorExists = true;
                            this.IsBusy = false;
                            this.IsPaused = false;
                            this.IsCommunicating = false;
                            ActivityLEDOff();
                            this.RunButtonText = Constants.STRING_START;
                            this.CanStop = false;
                            //this.CanStartStop = true;
                            this.StatusMessage = e.Description;
                            this.IsLive = false;
                            SendHapticFeedback();


                        //   {
                        //if(!_appShellModel.CommunicationService.IsConnected)
                        // {
                        //    _appShellModel.CommunicationService.CommunicationEvent -= this.OnCommunicationEvent;
                        // }

                        OnSelectedPIDsChanged(null, null);

                            if (!string.IsNullOrEmpty(e.Description))
                            {
                                this.StatusMessage = e.Description;
                                this.EmptyGridMessage = e.Description;
                            }

                            //var j = this.CanExitPage;
                            //OnPropertyChanged("CanExitPage");
                            break;
                        default:
                            break;
                    }
            }
            catch (Exception)
            {

            }
            finally
            {
            }
        }

        #region Device Reset

        private Task ResetDevice()
        {
            return new Task( () => {
                this.StatusMessage = this.EmptyGridMessage = "Resetting device...";
                this.OBD2Adapter.CreateQueue(QueueSets.DeviceReset);
                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                if (nextRequest != null)
                {
                    this.SendRequest(nextRequest);
                }
            });
        }

        #endregion Device Reset



        #region ClearSelectionsCommand

        public ICommand ClearSelectionsCommand => new Command(() =>
        {
            this.ClearSelections();
        }, () => CanStartStop);

        private void ClearSelections()
        {
            this.UserPIDs.ToList().ForEach(p => p.IsSelected = false) ;

        }

        #endregion ClearSelectionsCommand


        #region PauseCommand

        public ICommand PauseCommand => new Command(() =>
        {
            this.liveModeInitialized = false;
            if(this.IsPaused)
            {
                //this.OBD2Adapter.CurrentQueueSet = QueueSets.Live;
                this.SendLivePIDRequest().ConfigureAwait(false);
                this.IsCommunicating = true;
                this.PauseButtonText = Constants.STRING_PAUSE;
                this.IsPaused = false;
                this.StatusMessage = "Live...";
                // Restart 'No Response' timer...
                this.ResetCommTimout();
                return;
            }
            this.IsPaused = true;
            // Pause 'No Response' timer...
            //this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this.IsCommunicating = false;
            ActivityLEDOff();
            //this.OBD2Adapter.CurrentQueueSet = QueueSets.None;
            this.PauseButtonText = Constants.STRING_START;
            this.StatusMessage = "Paused...";

            this.LivePIDRequestIndex = 0;

        });

        private bool IsPaused = false;

        #endregion PauseCommand


        #region StartStopCommand

        public ICommand StartStopCommand => new Command(() => {


            this.CanStartStop = false;
            OnPropertyChanged("PlottablePidsSelected");
            OnPropertyChanged("PlotData");
            OnPropertyChanged("");

            //   await Task.Run(() =>
            //   {

            if (_appShellModel.CommunicationService.IsConnected || this.IsLive)// || this.IsConnecting)
                {
                    //this.Stop();
                    if (this.IsLive)
                    {

                        this.Back();
                    }
                    else
                    {
                        Stop(); // calls back
                    }
                    return;
                }

                this.LivePIDRequestIndex = 0;
                this.liveModeInitialized = false;
                this.IsPaused = false;
                this.IsCommunicating = true;
                this.StatusMessage = Constants.STRING_CONNECTING;
                this.ErrorExists = false;
                this.StopAll = false;
                //this.RunButtonText = Constants.STRING_BACK;
                //this.PauseButtonText = Constants.STRING_PAUSE;

                if (this.adService.DoAdPopup())
                {
                    // assign a method to call after the ad
                    this.adService.PostAdTask = this.StartMonitors();
                    // the post-ad callback where PostAdAction will be called.
                    this.adService.PopupClosed += AdClosed;
                    return;
                }

                // Start 'No Response' timer...
                this.ResetCommTimout();
                //var j = this.StartMonitors();//.Start();//.ConfigureAwait(false);   
                //j.ConfigureAwait(false);
                //j.Start();
               // await StartMonitors();
                Task.Run(this.StartMonitors); ;

         //   });

        }, () => CanStartStop);

        private async Task StartMonitors()
        {
            this.ActionQueue.Clear();
            // these next two are done (should be done) when the page loads
            //this.ActionQueue.Enqueue(ResetDevice());
            this.ActionQueue.Enqueue(InitializeDevice());
            //this.ActionQueue.Enqueue(StartListening());
            //this.ActionQueue.Enqueue(StartListening());
            this._RetryCounter= 0;
            ConnectTimeoutCount = 0;
            this.Open();
            Negotiating = false;
        }

        #endregion StartStopCommand




        private bool IsAddingPID = false;

        public ICommand AddPIDCommand => new Command(async () => {

            this.IsAddingPID = true;

            if(Constants.FORCE_LIMITS)
            {
                if(this.UserPIDs.Count >= Constants.APP_LIMIT_MAX_USER_PID_ROWS)
                {

                    if (this.NeedYesNoPopup == null)
                    {
                        this.StatusMessage = "Max User Pids Reached";
                        return;
                    }
                    await NeedYesNoPopup("Create New PID", $"This version is limited to {Constants.APP_LIMIT_MAX_USER_PID_ROWS} user-defined pids", false);

                    return;
                }
            }

            UserPID newPid = null;

            if(this.UserPIDs.Count < 1)
            {
                newPid = new UserPID("New PID", string.Empty, 0, new byte[] { }, "(A*256)+B") { Code = 0, ResponseByteCount = 1 };
            }
            else
            {
                newPid = new UserPID("New PID", string.Empty, 0, new byte[] { }, "(A*256)+B") { Code = this.UserPIDs.Max(p => p.Code) + 1, ResponseByteCount = 1 };
            }

            _appShellModel.tempIPid = newPid;


            Preferences.Set(Constants.APP_PROPERTY_KEY_ADD, true);
            Preferences.Set(Constants.APP_PROPERTY_KEY_EDIT, true);

            if (this.ModelEvent != null)
            {
                this.IsBusy = true;
                using (ViewModelEventArgs evt = new ViewModelEventArgs())
                {
                    evt.EventType = ViewModelEventEventTypes.NavigateTo;
                    evt.dataObject = "PIDDetailsPage";
                    this.ModelEvent(this, evt);
                }
            }

            //var qu = $"PIDDetail?add=true&edit=true";
            //await Shell.Current.GoToAsync(qu, false);

        });

        public ICommand RemovePIDsCommand => new Command(async () => {

            if (this.NeedYesNoPopup == null) return;
            var answer = await NeedYesNoPopup("Delete PIDS", "Delete All Checked User PIDs?");
            if (!answer)
            {
                return;
            }
            if (this.UserPIDs == null || !this.UserPIDs.Any(p => p.IsSelected)) return;
            try
            {
                // await Task.Run(() => {
                this.IsBusy = true;
                // remove list items' subscriptions as the list will be reloaded
                foreach (IPid pid in this.UserPIDs)
                {
                    pid.PropertyChanged -= this.OnSelectedPIDsChanged;
                }


                // generate a list of ids, then delete the pids by id
                this.UserPIDs.Where(p => p.IsSelected).ToList().ConvertAll(p => p.Code).ForEach(a =>
                {
                    var spid = this.UserPIDs.FirstOrDefault(pid => pid.Code == a);
                    if (spid != null)
                    {
                        if(this.PIDToEdit != null && this.PIDToEdit.Code == spid.Code)
                        {
                            this.PIDToEdit = null;
                        }
                        this.UserPIDs.Remove(spid);
                    }
                });
            }
            catch (Exception) { }
            finally
            {

                // re-assign index values to 'Code' field..
                uint i = 0;
                foreach (UserPID pid in UserPIDs)
                {
                    pid.Code = i++;
                }
                OBD2Device.SystemReport.SaveUserPIDs(this.UserPIDs.ToList().ConvertAll(p=>p as UserPID));
                this.UserPIDs.Clear();
                LoadUserPids();
                // reassign list items' subscriptions
                 foreach (IPid pid in this.UserPIDs)
                {
                    pid.PropertyChanged += this.OnSelectedPIDsChanged;
                }

                this.OnSelectedPIDsChanged(null, null);
                this.IsBusy = false;

            }

            //  });


        }, ()=>canStartStop);


        #region AddDeleteCommand

        public ICommand ViewPIDDetailsCommand => new Command(async () => {

            if (this.PIDToEdit == null) return;
            this.IsBusy = true;
            _appShellModel.tempIPid = this.PIDToEdit;

            // remove list items' subscriptions as the list will be reloaded
            //foreach (IPid pid in this.UserPIDs)
            //{
            //    pid.PropertyChanged -= this.OnSelectedPIDsChanged;
            //}

            Preferences.Set(Constants.APP_PROPERTY_KEY_ADD, false);
            Preferences.Set(Constants.APP_PROPERTY_KEY_EDIT, false);

            if (this.ModelEvent != null)
            {
                this.IsBusy = true;
                using (ViewModelEventArgs evt = new ViewModelEventArgs())
                {
                    evt.EventType = ViewModelEventEventTypes.NavigateTo;
                    evt.dataObject = "PIDDetailsPage";
                    this.ModelEvent(this, evt);
                }
            }

        }, ()=> CanEditPID);

        private Task AddDelete()
        {
            return new Task(() =>
            {
                this.NeedYesNoPopup("NewPID", "Create New PID", false);
            });
        }

        #endregion AddDeleteCommand

        private IEnumerable<IGrouping<string, IPid>> SelectedPIDs1 = null;

        protected Task StartListening()
        {
            return new Task(async () => {

                int plotIndex = 0;
                this.LivePIDRequestIndex = 0;

                this.ResetFields();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStop = false;
                this.CanStartStop = false;

                await MainThread.InvokeOnMainThreadAsync(() => {
                    this.SelectedPIDS.Clear();
                    this.PlotData.Clear();
                });
                // GC.Collect();

                // Prevent collectionchanged event
                foreach (UserPID pid in this.UserPIDs)
                {
                    pid.PropertyChanged -= OnSelectedPIDsChanged;
                }

                // Pickup the Selected PIDs, group by CANID (ecu address)
                this.SelectedPIDs1 = this.UserPIDs.Where(p => p.IsSelected == true).GroupBy(p => p.CANID);
                char[] emptyBytes = new char[4] { '0','0','0','0'};
                await MainThread.InvokeOnMainThreadAsync(() => {
                    this.SelectedPIDs1.ToList().ForEach(ecu => {
                        ecu.ToList().ForEach(pid => {
                            ((UserPID)pid).Parse(emptyBytes); // set to non value
                            this.SelectedPIDS.Add(pid);
                            if (plotIndex < Constants.PLOT_MAX_COUNT)
                            {
                                //plotArraySync.Reset();
                                //lock (_PlotsLock)
                                {
                                   // MainThread.BeginInvokeOnMainThread(() => { 
                                    this.PlotArray[plotIndex].Id = pid.Code;
                                    this.PlotArray[plotIndex].Title = pid.Name;
                                    this.PlotArray[plotIndex].PlotLabel = pid.Name;
                                    this.PlotArray[plotIndex].ResetData();
                                    this.PlotData.Add(this.PlotArray[plotIndex]);
                                   // });
                                    plotIndex++;
                                }
                                //plotArraySync.Set();
                            }

                        });
                    });
                });
                // Restore collectionchanged event
                foreach (UserPID pid in this.UserPIDs)
                {
                    pid.PropertyChanged += OnSelectedPIDsChanged;
                }


                this.OBD2Adapter.CurrentQueueSet = QueueSets.Live;
                this.RunButtonText = Constants.STRING_STOP;
                this.PauseButtonText = Constants.STRING_PAUSE;

                await this.SendLivePIDRequest();

                this.StatusMessage = Constants.STRING_ESTABLISHING_LINK;
                this.MonitorGridTitle = Constants.STRING_ESTABLISHING_LINK;

            });
        }


        private void InvalidatePlots()
        {
            Task.Factory.StartNew(()=>{
                if (!UsePlots) return;
                this.plotArraySync.WaitOne();
                lock (_PlotsLock)
                {
                    MainThread.InvokeOnMainThreadAsync(() => { 
                        for (int i = 0; i < this.PlotArray.Length; i++)
                        {
                            try
                            {
                                    this.PlotArray[i].InvalidatePlot(true);
                            }
                            catch (Exception)
                            {
                                //break;
                            }
                        }
                    });
                }
                //plotArraySync.Set();
            });
        }

        protected Task InitializeDevice()
        {
            return new Task(async () => { 
                this.StatusMessage = this.EmptyGridMessage = Constants.STRING_INITIALIZING;

                this.ResetFields();
                // this.ClearSupportedPIDS.Clear();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStop = false;
                this.CanStartStop = false;
                this.currentECUAddress = Constants.DEFAULT_DIAG_FUNCADDR_CAN_ID_11.ToString("X");

                this.OBD2Adapter.CreateQueue(QueueSets.InitializeForUserPIDS); ;
                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                if (nextRequest != null)
                {
                    ResetCommTimout();
                    await this.SendRequest(nextRequest);
                }
            });
        }

        //protected void Open()
        //{
        //    try
        //    {
        //        LEDOn();
        //        this.IsConnecting = true;
        //        //this.CloseCommService();

        //        _appShellModel.CommunicationService.CommunicationEvent += OnCommunicationEvent;

        //        var retryCounter = 0;
        //        bool success = false;
        //        do
        //        {
        //            // The last possible retry?
        //            if (retryCounter == Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT - 1)
        //            {
        //                this.IsConnecting = false;
        //            }
        //            success = _appShellModel.CommunicationService.Open();
        //            if (success)
        //            {
        //                // set IsCommunicating to true in the connect callback
        //                //this.IsCommunicating = true;
        //                LEDOff();
        //                return;
        //            }
        //            retryCounter++;
        //            //Thread.Sleep(1500);
        //            if (retryCounter < Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT)
        //            {
        //                this.EmptyGridMessage = this.StatusMessage = $"Retrying {retryCounter}...";
        //            }

        //        } while (retryCounter < Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT);

        //    }
        //    catch (Exception)
        //    {
        //        StatusMessage = Constants.STRING_BUSY;
        //    }
        //    finally
        //    {
        //        this.IsConnecting = false;
        //    }
        //    // unable to connect
        //    _appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
        //    this.IsCommunicating = false;
        //    LEDOff();
        //    _appShellModel.SendHapticFeedback();
        //}


        private void ResetFields()
        {
            //this.VIN = "00000000000000000000";
        }

        private string monitorGridTitle = Constants.DEFAULTS_DESCRIPTOR_MONITORS_SINCE_DTCS_CLEARED;
        public string MonitorGridTitle
        {
            get { return monitorGridTitle; }
            set { SetProperty(ref monitorGridTitle, value); }
        }

        private bool showOnlyNotReady = false;
        public bool ShowOnlyNotReady
        {
            get { return showOnlyNotReady; }
            set { SetProperty(ref showOnlyNotReady, value); }
        }
        public string PauseButtonText
        {
            get { return pauseButtonText; }
            set { SetProperty(ref pauseButtonText, value); }
        }
        private string pauseButtonText = Constants.STRING_PAUSE;

        private string communicationChannel = "<None>";
        public string CommunicationChannel
        {
            get { return communicationChannel; }
            set { SetProperty(ref communicationChannel, value); }
        }

        public IPid PIDToEdit
        {
            get
            {
                return pIDToEdit; 
            }
            set
            {
                SetProperty(ref pIDToEdit, value);
                //pIDToEdit = value;
                //OnPropertyChanged("PIDToEdit");
                OnPropertyChanged("CanEditPID");

                //OnPropertyChanged("CanExitPage");
                //OnPropertyChanged("CanEditPID");
                //OnPropertyChanged("CanAddUserPID");
                //OnPropertyChanged("CanRefreshSupportedPIDS");
            }
        }
        private IPid pIDToEdit = null;

        public bool CanSelectUserPID => CanStartStop && !IsCommunicating;
        public bool CanEditPID => PIDToEdit != null && !IsCommunicating;
        public bool CanAddUserPID => !IsCommunicating;

        public bool CanStop
        {
            get { return canStop; }
            set { SetProperty(ref canStop, value); }
        }
        private bool canStop = false;

        public bool CanStartStop
        {
            get => canStartStop; 
            set { 
                SetProperty(ref canStartStop, value);
                OnPropertyChanged("CanSelectUserPID");
            }
        }
        private bool canStartStop = false;

      //  public bool CanRefreshSupportedPIDS => !this.IsCommunicating;// !this.Negotiating && !this.IsLive;

  
        public bool Negotiating
        {
            get { return negotiating; }
            set 
            { 
                SetProperty(ref negotiating, value);
                OnPropertyChanged("CanRefreshSupportedPIDS");
            }
        }
        private bool negotiating = false;



        private string runButtonText = Constants.STRING_START;
        public string RunButtonText
        {
            get { return runButtonText; }
            set { SetProperty(ref runButtonText, value); }
        }



        protected override void AdClosed(object sender, EventArgs e)
        {
            base.AdClosed(sender, e);
            this.CanStartStop = true;
        }




        #region List DragDrop

        public UserPIDsGroup GroupedItems = null;

        public ICommand ItemDragged { get; }
        public ICommand ItemDraggedOver { get; }
        public ICommand ItemDragLeave { get; }
        public ICommand ItemDropped { get; }

        private async Task OnItemDropped(UserPID item)
        {

            if (GroupedItems == null || GroupedItems.Count < 1) GroupedItems = new UserPIDsGroup("Jammy", this.UserPIDs);


            var itemToMove = this.UserPIDs.First(i => i.IsBeingDragged);
            var itemToInsertBefore = item;
            if (itemToMove == null || itemToInsertBefore == null || itemToMove == itemToInsertBefore)
                return;

            //     var categoryToMoveFrom = GroupedItems.First(g => g.Contains(itemToMove));
            //     categoryToMoveFrom.Remove(itemToMove);

            var categoryToMoveTo = GroupedItems[0];//.First(g => g.Contains(itemToInsertBefore));
            var insertAtIndex = categoryToMoveTo.IndexOf(itemToInsertBefore);
            //itemToMove.Category = categoryToMoveFrom.Name;
            categoryToMoveTo.Insert(insertAtIndex, itemToMove);
            itemToMove.IsBeingDragged = false;
            itemToInsertBefore.IsBeingDraggedOver = false;
        }


        public class UserPIDsGroup : ObservableCollection<ObservableCollection<IPid>>
        {
            public UserPIDsGroup() { }
            public UserPIDsGroup(string groupName)
            {
                this.Name = groupName;
            }

            private ObservableCollection<IPid> dtcGroups = new ObservableCollection<IPid>();

            public UserPIDsGroup(string groupName, ObservableCollection<IPid> group)
            {
                //    this.CollectionChanged += OnCollectionChanged;
                this.Name = groupName;
                this.dtcGroups.Clear();
                group.ToList().ForEach(g => {
                    this.dtcGroups.Add(g);
                });

            }

            //[JsonIgnore]
            public bool IsEmpty => this.Count < 1;

            public string Name { get; set; }
            public string Category { get; set; }
        }











        #endregion List DragDrop








    }
}