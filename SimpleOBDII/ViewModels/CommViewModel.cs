using OS.OBDII.Models;
using OS.Communication;
using OS.OBDII.Interfaces;
using System.Text;

namespace OS.OBDII.ViewModels
{
    public abstract class CommViewModel : BaseViewModel_AdSupport
    {
        protected int _RetryCounter = 0;
        protected int ConnectTimeoutCount = 0;

        public bool ErrorExists
        {
            get { return errorExists; }
            set
            {
                SetProperty(ref errorExists, value);
            }
        }
        private bool errorExists = false;

        private IOBDIICommonUI _appShellModel = null;

        public CommViewModel(IOBDIICommonUI appShell) : base(appShell)
        {
            if (appShell == null) throw new NullReferenceException("CommViewModel..ctor - appShell cannot be null");
            this._appShellModel = appShell;
        }

        protected abstract OBD2DeviceAdapter OBD2Adapter { get; }
        protected abstract void OnAdapterEvent(object sender, OBD2AdapterEventArgs e);
        protected StringBuilder rawStringData = new StringBuilder();
        protected StringBuilder sb = new StringBuilder();

        protected virtual async Task OnCommunicationEvent(object sender, DeviceEventArgs e)
        {
                string nextRequest;
                switch (e.Event)
                {
                    case CommunicationEvents.Receive:
                    case CommunicationEvents.ReceiveEnd:
                    switch (this.OBD2Adapter.CurrentQueueSet)
                    {
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
                                    case DeviceRequestType.ISO_SetSlowInitAddress:
                                        // Set the ISO Init Address...
                                        await this.SendRequest($"{nextRequest}{this._appShellModel.KWPInitAddress}{Constants.CARRIAGE_RETURN}");
                                        break;
                                    case DeviceRequestType.SetISOBaudRate:
                                        // Set the ISO Init Address...
                                        await this.SendRequest($"{nextRequest}{ISOBaudRates.Items[this._appShellModel.ISOBaudRate]}{Constants.CARRIAGE_RETURN}");
                                        break;
                                    case DeviceRequestType.SET_OBD1Wakeup:
                                        // Whether to turn on the OBDI/KWP wakeup messagingl...
                                        // 0x5C, 92 is the default time value according to ELM327 specs - approx. 3 sec
                                        await this.SendRequest($"{nextRequest}{(this._appShellModel.UseKWPWakeup ? "5C" : "00")}{Constants.CARRIAGE_RETURN}");
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
                                switch (this.OBD2Adapter.CurrentRequest)
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
                }




        }
        

        /// <summary>
        /// Returns true if a connect attempt was started
        /// </summary>
        /// <returns></returns>
        protected bool TryConnect(out string descriptor)
        {
            descriptor = string.Empty;
            this.ResetCommTimout();
            LEDOn();
            if (ConnectTimeoutCount++ < Constants.DEFAULT_COMM_CONNECT_RETRY_COUNT)
            {
                descriptor = Constants.STRING_CONNECTING;
                var success = this._appShellModel.OpenCommunicationChannel();
                
                return success;
            }
            return false;
        }
        /// <summary>
        /// Starts and enables RX timeout timer
        /// </summary>
        protected void ResetCommTimout(bool clearRetries = true)
        {
            // Reset RX timeout timer
            this._CommTimer.Change(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT, Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT);

            if (!clearRetries) return;
            this._RetryCounter = 0;
        }
        /// <summary>
        /// Stops RX timeout timer
        /// </summary>
        protected void CancelCommTimout()
        {
            // Reset RX timeout timer
            this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this._RetryCounter = 0;
        }
        protected virtual void Open()
        {
            //if(string.Compare(Constants.MSG_NOT_SELECTED , this._appShellModel.CommunicationChannel) == 0)
            //{
            //    StatusMessage = Constants.COMMUNICATION_DEVICE_NOT_SETUP;
            //    this.ErrorExists = true;
                
            //    return;
            //}
            try
            {
                this.ErrorExists = false;
                // ensure data response timer is disabled during connect
                //this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);
                CancelCommTimout();
                


                //this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
                this._appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;

                this.OBD2Adapter.OBD2AdapterEvent += OnAdapterEvent;
                this._appShellModel.CommunicationService.CommunicationEvent += OnCommunicationEvent;

                this._RetryCounter = 0;
                //this.ConnectTimeoutCount = 0;
                this.IsConnecting = true;

                //}
                //catch (Exception)
                //{
                //    StatusMessage = Constants.STRING_BUSY;
                //    AppShellViewModel.Instance.CommunicationService.Close();
                //    AppShellViewModel.Instance.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
                //    this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
                //    LEDOff();
                //}
                //try
                //{
                if (!this.TryConnect(out this.statusMessage))
                {
                    
                    //throw new Exception($"Resource not available: {AppShellViewModel.Instance.CommunicationChannel}");
                }
                OnPropertyChanged("StatusMessage");

            }
            catch (Exception e)
            {
                //CancelCommTimout();
                //this.IsCommunicating = false;
                //this.IsConnecting = false;
                

                StatusMessage = Constants.STRING_BUSY;
                this._appShellModel.CommunicationService.Close();
                this._appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
                this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
                ActivityLEDOff();
               
            }
            finally
            {
            }
        }

        public virtual void CloseCommService()
        {
            try
            {
                // Halt timeouts...
                CancelCommTimout();
                

                this.OBD2Adapter.OBD2AdapterEvent -= OnAdapterEvent;
                this._appShellModel.CommunicationService.CommunicationEvent -= OnCommunicationEvent;
                this._appShellModel.CommunicationService?.Close();

                this.OBD2Adapter?.ClearQueue();
                this.ActionQueue?.Clear();
                //  if(!AppShellViewModel.Instance.CommunicationService.IsConnected)
                //   {

                this.IsCommunicating = false;
                this.IsBusy = false;

                if (!this.ErrorExists)
                {
                    this.StatusMessage = string.Empty;
                    //this.EmptyGridMessage = Constants.NO_DATA_STRING;
                }
                ActivityLEDOff();
                //this._appShellModel.SendHapticFeedback();
            }
            catch(Exception) { 
            }
        }


        private object sendLock = new object();
        protected virtual async Task SendRequest(string data)
        {
            LEDOn();
            lock (sendLock)
            {
                this._LastSentCommand = data;
                this._appShellModel.CommunicationService.Send(data).ConfigureAwait(false);
            }

        }
        protected string _LastSentCommand = string.Empty;


    }
}
