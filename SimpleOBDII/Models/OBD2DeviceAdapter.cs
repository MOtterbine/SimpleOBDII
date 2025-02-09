using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using OS.OBDII.Interfaces;
using OS.OBDII.Manufacturers;

namespace OS.OBDII.Models
{
    /// <summary>
    /// Underlying abstraction for the actual OBD2 device
    /// </summary>
    public class OBD2DeviceAdapter
    {
        protected char carriageReturn = (char)0x0D;
        private char[] dataEndTrimChars = new char[] { '\n', '\r', '>' };
        
        public List<ECU> ECUList { get; private set; } = new List<ECU>();

        #region Events

        public event OBD2AdapterEvent OBD2AdapterEvent;
        private void FireErrorEvent(string description)
        {
            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //});
            using (OBD2AdapterEventArgs evt = new OBD2AdapterEventArgs())
            {
                evt.EventType = OBD2AdapterEventTypes.Error;
                evt.Description = description;
                this.FireAdapterEvent(evt);
            }
        }
        private void FireAdapterUpdateEvent(string propertyName, object value)
        {
            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //});

            using (OBD2AdapterEventArgs evt = new OBD2AdapterEventArgs())
            {
                evt.EventType = OBD2AdapterEventTypes.Update;
                evt.propertyName = propertyName;
                evt.dataObject = value;
                evt.Description = $"Update {propertyName}";
                this.FireAdapterEvent(evt);
            }
        }
        private void FireDTCDataEvent(IList<OBDIIFaultCode> codes, OBD2AdapterEventTypes eventType)
        {

          //  MainThread.BeginInvokeOnMainThread(() =>
         //   {
            using (OBD2AdapterEventArgs evt = new OBD2AdapterEventArgs())
            {
                evt.EventType = eventType;
                evt.dataObject = new ObservableCollection<OBDIIFaultCode>(codes);
                evt.Description = $"DTC Codes";
                this.FireAdapterEvent(evt);
            }
         //   });
        }
        private void FireAdapterReadinessMonitorsEvent(Readiness readinessObject)
        {
            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //});
            using (OBD2AdapterEventArgs evt = new OBD2AdapterEventArgs())
            {
                evt.EventType = OBD2AdapterEventTypes.ReadinessMonitors;
                evt.dataObject = readinessObject;
                evt.Description = $"Readiness Monitors";
                this.FireAdapterEvent(evt);
            }
        }
        private void FireAdapterEvent(OBD2AdapterEventTypes eventType, string message = null)
        {
            using (OBD2AdapterEventArgs evt = new OBD2AdapterEventArgs())
            {
                evt.EventType = eventType;
                evt.Description = message;
                this.FireAdapterEvent(evt);
            }
        }
        protected void FireAdapterEvent(OBD2AdapterEventArgs e)
        {
          //  MainThread.BeginInvokeOnMainThread(() =>
          //  {
                if (this.OBD2AdapterEvent != null)
                {
                    OBD2AdapterEvent(this, e);
                }
          //  });
        }

        #endregion Events

        public DeviceRequestType CurrentRequest { get; set; } = DeviceRequestType.None;

        public IVehicleModel Manufacturer {
            get;
            set;
        }

        private string VIN { get; set; }

        protected Queue<DeviceRequestType> obd2Requests = new Queue<DeviceRequestType>();
        protected Queue<OBD2Command> obd2Commands = new Queue<OBD2Command>();
        public OBD2DeviceAdapter(IVehicleModel manufacturer = null)
        {
            //if (manufacturer == null)
            //{
            //    this.Manufacturer = new OBD2FaultCodes_generic();
            //}
        }
        public QueueSets CurrentQueueSet { get; set; } = QueueSets.InitializeCAN;
        public void ClearQueue()
        {
            this.obd2Requests.Clear();
            this.obd2Commands.Clear();

        }


        /// <summary>
        /// Loads up the queue with requests to accomplish the target of the queueSet
        /// </summary>
        /// <param name="queueSet">An enum <cref>QueueSets</cref></param>
        public void CreateQueue(QueueSets queueSet)
        {

            this.obd2Requests.Clear();
            this.CurrentQueueSet = queueSet;

            List<DeviceRequestType> requestTypesToEnqueue = null;
            List<OBD2Command> commandsToEnqueue = null;
            // will be altered when necessary
            OBD2Device.CurrentRequestSize = 2;

            switch (queueSet)
            {

                // NEW WAY
                case QueueSets.DTCReport:
                    commandsToEnqueue = InstructionSets.DTCReport;
                    break;
                case QueueSets.GetSupportedPids:
                    commandsToEnqueue = InstructionSets.GetSupportedPids;
                    break;
                case QueueSets.GetSupportedMonitorIds:
                    commandsToEnqueue = InstructionSets.GetSupportedMonitorIds;
                    break;
                case QueueSets.GetO2SensorLocations13:
                    commandsToEnqueue = InstructionSets.GetLocations13;
                    break;
                case QueueSets.GetO2SensorLocations1D:
                    commandsToEnqueue = InstructionSets.GetLocations1D;
                    break;
                case QueueSets.GeneralSnapshot:
                    commandsToEnqueue = InstructionSets.GeneralSnapshot1;
                    break;
                case QueueSets.SendCANMessage:
                    commandsToEnqueue = InstructionSets.SendCANData;
                    break;
                case QueueSets.DetectSystemProtocolID:
                    commandsToEnqueue = InstructionSets.DetectSystemProtocol;
                    break;
                case QueueSets.InitializeForUserPIDS:
                    commandsToEnqueue = InstructionSets.InitializeForUserPIDS;
                    break;


                // OLD WAY
                case QueueSets.CANMonitor:
                    requestTypesToEnqueue = InstructionSets.StartCANMonitor;
                    break;
                case QueueSets.Initialize:
                    requestTypesToEnqueue = InstructionSets.Initialize;
                    break;
                case QueueSets.InitializeWithHeaders:
                    requestTypesToEnqueue = InstructionSets.InitializeHeadersOn;
                    break;
                case QueueSets.Live:
                    //requestTypesToEnqueue = InstructionSets.IMMonitors;
                    break;
                case QueueSets.IMMonitors:
                    requestTypesToEnqueue = InstructionSets.IMMonitors;
                    break;
                case QueueSets.IMMonitorsDriveCyle:
                    requestTypesToEnqueue = InstructionSets.IMMonitorsDriveCyle;
                    break;
                case QueueSets.ClearDTCs:
                    requestTypesToEnqueue = InstructionSets.ClearDTCs;
                    break;
                case QueueSets.InitializeCAN:
                    requestTypesToEnqueue = InstructionSets.InitializeCAN;
                    break;
                case QueueSets.InitializeCANMonitor:
                    requestTypesToEnqueue = InstructionSets.InitializeCANMonitor;
                    break;
                case QueueSets.SetCANID:
                    requestTypesToEnqueue = InstructionSets.SetCANID;
                    break;
                case QueueSets.DeviceReset:
                    requestTypesToEnqueue = InstructionSets.DeviceReset;
                    break;
            }

            // NEW WAY: Load up commands from chosen queue set
            if(commandsToEnqueue != null)
            {
                commandsToEnqueue.ForEach(obd2Command =>
                {
                    this.obd2Commands.Enqueue(obd2Command);
                });
            }

            // OLD WAY: Load up commands from chosen queue set
            if(requestTypesToEnqueue != null)
            {
                requestTypesToEnqueue.ForEach(obd2Command =>
                {
                    this.obd2Requests.Enqueue(obd2Command);
                });
            }
        }

        private object reqLock = new object();
        public string GetNextQueuedRequest(bool dequeue = true, bool appendCR = true)
        {
            lock (reqLock)
            {
                if (this.obd2Requests.Count < 1)
                {
                    this.CurrentQueueSet = QueueSets.None;

                    return null;
                }

                string lastChar = appendCR ? Constants.CARRIAGE_RETURN.ToString() : "";
                this.CurrentRequest = this.obd2Requests.Peek();
                if (dequeue) return $"{OBD2Device.ELM327CommandDictionary[this.obd2Requests.Dequeue()].Code}{lastChar}";
                return $"{OBD2Device.ELM327CommandDictionary[this.CurrentRequest].Code}{lastChar}";
            }
        }

        public string GetNextQueuedCommand(bool dequeue = true, bool appendCR = true, string argument = null)
        {
            lock (reqLock)
            {
                if (this.obd2Commands.Count < 1)
                {
                    this.CurrentQueueSet = QueueSets.None;
                    return null;
                }
                OBD2Command nextCmd = this.obd2Commands.Peek();
                this.CurrentRequest = nextCmd.RequestType;
                if (dequeue) return OBD2Device.CreateRequest(this.obd2Commands.Dequeue().ServiceMode, this.CurrentRequest, appendCR, argument);
                return OBD2Device.CreateRequest(nextCmd.ServiceMode, this.CurrentRequest, appendCR, argument);
            }
        }

        private bool CheckForError(string data, out string errMsg)
        {
            errMsg = "No Error";
            return OBD2Device.CheckForDongleMessages(data, out errMsg, true);
        }
        private Readiness readinessObject = null;
        private StringBuilder tempStringBuilder = new StringBuilder();
        private StringBuilder CompositeReport = new StringBuilder();

        public IEnumerable<uint> ConfirmedDTCs = new List<uint>() { };
        public IEnumerable<uint> PendingDTCs = new List<uint>() { };
        public IEnumerable<uint> PermanentDTCs = new List<uint>() { };

        private string[] inputStringArray = null;

        private string tempString = string.Empty;
        public int CurrentLivePIDRequest { get; set; } = (int)OBD2PIDS.PIDS.None;
        OBD2PIDS PIDS = new OBD2PIDS();
        PID p = null;
        object tempVal = null;
        List<OBDIIFaultCode> codeList = new List<OBDIIFaultCode>();
        byte data;

        public string ParseResponse(string rawStringData)
        {
            tempVal = null;
            p = null;
            tempString = string.Empty;
            readinessObject = null;
            if (string.IsNullOrEmpty(rawStringData)) return "";
            try 
            { 
               // inputStringArray = System.Text.RegularExpressions.Regex.Replace(rawStringData.TrimEnd(dataEndTrimChars), @"(^a-zA-Z|\r\r\r|\r\r|\r\n|\n\r|\r|\n|'>')", "\r").Split('\r');
                //inputStringArray = System.Text.RegularExpressions.Regex.Replace(rawStringData, @"([^']{,10}|\r\r|\r|\n|>)", "\r").Split('\r');
                inputStringArray = System.Text.RegularExpressions.Regex.Replace(rawStringData, @"(^a-zA-Z|\r\r\r|\r\r|\r\n|\n\r|\r|\n|>)", "\r").Split('\r');

            }
            catch (Exception)
            {
                return string.Empty;
            }

            if (inputStringArray.Length < 1)
            {
                return string.Empty;
            }
            
            double dResult  = 0.0;
            int i = 0;
            try
            {
                switch (this.CurrentQueueSet)
                {
                    case QueueSets.DetectSystemProtocolID:
                        switch(CurrentRequest)
                        {
                            case DeviceRequestType.GetSystemProtocolID:
                                try
                                {
                                    int val;
                                    if(inputStringArray[0].Substring(0,1) =="A") // if the first value is 'A' then then 
                                    {
                                        if (int.TryParse(inputStringArray[0].Substring(1,1), out val))
                                        {
                                            this.FireAdapterUpdateEvent("SystemProtocolID", val);
                                        }
                                    }
                                    else
                                    {
                                        if (int.TryParse(inputStringArray[0].Substring(0,1), out val))
                                        {
                                            this.FireAdapterUpdateEvent("SystemProtocolID", val);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    //FireAdapterUpdateEvent("StatusText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                        }
                        return string.Empty;
                    case QueueSets.GetO2SensorLocations13:
                        foreach (string s in inputStringArray)
                        {
                            if (s.Length < OBD2Device.DataPositionOffset + 6) continue;
                        
                            OBD2Device.SystemReport.HasBank1 = false;
                            OBD2Device.SystemReport.HasBank2 = false;
                            OBD2Device.SystemReport.HasBank3 = false;
                            OBD2Device.SystemReport.HasBank4 = false;

                            data = byte.Parse(s.Substring(OBD2Device.DataPositionOffset + 4, 2), System.Globalization.NumberStyles.HexNumber);
                            // Bank 1
                            for (i = 0; i < 4; i++)
                            {
                                if (((data >> i) & 0x01) == 0) continue;
                                OBD2Device.SystemReport.HasBank1 = true;
                            }
                            // Bank 2
                            for (i = 4; i < 8; i++)
                            {
                                if (((data >> i) & 0x01) == 0) continue;
                                OBD2Device.SystemReport.HasBank2 = true;
                            }

                            if (OBD2Device.SystemReport.HasBank1)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations13, dataObject = "ST Fuel Trim - Bank 1", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_SHRTFT13 } });
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations13, dataObject = "LT Fuel Trim - Bank 1", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_LONGFT13 } });
                            }
                            if (OBD2Device.SystemReport.HasBank2)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations13, dataObject = "ST Fuel Trim - Bank 2", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_SHRTFT24 } });
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations13, dataObject = "LT Fuel Trim - Bank 2", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_LONGFT24 } });
                            }
                        }
                        return string.Empty;
                    case QueueSets.GetO2SensorLocations1D:
                        foreach (string s in inputStringArray)
                        {
                            if (s.Length < OBD2Device.DataPositionOffset + 6) continue;

                            OBD2Device.SystemReport.HasBank1 = false;
                            OBD2Device.SystemReport.HasBank2 = false;
                            OBD2Device.SystemReport.HasBank3 = false;
                            OBD2Device.SystemReport.HasBank4 = false;

                            data = byte.Parse(s.Substring(OBD2Device.DataPositionOffset + 4, 2), System.Globalization.NumberStyles.HexNumber);
                            // Bank 1
                            for (i = 0; i < 2; i++)
                            {
                                if (((data >> i) & 0x01) == 0) continue;
                                OBD2Device.SystemReport.HasBank1 = true;
                            }
                            // Bank 2
                            for (i = 2; i < 4; i++)
                            {
                                if (((data >> i) & 0x01) == 0) continue;
                                OBD2Device.SystemReport.HasBank2 = true;
                            }
                            // Bank 3
                            for (i = 4; i < 6; i++)
                            {
                                if (((data >> i) & 0x01) == 0) continue;
                                OBD2Device.SystemReport.HasBank3 = true;
                            }
                            // Bank 4
                            for (i = 6; i < 8; i++)
                            {
                                if (((data >> i) & 0x01) == 0) continue;
                                OBD2Device.SystemReport.HasBank4 = true;
                            }

                            if (OBD2Device.SystemReport.HasBank1)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "STFT Bank 1", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_SHRTFT13 } });
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "LTFT Bank 1", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_LONGFT13 } });
                            }
                            if (OBD2Device.SystemReport.HasBank2)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "STFT Bank 2", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_SHRTFT24 } });
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "LTFT Bank 2", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_LONGFT24 } });
                            }
                            if (OBD2Device.SystemReport.HasBank3)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "STFT Bank 3", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_SHRTFT24 } });
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "LTFT Bank 3", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_LONGFT24 } });
                            }
                            if (OBD2Device.SystemReport.HasBank4)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "STFT Bank 4", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_SHRTFT24 } });
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.O2Locations1D, dataObject = "LTFT Bank 4", Data = new byte[] { (byte)OBD2PIDS.PIDS.OBD2_LONGFT24 } });
                            }
                        }
                        return string.Empty;
                    case QueueSets.Live:
                        // HEADERS SHOULD BE OFF IN LIVE MODE
                        foreach (string s in inputStringArray)
                        {
                            try
                            {
                                p = (PID)(PIDS[(OBD2PIDS.PIDS)CurrentLivePIDRequest]);// as ICloneable).Clone();
                                tempVal = 0x00;
                                // if (s.Length < 11) continue; // no data or useful message, goto next
                                if (s.Length < 6) continue; // no data or useful message, goto next, assumes headers/CANID is off

                                // tempVal = p.function(s.Substring(9));
                                tempVal = p.function(s.Substring(4)); // skip over two response bytes
                            }
                            catch (Exception ex)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = null });
                                //var kk = ex.Message;
                                continue;
                            }
                            try
                            {
                                if (p.Value != null)
                                {
                                    if (p.ValueType == typeof(decimal))
                                    {

                                        tempVal = System.Math.Round(Convert.ToDouble(tempVal), p.DecimalPlaces);
                                    }
                                    else if (p.ValueType == typeof(decimal[]))
                                    {
                                        foreach (double d in (double[])tempVal)
                                        {
                                            tempVal = System.Math.Round(Convert.ToDouble(tempVal), p.DecimalPlaces);
                                        }
                                    }
                                    else// if (p.ValueType == typeof(O2PID))
                                    {
                                        p.Name = CurrentLivePIDRequest.ToString();
                                    }

                                    var t = tempVal;
                                    byte c = Convert.ToByte(p.Code);

                                    
                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = t, Data = new byte[] { c } });
                                    return string.Empty; // cheap way to exit...take only the first ecu that responds...
                                }

                            }
                            catch (Exception ex)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = null });
                                //var kk = ex.Message;
                            }
                        }
                        return string.Empty;
                    case QueueSets.FreezeFrameData:
                        bool dataRcvd = false;
                        // HEADERS SHOULD BE OFF IN LIVE MODE
                        foreach (string s in inputStringArray)
                        {
                            try
                            {
                                p = (PID)(PIDS[(OBD2PIDS.PIDS)CurrentLivePIDRequest]);// as ICloneable).Clone();
                                tempVal = 0x00;
                                // if (s.Length < 11) continue; // no data or useful message, goto next
                                if (s.Length < 8) continue; // no data or useful message, goto next, assumes headers/CANID is off

                                // tempVal = p.function(s.Substring(9));
                                tempVal = p.function(s.Substring(6)); // skip over 4 response bytes
                            }
                            catch (Exception ex)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = null });
                                var kk = ex.Message;
                                continue;
                            }
                            try
                            {
                                if (p.Value != null)
                                {
                                    if (p.ValueType == typeof(decimal))
                                    {
                                        tempVal = System.Math.Round(Convert.ToDouble(tempVal), p.DecimalPlaces);
                                    }
                                    else if (p.ValueType == typeof(decimal[]))
                                    {
                                        foreach (double d in (double[])tempVal)
                                        {
                                            tempVal = System.Math.Round(Convert.ToDouble(tempVal), p.DecimalPlaces);
                                        }
                                    }
                                    else
                                    {
                                        p.Name = CurrentLivePIDRequest.ToString();
                                    }
                                    dataRcvd = true;
                                    var t = tempVal;
                                    byte c = Convert.ToByte(p.Code);


                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = t, Data = new byte[] { c } });
                                    return string.Empty; // cheap way to exit...take only the first ecu that responds...
                                }

                            }
                            catch (Exception ex)
                            {
                                this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = null });
                                var kk = ex.Message;
                            }

                        }
                        if (!dataRcvd)
                        {
                            this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.LiveData, dataObject = null });
                        }
                        return string.Empty;
                    case QueueSets.GetSupportedMonitorIds:
                        switch (CurrentRequest)
                        {
                            case DeviceRequestType.HeadersOff:
                                try
                                {
                                    this.CurrentQueueSet = QueueSets.GetSupportedMonitorIds;
                                }
                                catch (Exception)
                                {
                                    //        //FireAdapterUpdateEvent("StatusText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPIDS_00:
                                this.ECUList.Clear();
                                try
                                {
                                    // Standard Operation for 'get pids' command
                                   // List<ECU> ecus = (List<ECU>)OBD2Device.OPBD2PIDSDictionary[CurrentRequest].function(inputStringArray);
                                    List<ECU> ecus = (List<ECU>)OBD2Device.GetSupportedMonitorIds(inputStringArray);

                                    // add supported pids to the class list
                                    ecus.ForEach(ecu => {
                                        // if this ecu doesn't exist in the main list, just add the whole thing
                                        if (!this.ECUList.Any(e => e.Id == ecu.Id))
                                        {
                                            this.ECUList.Add(ecu);
                                            // exit iteration function, not the loop...
                                            return;
                                        }
                                        this.ECUList.Where(e => e.Id == ecu.Id).ToList().ForEach(e => {
                                            e.SupportedTests.AddRange(ecu.SupportedTests);
                                        });
                                    });
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("LogText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPIDS_20:
                            case DeviceRequestType.OBD2_GetPIDS_40:
                            case DeviceRequestType.OBD2_GetPIDS_60:
                            case DeviceRequestType.OBD2_GetPIDS_80:
                            case DeviceRequestType.OBD2_GetPIDS_A0:
                            case DeviceRequestType.OBD2_GetPIDS_C0:
                                try
                                {
                                    //OBD2Device.DataPositionOffset += 2;
                                    // Standard Operation for 'get pids' command
                                    //List<ECU> ecus = (List<ECU>)OBD2Device.OPBD2PIDSDictionary[CurrentRequest].function(inputStringArray);
                                    List<ECU> ecus = (List<ECU>)OBD2Device.GetSupportedMonitorIds(inputStringArray);
                                    // add supported pids to the class list
                                    ecus.ForEach(ecu => {
                                        this.ECUList.Where(e => e.Id == ecu.Id).ToList().ForEach(e => {
                                            e.SupportedTests.AddRange(ecu.SupportedTests);
                                        });
                                    });

                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.SupportedPIDs, dataObject = ecus });
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("LogText", Constants.MSG_NOT_APPLICABLE);
                                }
                                finally
                                {
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPIDS_E0:
                                try
                                {
                                   // List<ECU> ecus = (List<ECU>)OBD2Device.OPBD2PIDSDictionary[CurrentRequest].function(inputStringArray);
                                    List<ECU> ecus = (List<ECU>)OBD2Device.GetSupportedMonitorIds(inputStringArray);

                                    // add supported pids to the class list
                                    ecus.ForEach(ecu => {
                                        this.ECUList.Where(e => e.Id == ecu.Id).ToList().ForEach(e => {
                                            e.SupportedTests.AddRange(ecu.SupportedTests);
                                        });
                                    });

                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.SupportedPIDs, dataObject = ecus });

                                    // indicate we're done
                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.SupportedPIDsLoaded });

                                    var a = this.ECUList;
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("LogText", Constants.MSG_NOT_APPLICABLE);
                                }
                                finally
                                {
                                }
                                break;
                        }
                        return string.Empty;
                    case QueueSets.GetSupportedPids:
                        switch (CurrentRequest)
                        {
                            case DeviceRequestType.HeadersOff:
                                try
                                {
                                    this.CurrentQueueSet = QueueSets.GetSupportedPids;
                                   // this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.SupportedPIDsLoaded });
                                }
                                catch (Exception)
                                {
                            //        //FireAdapterUpdateEvent("StatusText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPIDS_00:
                                this.ECUList.Clear();
                                try
                                {
                                    // Standard Operation for 'get pids' command
                                    List<ECU> ecus = (List<ECU>)OBD2Device.OPBD2PIDSDictionary[CurrentRequest].function(inputStringArray);

                                    // add supported pids to the class list
                                    ecus.ForEach(ecu => {
                                        // if this ecu doesn't exist in the main list, just add the whole thing
                                        if(!this.ECUList.Any(e => e.Id == ecu.Id))
                                        {
                                            this.ECUList.Add(ecu);
                                            // exit iteration function, not the loop...
                                            return;
                                        }
                                        this.ECUList.Where(e => e.Id == ecu.Id).ToList().ForEach(e => {
                                            e.SupportedPIDS.AddRange(ecu.SupportedPIDS);
                                        });
                                    });
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("LogText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPIDS_20:
                            case DeviceRequestType.OBD2_GetPIDS_40:
                            case DeviceRequestType.OBD2_GetPIDS_60:
                            case DeviceRequestType.OBD2_GetPIDS_80:
                            case DeviceRequestType.OBD2_GetPIDS_A0:
                            case DeviceRequestType.OBD2_GetPIDS_C0:
                                try
                                {
                                    //OBD2Device.DataPositionOffset += 2;
                                    // Standard Operation for 'get pids' command
                                    List<ECU> ecus = (List<ECU>)OBD2Device.OPBD2PIDSDictionary[CurrentRequest].function(inputStringArray);

                                    // add supported pids to the class list
                                    ecus.ForEach(ecu=> { 
                                        this.ECUList.Where(e => e.Id == ecu.Id).ToList().ForEach(e => {
                                            
                                            e.SupportedPIDS.AddRange(ecu.SupportedPIDS);
                                        });
                                    });

                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.SupportedPIDs, dataObject = ecus });
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("LogText", Constants.MSG_NOT_APPLICABLE);
                                }
                                finally
                                {
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPIDS_E0:
                                try
                                {
                                    List<ECU> ecus = (List<ECU>)OBD2Device.OPBD2PIDSDictionary[CurrentRequest].function(inputStringArray);
                                    
                                    // add supported pids to the class list
                                    ecus.ForEach(ecu => {
                                        this.ECUList.Where(e => e.Id == ecu.Id).ToList().ForEach(e => {
                                            e.SupportedPIDS.AddRange(ecu.SupportedPIDS);
                                        });
                                    });

                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.SupportedPIDs, dataObject = ecus });

                                    // indicate we're done
                                    //this.CurrentQueueSet = QueueSets.GetSupportedPids;
                                    this.FireAdapterEvent(new OBD2AdapterEventArgs() { EventType = OBD2AdapterEventTypes.SupportedPIDsLoaded });

                                    var a = this.ECUList;
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("LogText", Constants.MSG_NOT_APPLICABLE);
                                }
                                finally
                                {
                                }
                                break;
                        }
                        return string.Empty;
                    case QueueSets.ClearDTCs:
                    case QueueSets.IMMonitors:
                    case QueueSets.IMMonitorsDriveCyle:
                    case QueueSets.GeneralSnapshot:
                    case QueueSets.Initialize:
                        switch (CurrentRequest)
                        {
                            case DeviceRequestType.OBD2_ClearDTCs:
                                break;
                            case DeviceRequestType.CAN_ClearDTCs:
                                this.FireAdapterUpdateEvent("NoDataString", "DTCs Cleared");
                                break;
                            case DeviceRequestType.OBD2_GetPIDS_00:
                                try
                                {
                                   // this.CurrentPID.Switch(CurrentRequest);
                                  //  this.tempStringBuilder.Append((string)this.CurrentPID.Parse(inputStringArray));

                                    this.tempStringBuilder.Append((string)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_GetPIDS_00].function(inputStringArray));
                                    this.FireAdapterUpdateEvent("LogText", this.tempStringBuilder.ToString());
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("LogText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.OBD2_GetVIN:
                                try
                                {
                                    this.VIN = (string)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_GetVIN].function(inputStringArray);
                                    this.FireAdapterUpdateEvent("VIN", this.VIN.Trim('\0'));
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("VIN", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.IdentifyIC:
                                try
                                {
                                    this.FireAdapterUpdateEvent("ICDescription", inputStringArray[0]);
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("ICDescription", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.GetCurrentProtocolDescription:
                                try
                                {
                                    this.FireAdapterUpdateEvent("SystemProtocolDescription", inputStringArray[0]);
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("SystemProtocolDescription", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.GetSystemProtocolID:
                                try
                                {
                                    this.FireAdapterUpdateEvent("SystemProtocolID", inputStringArray[0]);
                                }
                                catch (Exception)
                                {
                                   //FireAdapterUpdateEvent("StatusText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;
                            case DeviceRequestType.OBD2_KmSinceDTCCleared:
                                try
                                {
                                    //  dResult = Math.Round((int)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_KmSinceDTCCleared].function(inputStringArray) / 1.60, 0, MidpointRounding.AwayFromZero);
                                    dResult = Convert.ToDouble(OBD2PIDS.PIDSDictionary[OBD2PIDS.PIDS.OBD2_KmSinceDTCCleared].function(inputStringArray[0].Substring(4)));

                                    this.FireAdapterUpdateEvent("DistSinceReset", $"{dResult} {OBD2PIDS.PIDSDictionary[OBD2PIDS.PIDS.OBD2_KmSinceDTCCleared].UnitDescriptor}");
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("DistSinceReset", Constants.MSG_NOT_APPLICABLE);
                                    //return $"Error: Unable to parse odometer data (Drive Cycle)";
                                }
                                break;
                            case DeviceRequestType.OBD2_KmWithMilOn:
                                try
                                {
                                    dResult = Convert.ToDouble(OBD2PIDS.PIDSDictionary[OBD2PIDS.PIDS.OBD2_KmWithMilOn].function(inputStringArray[0].Substring(4)));

                                    //dResult = Math.Round((int)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_KmWithMilOn].function(inputStringArray) / 1.60, 0, MidpointRounding.AwayFromZero);
                                    this.FireAdapterUpdateEvent("DistWithDTC", $"{dResult} {OBD2PIDS.PIDSDictionary[OBD2PIDS.PIDS.OBD2_KmWithMilOn].UnitDescriptor}");
                                }
                                catch (Exception er)
                                {
                                    FireAdapterUpdateEvent("DistWIthDTC", Constants.MSG_NOT_APPLICABLE);
                                    //return $"Error: Unable to parse odometer data (Drive Cycle)";
                                }
                                break;
                            case DeviceRequestType.OBD2_Odometer:
                                try
                                {
                                    var od = (string)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_Odometer].function(inputStringArray);
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("Odometer", Constants.MSG_NOT_APPLICABLE);
                                    //return $"Error: Unable to parse odometer data (Drive Cycle)";
                                }
                                break;
                            case DeviceRequestType.OBD2_StatusThisDriveCycle:
                                if (inputStringArray.Length > 0)
                                {
                                    try
                                    {
                                        readinessObject = (Readiness)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_StatusThisDriveCycle].function(inputStringArray);
                                        FireAdapterReadinessMonitorsEvent(readinessObject);
                                    }
                                    catch (Exception)
                                    {
                                        FireAdapterUpdateEvent("StatusMessage", Constants.MSG_ERROR);
                                        FireAdapterUpdateEvent("EmptyGridMessage", Constants.MSG_ERROR);
                                        FireErrorEvent(Constants.MSG_ERROR);
                                    }
                                }
                                else
                                {
                                    return "Error: No Response to Readiness Monitors query...";
                                }
                                break;
                            case DeviceRequestType.OBD2_StatusSinceCodesLastCleared:
                                if (inputStringArray.Length > 0)
                                {
                                    try
                                    {
                                        readinessObject = (Readiness)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_StatusSinceCodesLastCleared].function(inputStringArray);
                                        FireAdapterReadinessMonitorsEvent(readinessObject);
                                    }
                                    catch (Exception)
                                    {
                                        FireAdapterUpdateEvent("StatusMessage", Constants.MSG_ERROR);
                                        //FireAdapterUpdateEvent("EmptyGridMessage", Constants.MSG_ERROR);
                                        FireErrorEvent(Constants.MSG_ERROR);

                                        //return $"Error: Unable to parse Readiness data";
                                    }
                                }
                                else
                                {
                                    return "Error: No Response to Readiness Monitors query...";
                                }
                                break;
                            case DeviceRequestType.None:
                            default:
                                break;

                        }
                        return string.Empty;
                    case QueueSets.DTCReport:
                        switch (CurrentRequest)
                        {
                            case DeviceRequestType.ISO_SlowInit:
                                //Task.Delay(3000);
                                break;

                            case DeviceRequestType.GetSystemProtocolID:
                                try
                                {
                                    int val;
                                    if (inputStringArray[0].Substring(0, 1) == "A") // if the first value is 'A' then it's auto 
                                    {
                                        if (int.TryParse(inputStringArray[0].Substring(1, 1), out val))
                                        {
                                            this.FireAdapterUpdateEvent("SystemProtocolID", val);
                                        }
                                    }
                                    else
                                    {
                                        if (int.TryParse(inputStringArray[0].Substring(0, 1), out val))
                                        {
                                            this.FireAdapterUpdateEvent("SystemProtocolID", val);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    //FireAdapterUpdateEvent("StatusText", Constants.MSG_NOT_APPLICABLE);
                                }
                                break;

                            case DeviceRequestType.OBD2_FreezeFrameDTC:
                            case DeviceRequestType.OBD2_FreezeFrameDTC1:
                            case DeviceRequestType.OBD2_FreezeFrame22DTC:
                                try
                                {
                                    var dtc = OBD2Device.OPBD2PIDSDictionary[CurrentRequest].function(inputStringArray);
                                    if (Convert.ToUInt16(dtc) > 0)
                                    {
                                        this.FireAdapterUpdateEvent("FreezeFrameDTC", Convert.ToUInt16(dtc));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //FireAdapterUpdateEvent("VIN", Constants.MSG_NOT_APPLICABLE);
                                    //return $"Error: Unable to parse VIN data";
                                }
                                break;
                            case DeviceRequestType.OBD2_GetVIN:
                                try
                                {
                                    this.VIN = (string)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_GetVIN].function(inputStringArray);
                                    this.ConfirmedDTCs.ToList().Clear();
                                    this.CompositeReport.Clear();
                                    this.CompositeReport.Append($"VIN: {this.VIN}{Environment.NewLine}");
                                    this.FireAdapterUpdateEvent("VIN", this.VIN);
                                }
                                catch(Exception)
                                {
                                    FireAdapterUpdateEvent("VIN", Constants.MSG_NOT_APPLICABLE);
                                    //return $"Error: Unable to parse VIN data";
                                }
                                break;
                            case DeviceRequestType.OBD2_StatusSinceCodesLastCleared:
                                if (inputStringArray.Length > 0)
                                {
                                    try
                                    {
                                        readinessObject = (Readiness)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_StatusSinceCodesLastCleared].function(inputStringArray);

                                        this.CompositeReport.Append($"DTC Count: {readinessObject.DTCCount}{Environment.NewLine}");
                                        if(readinessObject?.DTCCount < 1)
                                        {
                                            // End the whole DTC inquiry here...
                                             //   this.ClearQueue();
                                             //   FireAdapterUpdateEvent("ExtendedMessage", this.CompositeReport.ToString());
                                             //   return "";
                                        }
                                    
                                        this.FireAdapterUpdateEvent("DTCCount", readinessObject.DTCCount);
                                        this.CompositeReport.Append(Environment.NewLine);
                                    }
                                    catch (Exception)
                                    {
                                        FireAdapterUpdateEvent("StatusMessage", Constants.MSG_ERROR);
                                        FireAdapterUpdateEvent("DTCCount", Constants.MSG_NOT_APPLICABLE);
                                        return $"Error: Unable to parse Readiness data";
                                    }
                                }
                                break;
                            case DeviceRequestType.OBD2_GetDTCs:
                                // pre-manufacturer-mapped, raw codes
                                try
                                {
                                    this.ConfirmedDTCs = (IList<uint>)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_GetDTCs].function(inputStringArray);
                                    codeList = new List<OBDIIFaultCode>();
                                    ConfirmedDTCs.ToList().ForEach(errCode =>
                                    {
                                        codeList.Add(new OBDIIFaultCode(errCode, this.Manufacturer));
                                        //OBD2FaultCode code = null;
                                        //if (this.Manufacturer.FaultCodes.TryGetValue(errCode, out code))
                                        //{
                                        //    codeList.Add(code);
                                        //}
                                        //else
                                        //{
                                        //    code = new OBD2FaultCode(errCode) { Description = $"{OBD2Device.TranslateToString(errCode)} - reported by ECU" };
                                        //    codeList.Add(code);
                                        //}
                                    });
                                    FireDTCDataEvent(codeList, OBD2AdapterEventTypes.ConfirmedDTCData);
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("StatusMessage", Constants.MSG_ERROR);
                                    FireAdapterUpdateEvent("DTCCount", Constants.MSG_NOT_APPLICABLE);
                                    return $"Error: Unable to parse Confirmed DTC data";
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPendingDTCs:
                                try
                                {
                                    // pre-manufacturer-mapped, raw codes
                                    this.PendingDTCs = (IList<uint>)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_GetDTCs].function(inputStringArray);

                                    codeList = new List<OBDIIFaultCode>();
                                    PendingDTCs.ToList().ForEach(errCode =>
                                    {
                                        codeList.Add(new OBDIIFaultCode(errCode, this.Manufacturer));
                                        //OBD2FaultCode code = null;
                                        //if (this.Manufacturer.FaultCodes.TryGetValue(errCode, out code))
                                        //{
                                        //    codeList.Add(code);
                                        //}
                                        //else
                                        //{
                                        //    code = new OBD2FaultCode(errCode) { Description = $"{OBD2Device.TranslateToString(errCode)} - reported by ECU" };
                                        //    codeList.Add(code);
                                        //}
                                    });
                                    FireDTCDataEvent(codeList, OBD2AdapterEventTypes.PendingtDTCData);
                                    this.CompositeReport.Append(Environment.NewLine);
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("StatusMessage", Constants.MSG_ERROR);
                                    return $"Error: Unable to parse Pending DTC data";
                                }
                                break;
                            case DeviceRequestType.OBD2_GetPermanentDTCs:
                                try
                                {
                                    // pre-manufacturer-mapped, raw codes
                                    this.PermanentDTCs = (IList<uint>)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.OBD2_GetPermanentDTCs].function(inputStringArray);
                                    codeList = new List<OBDIIFaultCode>();
                                    PermanentDTCs.ToList().ForEach(errCode =>
                                    {
                                        codeList.Add(new OBDIIFaultCode(errCode, this.Manufacturer));
                                        //OBD2FaultCode code = null;
                                        //if (this.Manufacturer.FaultCodes.TryGetValue(errCode, out code))
                                        //{
                                        //    codeList.Add(code);
                                        //}
                                        //else
                                        //{
                                        //    code = new OBD2FaultCode(errCode) { Description = $"{OBD2Device.TranslateToString(errCode)} - reported by ECU" };
                                        //    codeList.Add(code);
                                        //}
                                    });
                                    FireDTCDataEvent(codeList, OBD2AdapterEventTypes.PermanentDTCData);
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("StatusMessage", Constants.MSG_ERROR);
                                    return $"Error: Unable to parse Permanent DTC data";
                                }
                                break;
                            case DeviceRequestType.CAN_GetDTCByStatus:
                            case DeviceRequestType.CAN_GetDTCByStatus1:
                            case DeviceRequestType.CAN_GetAllDTC:
                                try
                                {
                                    codeList = new List<OBDIIFaultCode>();
                                    if (!OBD2Device.CheckForDongleMessages(inputStringArray[0], out tempString))
                                    {
                                        // pre-manufacturer-mapped, raw codes
                                        var manufacturerDTCs = (IList<uint>)OBD2Device.OPBD2PIDSDictionary[DeviceRequestType.CAN_GetDTCByStatus].function(inputStringArray);

                                        manufacturerDTCs.ToList().ForEach(errCode =>
                                        {
                                            codeList.Add(new OBDIIFaultCode(errCode, this.Manufacturer));

                                            //OBD2FaultCode code = null;
                                            //if (this.Manufacturer.FaultCodes.TryGetValue(errCode, out code))
                                            //{
                                            //    codeList.Add(code);
                                            //}
                                            //else
                                            //{

                                            //    code = new OBD2FaultCode(errCode) { Description = $"{OBD2Device.TranslateToString(errCode)} - reported by ECU" };
                                            //    codeList.Add(code);
                                            //}
                                        });
                                        this.CompositeReport.Append(Environment.NewLine);
                                    }
                                    FireDTCDataEvent(codeList, OBD2AdapterEventTypes.Mode18DTCData);
                                }
                                catch (Exception)
                                {
                                    FireAdapterUpdateEvent("StatusMessage", Constants.MSG_ERROR);
                                    return $"Error: Unable to parse extended DTC data";
                                }
                                break;
                            case DeviceRequestType.None:
                            default:
                                break;

                        }
                        return string.Empty;
                    case QueueSets.SetCANID:
                        this.FireAdapterUpdateEvent("StatusMessage", inputStringArray[0] ?? "SetCANID");
                        //this.FireAdapterUpdateEvent("CurrentCANID", inputStringArray[0] ?? "SetCANID");
                        return string.Empty;
                    case QueueSets.SendCANMessage:
                        tempStringBuilder.Clear();
                        foreach (string st in inputStringArray)
                        {
                            tempStringBuilder.Append($"{st}{ Environment.NewLine}");
                        }
                        this.FireAdapterUpdateEvent("StatusMessage", string.IsNullOrEmpty(tempStringBuilder.ToString())?"SendCANMessage":tempStringBuilder.ToString());
                        return string.Empty;
                    case QueueSets.InitializeCAN:
                    case QueueSets.InitializeCANMonitor:
                        switch (CurrentRequest)
                        {
                            case DeviceRequestType.IdentifyIC:
                                this.FireAdapterUpdateEvent("ICDescription", inputStringArray[0]);
                                //this.FireAdapterUpdateEvent("ICDescription", $"OBD2 Adapter Found...");
                                break;
                            case DeviceRequestType.GetSystemProtocolID:
                                //this.FireAdapterUpdateEvent("ICDescription", inputStringArray[0]);
                                //this.FireAdapterUpdateEvent("ICDescription", $"OBD2 Adapter Found...");
                                break;
                        }
                        return inputStringArray[0];
                    default:

                        return string.Empty;
                }
            }
            catch (Exception exm)
            {
                // The very special case of clearing DTCs
                if (CurrentRequest == DeviceRequestType.OBD2_ClearDTCs)
                {
                    this.FireAdapterUpdateEvent("NoDataString", "DTCs Cleared");
                    return "DTCs Cleared";
                }

                //this.tempStringBuilder.Clear();
                foreach (string st in inputStringArray)
                {
                    if (this.CheckForError(st, out tempString))
                    {
                        //this.tempStringBuilder.Append($"Error: {st}{Environment.NewLine}");

                        this.FireErrorEvent($"Error: {tempString}");
                        return st;
                    }

                }

                //this.tempStringBuilder.Clear();
                //this.tempStringBuilder.Append($"Error:{exm.Message}, String:'{rawStringData}', msg:{CurrentRequest}, set:{CurrentQueueSet}");

                //this.FireErrorEvent(this.tempStringBuilder.ToString());
                //return this.tempStringBuilder.ToString();

                this.FireErrorEvent($"DataError, cmd:{this.CurrentRequest},q:{this.CurrentQueueSet}");
                return $"DataError, cmd:{this.CurrentRequest},q:{this.CurrentQueueSet}";
            }
        }

        /// <summary>
        /// Parse CAN multi-line responses
        /// </summary>
        /// <param name="rawStringData"></param>
        /// <returns>true if more data is required</returns>
        public bool ParseCANResponse(string rawStringData)
        {
            if (string.IsNullOrEmpty(rawStringData)) return false;

            // inputStringArray = System.Text.RegularExpressions.Regex.Replace(rawStringData.TrimEnd(dataEndTrimChars), @"(^a-zA-Z|\r\r\r|\r\r|\r\n|\n\r|\r|\n|'>')", "\r").Split('\r');
            //inputStringArray = System.Text.RegularExpressions.Regex.Replace(rawStringData, @"([^']{,10}|\r\r|\r|\n|>)", "\r").Split('\r');
            inputStringArray = System.Text.RegularExpressions.Regex.Replace(rawStringData, @"(^a-zA-Z|\r\r\r|\r\r|\r\n|\n\r|\r|\n|>)", "\r").Split('\r');

            if (inputStringArray.Length < 1)
            {
                return false;
            }

            double dResult = 0.0;
            int i = 0;
            try
            {
                switch (this.CurrentQueueSet)
                {
                    case QueueSets.SendCANMessage:
                        tempStringBuilder.Clear();

                        foreach (string st in inputStringArray)
                        {
                            tempStringBuilder.Append($"{st}{ Environment.NewLine}");
                        }
                        this.FireAdapterUpdateEvent("StatusMessage", string.IsNullOrEmpty(tempStringBuilder.ToString()) ? "SendCANMessage" : tempStringBuilder.ToString());
                        return CANDevice.ParseMultiLineResponse(inputStringArray);
                        
                    default:
                        return false;
                }
            }
            catch (Exception exm)
            {

                //this.tempStringBuilder.Clear();
                foreach (string st in inputStringArray)
                {
                    if (this.CheckForError(st, out tempString))
                    {
                        //this.tempStringBuilder.Append($"Error: {st}{Environment.NewLine}");

                        this.FireErrorEvent($"Error: {tempString}");
                        return false;
                    }

                }

                return false;
            }
        }

        private int byteCountToRead = 0;
        private ELM327Command obd2Command = null;
        /// <summary>
        /// Sets a property based on a raw command code and a string of data
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <returns>The number of data bytes consumed in the string - not including the header (2 bytes)</returns>
        //private int SetParsedProperty(string command, string data)
        //{
        //    // determine the command response by the code returned in the data
        //    obd2Command = OBD2Device.OPBD2PIDSDictionary.Values.Where(v => v.Code.Length >= 2 && v.Code.Substring(0, 2).Contains(command)).FirstOrDefault();
        //    if (obd2Command == null) return 0;

        //    switch (obd2Command.RequestType)
        //    {
        //        //case DeviceRequestType.OBD2_GetVIN:
        //        //    byteCountToRead = 3;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.VIN = (string)obd2Command.function(data.Substring(2, byteCountToRead));
        //        //    break;
        //        //case DeviceRequestType.OBD2_OilTemperature:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.OilTemperature = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        //case DeviceRequestType.OBD2_IntakeAirTemperature:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.IntakeAirTemperature = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        //case DeviceRequestType.OBD2_FuelRailPressure:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.FuelRailPressure = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        //case DeviceRequestType.OBD2_ShortTermFuelTrimBank1:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    dataFormatString = "F1";
        //        //    this.dataLabelString = " %";
        //        //    this.ShortTermFuelTrimB1 = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead))).ToString(dataFormatString) + dataLabelString; ;
        //        //    break;
        //        //case DeviceRequestType.OBD2_LongTermFuelTrimBank1:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    dataFormatString = "F1";
        //        //    this.dataLabelString = " %";
        //        //    this.LongTermFuelTrimB1 = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead))).ToString(dataFormatString) + dataLabelString; ;
        //        //    break;
        //        //case DeviceRequestType.OBD2_ShortTermFuelTrimBank2:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    dataFormatString = "F1";
        //        //    this.dataLabelString = " %";
        //        //    this.ShortTermFuelTrimB2 = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead))).ToString(dataFormatString) + dataLabelString; ;
        //        //    break;
        //        //case DeviceRequestType.OBD2_LongTermFuelTrimBank2:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    dataFormatString = "F1";
        //        //    this.dataLabelString = " %";
        //        //    this.LongTermFuelTrimB2 = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead))).ToString(dataFormatString) + dataLabelString; ;
        //        //    break;
        //        //case DeviceRequestType.OBD2_GetEngineRPM:
        //        //    byteCountToRead = 4;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.RPM = Convert.ToInt32(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        //case DeviceRequestType.OBD2_MAFRate:
        //        //    byteCountToRead = 4;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.MAFRate = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        //case DeviceRequestType.OBD2_GetAmbientTemp:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.AmbientTemp = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        //case DeviceRequestType.OBD2_EngineCoolantTemp:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.CoolantTemp = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        //case DeviceRequestType.OBD2_GetEngineLoad:
        //        //    byteCountToRead = 2;
        //        //    if (data.Length < (byteCountToRead + 2))
        //        //    {
        //        //        return 0;
        //        //    }
        //        //    this.EngineLoad = Convert.ToDouble(obd2Command.function(data.Substring(2, byteCountToRead)));
        //        //    break;
        //        default:
        //            return 0;
        //    }
        //}



    }
}
