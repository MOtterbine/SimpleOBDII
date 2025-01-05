using OS.Communication;
using OS.OBDII.Models;
using OS.OBDII.Interfaces;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using OS.OBDII.Manufacturers;

namespace OS.OBDII.ViewModels
{
    public enum CodesViewOperations
    {
        None,
        ReadDTC,
        ClearDTC,
    }

    public class CodesViewModel : CommViewModel, IViewModel
    {
        public event ViewModelEvent ModelEvent;
        public event RequestPopup NeedYesNoPopup;
        public bool FreezeFrameDataExists => this.FreezeFrameDTC != 0;

        public uint FreezeFrameDTC
        {
            get => OBD2Device.SystemReport.FreezeFrameDTC;
            set
            {
                OBD2Device.SystemReport.FreezeFrameDTC = value;
                if (value != 0)
                {
                    this.DTCCodes.ToList().ForEach(dtc =>
                    {
                        dtc.ToList().ForEach(d =>
                        {
                            if (d.PIDName == this.FreezeFrameDTCString)
                            {
                                d.IsFreezeFramePID = true;
                            }
                        });
                    });
                    
                }


                OnPropertyChanged("FreezeFrameDataExists");
                OnPropertyChanged("FreezeFrameDTCString");
            }
        }

        public string FreezeFrameDTCString => OBD2Device.TranslateToString(this.FreezeFrameDTC);

        public string VIN
        {
            get => OBD2Device.SystemReport.VIN;
            set 
            {
                OBD2Device.SystemReport.VIN = value;
                OnPropertyChanged("VIN");
            }
        }

        public string CurrentCANId
        {
            get => this.currentCANId;
            set => SetProperty(ref currentCANId, value); 
        }
        private string currentCANId = string.Empty;

        public int SystemProtocolID
        {
            get => OBD2Device.SystemProtocolID;
            set  
            {
                OBD2Device.SystemProtocolID = value;
                OnPropertyChanged("SystemProtocolID");
            }
        }


        public override bool IsCommunicating
        {
            get => base.IsCommunicating;
            set
            {
                base.IsCommunicating = value;
                OnPropertyChanged("CanClearDTCs");
            }
        }

        public bool CanClearDTCs => !(IsCommunicating || Constants.FORCE_LIMITS); 

        public int DTCCount
        {
            get => OBD2Device.SystemReport.DTCCount;
            set
            {
                OBD2Device.SystemReport.DTCCount = value;
                OnPropertyChanged("DTCCount");
            }
            
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


        private ObservableCollection<DTCGroup> dTCCodes = new ObservableCollection<DTCGroup>() { 
            new DTCGroup(Constants.DTC_CODES_GROUP_CONFIRMED), 
            new DTCGroup(Constants.DTC_CODES_GROUP_PENDING),
            new DTCGroup(Constants.DTC_CODES_GROUP_PERMANENT),
            new DTCGroup(Constants.DTC_CODES_GROUP_STORED)
        };

        public ObservableCollection<DTCGroup> DTCCodes
        {
            get => dTCCodes; 
            set { SetProperty(ref dTCCodes, value); }
        }

        IOBDIICommonUI _appShellModel = null;
        public CodesViewModel(IOBDIICommonUI appShell) : base(appShell)
        {
            if (appShell == null) throw new NullReferenceException("CodesViewModel..ctor - appShell cannot be null");
            this._appShellModel = appShell;
            this._appShellModel.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "CodesViewModel..ctor");

            Title = "Trouble Codes";


          //  this.SelectedManufacturer = this.Manufacturers.Where(m => String.Compare(m.Name, Preferences.Get(Constants.PREFS_KEY_MANUFACTURER, "Generic")) == 0).FirstOrDefault();
            OnPropertyChanged("FreezeFrameDTC");
            OBD2Device.SystemReport.Clear();

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
                //if (this._appShellModel.CommunicationService.IsConnected)
                //{
                //    // Task.Run(SendRequest($"{base._LastSentCommand}").Start);
                //    SendRequest($"{base._LastSentCommand}");
                //}

                //if (this._appShellModel.CommunicationService.IsConnected)
                //{
                //    Task.Run(SendRequest($"{base._LastSentCommand}").Start);
                //}
                //switch (this.CurrentOperation)
                //{
                //    case CodesViewOperations.ClearDTC:
                //        Task.Run(ClearDTC().Start);
                //        break;
                //    case CodesViewOperations.ReadDTC:
                //        Task.Run(ReadDTC().Start);
                //        break;
                //}

                return;
            }

      //      CurrentOperation = CodesViewOperations.None;
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
                            this.CloseCommService();
                            return;
                        }

                        // Reset RX timeout timer
                        //this._CommTimer.Change(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT, Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT);
                        ResetCommTimout();



                        // PRE-ERROR CHECK - to ensure all Clear DTCs commands are called - even if no response
                        // This is a response to...
                        switch (this.OBD2Adapter.CurrentQueueSet)
                        {
                            case QueueSets.ClearDTCs:

                                switch (this.OBD2Adapter.CurrentRequest)
                                {
                                    // the last step in clearing DTCs, don't want to trip over 'CheckForDongleMessages
                                    case DeviceRequestType.CAN_ClearDTCs:
                                        this.ActionQueue.Clear();
                                        this.OBD2Adapter.ClearQueue();
                                        this.OBD2Adapter.CurrentRequest = DeviceRequestType.None;
                                        await MainThread.InvokeOnMainThreadAsync(() => {
                                        
                                            this.EmptyGridMessage = this.StatusMessage = "Reset DTC Command Sent";
                                            this.CloseCommService();
                                        });
                                        break;
                                }
                                nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                                this.rawStringData.Clear();
                                if (nextRequest != null)
                                {
                                    await this.SendRequest(nextRequest);
                                }
                                return;
                        }




                        switch (this.OBD2Adapter.CurrentQueueSet)
                        {
                            case QueueSets.DTCReport:

                                this.OBD2Adapter.ParseResponse(this.rawStringData.ToString());
                                this.rawStringData.Clear();

                                nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                                if (nextRequest == null)
                                {
                                    await MainThread.InvokeOnMainThreadAsync(() =>
                                    {
                                        this.EmptyGridMessage = Constants.STRING_NO_CODES_FOUND;
                                        this.StatusMessage = "Stopped";
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
                                }
                                break;
                            case QueueSets.Initialize:

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
                                            if (this._appShellModel.UseHeader)
                                            {

                                                if (this._appShellModel.UserCANID.Length > 6)
                                                {
                                                    var data1 = this._appShellModel.UserCANID.Substring(this._appShellModel.UserCANID.Length - 6).ToString();
                                                    await this.SendRequest($"{nextRequest}{data1}{Constants.CARRIAGE_RETURN}");
                                                }
                                                else
                                                {
                                                    var data1 = this._appShellModel.UserCANID.PadLeft(6, '0').ToString();
                                                    await this.SendRequest($"{nextRequest}{data1}{Constants.CARRIAGE_RETURN}");
                                                }



                                                // Set the selected protocol...
                                               // await this.SendRequest($"{nextRequest}{this._appShellModel.UserCANID}{Constants.CARRIAGE_RETURN}");
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
                                            //this._appShellModel.DeviceIsInitialized = true;
                                            break;
                                        case DeviceRequestType.SET_Timeout:
                                            await this.SendRequest($"{nextRequest}80{Constants.CARRIAGE_RETURN}");
                                            break;
                                        //case DeviceRequestType.SET_OBD1WakeupOff:
                                        //    await this.SendRequest($"{nextRequest}{Constants.CARRIAGE_RETURN}");
                                        //    break;
                                        //case DeviceRequestType.SET_ISOInitAddress_13:
                                        //    await this.SendRequest($"{nextRequest}{Constants.CARRIAGE_RETURN}");
                                        //    break;
                                        default:
                                            //queueIndexed = true;
                                            await this.SendRequest($"{nextRequest}{Constants.CARRIAGE_RETURN}");
                                            break;
                                    }

                                    return;
                                }
                                else
                                {
                                    switch(this.OBD2Adapter.CurrentRequest)
                                    {
                                        case DeviceRequestType.OBD2_GetPIDS_00:
                                            this.OBD2Adapter.ParseResponse(this.rawStringData.ToString());
                                            break;
                                    }
                                    this.rawStringData.Clear();
                                    if (this.ActionQueue.Count > 0)
                                    {
                                        this.ActionQueue.Dequeue().Start();
                                    }
                                    else
                                    {
                                        //this.StatusMessage = "Closing...";
                                        this.CloseCommService();
                                    }
                                    // ????????????????????????????????????????????????????????????????????????  needed?
                                    // Declare device has been through a basic inititialization
                                    //this._appShellModel.DeviceIsInitialized = true;
                                }
                                break;
                        }
                        break;
                    case CommunicationEvents.ConnectedAsClient:
                        this.IsCommunicating = true;
                        IsConnecting = false;
                        //CancelCommTimout();
                        if (this.ActionQueue.Count > 0)
                        {
                            this.ActionQueue.Dequeue().Start();
                        }
                        this.StatusMessage = "Connected";
                        break;
                    case CommunicationEvents.Disconnected:
                        this.CurrentOperation = CodesViewOperations.None;
                        CancelCommTimout();
                        this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
                        this._appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
                        // Reset RX timeout timer
                        if (!this.ErrorExists)
                        {
                            this.StatusMessage = string.Empty;
                            SendHapticFeedback();
                        }
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            this.IsCommunicating = false;
                            this.IsBusy = false;
                        });
                        ActivityLEDOff();
                        break;
                    case CommunicationEvents.Error:
                        CloseCommService();

                        lock (dLock)
                        {
                            if (!string.IsNullOrEmpty(this._appShellModel.CommunicationService.DeviceName)  && 
                                    this.IsConnecting && this.ConnectTimeoutCount++ < Constants.DEFAULT_COMM_CONNECT_RETRY_COUNT)
                            {
                                this.EmptyGridMessage = "Retrying...";
                                switch(CurrentOperation)
                                {
                                    case CodesViewOperations.ClearDTC:
                                        this.ClearDTC(false).Start();
                                        break;
                                    case CodesViewOperations.ReadDTC:
                                        ReadDTC(false).Start();
                                        break;
                                }
                                return;
                            }
                        }
                        ActivityLEDOff();
                        this.OBD2Adapter.ClearQueue();
                        if (!string.IsNullOrEmpty(e.Description))
                        {
                            this.EmptyGridMessage = this.StatusMessage = e.Description;
                        }
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            this.ErrorExists = true;
                            this.IsBusy = false;
                        });
                        SendHapticFeedback();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }




        protected override void OnAdapterEvent(object sender, OBD2AdapterEventArgs e)
        {
            _appShellModel.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug,
                                    $"OnAdapterEvent() {e.EventType.ToString()}, SelectedManufacturer: {this.SelectedManufacturer}");
                try
                {
                    string DTCTypeDescriptor = "";
                    switch (e.EventType)
                    {
                        case OBD2AdapterEventTypes.Update:
                            try
                            {
                                System.Reflection.PropertyInfo propInf = this.GetType().GetProperty(e.propertyName);
                                propInf.SetValue(this, e.dataObject);
                            }
                            catch (Exception)
                            {

                            }

                            break;
                        case OBD2AdapterEventTypes.ConfirmedDTCData:
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                try
                                {

                                    var newList = (ObservableCollection<OBDIIFaultCode>)e.dataObject;

                                    DTCTypeDescriptor = Constants.DTC_CODES_GROUP_CONFIRMED;
                                    var existingList = this.DTCCodes.Where(l => l.Name == DTCTypeDescriptor).FirstOrDefault();

                                    // Ensure a list exists
                                    if (existingList == null)
                                    {
                                        this.DTCCodes.Add(new DTCGroup(DTCTypeDescriptor));
                                        existingList = this.DTCCodes.Where(l => l.Name == DTCTypeDescriptor).FirstOrDefault();
                                    }

                                    //existingList.Name = "junk";
                                    // add new codes to exising list
                                    existingList.Clear();
                                    newList.ToList().ForEach(code => existingList.Add(code));
                                    //OnPropertyChanged("DTCCodes");
                                    _appShellModel.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug,
                                                $"OnAdapterEvent() {newList.Count} confirmed dtcs reported");


                                }
                                catch (Exception ex)
                                {

                                }
                            });
                            break;
                        case OBD2AdapterEventTypes.PendingtDTCData:
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                try
                                {

                                    var newList = (ObservableCollection<OBDIIFaultCode>)e.dataObject;



                                    DTCTypeDescriptor = Constants.DTC_CODES_GROUP_PENDING;
                                    var existingList = this.DTCCodes.Where(l => l.Name == DTCTypeDescriptor).FirstOrDefault();

                                    // Ensure a list exists
                                    if (existingList == null)
                                    {
                                        this.DTCCodes.Add(new DTCGroup(DTCTypeDescriptor));
                                        existingList = this.DTCCodes.Where(l => l.Name == DTCTypeDescriptor).FirstOrDefault();
                                    }

                                    // add new codes to exising list
                                    existingList.Clear();
                                    // get the confirmed dtc list...
                                    var confirmedList = this.DTCCodes.Where(l => l.Name == Constants.DTC_CODES_GROUP_CONFIRMED).FirstOrDefault();
                                    newList.ToList().ForEach(code =>
                                    {
                                        // Don't list 'pending' dtcs if they already exist in the confirmed list
                                        if (!confirmedList.Any(c => c.PIDValue == code.PIDValue))
                                        {
                                            existingList.Add(code);
                                        }
                                    });
                                    //var isEmpty = existingList.IsEmpty;

                                }
                                catch (Exception)
                                {

                                }
                            });
                            break;
                        case OBD2AdapterEventTypes.PermanentDTCData:
                            MainThread.BeginInvokeOnMainThread(() => {
                                try
                                {
                                    var newList = (ObservableCollection<OBDIIFaultCode>)e.dataObject;

                                    DTCTypeDescriptor = "Permanent";
                                    var existingList = this.DTCCodes.Where(l => l.Name == DTCTypeDescriptor).FirstOrDefault();

                                    // Ensure a list exists
                                    if (existingList == null)
                                    {
                                        this.DTCCodes.Add(new DTCGroup(DTCTypeDescriptor));
                                        existingList = this.DTCCodes.Where(l => l.Name == DTCTypeDescriptor).FirstOrDefault();
                                    }

                                    // add new codes to exising list
                                    existingList.Clear();
                                    newList.ToList().ForEach(code =>
                                    {
                                        existingList.Add(code);
                                    });
                                    //existingList.up
                                    // REMOVE DTC GROUPS THAT HAVE NO CODES
                                    //var toRemove = this.DTCCodes.Where(c => c.Count() == 0);
                                    //toRemove.ToList().ForEach(r => this.DTCCodes.Remove(r));

                                    if (!this.DTCCodes.Any(gr => gr.Count() > 0))
                                    {
                                        this.EmptyGridMessage = "Compiling...";
                                        // Preferences.Clear(Constants.PREFS_KEY_DTC_LIST);
                                    }
                                    else
                                    {
                                        OBD2Device.SystemReport.DTCList = null;
                                        OBD2Device.SystemReport.DTCList = this.DTCCodes.ToList().ConvertAll(o => new DTCTransportData() { Name = o.Name, DTCGroup = o });//.ConvertAll(grp=>new DTCGroupWrapper(grp));
                                                                                                                                                                         //var ugre = this.DTCCodes.ToList().ConvertAll(o => new DTCTransportData() { Name = o.Name, DTCGroup = o });//.ConvertAll(grp=>new DTCGroupWrapper(grp));
                                    }
                                    InvokeUpdate("DTCCodes");



                                }
                                catch (Exception)
                                {

                                }
                            });
                            break;
                        case OBD2AdapterEventTypes.Mode18DTCData:
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                try
                                {

                                    var newList = (ObservableCollection<OBDIIFaultCode>)e.dataObject;

                                    // get the confirmed dtc list...
                                    var storedList = this.DTCCodes.Where(l => l.Name == Constants.DTC_CODES_GROUP_STORED).FirstOrDefault();

                                    // get the confirmed dtc list...
                                    var confirmedList = this.DTCCodes.Where(l => l.Name == Constants.DTC_CODES_GROUP_CONFIRMED).FirstOrDefault();

                                    // get the pending dtc list...
                                    var pendingList = this.DTCCodes.Where(l => l.Name == Constants.DTC_CODES_GROUP_PENDING).FirstOrDefault();

                                    // add from the new Mode18 dtcs
                                    newList.ToList().ForEach(code =>
                                    {
                                        // Only add to the list if the pid value doesn't exist
                                        if (!storedList.Any(c => c.PIDValue == code.PIDValue))
                                        {
                                            if (!confirmedList.Any(c => c.PIDValue == code.PIDValue))
                                            {
                                                if (!pendingList.Any(c => c.PIDValue == code.PIDValue))
                                                {
                                                    storedList.Add(code);
                                                }
                                            }
                                        }
                                    });



                                }
                                catch (Exception)
                                {

                                }
                            });
                            break;
                        case OBD2AdapterEventTypes.Error:
                            this.ActionQueue.Clear();
                            this.OBD2Adapter.ClearQueue();
                            // this.ExtendedMessage = e.Description;
                            this.EmptyGridMessage = e.Description;
                            this.CloseCommService();
                            this.ErrorExists = true;
                            _appShellModel.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug,
                                                $"OnAdapterEvent() error: {e.Description}");

                        break;
                    }
                }
                catch (Exception)
                {

                }
        }

        private void ResetFields(bool clearVIN = true)
        {
            OBD2Device.SystemReport.Clear(clearVIN);

            OnPropertyChanged("VIN");
            OnPropertyChanged("DTCCount");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.DTCCodes.ToList().ForEach(l => l.Clear()); 
            });
        }

        protected SynchronizationContext syncContext;

        protected override OBD2DeviceAdapter OBD2Adapter { get; } = new OBD2DeviceAdapter();

        public string SelectedCommMethod
        {
            get => this._appShellModel.SelectedCommMethod;
            set { this._appShellModel.SelectedCommMethod = value; }
        }

        public bool IsBluetooth
        {
            get => this._appShellModel.IsBluetooth;
            //set { this._appShellModel.IsBluetooth = value; }
        }

        public int IPPort
        {
            get => this._appShellModel.IPPort;
            set { this._appShellModel.IPPort = value; }
        }

        public string IPAddress
        {
            get => this._appShellModel.IPAddress;
            set { this._appShellModel.IPAddress = value; }
        }

        private string communicationChannel = "<None>";
        public string CommunicationChannel
        {
            get { return communicationChannel; }
            set { SetProperty(ref communicationChannel, value); }
        }


        #region ClearDTCsCommand

        public ICommand ClearDTCsCommand => new Command(async () => {


            if (this.NeedYesNoPopup == null) return;
            var answer = await NeedYesNoPopup("Clear Diagnostic Trouble Codes", $"Clear DTCs and freeze frame data.{Environment.NewLine}{Environment.NewLine}Confirm?");
            if (!answer)
            {
                this.StatusMessage = "Clear DTCs Cancelled";
                return;
            }
            await Task.Run(() => { 
                if (this.adService.DoAdPopup(true))
                {
                    // assign a method to call after the ad
                    this.adService.PostAdTask = this.ClearDTC();
                    // the post-ad callback where PostAdAction will be called.
                    this.adService.PopupClosed -= AdClosed;
                    this.adService.PopupClosed += AdClosed;
                    return;
                }
                ResetCommTimout();
                this.ClearDTC().Start();
            });

        }, ()=>CanClearDTCs);

        private Task ClearDTC(bool resetRetries = true)
        {
            return new Task(() => {
                ResetCommTimout();
                this.IsCommunicating = true;
                IsConnecting = true;
                this.CurrentOperation = CodesViewOperations.ClearDTC;
                this.ErrorExists = false;
                this.FreezeFrameDTC = 0;
                this.ResetFields(false);

                //this.statusMessage = this.EmptyGridMessage = "Clearing DTCs...";
                this.EmptyGridMessage = this.StatusMessage = $"Connecting to {_appShellModel.CommunicationService.DeviceName}";
                this.ActionQueue.Clear();
               // this.ActionQueue.Enqueue(ResetDevice());
                this.ActionQueue.Enqueue(InitializeDevice());
                // Add anonymous ClearDTCs command to the ActionQueue
                this.ActionQueue.Enqueue(new Task(async () => {
                    this.OBD2Adapter.CreateQueue(QueueSets.ClearDTCs); ;
                    var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                    if (nextRequest != null)
                    {
                        await this.SendRequest(nextRequest);
                    }
                }));

                ResetCommTimout();
                this.IsCommunicating = true;
                this._RetryCounter = 0;
                this.ResetFields();
                if (resetRetries)
                {
                    ConnectTimeoutCount = 0;
                }
                this.Open();
            });
        }

        #endregion ClearDTCsCommand

        private CodesViewOperations CurrentOperation = CodesViewOperations.None;

        #region ReadDTCsCommand

        public ICommand ReadDTCsCommand => new Command(async () => {

            // Ensure parsing mechanism knows which model to use
            this.OBD2Adapter.Manufacturer = this.SelectedManufacturer;

            IsCommunicating = true;

            // Start 'No Response' timer...
            ResetCommTimout();


            if (this.adService.DoAdPopup())
            {
                // assign a method to call after the ad
                this.adService.PostAdTask = this.ReadDTC();
                // the post-ad callback where PostAdAction will be called.
                this.adService.PopupClosed -= AdClosed;
                this.adService.PopupClosed += AdClosed;
                return;
            }

           // await this.ReadDTC();//.ConfigureAwait(false);
            this.ReadDTC().Start();
        }, () => !IsCommunicating);

        private Task ReadDTC(bool resetRetries = true)
        {

            return new Task(() => {
              //  this.EmptyGridMessage = this.StatusMessage = Constants.STRING_CONNECTING;
                this.EmptyGridMessage = this.StatusMessage = $"Connecting to {_appShellModel.CommunicationService.DeviceName}";

                // await Task.Delay(0);
                //await ResetFields();
                IsConnecting = true;
                ResetCommTimout();
                IsCommunicating = true;
                this.CurrentOperation = CodesViewOperations.ReadDTC;
                this.ErrorExists = false;
                this.FreezeFrameDTC = 0;
                MainThread.InvokeOnMainThreadAsync(() => {

                    // Needed to get the list updating when the list is cleared, a change is detected (I think)
                    this.DTCCodes[0].Add(new OBDIIFaultCode());
                    this.ResetFields();
                }).Wait();
                         
                this.OBD2Adapter.ClearQueue();
                this.ActionQueue.Clear();
                //this.ActionQueue.Enqueue(ResetDevice());
                this.ActionQueue.Enqueue(InitializeDevice());
                this.ActionQueue.Enqueue(ReadDTCFromVehicle());
                this._RetryCounter = 0;
                if(resetRetries)
                {
                    ConnectTimeoutCount = 0;
                }
                this.Open();
            });
        }

        protected Task ReadDTCFromVehicle()
        {
            return new Task(async () => {
                ResetCommTimout();
                this.EmptyGridMessage = "Reading DTCs...";

                this.OBD2Adapter.CreateQueue(QueueSets.DTCReport);
                var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                if (nextRequest != null)
                {
                    await this.SendRequest(nextRequest);
                }
            });
        }

        #endregion ReadDTCsCommand


        #region Device Reset


        private Task ResetDevice()
        {

            return new Task(async () => {
                ResetCommTimout();

                this.OBD2Adapter.CreateQueue(QueueSets.DeviceReset);
                var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                if (nextRequest != null)
                {
                    await this.SendRequest(nextRequest);
                    return;
                }
            });
        }

        #endregion Device Reset


        #region InitializeDeviceCommand

        private Task InitializeDevice()
        {
            return new Task(async () => {
                ResetCommTimout();
                StatusMessage = "Initializing...";
                this.OBD2Adapter.CreateQueue(QueueSets.Initialize); ;
                var nextRequest = this.OBD2Adapter.GetNextQueuedRequest();
                if (nextRequest != null)
                {
                    await this.SendRequest(nextRequest);
                }
            });
        }

        #endregion InitializeDeviceCommand

        private async Task GoToFreezeFramePage()
        {

            if (this.ModelEvent != null)
            {
                this.IsBusy = true;
                // global indicator to discover pids - once at the page, model will read this
                Preferences.Set(Constants.APP_PROPERTY_KEY_INITIAL_VIEW, true);
                using (ViewModelEventArgs evt = new ViewModelEventArgs())
                {
                    evt.EventType = ViewModelEventEventTypes.NavigateTo;
                    evt.dataObject = "FreezeFramePage";
                    evt.Description = this.CurrentCANId;
                    this.ModelEvent(this, evt);
                }
            }



            //var qu = $"{Shell.Current.CurrentState.Location}/FreezeFrame?canid={this.CurrentCANId}";
            //await Shell.Current.GoToAsync(qu, false);

        }

        public ICommand GridCommand => new Command<GridEventArgs>(async (GridEventArgs arg) => {


            switch (arg.EventType)
            {
                case GridEventTypes.RowButtonClicked:
                    if (Constants.FORCE_LIMITS)
                    {
                        if (this.NeedYesNoPopup == null) return;
                        await NeedYesNoPopup("Freeze Frame Data", $"Freeze Frame Data in this version is limited to the first {Constants.APP_LIMIT_MAX_FF_PID_ROWS} pids so you can evaluate compatibility.{Environment.NewLine}{Environment.NewLine}Thank you for trying out OS OBDII Interface!", false);
                    }
                    await this.GoToFreezeFramePage();
                    break;
                case GridEventTypes.RowClicked:
                    break;
                default:
                    break;
            }

        });



        #region ViewFreezeFrameDataCommand

        public ICommand ViewFreezeFrameDataCommand => new Command(async () => {
            await this.GoToFreezeFramePage();
        });

        #endregion ViewFreezeFrameDataCommand


        #region NavigateHomeCommand

        public ICommand NavigateHomeCommand => new Command(async () => {
            this.FreezeFrameDTC = 0x0000;
            OBD2Device.SystemReport.DTCList.Clear();

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



           // await Shell.Current.GoToAsync("//Home", true);

        });

        #endregion NavigateHomeCommand



        public void Stop()
        {
         //   this.IsCommunicating = false;
            //this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
            //this._appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
            //this.adService.PopupClosed -= AdClosed;
            this.CloseCommService();
            //this.Initialize();

        }

        public virtual void Start()
        {

            this._appShellModel.LogService.AppendLog(Microsoft.Extensions.Logging.LogLevel.Debug, "CodesViewModel.Start()");


            OnPropertyChanged("SelectedManufacturer");
            // Ensure parsing mechanism knows which model to use
            this.OBD2Adapter.Manufacturer = this.SelectedManufacturer;
            //OnPropertyChanged("");
            this.IsCommunicating = true;
            this.IsCommunicating = false;
            this.DataIsTransmitting = false;
            this.ErrorExists = false;
            if (Preferences.ContainsKey(Constants.APP_PROPERTY_KEY_INITIAL_VIEW))
            {
                // Clear the flag
                Preferences.Remove(Constants.APP_PROPERTY_KEY_INITIAL_VIEW);

                this.Initialize();
            }


            this.IsBusy = false;

            try
            {


                //OBD2Device.SystemReport.DTCList.ForEach(dataPacket => {
                //    DTCGroup codeGroup = this.DTCCodes.FirstOrDefault(dc => string.Compare(dc.Name, dataPacket.Name) == 0);
                //    if (codeGroup == null) return; // only exits this iteration...
                //    // add the dtc
                //    dataPacket.DTCGroup.ToList().ForEach(fault => {
                //        codeGroup.Add(new OBD2FaultCode(fault));
                //    });
                //});

                //this.FreezeFrameDTC = OBD2Device.SystemReport.FreezeFrameDTC;
            }
            catch (Exception)
            {

            }
            //CancelCommTimout();
            //CloseCommService();
            // ul.CopyTo(this.DTCCodes);
            //MainThread.BeginInvokeOnMainThread(() =>
            //{

            //});
        }
        public void Initialize()
        {
       //     CloseCommService();
            OnPropertyChanged("FreezeFrameDTC");
            OBD2Device.SystemReport.Clear();
            this.EmptyGridMessage = Constants.STRING_NO_DATA;
            this.ResetFields();
        }

        protected StringBuilder rawStringData = new StringBuilder();

        protected void SetStatusText(string text = null)
        {
            if (!String.IsNullOrEmpty(text))
            {
                this.StatusMessage = text;
                return;
            }
            if (this._appShellModel.CommunicationService.IsConnected)
            {
                this.StatusMessage = "** Connected **";
            }
            else
            {
                this.StatusMessage = "Disconnected";
            }
        }

    }
}