using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using OS.Communication;

namespace OS.OBDII.ViewModels
{

    public class FreezeFrameViewModel : CommViewModel, IViewModel
    {

        public event ViewModelEvent ModelEvent;
        public event RequestPopup NeedYesNoPopup;

        public List<ECU> ECUs => this.OBD2Adapter.ECUList;

        protected StringBuilder rawStringData = new StringBuilder();

        protected override OBD2DeviceAdapter OBD2Adapter { get; } = new OBD2DeviceAdapter();

        public override void Back()
        {
            this.CanStartStop = false;
            if (this.IsLive)
            {
                this.CloseCommService();
                this.RunButtonText = Constants.STRING_START;
                return;
            }


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



            //Shell.Current.GoToAsync("//Home/DTCs");
        }

        public bool CanExitPage => !this.IsCommunicating && !this.IsPaused;

        //public override bool UseMetric
        //{
        //    get => _appShellModel.UseMetric;
        //    set
        //    {
        //        _appShellModel.UseMetric = value;
        //        OnPropertyChanged("UseMetric");
        //    }
        //}

        public override bool IsCommunicating
        {
            get => base.IsCommunicating;
            set
            {
                base.IsCommunicating = value;
                OnPropertyChanged("CanExitPage");
            }
        }

        private bool isLive = false;
        public bool IsLive
        {
            get { return isLive; }
            set 
            { 
                SetProperty(ref isLive, value);
                OnPropertyChanged("CanRefreshSupportedPIDS");
            }
        }

        private string vin = string.Empty;
        public string VIN
        {
            get { return vin; }
            set { SetProperty(ref vin, value); }
        }

        private int dtcCount = 0;
        public int DTCCount
        {
            get { return dtcCount; }
            set { SetProperty(ref dtcCount, value); }
        }

        private bool StopAll = false;

        private List<IPid> SupportedPIDs = new List<IPid>();
        public ObservableCollection<IPid> FreezeFramePIDs
        {
            get { return freezeFramePIDs; }
            set { SetProperty(ref freezeFramePIDs, value); }
        }
        private ObservableCollection<IPid> freezeFramePIDs = null;

        private IOBDIICommonUI _appShellModel = null;
        public FreezeFrameViewModel(IOBDIICommonUI appShell) : base(appShell)
        {
            if (appShell == null) throw new NullReferenceException("FreezeFrameViewModel..ctor - appShell cannot be null");

            this._appShellModel = appShell;

            Title = "Freeze Frame Data";
            this.FreezeFramePIDs = new ObservableCollection<IPid>();
            this.ResetFields();
            this.IsCommunicating = false;
            this.IsPaused = false;
            this.EmptyGridMessage = string.Empty;

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

            // Retrying, only two cases 1) Init or 2) Live
            if (this._RetryCounter++ < Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT)
            {
                this._CommTimer.Change(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT, Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT);
                // Resend the last command
                if (_appShellModel.CommunicationService.IsConnected)
                {
                    switch (this.OBD2Adapter.CurrentQueueSet)
                    {
                        case QueueSets.FreezeFrameData:
                            if(this.IncrementPIDRequestIndex())
                            {
                                Task.Factory.StartNew(this.SendFreezeFramePIDRequest);
                            }
                            //if (++this.LivePIDRequestIndex > this.FreezeFramePIDs.Count - 1)
                            //{
                            //    //this.LivePIDRequestIndex = 0;
                            //    CloseCommService();
                            //    return;
                            //}
                            return;
                        default:
                            //Task.Run(this.StartInitDevice().Start);
                            //Task.Run(SendRequest($"{base._LastSentCommand}").Start);
                            //SendRequest($"{base._LastSentCommand}");
                            //return;
                            break;
                    }

                    // error during init, restart init...
                    //Task.Run(this.StartInitDevice().Start);
                    //this.StartInitDevice().Start();
                }

                return;
            }


            // Timed out
            this.IsBusy = false;
            this._RetryCounter = 0;
            this.LivePIDRequestIndex = 0;
            this.ErrorExists = true;
            IsCommunicating = false;
            this.CloseCommService();
            this.StatusMessage = Constants.MSG_NO_RESPONSE_VEHICLE;
            this.EmptyGridMessage = Constants.STRING_NO_DATA;
        }



        //public void CloseCommService()
        //{
        //    // Halt timeouts...
        //    this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);

        //    _RetryCounter = 0;
        //    this.OBD2Adapter?.ClearQueue();
        //    this.ActionQueue?.Clear();
        //    this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
        //    _appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
        //    this.IsCommunicating = false;
        //    _appShellModel.CommunicationService?.Close();
        //    LEDOff();
        //    _appShellModel.SendHapticFeedback();

        //}

        public void Stop()
        {

            //this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
            //_appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
            //this.adService.PopupClosed -= AdClosed;
            //this.FreezeFramePIDs.Clear();
            //this.FreezeFramePIDs = null;
            //this.SupportedPIDs.Clear();
            //this.SupportedPIDs = null;
            this.CloseCommService();

        }

        public virtual void Start()
        {
            this.DataIsTransmitting = false;
            this.ErrorExists = false;
            OnPropertyChanged("UseMetric");
            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_INITIAL_VIEW))
            {
                Preferences.Remove(Constants.APP_PROPERTY_KEY_INITIAL_VIEW);
                this.ErrorExists = false;
                this.Initialize();
                return;
            }
            //this.OBD2Adapter.OBD2AdapterEvent += OnAdapterEvent;
            //_appShellModel.CommunicationService.CommunicationEvent += OnCommunicationEvent;
            if (!this.IsCommunicating)
            {
                this.ErrorExists = false;
                this.IsBusy = false;
            }

            // Settings might have changed
        }

        public void Initialize()
        {
            //  CloseCommService();
            Application.Current.Dispatcher.Dispatch(this.FreezeFramePIDs.Clear);
            ResetFields();
            StartInitDevice().Start();
        }

        private async Task SendFreezeFramePIDRequest()
        {
            tmpPid = this.FreezeFramePIDs[this.LivePIDRequestIndex];
            this.OBD2Adapter.CurrentLivePIDRequest = (int)tmpPid.Code;
            // put together the next PID request
            //var sd = $"01{tmpPid.Code:x2}{tmpPid.ResponseByteCount}{Constants.CARRIAGE_RETURN}";
            var sd = $"02{tmpPid.Code:X2}00{tmpPid.ResponseByteCount}{Constants.CARRIAGE_RETURN}";
            await this.SendRequest(sd);
        }

        /// <summary>
        /// Will close connection when all pids have been exhausted
        /// </summary>
        /// <returns></returns>
        private bool IncrementPIDRequestIndex()
        {

            if (++this.LivePIDRequestIndex > this.FreezeFramePIDs.Count - 1)
            {
                //this.LivePIDRequestIndex = 0;
                this.IsBusy = false;

                this.StopAll = true;
                CloseCommService();
                this.Negotiating = false;
                return false;
            }
            return true;

        }

        protected override void OnAdapterEvent(object sender, OBD2AdapterEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                try
                { 

                    if (this.StopAll || this.IsPaused) return;
                    byte code;
                    object value = null;
                    switch (e.EventType)
                    {
                        case OBD2AdapterEventTypes.LiveData:

                            if (e.Data == null)
                            {
                                // something went wrong with last request...just continue to next
                                if (this.IncrementPIDRequestIndex())
                                {
                                    this.SendFreezeFramePIDRequest();
                                }
                        
                                return;
                            }

                            code = e.Data[0];
                            value = e.dataObject;
                            try
                            {
                                if (e.dataObject == null || e.Data == null)
                                {
                                    return;
                                }
                                if (value != null)
                                {
                                    IPid tPid = this.FreezeFramePIDs[this.LivePIDRequestIndex];

                                    if (tPid == null) return;

                                        switch (code)
                                        {
                                            case 0x03:
                                                try
                                                {

                                                    IPid jpid = null;
                                                    var sysStats = value as FuelSystemStatus[];
                                                    if (sysStats == null) break;

                                                    uint exMask = 0x8000;
                                                    // fuel system 1
                                                    if (sysStats.Count() >= 1 && sysStats[0].Active)
                                                    {
                                                        if ((jpid = this.FreezeFramePIDs.FirstOrDefault(p => p.Code == code)) != null)
                                                        {
                                                            jpid.Name = "Fuel1Status";
                                                            jpid.Description = "Fuel System 1 Status";
                                                            jpid.Value = (value as FuelSystemStatus[])[0];
                                                        }
                                                    }
                                                    // expanded codes derive from multiple data objects from a single request, but the codes are invalid to call as a request
                                                    var nextExpandedCode = (uint)(code | exMask);

                                                    // fuel system 2 
                                                    if(sysStats.Count() >= 2 && sysStats[1].Active)
                                                    {
                                                        var j1 = new PID(nextExpandedCode, "Fuel2Status", "Fuel System 2 Status", typeof(String), (i) => { return i; }, true);
                                                        j1.Value = (value as FuelSystemStatus[])[1];
                                                        this.FreezeFramePIDs.Insert(this.FreezeFramePIDs.IndexOf(tPid) + 1, j1);
                                                    }

                                                    nextExpandedCode = (uint)(code | exMask >> 1);
                                                    // fuel system 3
                                                    if (sysStats.Count() >= 3 && sysStats[2].Active)
                                                    {
                                                        var j = new PID(nextExpandedCode, "Fuel3Status", "Fuel System 3 Status", typeof(String), (i) => { return i; }, true);
                                                        j.Value = (value as FuelSystemStatus[])[2];
                                                        this.FreezeFramePIDs.Insert(this.FreezeFramePIDs.IndexOf(tPid) + 2, j);
                                                    }

                                                    nextExpandedCode = (uint)(code | exMask >> 2);
                                                    // fuel system 4
                                                    if (sysStats.Count() >= 4 && sysStats[3].Active)
                                                    {
                                                        var j = new PID(nextExpandedCode, "Fuel4Status", "Fuel System 4 Status", typeof(String), (i) => { return i; }, true);
                                                        j.Value = (value as FuelSystemStatus[])[3];
                                                        this.FreezeFramePIDs.Insert(this.FreezeFramePIDs.IndexOf(tPid) + 3, j);
                                                    }

                                                }
                                                catch (Exception)
                                                {

                                                }
                                                break;
                                            case 0x14:
                                            case 0x15:
                                            case 0x16:
                                            case 0x17:
                                            case 0x18:
                                            case 0x19:
                                            case 0x1A:
                                            case 0x1B:
                                                var o2pid = ((O2PID)value);
                                                if (o2pid == null) return;
                                                // tPid.Value = o2pid.Trim;
                                                // tPid.UnitDescriptor = "%";
                                                tPid.Value = Math.Round(o2pid.Volts, 3);   // J1979
                                                tPid.MetricUnitDescriptor = "V";
                                                tPid.EnglishUnitDescriptor = "V";
                                                //this.UserPIDS.Add(new PID(code, o2pid.Name, o2pid.Name, typeof(double), OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.Volts]));
                                                // this.UserPIDS.Add(new PID(code, o2pid.Name, o2pid.Name, typeof(double), new UnitMeasureScale(UnitMeasure.Volts, null, 2, "V")));
                                                break;
                                            case 0x24:
                                            case 0x25:
                                            case 0x26:
                                            case 0x27:
                                            case 0x28:
                                            case 0x29:
                                            case 0x2A:
                                            case 0x2B:
                                                var o2WidepidV = ((O2WidePIDVolts)value);
                                                if (o2WidepidV == null) return;
                                                //tPid.Value = o2pid.EqRatio;
                                                //tPid.UnitDescriptor = "";
                                                tPid.Value = Math.Round(o2WidepidV.Volts, 3);   // J1979
                                                tPid.MetricUnitDescriptor = "V";
                                                tPid.EnglishUnitDescriptor = "V";
                                                //this.UserPIDS.Add(new PID(code, o2pid.Name, o2pid.Name, typeof(double), OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.Volts]));
                                                // this.UserPIDS.Add(new PID(code, o2pid.Name, o2pid.Name, typeof(double), new UnitMeasureScale(UnitMeasure.Volts, null, 2, "V")));
                                                break;
                                            case 0x34:
                                            case 0x35:
                                            case 0x36:
                                            case 0x37:
                                            case 0x38:
                                            case 0x39:
                                            case 0x3A:
                                            case 0x3B:
                                                var o2Widepid = ((O2WidePIDmA)value);
                                                if (o2Widepid == null) return;
                                                //tPid.Value = o2pid.EqRatio;
                                                //tPid.UnitDescriptor = "";
                                                tPid.Value = Math.Round(o2Widepid.mA, 2);   // J1979
                                                tPid.MetricUnitDescriptor = "mA";
                                                tPid.EnglishUnitDescriptor = "mA";
                                                //this.UserPIDS.Add(new PID(code, o2pid.Name, o2pid.Name, typeof(double), OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.Volts]));
                                                // this.UserPIDS.Add(new PID(code, o2pid.Name, o2pid.Name, typeof(double), new UnitMeasureScale(UnitMeasure.Volts, null, 2, "V")));
                                                break;
                                            default:
                                                tPid.Value = value;
                                                break;
                                        }


                                        // allow user to stop/cancel operations
                                        //this.IsLive = true;
                                        this.CanStartStop = true;

                                }
                            }
                            catch (Exception)
                            {

                            }
                            finally
                            {
                                if(this.IncrementPIDRequestIndex())
                                {
                                    this.SendFreezeFramePIDRequest();
                                }
                            }

                                break;
                        case OBD2AdapterEventTypes.O2Locations13:
                            code = e.Data[0];
                            value = e.dataObject;
                            if (value != null)
                            {
                                var tPid = this.FreezeFramePIDs.FirstOrDefault(p => p.Code == code);
                                //var tPid = this.SupportedPIDS.FirstOrDefault(p => p.Code == code);
                                if (tPid == null) return;
                                tPid.Description = (string)value;
                                tPid.ResponseByteCount = 1;
                            }
                            break;
                        case OBD2AdapterEventTypes.O2Locations1D:
                            code = e.Data[0];
                            value = e.dataObject;
                            if (value != null)
                            {
                                switch (code)
                                {
                                    case (byte)OBD2PIDS.PIDS.OBD2_SHRTFT13:
                                        this.FreezeFramePIDs.Where(p => p.Code == code).FirstOrDefault().ResponseByteCount = 2;
                                        break;
                                    case (byte)OBD2PIDS.PIDS.OBD2_LONGFT13:
                                        this.FreezeFramePIDs.Where(p => p.Code == code).FirstOrDefault().ResponseByteCount = 2;
                                        break;
                                    case (byte)OBD2PIDS.PIDS.OBD2_SHRTFT24:
                                        this.FreezeFramePIDs.Where(p => p.Code == code).FirstOrDefault().ResponseByteCount = 2;
                                        break;
                                    case (byte)OBD2PIDS.PIDS.OBD2_LONGFT24:
                                        this.FreezeFramePIDs.Where(p => p.Code == code).FirstOrDefault().ResponseByteCount = 2;
                                        break;
                                }
                                var tPid = this.FreezeFramePIDs.FirstOrDefault(p => p.Code == code);
                                if (tPid == null) return;
                                tPid.Description = (string)value;
                                tPid.ResponseByteCount = 2;
                            }
                            break;
                        case OBD2AdapterEventTypes.SupportedPIDsLoaded:
                                string sndStr = string.Empty;

                                // store up the data and shut the headers off of the response
                                this.IsBusy = true;

                                // Powertrain ecu is the lowest address, usually 0x7E0 (or 7E1) => 7E8/7E9
                                var powerTrainECU = this.ECUs.Where(v => v.Id == this.ECUs.Min(vEcu => vEcu.Id)).FirstOrDefault();

                                int pidCount = 0;


                                if (powerTrainECU != null)
                                {

                                    
                                    // Each ecu has different set of supported pids
                                    powerTrainECU.SupportedPIDS.ForEach(sPid =>
                                    {
                                        // all supported pids
                                        if (!this.SupportedPIDs.Any(p => p.Code == sPid.Code))
                                        {
                                            this.SupportedPIDs.Add(sPid);
                                        }
                                        if (sPid.IsSupportable && sPid.IsVisible)
                                        {
                                            // applying a limit for free version
                                            if((pidCount++ < (Constants.APP_LIMIT_MAX_FF_PID_ROWS) && Constants.FORCE_LIMITS) || !Constants.FORCE_LIMITS)
                                            {
                                                // add displayed freeze frame pids - 
                                                if (!this.FreezeFramePIDs.Any(p => p.Code == sPid.Code))
                                                {
                                                    this.FreezeFramePIDs.Add(sPid);
                                                }
                                            }
                                        }
                                    });
                                    
                        }


                                this.EmptyGridMessage = "No Freeze Frame Data";
                                this.SupportedPidsLoaded.Set();
                                break;
                            case OBD2AdapterEventTypes.SupportedPIDs:
                            break;
                        case OBD2AdapterEventTypes.Update:
                            try
                            {
                               
                                // General use of reflection to assign values to properties...
                                System.Reflection.PropertyInfo propInf = this.GetType().GetProperty(e.propertyName);
                                if(propInf != null) propInf.SetValue(this, e.dataObject);
                            }
                            catch (Exception)
                            {

                            }

                            break;
                        case OBD2AdapterEventTypes.Error:
                            this.ErrorExists = true;
                            this.OBD2Adapter.ClearQueue();
                            this.CloseCommService();
                            this.EmptyGridMessage = this.StatusMessage = e.Description;
                            this.IsCommunicating = false;
                            this.IsPaused = false;
                            this.CanStartStop = true;
                            this.IsBusy = false;

                            break;
                    }
                }
                catch (Exception)
                {

                }
            });
        }


        StringBuilder sb = new StringBuilder();

        private int RequestCount = 0;
        private int LivePIDRequestIndex = 0;
        private IPid tmpPid = null; // for live data
        private Task tsk = null;
        protected override async Task OnCommunicationEvent(object sender, DeviceEventArgs e)
        {
            tmpPid = null;
            tsk = null;
            try 
            { 
                //MainThread.BeginInvokeOnMainThread(() =>
                //{
                string nextRequest;
                switch (e.Event)
                {
                    case CommunicationEvents.Receive:
                    case CommunicationEvents.ReceiveEnd:
                        ActivityLEDOff();
                        if (this.StopAll)
                        {
                            this.OBD2Adapter.ClearQueue();
                            this.IsCommunicating = false;
                            this.IsPaused = false;
                            this.CanStartStop = false;
                            this.IsBusy = false;
                            this.ErrorExists = false;
                            
                            //this.CloseCommService();
                            _appShellModel.CommunicationService?.Close();
                            //this.StatusMessage = this.EmptyGridMessage = "Communication Stopped";
                            return;
                        }

                        this.rawStringData.Append(Encoding.UTF8.GetString(e.data));

                        // from elm327 datasheet '>' is end of message
                        if (e.data[e.data.Length - 1] != '>')
                        {
                            return;
                        }

                        if (OBD2Device.CheckForDongleMessages(this.rawStringData.ToString(), out nextRequest, true))
                        {
                            this.ErrorExists = true;
                            this.EmptyGridMessage = this.StatusMessage = nextRequest;
                            this.rawStringData.Clear();
                            this.CloseCommService();
                            return;
                        }

                        // Reset RX timeout timer
                        this.ResetCommTimout();

                        switch (this.OBD2Adapter.CurrentQueueSet)
                        {
                            // case QueueSets.None:
                            case QueueSets.DetectSystemProtocolID:
 //                         case QueueSets.GetFFPids_Mode22:
//                          case QueueSets.GetFFPids:
                            case QueueSets.GetSupportedPids:

                                this.OBD2Adapter.ParseResponse(this.rawStringData.ToString());
                                this.rawStringData.Clear();

                                nextRequest = this.OBD2Adapter.GetNextQueuedCommand(true, true);
                                //switch (this.OBD2Adapter.CurrentQueueSet)
                                //{
                                //    case QueueSets.GetFFPids_Mode22:
                                //        nextRequest = this.OBD2Adapter.GetNextQueuedCommand(true, true, "00");
                                //        break;
                                //    default:
                                //        nextRequest = this.OBD2Adapter.GetNextQueuedCommand(true, true);
                                //        break;
                                //}


                                if (nextRequest == null)
                                {

                                    if (this.ActionQueue.Count < 1)
                                    {
                                        //this.StatusMessage = "Closing...";
                                        this.CloseCommService();
                                    }
                                    else
                                    {
                                        this.ActionQueue.Dequeue().Start();
                                    }
                                }
                                else
                                {
                                    // need this to be synchronous so that all the fields are completed via adapter event before moving to next.
                                    await this.SendRequest(nextRequest);
                                    return;
                                }
                                return;
                        //case QueueSets.GetFFO2SensorLocations13:
                        //case QueueSets.GetFFO2SensorLocations1D:
                        case QueueSets.GetO2SensorLocations13:
                        case QueueSets.GetO2SensorLocations1D:
                            this.OBD2Adapter.ParseResponse(this.rawStringData.ToString());
                            this.rawStringData.Clear();
                            nextRequest = this.OBD2Adapter.GetNextQueuedCommand(true, true);
                            if (nextRequest == null)
                            {
                                if (this.ActionQueue.Count < 1)
                                {
                                    //this.StatusMessage = "Closing...";
                                    this.CloseCommService();
                                }
                                else
                                {
                                    this.ActionQueue.Dequeue().Start();
                                }
                            }
                            else
                            {
                                // need this to be synchronous so that all the fields are completed via adapter event before moving to next.
                                await this.SendRequest(nextRequest);
                                return;
                            }
                            return;
                        //case QueueSets.GetFFO2SensorLocations1D_Mode22:
                        //case QueueSets.GetFFO2SensorLocations13_Mode22:
                        //    this.OBD2Adapter.ParseResponse(this.rawStringData.ToString());
                        //        this.rawStringData.Clear();
                        //        nextRequest = this.OBD2Adapter.GetNextQueuedCommand(true, true, "00"); ;
                        //        if (nextRequest == null)
                        //        {
                        //            if (this.ActionQueue.Count < 1)
                        //            {
                        //                //this.StatusMessage = "Closing...";
                        //                this.CloseCommService();
                        //               // OBD2Device.CurrentRequestSize = 2;
                        //            }
                        //            else
                        //            {
                        //                this.ActionQueue.Dequeue().Start();
                        //            }
                        //        }
                        //        else
                        //        {
                        //            // need this to be synchronous so that all the fields are completed via adapter event before moving to next.
                        //            await this.SendRequest(nextRequest);
                        //            return;
                        //        }
                        //        return;
                            case QueueSets.FreezeFrameData:
                                switch (this.OBD2Adapter.CurrentRequest)
                                {
                                case DeviceRequestType.HeadersOff:
                                    // Set the header specifically to powertrain ecu - to avoid multiple responses from other cpus
                                    var sndStr = OBD2Device.CreateRequest(OBD2Device.ServiceModes[0], DeviceRequestType.SetHeader, true, PowerTrainECUID.ToString("X"));
                                    this.OBD2Adapter.CurrentRequest = DeviceRequestType.SetHeader; // first thing to do...
                                    await this.SendRequest(sndStr);
                                    return;
                                case DeviceRequestType.SetHeader:
                                    this.rawStringData.Clear();
                                    this.OBD2Adapter.CurrentRequest = DeviceRequestType.None;
                                    this.StatusMessage = this.EmptyGridMessage = "Freeze Frame Data...";
                                    this.MonitorGridTitle = "Freeze Frame Data";
                                    this.LivePIDRequestIndex = 0;
                                    this.RequestCount = 0;
                                    await this.SendFreezeFramePIDRequest();
                                    return;
                                }
                                this.EmptyGridMessage = Constants.STRING_NO_DATA;

                               // this.IsBusy = false;
                                this.Negotiating = false;

                                if (this.IsPaused)
                                {
                                    this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);
                                    this.LivePIDRequestIndex = 0;
                                    this.rawStringData.Clear();
                                    // timeout callback interrogates IsPaused   
                                    return;
                                }

                                this.OBD2Adapter.ParseResponse(this.rawStringData.ToString());
                                if (this.rawStringData.ToString().Length < 1)
                                {
                                    this.OBD2Adapter.ClearQueue();
                                    //this.IsCommunicating = false;
                                    this.IsPaused = false;
                                    this.IsBusy = false;
                                    this.IsLive = false;
                                    await this.SendFreezeFramePIDRequest();
                                }
                                this.rawStringData.Clear();

                            return;
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
                                    //_appShellModel.DeviceIsInitialized = true;
                                }
                                break;
                            case QueueSets.Initialize:
                            case QueueSets.InitializeWithHeaders:
                                switch (this.OBD2Adapter.CurrentRequest)
                                {
                                    case DeviceRequestType.SetHeader:
                                        if (this._appShellModel.UserCANID.Length > 6)
                                        {
                                            sb.Clear();
                                            sb.Append($"ATCP{this._appShellModel.UserCANID.Substring(0, 2).PadLeft(2, '0')}");
                                            sb.Append(Constants.CARRIAGE_RETURN);
                                            OBD2Adapter.CurrentRequest = DeviceRequestType.SetCANPriorityBits;
                                            await this.SendRequest(this.sb.ToString());
                                            OBD2Adapter.CurrentRequest = DeviceRequestType.None;
                                            return;
                                        }
                                        break;
                                }

                                nextRequest = this.OBD2Adapter.GetNextQueuedRequest(true, false);
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
                                            if (_appShellModel.UseHeader)
                                            {
                                                // Set the selected protocol...
                                                await this.SendRequest($"{nextRequest}{_appShellModel.UserCANID}{Constants.CARRIAGE_RETURN}");
                                                break;
                                            }
                                            if (this.ActionQueue.Count > 0)
                                            {
                                                this.ActionQueue.Dequeue().Start();
                                            }
                                            else
                                            {
                                                this.CloseCommService();
                                            }
                                            // ????????????????????????????????????????????????????????????????????????  needed?
                                            // Declare device has been through a basic inititialization
                                            //_appShellModel.DeviceIsInitialized = true;
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
                                else
                                {
                                    // Declare device has been through a basic inititialization
                                    //_appShellModel.DeviceIsInitialized = true;


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

                                }
                                break;
                        }
                        break;
                    case CommunicationEvents.ConnectedAsClient:
                        this.IsPaused = false;
                        IsConnecting = false;
                        if (this.ActionQueue.Count > 0)
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
                        this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
                        _appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
                         MainThread.BeginInvokeOnMainThread(() =>
                         {
                            this.RunButtonText = Constants.STRING_START;
                            this.IsBusy = false;
                            this.Negotiating = false;
                            this.IsCommunicating = false;
                            this.IsPaused = false;
                            this.IsLive = false;
                             this.CanStartStop = true;

                             if (!this.ErrorExists)
                            {
                                this.StatusMessage = "Ready";
                                 SendHapticFeedback();
                             }
                         });

                        ActivityLEDOff();

                        break;
                    case CommunicationEvents.Error:
                        CloseCommService();
                        lock (dLock)
                        {
                            if (!string.IsNullOrEmpty(_appShellModel.CommunicationService.DeviceName) &&
                                    this.IsConnecting && this.ConnectTimeoutCount++ < Constants.DEFAULT_COMM_CONNECT_RETRY_COUNT)
                            {
                                this.EmptyGridMessage = "Retrying...";
                                this.StartInitDevice(false).Start();
                                return;
                            }
                        }
                        ActivityLEDOff();
                        this.Negotiating = false;
                        this.ErrorExists = true;
                        this.IsBusy = false;
                        this.IsCommunicating = false;
                        this.IsPaused = false;
                        this.RunButtonText = Constants.STRING_START;
                        this.CanStartStop = true;
                        this.EmptyGridMessage = "Error";
                        this.StatusMessage = e.Description;
                        this.IsLive = false;
                        //SupportedPIDSelectionChanged(null, null);

                        if (!string.IsNullOrEmpty(e.Description) && this._RetryCounter >= Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT)
                        {
                            this.StatusMessage = e.Description;
                            this.EmptyGridMessage = Constants.STRING_NO_DATA;// e.Description;
                        }
                        SendHapticFeedback();
                        break;
                    default:
                        break;
                }
                //       });
            }
            catch (Exception)
            {

            }
        }

        #region Device Reset

        //private async Task PrepareReset()
        //{
        //    await Task.Run(() => {
        //        MainThread.BeginInvokeOnMainThread(() =>
        //        {
        //           // this.StatusMessage = "Resetting Device...";
        //            this.ErrorExists = false;
        //            this.IsCommunicating = true;
        //            this.IsPaused = false;
        //        });

        //        this.EmptyGridMessage = Constants.NO_DATA_STRING;
        //        this.ActionQueue.Enqueue(ResetDevice());
        //        this.Open();
        //    });
        //}

        private Task ResetDevice()
        {
            return new Task( () => {

                this.StatusMessage = this.EmptyGridMessage = Constants.STRING_CONNECTING;
                this.OBD2Adapter.CreateQueue(QueueSets.DeviceReset);
                var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                if (nextRequest != null)
                {
                    this.SendRequest(nextRequest);
                }
            });
        }

        #endregion Device Reset

        #region InitializeDeviceCommand

        public ICommand InitializeDeviceCommand => new Command(async () => {
           // _appShellModel.SendHapticFeedback();
            await Task.Run(() => {
                // Start 'No Response' timer...
                this.ResetCommTimout();
                this.StartInitDevice().Start();
            });
        }, () => !IsCommunicating);

        private Task StartInitDevice(bool resetRetries = true)
        {
            return new Task(() =>
            {
                this.IsConnecting = true;
                this.Negotiating = true;
                this.ErrorExists = false;
                this.IsCommunicating = true;
                //this.EmptyGridMessage = this.StatusMessage = Constants.STRING_CONNECTING;
                this.EmptyGridMessage = this.StatusMessage = $"Connecting to {_appShellModel.CommunicationService.DeviceName}";
                this.IsPaused = false;
                this.StopAll = false;
                this.CanStartStop = false;

                // this.StatusMessage = this.EmptyGridMessage = "Connecting...";
                this.ActionQueue.Clear();

                Application.Current.Dispatcher.Dispatch(this.FreezeFramePIDs.Clear);

                // Need to enable headers
                // 1) Reset device
                // 2) Init ELM327 with headers on
                // 3) Get supported pids by CAN ID (reason why headers are on)
                // 4) Turn Headers off, so data is the same for any protocol
                // 4) Identify the vehicle's protocol. Once responsese have come from the vehicle
                //    then the ELM327 would be operating on a specific protocol (accounts for case
                //    where protocol is set to auto initially)
                //
                // */

                this.ActionQueue.Enqueue(ResetDevice());
                this.ActionQueue.Enqueue(InitializeDevice());
                // need to know the protocol, to know what size header to expect
                this.ActionQueue.Enqueue(GetProtocolID());
                this.ActionQueue.Enqueue(GetSupportedPIDs());
                //this.ActionQueue.Enqueue(GetFreezeFramePIDs());
                this.ActionQueue.Enqueue(GetO2Locations());
                //this.ActionQueue.Enqueue(GetO2Locations());
                this.ActionQueue.Enqueue(StartListening());
                if (resetRetries)
                {
                    this._RetryCounter = 0;
                    ConnectTimeoutCount = 0;
                }
                this.Open();
            });
        }


        #endregion InitializeDeviceCommand



        //public ICommand BackCommand => new Command(async () => {

        //    if (this.ModelEvent != null)
        //    {
        //        this.IsBusy = true;
        //        using (ViewModelEventArgs evt = new ViewModelEventArgs())
        //        {
        //            evt.EventType = ViewModelEventEventTypes.NavigateTo;
        //            evt.dataObject = "CodesPage";
        //            this.ModelEvent(this, evt);
        //        }
        //    }

        //   // await Shell.Current.GoToAsync("//Home/LiveHome");
        //});



        #region PauseCommand

        public ICommand PauseCommand => new Command(() =>
        {
            //this.liveModeInitialized = false;
            if(this.IsPaused)
            {
                //this.OBD2Adapter.CurrentQueueSet = QueueSets.FreezFrameData;
                this.SendFreezeFramePIDRequest();
                this.IsCommunicating = true;
                this.PauseButtonText = Constants.STRING_PAUSE;
                this.IsPaused = false;
                // Restart 'No Response' timer...
                this.ResetCommTimout();
                return;
            }
            this.IsPaused = true;
            // Pause 'No Response' timer...
            //this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this.IsCommunicating = false;
            //this.OBD2Adapter.CurrentQueueSet = QueueSets.None;
            this.PauseButtonText = Constants.STRING_START;

            this.LivePIDRequestIndex = 0;

        });

        private bool IsPaused = false;

        #endregion PauseCommand


        #region StartStopCommand

        public ICommand StartStopCommand => new Command(async () =>
        {
            this.CanStartStop = false;
            await Task.Run(() => { 

                if (_appShellModel.CommunicationService.IsConnected)
                {
                    this.PauseButtonText = Constants.STRING_START;

                    this.StopAll = true;
                    this.CloseCommService();
                    // this.RunButtonText = Constants.STRING_START;
                    // SupportedPIDSelectionChanged(null, null);
                    return;
                }
                this.Negotiating = true;
                this.IsBusy = false;
                this.LivePIDRequestIndex = 0;
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.StatusMessage = Constants.STRING_CONNECTING;
                this.ErrorExists = false;
                this.StopAll = false;
                this.RunButtonText = Constants.STRING_BACK;
                this.PauseButtonText = Constants.STRING_PAUSE;

                // Start 'No Response' timer...
                this.ResetCommTimout();


                if (this.adService.DoAdPopup())
                {
                    // assign a method to call after the ad
                    this.adService.PostAdTask = this.StartMonitors();
                    // the post-ad callback where PostAdAction will be called.
                    this.adService.PopupClosed -= AdClosed;
                    this.adService.PopupClosed += AdClosed;
                    return;
                }

                this.StartMonitors().Start();
            });

        }, () => CanStartStop);

        private Task StartMonitors()
        {
         //   await Task.Run(()=>{ 
            return new Task(() =>
            {
                this.StatusMessage = this.EmptyGridMessage = Constants.STRING_CONNECTING;
                this.ActionQueue.Clear();
                this.ActionQueue.Enqueue(StartListening());

                this._RetryCounter = 0;
                ConnectTimeoutCount = 0;
                this.Open();
            });
         //   });
        }

        #endregion StartStopCommand


        #region NavigateHomeCommand

        public ICommand CancelCommand => new Command(() => {
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


       //     await Shell.Current.GoToAsync("//Home/DTCs");
        });

        #endregion NavigateHomeCommand


        protected Task BeginGetO2Locations13()
        {
            return new Task(async () => {

                this.StatusMessage = this.EmptyGridMessage = "Adjusting PIDs...";
                this.MonitorGridTitle = "Freeze Frame Data";
                this.LivePIDRequestIndex = 0;

                this.ResetFields();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStartStop = false;
                await Application.Current.Dispatcher.DispatchAsync(this.FreezeFramePIDs.Clear);

                this.OBD2Adapter.CreateQueue(QueueSets.GetO2SensorLocations13); ;
                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                if (nextRequest != null)
                {
                    await this.SendRequest(nextRequest);
                }

            });
        }
        protected Task BeginGetO2Locations1D()
        {
            return new Task(async () => {

                this.StatusMessage = this.EmptyGridMessage = "Adjusting PIDs...";
                this.MonitorGridTitle = "Freeze Frame Data";
                this.LivePIDRequestIndex = 0;

                this.ResetFields();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStartStop = false;
                await Application.Current.Dispatcher.DispatchAsync(this.FreezeFramePIDs.Clear);

                this.OBD2Adapter.CreateQueue(QueueSets.GetO2SensorLocations1D); ;
                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                if (nextRequest != null)
                {
                    await this.SendRequest(nextRequest);
                }

            });
        }

        protected Task StartListening()
        {
            return new Task(async () => {

                this.StatusMessage = this.EmptyGridMessage = "Freeze Frame Data...";
                this.MonitorGridTitle = "Freeze Frame Data";
                this.LivePIDRequestIndex = 0;

                this.ResetFields();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStartStop = false;

                this.OBD2Adapter.CurrentQueueSet = QueueSets.FreezeFrameData;

                // Powertrain ecu is the lowest address, usuall 0x7E0 (or 7E1)
                PowerTrainECUID = this.ECUs.Min(vEcu => vEcu.Id) & 0xFFFFFFF7;


                var sndStr = OBD2Device.CreateRequest(OBD2Device.ServiceModes[0], DeviceRequestType.HeadersOff, true);
                this.OBD2Adapter.CurrentRequest = DeviceRequestType.HeadersOff; // first thing to do...
                this.SendRequest(sndStr);




                // Set the header specifically to powertrain ecu - to avoid multiple responses from other cpus
                //var sndStr = OBD2Device.CreateRequest(OBD2Device.ServiceModes[0], DeviceRequestType.SetHeader, true, powerTrainECUID.ToString("X"));
                //this.OBD2Adapter.CurrentRequest = DeviceRequestType.SetHeader; // first thing to do...
                //this.SendRequest(sndStr);


            });
        }
        ManualResetEvent SupportedPidsLoaded = new ManualResetEvent(true);

        public uint PowerTrainECUID { get; set; }
        protected Task GetO2Locations()
        {
            return new Task(async () => {
                //this.StatusMessage = this.EmptyGridMessage = "Reading PIDs, O2 sensors...";
                this.MonitorGridTitle = "Freeze Frame Data";
                this.LivePIDRequestIndex = 0;

                this.ResetFields();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStartStop = false;

                //Should have the supported pids before this call, next is to get the O2 sensor locations
                if (!this.SupportedPidsLoaded.WaitOne(5000))
                {
                    this.OBD2Adapter.CreateQueue(QueueSets.GetO2SensorLocations13); ;
                    // this.OBD2Adapter.CreateQueue(QueueSets.GetO2SensorLocations1D); ;
                }
                else
                {
                    // According to J1979 standard, only 0x13 OR 0x1D can be supported.
                    if (this.SupportedPIDs.Any(p => p.Code == (uint)OBD2PIDS.PIDS.OBD2_OS2LOC)) // 0x13
                    {
                        // usually used when 2 banks or less exist
                        this.OBD2Adapter.CreateQueue(QueueSets.GetO2SensorLocations13); ;
                    }
                    else if (this.SupportedPIDs.Any(p => p.Code == (uint)OBD2PIDS.PIDS.OBD2_OS2LOC2)) // 0x1D
                    {
                        // Handles Banks 1,2,3 and 4
                        this.OBD2Adapter.CreateQueue(QueueSets.GetO2SensorLocations1D); ;
                    }
                }

                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                if (nextRequest != null)
                {
                    await this.SendRequest(nextRequest);
                }

            });
        }
        protected Task GetProtocolID()
        {
            return new Task(async () => {

                //this.StatusMessage = this.EmptyGridMessage = "Reading System Protocol...";
                this.LivePIDRequestIndex = 0;

                this.ResetFields();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStartStop = false;
                await Application.Current.Dispatcher.DispatchAsync(this.FreezeFramePIDs.Clear);

                this.OBD2Adapter.CreateQueue(QueueSets.DetectSystemProtocolID);

                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand();
                if (nextRequest != null)
                {
                    await this.SendRequest(nextRequest);
                }

            });
        }

        protected Task GetSupportedPIDs()
        {
            return new Task(async () => {
                this.StatusMessage = this.EmptyGridMessage = "Reading Freeze Frame Data";
                this.ResetFields();
                // this.ClearSupportedPIDS.Clear();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStartStop = false;
                //this.StatusMessage = "Starting...";
                 this.OBD2Adapter.CreateQueue(QueueSets.GetSupportedPids); 
                //this.OBD2Adapter.CreateQueue(QueueSets.GetFFPids_Mode22);
                var nextRequest = this.OBD2Adapter.GetNextQueuedCommand(true, true);
                if (nextRequest != null)
                {
                    this.SupportedPidsLoaded.Reset();
                    await this.SendRequest(nextRequest);
                }
            });
        }

        protected Task InitializeDevice()
        {
            return new Task(async () => { 
                //this.StatusMessage = this.EmptyGridMessage = "Initializing...";

                this.ResetFields();
                // this.ClearSupportedPIDS.Clear();
                this.IsCommunicating = true;
                this.IsPaused = false;
                this.CanStartStop = false;
                //this.StatusMessage = "Starting...";
                this.OBD2Adapter.CreateQueue(QueueSets.InitializeWithHeaders); ;
                //this.OBD2Adapter.CreateQueue(QueueSets.Initialize); ;
                var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                if (nextRequest != null)
                {
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


        //        this.OBD2Adapter.OBD2AdapterEvent += OnAdapterEvent;
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
        //    this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
        //    _appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
        //    this.IsCommunicating = false;
        //    LEDOff();
        //    _appShellModel.SendHapticFeedback();
        //}

        private void ResetFields()
        {
            this.VIN = "00000000000000000000";
        }

        private bool isDriveCycle = false;
        public bool IsDriveCycle
        {
            get { return isDriveCycle; }
            set 
            { 
                SetProperty(ref isDriveCycle, value);
                OnPropertyChanged("DTCCount");
            }
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

        public string StopButtonText
        {
            get { return startStopButtonText; }
            set { SetProperty(ref startStopButtonText, value); }
        }
        private string startStopButtonText = Constants.STRING_BACK;

        private string communicationChannel = "<None>";
        public string CommunicationChannel
        {
            get { return communicationChannel; }
            set { SetProperty(ref communicationChannel, value); }
        }

        public bool CanStartStop
        {
            get { return canStartStop; }
            set { SetProperty(ref canStartStop, value); }
        }
        private bool canStartStop = true;

        public bool CanRefreshSupportedPIDS => !this.Negotiating && !this.IsLive;

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

    }
}