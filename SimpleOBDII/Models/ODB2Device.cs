using OS.OBDII.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Forms;
//using OS.Communication;

namespace OS.OBDII.Models
{

    public enum DeviceRequestType
    {
        None = -1,
        Connect,
        AllowLongMessages,
        LineFeedsOff,
        SpacesOff,
        ForgetEvents,
        BufferDump,
        DeviceSetDefaults,
        DeviceDescription,
        DeviceReset,
        WarmStart,
        EchoOff,
        MonitorAll,
        GetCurrentProtocolDescription,
        //GetCurrentProtocolID,
        ProtocolSearch,
        SerialNumber,
        SupplyVoltage,
        BaudRateFastTimeout,
        BaudRateFastDivisor,
        SetISOBaudRate, // argument is dynamic
        SetHeader,
        SetDefaultHeader, // Sets header to default tester addresss 7DF
        HeadersOn,
        HeadersOff,
        MemoryOff,
        SetCAN_11_500,
        SetJ1850_PWM,
        SetJ1850_VPW,
        Set9141_2,
        SetProtocol,
        GetSystemProtocolID,
        SetCANPriorityBits,
        SetCANRxAddress,
        SetDefaultCANRxAddress,
        CANAddressFilter,
        CANAddressMask,
        CANAddressMaskReset,
        CANAutoAddress,
        CANAutoFormatOn,
        CANAutoFormatOff,
        CANFlowControlAutoOn,
        CANFlowControlAutoOff,
        CANSilentMonitoringOn,
        CANSilentMonitoringOff,
        CANVariableLengthOn,
        CANVariableLengthOff,
        CAN_IFRH, // header bytes are tester address, or as assigned
        CAN_GetDTCByStatus,
        CAN_GetDTCByStatus1,
        CAN_GetAllDTC,
        CAN_ClearDTCs,
        CAN_ClearDTCs_2,
        ISO_SlowInit,
        ISO_SetSlowInitAddress,
        SET_ISOInitAddress_13,
        SET_OBD1Wakeup,
        SET_OBD1WakeupOff,
        SET_Timeout,
        SETAdaptiveTiming,
        IdentifyIC,
        DisplayDataByteLengthOn,
        DisplayDataByteLengthOff,
        GetLiveData,
        OBD2_MAFRate,
        OBD2_StatusSinceCodesLastCleared,
        OBD2_StatusThisDriveCycle,
        OBD2_FreezeFrameDTC,
        OBD2_FreezeFrameDTC1,
        OBD2_FreezeFrame22DTC,
        OBD2_GetEngineRPM,
        OBD2_VehicleSpeed,
        OBD2_EngineCoolantTemp,
        OBD2_GetEngineLoad,
        OBD2_GetPIDINFO_13,
        OBD2_GetPIDINFO_1D,
        OBD2_GetPIDS_00,
        OBD2_GetPIDS_20,
        OBD2_GetPIDS_40,
        OBD2_GetPIDS_60,
        OBD2_GetPIDS_80,
        OBD2_GetPIDS_A0,
        OBD2_GetPIDS_C0,
        OBD2_GetPIDS_E0,
        OBD2_GetMonitorIds_00,
        OBD2_GetMonitorIds_20,
        OBD2_GetMonitorIds_40,
        OBD2_GetMonitorIds_60,
        OBD2_GetMonitorIds_80,
        OBD2_GetMonitorIds_A0,
        OBD2_GetMonitorIds_C0,
        OBD2_GetMonitorIds_E0,
        OBD2_GetVIN,
        OBD2_FuelLevel,
        OBD2_ShortTermFuelTrimBank1,
        OBD2_LongTermFuelTrimBank1,
        OBD2_ShortTermFuelTrimBank2,
        OBD2_LongTermFuelTrimBank2,
        OBD2_FuelRailPressureRelativeToManifoldVa,
        OBD2_FuelRailPressure,
        OBD2_OilTemperature,
        OBD2_FuelInjectorTiming,
        OBD2_FuelRate,
        OBD2_IntakeMAP,
        OBD2_IntakeAirTemperature,
        OBD2_Odometer,

        OBD2_ThrottlePostion,
        OBD2_RelativeThrottlePostion,
        OBD2_AbsoluteThrottlePostionB,
        OBD2_AbsoluteThrottlePostionC,
        OBD2_AcceleratorPedalPostionD,
        OBD2_AcceleratorPedalPostionE,
        OBD2_AcceleratorPedalPostionF,
        OBD2_CommandedThrottleActuator,
        OBD2_FuelSystemStatus,
        OBD2_WarmUpsSinceDTCCleared,
        OBD2_KmSinceDTCCleared,
        OBD2_KmWithMilOn,
        OBD2_GetAmbientTemp,
        OBD2_GetDTCs,
        OBD2_GetPendingDTCs,
        OBD2_GetPermanentDTCs,
        OBD2_ClearDTCs
    }


    public class OBD2Device
    {

        public static List<Protocol> Protocols { get; } = new List<Protocol>()
                        {
                            new Protocol(0, Constants.PROTOCOL_AUTO),
                            new Protocol(1, Constants.PROTOCOL_SAE_J1850_PWM),
                            new Protocol(2, Constants.PROTOCOL_SAE_J1850_VPW),
                            new Protocol(3, Constants.PROTOCOL_ISO_9141_2),
                            new Protocol(4, Constants.PROTOCOL_ISO_14230_4_KWP_SLOW),
                            new Protocol(5, Constants.PROTOCOL_ISO_14230_4_KWP),
                            new Protocol(6, Constants.PROTOCOL_ISO_15765_4_CAN_11_500),
                            new Protocol(7, Constants.PROTOCOL_ISO_15765_4_CAN_29_500),
                            new Protocol(8, Constants.PROTOCOL_ISO_15765_4_CAN_11_250),
                            new Protocol(9, Constants.PROTOCOL_ISO_15765_4_CAN_29_250),
                            new Protocol(10, Constants.PROTOCOL_SAE_J1939_CAN_29_250),
                        };

        // assigned in App class constructor 
        public static SystemReport SystemReport = null;// new SystemReport(DependencyService.Get<IPIDManager>());
        public static bool UseMetricUnits = false;
        public static readonly Dictionary<int, OBD2ServiceMode> ServiceModes = new Dictionary<int, OBD2ServiceMode>
        {
            // J1979
            { -1, new OBD2ServiceMode(-1, "", "Direct Mode") },
            { 0, new OBD2ServiceMode(0, "AT", "Adapter") },
            { 1, new OBD2ServiceMode(1, "01", "Service Mode 01") },
            { 2,  new OBD2ServiceMode(2, "02", "Service Mode 02") },
            { 3,  new OBD2ServiceMode(3, "03", "Service Mode 03") },
            { 4,  new OBD2ServiceMode(4, "04", "Service Mode 04") },
            { 5,  new OBD2ServiceMode(5, "05", "Service Mode 05") },
            { 6,  new OBD2ServiceMode(6, "06", "Service Mode 06") },
            { 7,  new OBD2ServiceMode(7, "07", "Service Mode 07") },
            { 8,  new OBD2ServiceMode(8, "08", "Service Mode 08") },
            { 9, new OBD2ServiceMode(9, "09", "Service Mode 09") },
           // { 10, new OBD2ServiceMode(10, "0A", "Service Mode 0A") },
            // J2190
            { 0x13, new OBD2ServiceMode(0x13, "13", "Service Mode 13") },
            { 0x14, new OBD2ServiceMode(0x14, "14", "Service Mode 14") },
            { 0x18,  new OBD2ServiceMode(0x18, "18", "Service Mode 18") },
            { 0x22, new OBD2ServiceMode(0x22, "22", "Service Mode 22") }
        };
        public static int CurrentRequestSize = 2;
        public static int SystemProtocolID
        {
            get => systemProtocolID;
            set
            {
                systemProtocolID = value;
                switch (systemProtocolID)
                {
                    case 1:
                    case 2:
                        DataPositionOffset = 8; //
                        break;
                    case 3: // ISO 9141
                    case 4: // ISO 14230 KWP Slow init
                    case 5: // ISO 14230 KWP 
                        DataPositionOffset = 6; //
                        break;
                    case 7: // 29-bit ISO-15765
                    case 9:
                        DataPositionOffset = 10; //
                        break;
                    case 6: // 11-bit ISO-15765
                    case 8:
                        DataPositionOffset = 5; //
                        break;
                    default:
                        DataPositionOffset = 0; //
                        break;
                }
            }
        }
        private static int systemProtocolID = 0;

        /// <summary>
        /// J2190 CAN Response codes
        /// </summary>
        public static readonly List<CANResponse> CANResponses = new List<CANResponse>()
        {
            // J2190
            new CANResponse(0x00, "00", "Success"),
            new CANResponse(0x10, "11", "General Reject"),
            new CANResponse(0x11, "11", "No Support"),
            new CANResponse(0x12, "12", "No Sub-Function Support or Invalid format"),
            new CANResponse(0x21, "21", "Busy - Repeat Request"),
            new CANResponse(0x22, "22", "Conditions Not Correct or Sequence Error"),
            new CANResponse(0x23, "23", "Routine Not Complete"),
            new CANResponse(0x31, "31", "Out of Range"),
            new CANResponse(0x33, "33", "Security Access Denied")
        };

        public static string CreateRequest(OBD2ServiceMode serviceMode, DeviceRequestType requestType, bool appendCR = true, string argument = "")
        {
            string lastChar = appendCR ? Constants.CARRIAGE_RETURN.ToString() : "";
           // var svcMode = ServiceModes.FirstOrDefault(sm => sm.Mode == serviceMode)?.Value; // should be the actual 2-character command string
            string str = serviceMode.Value + OPBD2PIDSDictionary[requestType].Code + argument + lastChar;
            return str;
        }

        public static readonly List<ELM327ResponseText> DongleResponses = new List<ELM327ResponseText>()
        {
            new ELM327ResponseText("BUS ERROR"),
            new ELM327ResponseText("BUS BUSY"),
            new ELM327ResponseText("CAN ERROR"),
            new ELM327ResponseText("DATA ERROR"),
            new ELM327ResponseText("<DATA ERROR"),
            new ELM327ResponseText("<RX ERROR"),
            new ELM327ResponseText("ACT ALERT"),
            new ELM327ResponseText("!ACT ALERT"),
            new ELM327ResponseText("LV RESET"),
            new ELM327ResponseText("FB ERROR"),
            new ELM327ResponseText("LP ALERT"),
            new ELM327ResponseText("!LP ALERT"),
            new ELM327ResponseText("?", Constants.DONGLE_UNRECOGNIZED_COMMAND_STRING, false),
            new ELM327ResponseText("UNABLE TO CONNECT", Constants.MSG_NO_RESPONSE_VEHICLE),
            new ELM327ResponseText("STOPPED", "Stopped", false),
            new ELM327ResponseText("SEARCHING...","Searching...", false),
            new ELM327ResponseText("NO DATA", "No Data", false),
            new ELM327ResponseText("BUFFER FULL", "Buffer Full", false),
        };

        public static bool CheckForDongleMessages(string data, out string errMsg, bool errorsOnly = false)
        {

            var responses = new List<ELM327ResponseText>() { };

            foreach(var respType in DongleResponses)
            {
                if(errorsOnly)
                {
                    if(data.IndexOf(respType.Text) > -1 && respType.IsError == true)
                    {
                        responses.Add(respType);
                    }
                }
                else
                {
                    if(data.IndexOf(respType.Text) > -1)
                    {
                        responses.Add(respType);
                    }

                }
            }
            // Right now we're returning only the last response message in the list.
            if (responses.Count > 0)
            {
                errMsg = responses.Last().Description;
                return true;
            }
            errMsg = data;
            return false;

        }

        public static object GetManufacturerDTCs(object obj)
        {
            string[] strArray = (string[])obj;
            int objectByteSize = 6;
            var byteCnt = 0;
            var tmpByteCnt = 0;

            string sBuf = "";
            string str;
            int objectCount = 0;
            int tmpObjectCount = 0;
            int objectByteCount = 0;
            int tmpObjectByteCount = 0;
            int objectBytesRead = 0;
            int remainingByteCount = 0;

            // Make one long string...
            foreach (string s in strArray)
            {
                // multi-line
                if (s.Length == 3 || s.IndexOf(':') > -1)
                {
                    if (s.Length == 3) // this string/line (if 3 chars long) is the number of bytes in the response
                    {
                        int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out tmpByteCnt);
                        byteCnt += tmpByteCnt;
                        continue;
                    }
                    if (s.Length >= 6)
                    {
                        remainingByteCount = objectByteCount - objectBytesRead;
                        // is this the first line?
                        if (s.Substring(0, 2) == "0:") // is the first line
                        {
                            // Get the PID count (four characters a piece)
                            int.TryParse(s.Substring(4, 2), out tmpObjectCount);
                            tmpObjectByteCount = objectByteSize * tmpObjectCount;
                            if (tmpObjectByteCount > 0)
                            {
                                // if this is the first line of a multiline message
                                // then we can just grab all of the rest of the line
                                // assuming it's all valid data
                                sBuf += s.Substring(6);
                                objectBytesRead += s.Length - 6;
                            }
                            objectCount += tmpObjectCount;
                            objectByteCount += tmpObjectByteCount;
                        }
                        else
                        {
                            // is multiline, and not the first line, so should be data

                            // if what's left to read is greater than the string we have - take the whole string
                            if (remainingByteCount >= s.Length - 2)
                            {
                                sBuf += s.Substring(2);
                                byteCnt += s.Length;
                            }
                            else
                            {
                                sBuf += s.Substring(2, remainingByteCount);
                                byteCnt += remainingByteCount;
                            }
                        }
                    }

                }
                else
                {
                    // Single Line Response (more than 3 characters and no ':' found)

                    if (s.Length >= 4)
                    {
                        // Get the number of reported pids in this one line - should be only one or two
                        int.TryParse(s.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out tmpObjectCount);
                        tmpObjectByteCount = objectByteSize * tmpObjectCount;

                        if (tmpObjectByteCount > 0)
                        {
                            if (s.Length >= tmpObjectByteCount + 4)
                            {
                                sBuf += s.Substring(4, tmpObjectByteCount);
                            }
                        }
                        objectCount += tmpObjectCount;
                        objectByteCount += tmpObjectByteCount;
                    }
                }

            }

            // The return object is a list of dtc values
            var dtcList = new List<uint>();
            byteCnt = objectByteCount;
            for (int strIdx = 0; strIdx < byteCnt; strIdx += objectByteSize)
            {
                if (sBuf.Length >= strIdx + objectByteSize)
                {
                    var j = uint.Parse(sBuf.Substring(strIdx, objectByteSize-2), System.Globalization.NumberStyles.HexNumber);
                    dtcList.Add(j);
                }
            }

            return dtcList;
        }

        public static object GetVIN(object obj)
        {
            string[] strArray = (string[])obj;
            int objectByteSize = 17; // size of vin in bytes
            var byteCnt = 0;
            var tmpByteCnt = 0;

            StringBuilder sBuf = new StringBuilder();
            string str = "";
            int objectCount = 0;
            int tmpObjectCount = 0;
            int objectByteCount = 0;
            int tmpObjectByteCount = 0;
            int objectBytesRead = 0;
            int remainingByteCount = 0;
            int idx;
            string[] newVin = new string[5];

            // Make one long string...
            foreach (string s in strArray)
            {
                // multi-line - extra care to assemble in correct order
                if (s.Length == 3 || s.IndexOf(':') > -1)
                {
                    if (s.Length == 3) // this string/line (if 3 chars long) is the number of bytes in the response
                    {
                        int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out tmpByteCnt);
                        byteCnt += tmpByteCnt;
                        continue;
                    }
                    if (s.Length >= 6)
                    {

                        // is this the first line?
                        if (s.Substring(0, 2) == "0:") // is the first line
                        {
                            // Get the PID count (four characters a piece)
                            int.TryParse(s.Substring(6, 2), out tmpObjectCount);
                            tmpObjectByteCount = objectByteSize * tmpObjectCount;
                            if (tmpObjectByteCount > 0)
                            {
                                // if this is the first line of a multiline message
                                // then we can just grab all of the rest of the line
                                // assuming it's all valid data
                                str = s.Substring(8);

                                for (int i = 0; i < str.Length; i += 2)// char  letter in values)
                                {
                                    sBuf.Append((char)byte.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                                    objectBytesRead++;
                                }
                                newVin[0] = sBuf.ToString();
                                sBuf.Clear();
                            }
                            objectCount += tmpObjectCount;
                            objectByteCount += tmpObjectByteCount;
                        }
                        else
                        {
                          
                            if (int.TryParse(s.Substring(0, 1), out idx))
                            {
                                // is multiline, and not the first line, so should be data
                                remainingByteCount = objectByteCount - objectBytesRead;
                                // if what's left to read is greater than the string we have - take the whole string
                                if (remainingByteCount >= s.Length - 2)
                                {
                                    str = s.Substring(2);

                                    for (int i = 0; i < str.Length; i += 2)// char  letter in values)
                                    {
                                        sBuf.Append((char)byte.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                                        byteCnt++;
                                    }
                                    newVin[idx] = sBuf.ToString();
                                    sBuf.Clear();
                                }
                                else
                                {
                                    str = s.Substring(2, remainingByteCount);

                                    for (int i = 0; i < str.Length; i += 2)// char  letter in values)
                                    {
                                        sBuf.Append((char)byte.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                                        byteCnt++;
                                    }

                                    newVin[idx] = sBuf.ToString();
                                    sBuf.Clear();

                                }
                            }
                        }
                    }
                }
                else
                {
                    if (s.Length < 4) continue;
                    // Alternate version of VIN parsing - older protocols, also multi line
                    if (string.Compare(s.Substring(0, 4), "4902") == 0)
                    {

                        if (int.TryParse(s.Substring(4, 2), out idx))
                        {
                            if (idx == 1)
                            {
                                str = s.Substring(12);
                            }
                            else
                            {
                                str = s.Substring(6);
                            }
                            for (int i = 0; i < str.Length; i += 2)// char  letter in values)
                            {
                                sBuf.Append((char)byte.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                                byteCnt++;
                            }
                            newVin[idx - 1] = sBuf.ToString();
                            sBuf.Clear();
                        }
                    }
                }
            }

            sBuf.Clear();
            for(int i=0;i<newVin.Length;i++)
            {
                sBuf.Append(newVin[i]);
            }
            return sBuf.ToString();
        }

        public static object GetDTCs(object obj)
        {
            string[] strArray = (string[])obj;
            int objectByteSize = 4;
            var byteCnt = 0;
            var tmpByteCnt = 0;

            string sBuf = "";
            string str;
            int objectCount = 0;
            int tmpObjectCount = 0;
            int objectByteCount = 0;
            int tmpObjectByteCount = 0;
            int objectBytesRead = 0;
            int remainingByteCount = 0;

            var dtcList = new List<uint>();

            switch (SystemProtocolID)
            {
                case 3:
                case 4:
                case 5:
                    // Make one long string...
                    foreach (string s in strArray)
                    {
                        // multi-line
                        //if (s.Length == 3 || s.IndexOf(':') > -1)
                        //{
                            if (s.Length == 3) // this string/line (if 3 chars long) is the number of bytes in the response
                            {
                                int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out tmpByteCnt);
                                byteCnt += tmpByteCnt;
                                continue;
                            }
                            if (s.Length >= 6)
                            {
                                    sBuf += s.Substring(2);
                                    objectBytesRead += s.Length - 2;
                                    objectCount += objectBytesRead/4;
                                    objectByteCount += objectByteSize * objectCount;

                                    //remainingByteCount = objectByteCount - objectBytesRead;
                                    //// if what's left to read is greater than the string we have - take the whole string
                                    //if (remainingByteCount >= s.Length - 2)
                                    //{
                                    //    sBuf += s.Substring(2);
                                    //    byteCnt += s.Length;
                                    //}
                                    //else
                                    //{
                                    //    sBuf += s.Substring(2, remainingByteCount);
                                    //    byteCnt += remainingByteCount;
                                    //}
                              //  }
                            }

                        //}
                        //else
                        //{
                        //    // Single Line Response (more than 3 characters and no ':' found)

                        //    if (s.Length >= 4)
                        //    {
                        //        // Get the number of reported pids in this one line - should be only one or two
                        //        int.TryParse(s.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out tmpObjectCount);
                        //        tmpObjectByteCount = objectByteSize * tmpObjectCount;

                        //        if (tmpObjectByteCount > 0)
                        //        {
                        //            if (s.Length >= tmpObjectByteCount + 4)
                        //            {
                        //                sBuf += s.Substring(4, tmpObjectByteCount);
                        //            }
                        //        }
                        //        objectCount += tmpObjectCount;
                        //        objectByteCount += tmpObjectByteCount;
                        //    }
                        //}

                    }

                    // The return object is a list of dtc values
                    byteCnt = objectByteCount;
                    for (int strIdx = 0; strIdx < byteCnt; strIdx += 4)
                    {
                        if (sBuf.Length >= strIdx + 4)
                        {
                            try
                            {
                                var j = uint.Parse(sBuf.Substring(strIdx, 4), System.Globalization.NumberStyles.HexNumber);
                                if(j > 0) dtcList.Add(j);
                            }
                            catch (Exception e) { }
                        }
                    }
                    break;
                default:
                    // Make one long string...
                    foreach (string s in strArray)
                    {
                        // multi-line
                        if (s.Length == 3 || s.IndexOf(':') > -1)
                        {
                            if (s.Length == 3) // this string/line (if 3 chars long) is the number of bytes in the response
                            {
                                int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out tmpByteCnt);
                                byteCnt += tmpByteCnt;
                                continue;
                            }
                            if (s.Length >= 6)
                            {
                                // is this the first line?
                                if (s.Substring(0, 2) == "0:") // is the first line
                                {
                                    // Get the PID count (four characters a piece)
                                    int.TryParse(s.Substring(4, 2), out tmpObjectCount);
                                    tmpObjectByteCount = objectByteSize * tmpObjectCount;
                                    if (tmpObjectByteCount > 0)
                                    {
                                        // if this is the first line of a multiline message
                                        // then we can just grab all of the rest of the line
                                        // assuming it's all valid data
                                        sBuf += s.Substring(6);
                                        objectBytesRead += s.Length - 6;
                                    }
                                    objectCount += tmpObjectCount;
                                    objectByteCount += tmpObjectByteCount;
                                }
                                else
                                {
                                    // is multiline, and not the first line, so should be data

                                    remainingByteCount = objectByteCount - objectBytesRead;
                                    // if what's left to read is greater than the string we have - take the whole string
                                    if (remainingByteCount >= s.Length - 2)
                                    {
                                        sBuf += s.Substring(2);
                                        byteCnt += s.Length;
                                    }
                                    else
                                    {
                                        sBuf += s.Substring(2, remainingByteCount);
                                        byteCnt += remainingByteCount;
                                    }
                                }
                            }

                        }
                        else
                        {
                            // Single Line Response (more than 3 characters and no ':' found)

                            if (s.Length >= 4)
                            {
                                // Get the number of reported pids in this one line - should be only one or two
                                int.TryParse(s.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out tmpObjectCount);
                                tmpObjectByteCount = objectByteSize * tmpObjectCount;

                                if (tmpObjectByteCount > 0)
                                {
                                    if (s.Length >= tmpObjectByteCount + 4)
                                    {
                                        sBuf += s.Substring(4, tmpObjectByteCount);
                                    }
                                }
                                objectCount += tmpObjectCount;
                                objectByteCount += tmpObjectByteCount;
                            }
                        }

                    }

                    // The return object is a list of dtc values
                    byteCnt = objectByteCount;
                    for (int strIdx = 0; strIdx < byteCnt; strIdx += 4)
                    {
                        if (sBuf.Length >= strIdx + 4)
                        {
                            var j = uint.Parse(sBuf.Substring(strIdx, 4), System.Globalization.NumberStyles.HexNumber);
                            dtcList.Add(j);
                        }
                    }
                    break;
            }

            return dtcList;
        }
        public static int DataPositionOffset { get; set; } = 5;
        public static object ParsO2Locations13(object obj)
        {
            var list = new List<O2Location>();
            string str = obj as string;
            // 2 bytes (4 characters) for mode and function response values
            var data = byte.Parse(str.Substring(DataPositionOffset + 4, 2), System.Globalization.NumberStyles.HexNumber);
            for (int i = 0; i < 8; i++)
            {
                if (((data >> i) & 0x01) == 0) continue;
                list.Add(O2Location.Locations[i]);
            }

            return list;
        }

        public static object ParsO2Locations1D(object obj)
        {
            var list = new List<O2Location>();
            string str = obj as string;
            var data = byte.Parse(str.Substring(DataPositionOffset + 4, 2), System.Globalization.NumberStyles.HexNumber);
            for (int i = 8; i < 16; i++)
            {
                if (((data >> i) & 0x01) == 0) continue;
                list.Add(O2Location.Locations[i]);
            }

            return list;
        }
        public static object GetSupportedMonitorIds(object obj)
        {
            int i = 0;
            uint pidVal = 0;
            string[] strArray = obj as string[];
            int byteBufLen = 4;
            byte[] byteBuf = new byte[byteBufLen];
            byte pidMSB = 0x00;
            uint ecuId = 0;

            List<ECU> ecus = new List<ECU>();

            pidVal = 0;
            uint mask = 0b10000000; // init to binary 10000000

            PID pidToAdd = null;
            ECU ecu = null;

            string[] socl = new string[4];

            for (int sdx = 0; sdx < strArray.Length; sdx++)
            {
                if (strArray[sdx].Length < 4) continue;
                if (strArray[sdx].CompareTo("NO DATA") == 0) continue; // Specific to ELM327 
                try
                {
                    // Get the ECU CAN ID...the data position - 1 byte, or 2 chars - for CAN byte count
                    ecuId = uint.Parse(strArray[sdx].Substring(0, DataPositionOffset - 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    ecu = ecus.Where(e => e.Id == ecuId).FirstOrDefault();
                    if (ecu == null) ecu = new ECU(ecuId);

                    int aIndex = 0;
                    // Get the supported PID data
                    for (i = 0; i < 4; i++)
                    {
                        try
                        {
                            socl[i] = strArray[sdx].Substring((DataPositionOffset + 2 + CurrentRequestSize) + (i * 2), 2);
                        }
                        catch (Exception)
                        {
                            socl[i] = "00";
                        }
                    }

                    for (i = 0; i < byteBufLen; i++)
                    {
                        byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                    }
                    pidMSB = byte.Parse(strArray[sdx].Substring(DataPositionOffset + 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    for (i = 0; i < byteBufLen; i++)
                    {
                        mask = 0b10000000; // reset to binary 10000000 (or hex 0x80), then shift rightward
                        switch (i)
                        {
                            case 0:
                                for (pidVal = 1; pidVal < 9; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2MIDS()[(OBD2MIDS.MonitorIds)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedTest((PID)(pidToAdd as ICloneable).Clone());
                                    }
                                    mask >>= 1;
                                }
                                break;
                            case 1:
                                for (pidVal = 9; pidVal < 17; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2MIDS()[(OBD2MIDS.MonitorIds)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedTest((PID)(pidToAdd as ICloneable).Clone());
                                    }
                                    mask >>= 1;
                                }
                                break;
                            case 2:
                                for (pidVal = 17; pidVal < 25; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2MIDS()[(OBD2MIDS.MonitorIds)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedTest((PID)(pidToAdd as ICloneable).Clone());
                                    }
                                    mask >>= 1;
                                }
                                break;
                            case 3:
                                for (pidVal = 25; pidVal < 33; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2MIDS()[(OBD2MIDS.MonitorIds)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedTest((PID)(pidToAdd as ICloneable).Clone());
                                    }
                                    mask >>= 1;
                                }
                                break;
                        }
                    }
                    if (ecus.Contains(ecu)) continue;
                    ecus.Add(ecu);
                }
                catch (Exception ex)
                {
                    // something broke, most likely an ELM327 message such as "STOPPED", "NO DATA" "OK" etc
                    // go to the next string in the array.
                }

            }
            return ecus;
        }

        public static object GetSupportedPIDS(object obj) 
        {
            int i = 0;
            uint pidVal = 0;
            string[] strArray = obj as string[];
            int byteBufLen = 4;
            byte[] byteBuf = new byte[byteBufLen];
            byte pidMSB = 0x00;
            uint ecuId = 0;

            List<ECU> ecus = new List<ECU>();

            pidVal = 0;
            uint mask = 0b10000000; // init to binary 10000000

            PID pidToAdd = null;
            ECU ecu = null;

            string[] socl = new string[4];

            for (int sdx = 0; sdx < strArray.Length; sdx++)
            {
                if (strArray[sdx].Length < 4) continue;
                if (strArray[sdx].CompareTo("NO DATA") == 0) continue; // Specific to ELM327 
                try
                {
                    // Get the ECU CAN ID...the data position - 1 byte, or 2 chars - for CAN byte count
                    ecuId = uint.Parse(strArray[sdx].Substring(0, DataPositionOffset - 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    ecu = ecus.Where(e => e.Id == ecuId).FirstOrDefault();
                    if (ecu == null) ecu = new ECU(ecuId);

                    int aIndex = 0;
                    // Get the supported PID data
                    for (i = 0; i < 4; i++)
                    {
                        try
                        {
                            socl[i] = strArray[sdx].Substring((DataPositionOffset + 2 + CurrentRequestSize) + (i * 2), 2);
                        }
                        catch(Exception)
                        {
                            socl[i] = "00";
                        }
                    }

                    for (i = 0; i < byteBufLen; i++)
                    {
                        byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                    }
                    pidMSB = byte.Parse(strArray[sdx].Substring(DataPositionOffset + 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    for (i = 0; i < byteBufLen; i++)
                    {
                        mask = 0b10000000; // reset to binary 10000000 (or hex 0x80), then shift rightward
                        switch (i)
                        {
                            case 0:
                                for (pidVal = 1; pidVal < 9; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2PIDS()[(OBD2PIDS.PIDS)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedPID((PID)(pidToAdd as ICloneable).Clone());
                                        // ecu.AddSupportedPID(new PID(pidVal, pidToAdd.Name, pidToAdd.Description, pidToAdd.function, false));
                                    }
                                    mask >>= 1;
                                }
                                break;
                            case 1:
                                for (pidVal = 9; pidVal < 17; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2PIDS()[(OBD2PIDS.PIDS)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedPID((PID)(pidToAdd as ICloneable).Clone());
                                    }
                                    mask >>= 1;
                                }
                                break;
                            case 2:
                                for (pidVal = 17; pidVal < 25; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2PIDS()[(OBD2PIDS.PIDS)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedPID((PID)(pidToAdd as ICloneable).Clone());
                                    }
                                    mask >>= 1;
                                }
                                break;
                            case 3:
                                for (pidVal = 25; pidVal < 33; pidVal++)
                                {
                                    if ((mask & byteBuf[i]) != 0)
                                    {
                                        pidToAdd = new OBD2PIDS()[(OBD2PIDS.PIDS)(pidVal | pidMSB)];
                                        if (pidToAdd == null)
                                        {
                                            mask >>= 1;
                                            continue;
                                        }
                                        pidToAdd.CANID = ecu.Id.ToString("X");
                                        ecu.AddSupportedPID((PID)(pidToAdd as ICloneable).Clone());
                                    }
                                    mask >>= 1;
                                }
                                break;
                        }
                    }
                    if (ecus.Contains(ecu)) continue;
                    ecus.Add(ecu);
                }
                catch(Exception ex)
                {
                    // something broke, most likely an ELM327 message such as "STOPPED", "NO DATA" "OK" etc
                    // go to the next string in the array.
                }

            }
            return ecus;
        }

        public static string TranslateToString(uint DTC)
        {

            uint prefix = (DTC & 0xC000) >> 14;
            switch(prefix)
            {
                // cases are in binary 0bxx format
                case 0b00: // 0x0 = b000X, where X=0
                    return $"P{DTC & 0x3FFF:X4}";
                case 0b01: // 0x4 = b010X, where X=0
                    return $"C{DTC & 0x3FFF:X4}";
                case 0b10: // 0x8 = b100X, where X=0
                    return $"B{DTC & 0x3FFF:X4}";
                case 0b11: // 0xC = b110X, where X=0
                    return $"U{DTC & 0x3FFF:X4}";
            }

            return "";
        }


        public static readonly Dictionary<DeviceRequestType, ELM327Command> OPBD2PIDSDictionary = new Dictionary<DeviceRequestType, ELM327Command>
        {
            {DeviceRequestType.None, new ELM327Command{IsUserFunction = false, Code = "", Name = "No Request", Description = "No Request", RequestType = DeviceRequestType.None, function = (obj)=>{ return null; } } },
            {DeviceRequestType.DeviceReset,  new ELM327Command{ Code = "Z", Name = "Reset Device", Description = "Resets the device", RequestType = DeviceRequestType.DeviceReset } },
            {DeviceRequestType.WarmStart,  new ELM327Command{ Code = "WS", Name = "Warm Start", Description = "Resets the device w/out LED test", RequestType = DeviceRequestType.WarmStart } },
            {DeviceRequestType.EchoOff, new ELM327Command{ IsUserFunction = false, Code = "E0", Name = "Set Echo Off", Description = "Turns off response echo" } },
            {DeviceRequestType.BufferDump, new ELM327Command{ IsUserFunction = false, Code = "BD", Name = "Buffer Dump", Description = "Perform a buffer dump", RequestType = DeviceRequestType.BufferDump } },
            {DeviceRequestType.AllowLongMessages, new ELM327Command{ IsUserFunction = false, Code = "AL", Name = "Allow Long Messages", Description = "Allows messages longer than 7 bytes", RequestType = DeviceRequestType.AllowLongMessages } },
            {DeviceRequestType.LineFeedsOff, new ELM327Command{ IsUserFunction = false, Code = "L0", Name = "Line Feeds Off", Description = "Don't return line feeds with <cr>", RequestType = DeviceRequestType.LineFeedsOff } },
            {DeviceRequestType.SpacesOff, new ELM327Command{ IsUserFunction = false, Code = "S0", Name = "Spaces Off", Description = "Don't return Spaces with responses", RequestType = DeviceRequestType.SpacesOff } },
            {DeviceRequestType.ForgetEvents, new ELM327Command{ IsUserFunction = false, Code = "FE", Name = "Forget Events", Description = "Forget Events like fatal can errors", RequestType = DeviceRequestType.ForgetEvents } },
            {DeviceRequestType.GetCurrentProtocolDescription, new ELM327Command{ Code = "DP", Name = "Get Protocol", Description = "Gets the currently active OBD2 protocol", RequestType = DeviceRequestType.GetCurrentProtocolDescription } },
            {DeviceRequestType.ProtocolSearch, new ELM327Command{ IsUserFunction = false, Code = "SP0", Name = "Search for protocol", Description = "Sets device to search for appropriate protocol with vehicle", RequestType = DeviceRequestType.ProtocolSearch } },
            {DeviceRequestType.SetProtocol, new ELM327Command{ IsUserFunction = false, Code = "TP", Name = "Set protocol", Description = "Set protocol", RequestType = DeviceRequestType.SetProtocol  } },
            {DeviceRequestType.GetSystemProtocolID, new ELM327Command{ IsUserFunction = false, Code = "DPN", Name = "Get protocol ID", Description = "Get protocol ID", RequestType = DeviceRequestType.GetSystemProtocolID  } },
            {DeviceRequestType.SetJ1850_PWM, new ELM327Command{ IsUserFunction = false, Code = "TP1", Name = "Set J1850 PWM protocol", Description = "Set J1850 PWM protocol", RequestType = DeviceRequestType.SetJ1850_PWM } },
            {DeviceRequestType.SetJ1850_VPW, new ELM327Command{ IsUserFunction = false, Code = "TP2", Name = "Set J1850 VPW protocol", Description = "Set J1850 VPW protocol", RequestType = DeviceRequestType.SetJ1850_VPW } },
            {DeviceRequestType.Set9141_2, new ELM327Command{ IsUserFunction = false, Code = "TP3", Name = "Set 9141-2 protocol", Description = "Set 9141-2 protocol", RequestType = DeviceRequestType.Set9141_2 } },
            {DeviceRequestType.SetCAN_11_500, new ELM327Command{ IsUserFunction = false, Code = "TP6", Name = "Set CAN 11-bit, 500k protocol", Description = "Set CAN 11-bit, 500k protocol", RequestType = DeviceRequestType.SetCAN_11_500 } },
            {DeviceRequestType.SetHeader, new ELM327Command{ IsUserFunction = false, Code = "SH", Name = "Set device obd2 header value", Description = "Set device obd2 protocol header value", RequestType = DeviceRequestType.SetHeader } },
            {DeviceRequestType.BaudRateFastTimeout, new ELM327Command{ IsUserFunction = false, Code = "BRT10", Name = "Set device obd2 baud rate fast timeout", Description = "Set device obd2 baud rate fast timeout", RequestType = DeviceRequestType.BaudRateFastTimeout } },
            {DeviceRequestType.SetISOBaudRate, new ELM327Command{ IsUserFunction = false, Code = "IB", Name = "Set ISO baud rate", Description = "Set ISO baud rate", RequestType = DeviceRequestType.SetISOBaudRate } },
            {DeviceRequestType.BaudRateFastDivisor, new ELM327Command{ IsUserFunction = false, Code = "BRDFF", Name = "Set device obd2 baud rate fast divisor", Description = "Set device obd2 baud rate fast divisor", RequestType = DeviceRequestType.BaudRateFastDivisor } },
            {DeviceRequestType.SetDefaultHeader, new ELM327Command{ IsUserFunction = false, Code = "SH7DF", Name = "Set device can header value (7DF)", Description = "Set device CAN header value (7DF)", RequestType = DeviceRequestType.SetDefaultHeader } },
            {DeviceRequestType.HeadersOn, new ELM327Command{ IsUserFunction = false, Code = "H1", Name = "Headers On", Description = "Headers On", RequestType = DeviceRequestType.HeadersOn } },
            {DeviceRequestType.HeadersOff, new ELM327Command{ IsUserFunction = false, Code = "H0", Name = "Headers Off", Description = "Headers Off", RequestType = DeviceRequestType.HeadersOff } },
            {DeviceRequestType.DisplayDataByteLengthOn, new ELM327Command{ IsUserFunction = false, Code = "D1", Name = "Display data length byte on", Description = "Display data length byte on", RequestType = DeviceRequestType.DisplayDataByteLengthOn } },
            {DeviceRequestType.DisplayDataByteLengthOff, new ELM327Command{ IsUserFunction = false, Code = "D0", Name = "Display data length byte off", Description = "Display data length byte off", RequestType = DeviceRequestType.DisplayDataByteLengthOff } },
            {DeviceRequestType.MemoryOff, new ELM327Command{ IsUserFunction = false, Code = "M0", Name = "Memory Off", Description = "Do Not store last used protocol to non-volitile memory", RequestType = DeviceRequestType.MemoryOff } },
            {DeviceRequestType.IdentifyIC, new ELM327Command{ IsUserFunction = false, Code = "I", Name = "Identify IC version", Description = "Identify IC version", RequestType = DeviceRequestType.IdentifyIC } },
            {DeviceRequestType.CAN_IFRH, new ELM327Command{ IsUserFunction = false, Code = "IFRH", Name = "Set CAN header", Description = "Set CAN header", RequestType = DeviceRequestType.CAN_IFRH } },
            {DeviceRequestType.SetDefaultCANRxAddress, new ELM327Command{ IsUserFunction = false, Code = "CRA7DF", Name = "Set Default CAN receive address (7DF)", Description = "Set Default CAN receive address (7DF)", RequestType = DeviceRequestType.SetDefaultCANRxAddress } },
            {DeviceRequestType.SetCANRxAddress, new ELM327Command{ IsUserFunction = false, Code = "CRA", Name = "Set CAN receive address", Description = "Set CAN receive address", RequestType = DeviceRequestType.SetCANRxAddress } },

            {DeviceRequestType.ISO_SlowInit, new ELM327Command{ IsUserFunction = false, Code = "SI", Name = "Run ISO Slow Init", Description = "Run ISO Slow Init", RequestType = DeviceRequestType.ISO_SlowInit } },
            {DeviceRequestType.ISO_SetSlowInitAddress, new ELM327Command{ IsUserFunction = false, Code = "IIA", Name = "Set ISO Slow Init Address", Description = "Set ISO Slow Init Address", RequestType = DeviceRequestType.ISO_SetSlowInitAddress } },
            {DeviceRequestType.SET_ISOInitAddress_13, new ELM327Command{ IsUserFunction = false, Code = "IIA13", Name = "Set ISO Init Address", Description = "Set ISO Init Address", RequestType = DeviceRequestType.SET_ISOInitAddress_13 } },
            {DeviceRequestType.SET_OBD1Wakeup, new ELM327Command{ IsUserFunction = false, Code = "SW", Name = "KWP Wakeup messages", Description = "KWP Wakeup messages", RequestType = DeviceRequestType.SET_OBD1Wakeup } },
            {DeviceRequestType.SET_OBD1WakeupOff, new ELM327Command{ IsUserFunction = false, Code = "SW00", Name = "KWP Wakeup messages off", Description = "KWP Wakeup messages off", RequestType = DeviceRequestType.SET_OBD1WakeupOff } },

            {DeviceRequestType.SET_Timeout, new ELM327Command{ IsUserFunction = false, Code = "ST", Name = "Set timeouts", Description = "Set timeouts", RequestType = DeviceRequestType.SET_Timeout } },
            {DeviceRequestType.SETAdaptiveTiming, new ELM327Command{ IsUserFunction = false, Code = "AT", Name = "Set Adaptive timing", Description = "Set Adaptive timing", RequestType = DeviceRequestType.SETAdaptiveTiming } },
            {DeviceRequestType.CANAddressFilter, new ELM327Command{ IsUserFunction = false, Code = "CF", Name = "Set CAN address filter", Description = "Set CAN address filter", RequestType = DeviceRequestType.CANAddressFilter } },
            {DeviceRequestType.CANAddressMask, new ELM327Command{ IsUserFunction = false, Code = "CM", Name = "Set CAN address mask", Description = "Set CAN address mask", RequestType = DeviceRequestType.CANAddressMask } },
            {DeviceRequestType.CANAddressMaskReset, new ELM327Command{ IsUserFunction = false, Code = "CM00000000", Name = "Reset CAN address mask", Description = "Reset CAN address mask", RequestType = DeviceRequestType.CANAddressMaskReset } },
            {DeviceRequestType.CANAutoAddress, new ELM327Command{ IsUserFunction = false, Code = "AR", Name = "Set CAN Auto address", Description = "Set CAN Auto address", RequestType = DeviceRequestType.CANAutoAddress } },
            {DeviceRequestType.CANAutoFormatOn, new ELM327Command{ IsUserFunction = false, Code = "CAF1", Name = "Auto-format CAN messages on", Description = "Auto-format sent and received CAN messages on", RequestType = DeviceRequestType.CANAutoFormatOn } },
            {DeviceRequestType.CANAutoFormatOff, new ELM327Command{ IsUserFunction = false, Code = "CAF0", Name = "Auto-format CAN messages off", Description = "Auto-format sent and received CAN messages off", RequestType = DeviceRequestType.CANAutoFormatOff } },
            {DeviceRequestType.CANFlowControlAutoOn, new ELM327Command{ IsUserFunction = false, Code = "CFC1", Name = "Auto-Flow CAN messages on", Description = "Auto-flow CAN messages on", RequestType = DeviceRequestType.CANFlowControlAutoOn } },
            {DeviceRequestType.CANFlowControlAutoOff, new ELM327Command{ IsUserFunction = false, Code = "CFC0", Name = "Auto-Flow CAN messages off", Description = "Auto-flow CAN messages off", RequestType = DeviceRequestType.CANFlowControlAutoOff } },
            {DeviceRequestType.CANSilentMonitoringOn, new ELM327Command{ IsUserFunction = false, Code = "CSM1", Name = "CAN Passive read on", Description = "CAN Passive read on", RequestType = DeviceRequestType.CANSilentMonitoringOn } },
            {DeviceRequestType.CANSilentMonitoringOff, new ELM327Command{ IsUserFunction = false, Code = "CSM0", Name = "CAN Passive read off", Description = "CAN Passive read off", RequestType = DeviceRequestType.CANSilentMonitoringOff } },
            {DeviceRequestType.CANVariableLengthOn, new ELM327Command{ IsUserFunction = false, Code = "V1", Name = "CAN variable byte length on", Description = "CAN variable byte length on", RequestType = DeviceRequestType.CANVariableLengthOn } },
            {DeviceRequestType.CANVariableLengthOff, new ELM327Command{ IsUserFunction = false, Code = "V0", Name = "CAN variable byte length off", Description = "CAN variable byte length off", RequestType = DeviceRequestType.CANVariableLengthOff } },
            {DeviceRequestType.MonitorAll, new ELM327Command{IsUserFunction = false, Code = "MA", Name = "Start Bus Monitor", Description = "Monitors vehicle's data bus", RequestType = DeviceRequestType.MonitorAll } },
            {DeviceRequestType.GetLiveData, new ELM327Command{IsUserFunction = false, Code = "", Name = "Live Data", Description = "Live Data", RequestType = DeviceRequestType.GetLiveData } },
           // {DeviceRequestType.OBD2_ClearDTCs, new ELM327Command{ Code = "", Name = "Clear DTCs", Description = "Clear OBD2 DTCs", RequestType = DeviceRequestType.OBD2_ClearDTCs } },
            //{DeviceRequestType.CAN_ClearDTCs, new ELM327Command{ Code = "FF00", Name = "Clear CAN DTCs", Description = "Clear CAN DTCs", RequestType = DeviceRequestType.CAN_ClearDTCs } },
            {DeviceRequestType.CAN_GetAllDTC, new ELM327Command{ Code = "FF00", Name = "Get all CAN Fault Codes", Description = "Ask vehicle for error codes from CAN", RequestType = DeviceRequestType.CAN_GetAllDTC,  // } },
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.CAN_GetDTCByStatus, new ELM327Command{ Code = "00FF00", Name = "Get Fault Codes by status", Description = "Ask vehicle for error codes by status", RequestType = DeviceRequestType.CAN_GetDTCByStatus,  // } },
                                function = OBD2Device.GetManufacturerDTCs } },
            {DeviceRequestType.CAN_GetDTCByStatus1, new ELM327Command{ Code = "FFFF00", Name = "Get Fault Codes by status", Description = "Ask vehicle for error codes by status", RequestType = DeviceRequestType.CAN_GetDTCByStatus1,  // } },
                                function = OBD2Device.GetManufacturerDTCs } },
            {DeviceRequestType.OBD2_GetDTCs, new ELM327Command{ Code = "03", Name = "Get Fault Codes", Description = "Ask vehicle for error codes", RequestType = DeviceRequestType.OBD2_GetDTCs, //} },
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.OBD2_GetPermanentDTCs, new ELM327Command{ Code = "0A", Name = "Get Permanent Fault Codes", Description = "Ask vehicle for permanent error codes", RequestType = DeviceRequestType.OBD2_GetPermanentDTCs, //} },
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.OBD2_GetPendingDTCs, new ELM327Command{IsUserFunction = false, Code = "07", Name = "Get Pending Fault Codes", Description = "Ask vehicle for pending error codes", RequestType = DeviceRequestType.OBD2_GetPendingDTCs,
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.OBD2_GetVIN, new ELM327Command{ Code = "02", Name = "Get VIN", Description = "Ask vehicle for its Vehice Identification Number (VIN)", RequestType = DeviceRequestType.OBD2_GetVIN, function=GetVIN } },
            {DeviceRequestType.DeviceDescription, new ELM327Command{ Code = "@1", Name = "Get Description", Description = "Gets a description from the device", RequestType = DeviceRequestType.DeviceDescription } },
            {DeviceRequestType.SerialNumber, new ELM327Command{ Code = "@2", Name = "Get Serial Number", Description = "Gets the permanent serial number (if programmed)", RequestType = DeviceRequestType.SerialNumber } },
            {DeviceRequestType.SupplyVoltage, new ELM327Command{ Code = "RV", Name = "Get Voltage", Description = "Gets the voltage of the connected vehicle", RequestType = DeviceRequestType.SupplyVoltage } },

            {DeviceRequestType.OBD2_GetPIDINFO_13, new ELM327Command{ Code = "13", Name = "O2 Locations 13", Description = "O2 Locations 13", RequestType = DeviceRequestType.OBD2_GetPIDINFO_13, function = ParsO2Locations13 } },
            {DeviceRequestType.OBD2_GetPIDINFO_1D, new ELM327Command{ Code = "1D", Name = "O2 Locations 1D", Description = "O2 Locations 1D", RequestType = DeviceRequestType.OBD2_GetPIDINFO_1D, function = ParsO2Locations1D } },
 
            {DeviceRequestType.OBD2_GetPIDS_00, new ELM327Command{ Code = "00", Name = "Supported PIDs 00", Description = "PIDS Request 0x00", RequestType = DeviceRequestType.OBD2_GetPIDS_00, function = GetSupportedPIDS } },
            {DeviceRequestType.OBD2_GetPIDS_20, new ELM327Command{ Code = "20", Name = "Supported PIDs 20", Description = "PIDS Request 0x20", RequestType = DeviceRequestType.OBD2_GetPIDS_20, function = GetSupportedPIDS } },
            {DeviceRequestType.OBD2_GetPIDS_40, new ELM327Command{ Code = "40", Name = "Supported PIDs 40", Description = "PIDS Request 0x40", RequestType = DeviceRequestType.OBD2_GetPIDS_40, function = GetSupportedPIDS } },
            {DeviceRequestType.OBD2_GetPIDS_60, new ELM327Command{ Code = "60", Name = "Supported PIDs 60", Description = "PIDS Request 0x60", RequestType = DeviceRequestType.OBD2_GetPIDS_60, function = GetSupportedPIDS } },
            {DeviceRequestType.OBD2_GetPIDS_80, new ELM327Command{ Code = "80", Name = "Supported PIDs 80", Description = "PIDS Request 0x80", RequestType = DeviceRequestType.OBD2_GetPIDS_80, function = GetSupportedPIDS } },
            {DeviceRequestType.OBD2_GetPIDS_A0, new ELM327Command{ Code = "A0", Name = "Supported PIDs A0", Description = "PIDS Request 0xA0", RequestType = DeviceRequestType.OBD2_GetPIDS_A0, function = GetSupportedPIDS } },
            {DeviceRequestType.OBD2_GetPIDS_C0, new ELM327Command{ Code = "C0", Name = "Supported PIDs C0", Description = "PIDS Request 0xC0", RequestType = DeviceRequestType.OBD2_GetPIDS_C0, function = GetSupportedPIDS } },
            {DeviceRequestType.OBD2_GetPIDS_E0, new ELM327Command{ Code = "E0", Name = "Supported PIDs E0", Description = "PIDS Request 0xE0", RequestType = DeviceRequestType.OBD2_GetPIDS_E0, function = GetSupportedPIDS } },

            {DeviceRequestType.OBD2_GetMonitorIds_00, new ELM327Command{ Code = "00", Name = "Monitor Ids 00", Description = "Monitor Ids Request 0x00", RequestType = DeviceRequestType.OBD2_GetMonitorIds_00, function = GetSupportedMonitorIds } },
            {DeviceRequestType.OBD2_GetMonitorIds_20, new ELM327Command{ Code = "20", Name = "Monitor Ids 20", Description = "Monitor Ids Request 0x20", RequestType = DeviceRequestType.OBD2_GetMonitorIds_20, function = GetSupportedMonitorIds } },
            {DeviceRequestType.OBD2_GetMonitorIds_40, new ELM327Command{ Code = "40", Name = "Monitor Ids 40", Description = "Monitor Ids Request 0x40", RequestType = DeviceRequestType.OBD2_GetMonitorIds_40, function = GetSupportedMonitorIds } },
            {DeviceRequestType.OBD2_GetMonitorIds_60, new ELM327Command{ Code = "60", Name = "Monitor Ids 60", Description = "Monitor Ids Request 0x60", RequestType = DeviceRequestType.OBD2_GetMonitorIds_60, function = GetSupportedMonitorIds } },
            {DeviceRequestType.OBD2_GetMonitorIds_80, new ELM327Command{ Code = "80", Name = "Monitor Ids 80", Description = "Monitor Ids Request 0x80", RequestType = DeviceRequestType.OBD2_GetMonitorIds_80, function = GetSupportedMonitorIds } },
            {DeviceRequestType.OBD2_GetMonitorIds_A0, new ELM327Command{ Code = "A0", Name = "Monitor Ids A0", Description = "Monitor Ids Request 0xA0", RequestType = DeviceRequestType.OBD2_GetMonitorIds_A0, function = GetSupportedMonitorIds } },
            {DeviceRequestType.OBD2_GetMonitorIds_C0, new ELM327Command{ Code = "C0", Name = "Monitor Ids C0", Description = "Monitor Ids Request 0xC0", RequestType = DeviceRequestType.OBD2_GetMonitorIds_C0, function = GetSupportedMonitorIds } },
            {DeviceRequestType.OBD2_GetMonitorIds_E0, new ELM327Command{ Code = "E0", Name = "Monitor Ids E0", Description = "Monitor Ids Request 0xE0", RequestType = DeviceRequestType.OBD2_GetMonitorIds_E0, function = GetSupportedMonitorIds } },

            {DeviceRequestType.OBD2_StatusSinceCodesLastCleared, new ELM327Command{ Code = "01", Name = "Status Since Last Fault Clearing", Description = "Gets status since codes last cleared", RequestType = DeviceRequestType.OBD2_StatusSinceCodesLastCleared,
                                function = (obj)=>
                                {

                                    Readiness readinessObject = new Readiness();

                                    string [] strArray = obj as string[];
                                    StringBuilder sb = new StringBuilder($"{Environment.NewLine}");
                                    foreach(string arrayString in strArray)
                                    {
                                        // skip the summary, get details...
                                        if(arrayString.Substring(8,4) == "0000") continue;
                                        int byteBufLen = 4;
                                        byte[] byteBuf = new byte[byteBufLen];

                                        string[] socl = new string[byteBufLen];
                                        int i = 0;
                                        var x = 0;
                                        for(i=0;i<byteBufLen;i++)
                                        {
                                            socl[i] = arrayString.Substring(4+(i*2),2);
                                        }

                                        for(i = 0;i<byteBufLen;i++)
                                        {
                                            byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                                        }

                                        x = 0;
                                        uint mask = 0b10000000; // init to binary 10000000
                                        ReadinessMonitor tempPIDCat = null;

                                        bool isGasolineEngine = (byteBuf[1] & 0x08) == 0x08; // bit 3 of byte 2 (spark vs compression monitors)
                                        int maxRng = 0;

                                        // for each byte...
                                        for (i = 0; i < byteBufLen; i++)
                                        {
                                            mask = 0b10000000; // reset to binary 10000000
                                            switch (i)
                                            {
                                                case 0:
                                                    // bit 7 (mil light)
                                                    tempPIDCat = OBD2Device.OBD2MONITORSTATUS[1];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        readinessObject.MonitorTestList.Add(tempPIDCat);
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }

                                                    readinessObject.DTCCount =  SystemReport.DTCCount = byteBuf[i] & 0x7F; // everything but bit 7 (mil light)
                                                    sb.Append($"DTC Count: {readinessObject.DTCCount}{Environment.NewLine}");

                                                    break;
                                                case 1:

                                                    readinessObject.IsSparkSystem = isGasolineEngine = (byteBuf[i] & 0x08) == 0x00; // bit 3 of byte 2 (spark vs compression monitors)

                                                    // tests that are incomplete
                                                    mask = 0b01000000; // reset to binary 01000000 (bit 6)
                                                    for (x = 4; x < 7; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }

                                                    // tests that are available (complete)
                                                    mask = 0b00000100; // reset to binary 00000100 (bit 2)
                                                    for (x = 8; x < 0x0B; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 2:
                                                    // Tests that are available (complete)
                                                    if(isGasolineEngine) x = 0x0B;
                                                    else x = 0x1B;

                                                    maxRng = x + 8;

                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 3:
                                                    // Tests that are incomplete
                                                    if(isGasolineEngine) x = 0x13;
                                                    else x = 0x23;
                                                    maxRng = x + 8;
                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                    return readinessObject;
                                } } },
            {DeviceRequestType.OBD2_StatusThisDriveCycle, new ELM327Command{ Code = "41", Name = "Status For This Drive Cycle", Description = "Gets monitor status For This Drive Cycle", RequestType = DeviceRequestType.OBD2_StatusThisDriveCycle,
                                function = (obj)=>
                                {

                                    Readiness readinessObject = new Readiness();

                                    string [] strArray = obj as string[];
                                    StringBuilder sb = new StringBuilder($"{Environment.NewLine}");
                                    foreach(string arrayString in strArray)
                                    {
                                        // skip the summary, get details...
                                        if(arrayString.Substring(8,4) == "0000") continue;
                                        int byteBufLen = 4;
                                        byte[] byteBuf = new byte[byteBufLen];

                                        string[] socl = new string[byteBufLen];
                                        int i = 0;
                                        var x = 0;
                                        for(i=0;i<byteBufLen;i++)
                                        {
                                            socl[i] = arrayString.Substring(4+(i*2),2);
                                        }

                                        for(i = 0;i<byteBufLen;i++)
                                        {
                                            byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                                        }

                                        x = 0;
                                        uint mask = 0b10000000; // init to binary 10000000
                                        ReadinessMonitor tempPIDCat = null;

                                        bool isGasolineEngine = (byteBuf[1] & 0x08) == 0x08; // bit 3 of byte 2 (spark vs compression monitors)
                                        int maxRng = 0;

                                        // for each byte...
                                        for (i = 0; i < byteBufLen; i++)
                                        {
                                            mask = 0b10000000; // reset to binary 10000000
                                            switch (i)
                                            {
                                                case 0:
                                                    // bit 7 (mil light)
                                                    tempPIDCat = OBD2Device.OBD2MONITORSTATUS[1];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        readinessObject.MonitorTestList.Add(tempPIDCat);
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }

                                                    readinessObject.DTCCount = SystemReport.DTCCount = byteBuf[i] & 0x7F; // everything but bit 7 (mil light)
                                                    sb.Append($"DTC Count: {readinessObject.DTCCount}{Environment.NewLine}");

                                                    break;
                                                case 1:

                                                    readinessObject.IsSparkSystem = isGasolineEngine = (byteBuf[i] & 0x08) == 0x00; // bit 3 of byte 2 (spark vs compression monitors)

                                                    // tests that are incomplete
                                                    mask = 0b01000000; // reset to binary 01000000 (bit 6)
                                                    for (x = 4; x < 7; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }

                                                    // tests that are available (complete)
                                                    mask = 0b00000100; // reset to binary 00000100 (bit 2)
                                                    for (x = 8; x < 0x0B; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 2:
                                                    // Tests that are available (complete)
                                                    if(isGasolineEngine) x = 0x0B;
                                                    else x = 0x1B;

                                                    maxRng = x + 8;

                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 3:
                                                    // Tests that are incomplete
                                                    if(isGasolineEngine) x = 0x13;
                                                    else x = 0x23;
                                                    maxRng = x + 8;
                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                    return readinessObject;
                                } } },
            {DeviceRequestType.OBD2_MAFRate, new ELM327Command{ Code = "10", Name = "MAF Rate (grams/sec)", Description = "Mass Air Flow Rate in grams/second", RequestType = DeviceRequestType.OBD2_MAFRate,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    var t = int.Parse(str.Substring(0,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    t += int.Parse(str.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
                                    return Convert.ToDouble(t)/100;
                                } } },
            {DeviceRequestType.OBD2_FreezeFrameDTC, new ELM327Command{ Code = "022", Name = "Freeze Frame Fault", Description = "Gets the fault code that caused a freeze frame to be save", RequestType = DeviceRequestType.OBD2_FreezeFrameDTC,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    uint retVal = 0;
                                    foreach(String st in strArray) // return with first hit...
                                    {
                                        if(st.Length < 9) continue;
                                        retVal = uint.Parse(st.Substring(6, 4), System.Globalization.NumberStyles.HexNumber);
                                        if(retVal != 0) return retVal;
                                    }

                                    return 0;
                                } } },
            {DeviceRequestType.OBD2_FreezeFrameDTC1, new ELM327Command{ Code = "02002", Name = "Freeze Frame Fault", Description = "Gets the fault code that caused a freeze frame to be save", RequestType = DeviceRequestType.OBD2_FreezeFrameDTC1,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    uint retVal = 0;
                                    foreach(String st in strArray) // return with first hit...
                                    {
                                        if(st.Length < 9) continue;
                                        retVal = uint.Parse(st.Substring(6, 4), System.Globalization.NumberStyles.HexNumber);
                                        if(retVal != 0) return retVal;
                                    }

                                    return 0;
                                } } },
            {DeviceRequestType.OBD2_FreezeFrame22DTC, new ELM327Command{ Code = "00022", Name = "Freeze Frame Fault", Description = "Gets the fault code that caused a freeze frame to be save", RequestType = DeviceRequestType.OBD2_FreezeFrameDTC,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    uint retVal = 0;
                                    foreach(String st in strArray) // return with first hit...
                                    {
                                        if(st.Length < 9) continue;
                                        //if(st.Length < DataPositionOffset + 4) continue;
                                        retVal = uint.Parse(st.Substring(6, 4), System.Globalization.NumberStyles.HexNumber);
                                        if(retVal != 0) return retVal;
                                    }

                                    return 0;
                                } } },
            {DeviceRequestType.OBD2_FuelSystemStatus, new ELM327Command{ Code = "03", Name = "Fuel System Status", Description = "Gets vehicle fuel system status", RequestType = DeviceRequestType.OBD2_FuelSystemStatus,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];

                                    var fuelStatus = new FuelSystemStatus[2];


                                    fuelStatus[0].ApplyValue(uint.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber));
                                    fuelStatus[1].ApplyValue(uint.Parse(strArray[0].Substring(8,2), System.Globalization.NumberStyles.HexNumber));
                                    // (A*256)+B
                                    //var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    //return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);



                                    return fuelStatus;
                                } } },
            {DeviceRequestType.OBD2_EngineCoolantTemp, new ELM327Command{ Code = "05", Name = "Get Coolant Temperature", Description = "Ask vehicle for coolant temperature", RequestType = DeviceRequestType.OBD2_EngineCoolantTemp,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (Convert.ToDouble((int.Parse(str, System.Globalization.NumberStyles.HexNumber)) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_GetAmbientTemp, new ELM327Command{ Code = "461", Name = "Get Ambient Temperature", Description = "Ask vehicle for ambient air temperature", RequestType = DeviceRequestType.OBD2_GetAmbientTemp,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return ((int.Parse(str, System.Globalization.NumberStyles.HexNumber) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_FuelRailPressure, new ELM327Command{ Code = "591", Name = "Fuel Rail Pressure", Description = "Vehicle fuel rail pressure", RequestType = DeviceRequestType.OBD2_FuelRailPressure,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 10 * 0.14503774;
                                } } },
            {DeviceRequestType.OBD2_OilTemperature, new ELM327Command{ Code = "5C1", Name = "Oil Temperature", Description = "Vehicle oil temperature", RequestType = DeviceRequestType.OBD2_OilTemperature,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return ((int.Parse(str, System.Globalization.NumberStyles.HexNumber) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_FuelInjectorTiming, new ELM327Command{ Code = "5D1", Name = "Fuel Injection Timing", Description = "Vehicle fuel injection timing", RequestType = DeviceRequestType.OBD2_FuelInjectorTiming,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber)/128) - 10;
                                } } },
            {DeviceRequestType.OBD2_FuelRate, new ELM327Command{ Code = "5E1", Name = "Fuel Rate", Description = "Vehicle fuel rate", RequestType = DeviceRequestType.OBD2_FuelRate,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 20;
                                } } },
            {DeviceRequestType.OBD2_ShortTermFuelTrimBank1, new ELM327Command{ Code = "061", Name = "Short-Term FT B1 %", Description = "Short-term fuel trim for bank 1 (%)", RequestType = DeviceRequestType.OBD2_ShortTermFuelTrimBank1,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_LongTermFuelTrimBank1, new ELM327Command{ Code = "071", Name = "Long-Term FT B1 %", Description = "Long-term fuel trim for bank 1 (%)", RequestType = DeviceRequestType.OBD2_LongTermFuelTrimBank1,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_ShortTermFuelTrimBank2, new ELM327Command{ Code = "081", Name = "Short-Term FT B2 %", Description = "Short-term fuel trim for bank 2 (%)", RequestType = DeviceRequestType.OBD2_ShortTermFuelTrimBank2,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_LongTermFuelTrimBank2, new ELM327Command{ Code = "091", Name = "Long-Term FT B2 %", Description = "Long-term fuel trim for bank 2 (%)", RequestType = DeviceRequestType.OBD2_LongTermFuelTrimBank2,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_IntakeMAP, new ELM327Command{ Code = "0B1", Name = "Intake Manifold Absolute Pressure", Description = "Vehicle intake MAP", RequestType = DeviceRequestType.OBD2_IntakeMAP,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 0.14503774;
                                } } },
            {DeviceRequestType.OBD2_GetEngineRPM, new ELM327Command{ Code = "0C1", Name = "Get Engine RPM", Description = "Ask vehicle for Engine RPM", RequestType = DeviceRequestType.OBD2_GetEngineRPM,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) / 4;
                                } } },
            {DeviceRequestType.OBD2_VehicleSpeed, new ELM327Command{ Code = "0D1", Name = "Get Vehicle Speed", Description = "Ask vehicle for the speed", RequestType = DeviceRequestType.OBD2_VehicleSpeed,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 0.6213712;
                                } } },
            {DeviceRequestType.OBD2_IntakeAirTemperature, new ELM327Command{ Code = "0F1", Name = "Intake Air Temperature", Description = "Vehicle intake air temperature", RequestType = DeviceRequestType.OBD2_IntakeAirTemperature,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (Convert.ToDouble((int.Parse(str, System.Globalization.NumberStyles.HexNumber)) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_ThrottlePostion, new ELM327Command{ Code = "111", Name = "Throttle Position", Description = "Ask vehicle for the throttle position", RequestType = DeviceRequestType.OBD2_ThrottlePostion,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_RelativeThrottlePostion, new ELM327Command{ Code = "451", Name = "Relative Throttle Position", Description = "Ask vehicle for the relative throttle position", RequestType = DeviceRequestType.OBD2_RelativeThrottlePostion,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AbsoluteThrottlePostionB, new ELM327Command{ Code = "471", Name = "Absolute Throttle Position B", Description = "Ask vehicle for the absolute throttle position B", RequestType = DeviceRequestType.OBD2_AbsoluteThrottlePostionB,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AbsoluteThrottlePostionC, new ELM327Command{ Code = "481", Name = "Absolute Throttle Position C", Description = "Ask vehicle for the absolute throttle position C", RequestType = DeviceRequestType.OBD2_AbsoluteThrottlePostionC,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AcceleratorPedalPostionD, new ELM327Command{ Code = "491", Name = "Accelerator Pedal Position D", Description = "Ask vehicle for the accelerator position D", RequestType = DeviceRequestType.OBD2_AcceleratorPedalPostionD,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AcceleratorPedalPostionE, new ELM327Command{ Code = "4A1", Name = "Accelerator Pedal Position E", Description = "Ask vehicle for the accelerator position E", RequestType = DeviceRequestType.OBD2_AcceleratorPedalPostionE,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AcceleratorPedalPostionF, new ELM327Command{ Code = "4B1", Name = "Accelerator Pedal Position F", Description = "Ask vehicle for the accelerator position F", RequestType = DeviceRequestType.OBD2_AcceleratorPedalPostionF,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_CommandedThrottleActuator, new ELM327Command{ Code = "4C1", Name = "Commanded Throttle Actuator Position", Description = "Ask vehicle for the commanded throttle actuator position", RequestType = DeviceRequestType.OBD2_CommandedThrottleActuator,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_GetEngineLoad, new ELM327Command{ Code = "041", Name = "Engine Load Calculated %", Description = "Gets vehicle engine calculated load %", RequestType = DeviceRequestType.OBD2_GetEngineLoad,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (Convert.ToDouble(int.Parse(str, System.Globalization.NumberStyles.HexNumber)) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_FuelLevel, new ELM327Command{ Code = "2F1", Name = "Fuel Level %", Description = "Ask vehicle for fuel level in percent", RequestType = DeviceRequestType.OBD2_FuelLevel ,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    return (int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_WarmUpsSinceDTCCleared, new ELM327Command{ Code = "301", Name = "Warmups since DTCs Cleared", Description = "Ask vehicle for number of warmups since DTCs Cleared", RequestType = DeviceRequestType.OBD2_WarmUpsSinceDTCCleared,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    return int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_KmSinceDTCCleared, new ELM327Command{ Code = "311", Name = "Distance since DTCs Cleared", Description = "Distance since DTCs Cleared", RequestType = DeviceRequestType.OBD2_KmSinceDTCCleared,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    // (A*256)+B
                                    var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_KmWithMilOn, new ELM327Command{ Code = "211", Name = "Distance with MIL on", Description = "Distance With MIL on", RequestType = DeviceRequestType.OBD2_KmWithMilOn,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    // (A*256)+B
                                    var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_Odometer, new ELM327Command{ Code = "A6", Name = "Odometer", Description = "Odometer", RequestType = DeviceRequestType.OBD2_Odometer,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    //var A = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 16777216;
                                    //A += int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber) * 65536;
                                    //A += int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return int.Parse(strArray[0].Substring(4,8), System.Globalization.NumberStyles.HexNumber)/10;
                                } } }

        };

        public static readonly Dictionary<DeviceRequestType, ELM327Command> ELM327CommandDictionary = new Dictionary<DeviceRequestType, ELM327Command>
        {
            {DeviceRequestType.None, new ELM327Command{IsUserFunction = false, Code = "", Name = "No Request", Description = "No Request", RequestType = DeviceRequestType.None, function = (obj)=>{ return null; } } },
            {DeviceRequestType.DeviceReset,  new ELM327Command{ Code = "ATZ", Name = "Reset Device", Description = "Resets the device", RequestType = DeviceRequestType.DeviceReset } },
            {DeviceRequestType.WarmStart,  new ELM327Command{ Code = "ATWS", Name = "Warm Start", Description = "Resets the device w/out LED test", RequestType = DeviceRequestType.WarmStart } },
            {DeviceRequestType.EchoOff, new ELM327Command{ IsUserFunction = false, Code = "ATE0", Name = "Set Echo Off", Description = "Turns off response echo" } },
            {DeviceRequestType.BufferDump, new ELM327Command{ IsUserFunction = false, Code = "ATBD", Name = "Buffer Dump", Description = "Perform a buffer dump", RequestType = DeviceRequestType.BufferDump } },
            {DeviceRequestType.AllowLongMessages, new ELM327Command{ IsUserFunction = false, Code = "ATAL", Name = "Allow Long Messages", Description = "Allows messages longer than 7 bytes", RequestType = DeviceRequestType.AllowLongMessages } },
            {DeviceRequestType.LineFeedsOff, new ELM327Command{ IsUserFunction = false, Code = "ATL0", Name = "Line Feeds Off", Description = "Don't return line feeds with <cr>", RequestType = DeviceRequestType.LineFeedsOff } },
            {DeviceRequestType.SpacesOff, new ELM327Command{ IsUserFunction = false, Code = "ATS0", Name = "Spaces Off", Description = "Don't return Spaces with responses", RequestType = DeviceRequestType.SpacesOff } },
            {DeviceRequestType.ForgetEvents, new ELM327Command{ IsUserFunction = false, Code = "ATFE", Name = "Forget Events", Description = "Forget Events like fatal can errors", RequestType = DeviceRequestType.ForgetEvents } },
            {DeviceRequestType.GetCurrentProtocolDescription, new ELM327Command{ Code = "ATDP", Name = "Get Protocol", Description = "Gets the currently active OBD2 protocol", RequestType = DeviceRequestType.GetCurrentProtocolDescription } },
            {DeviceRequestType.GetSystemProtocolID, new ELM327Command{ Code = "ATDPN", Name = "Get Protocol ID", Description = "Sets device to search for appropriate protocol's ID", RequestType = DeviceRequestType.GetSystemProtocolID} },
            {DeviceRequestType.ProtocolSearch, new ELM327Command{ IsUserFunction = false, Code = "ATSP0", Name = "Search for protocol", Description = "Sets device to search for appropriate protocol with vehicle", RequestType = DeviceRequestType.ProtocolSearch } },
            {DeviceRequestType.SetProtocol, new ELM327Command{ IsUserFunction = false, Code = "ATTP", Name = "Set protocol", Description = "Set protocol", RequestType = DeviceRequestType.SetProtocol  } },
            {DeviceRequestType.SetJ1850_PWM, new ELM327Command{ IsUserFunction = false, Code = "ATTP1", Name = "Set J1850 PWM protocol", Description = "Set J1850 PWM protocol", RequestType = DeviceRequestType.SetJ1850_PWM } },
            {DeviceRequestType.SetJ1850_VPW, new ELM327Command{ IsUserFunction = false, Code = "ATTP2", Name = "Set J1850 VPW protocol", Description = "Set J1850 VPW protocol", RequestType = DeviceRequestType.SetJ1850_VPW } },
            {DeviceRequestType.Set9141_2, new ELM327Command{ IsUserFunction = false, Code = "ATTP3", Name = "Set 9141-2 protocol", Description = "Set 9141-2 protocol", RequestType = DeviceRequestType.Set9141_2 } },
            {DeviceRequestType.SetCAN_11_500, new ELM327Command{ IsUserFunction = false, Code = "ATTP6", Name = "Set CAN 11-bit, 500k protocol", Description = "Set CAN 11-bit, 500k protocol", RequestType = DeviceRequestType.SetCAN_11_500 } },
            {DeviceRequestType.SetHeader, new ELM327Command{ IsUserFunction = false, Code = "ATSH", Name = "Set device obd2 header value", Description = "Set device obd2 protocol header value", RequestType = DeviceRequestType.SetHeader } },
            {DeviceRequestType.SetISOBaudRate, new ELM327Command{ IsUserFunction = false, Code = "ATIB", Name = "Set ISO baud rate", Description = "Set ISO baud rate", RequestType = DeviceRequestType.SetISOBaudRate } },
            {DeviceRequestType.BaudRateFastTimeout, new ELM327Command{ IsUserFunction = false, Code = "ATBRT10", Name = "Set device obd2 baud rate fast timeout", Description = "Set device obd2 baud rate fast timeout", RequestType = DeviceRequestType.BaudRateFastTimeout } },
            {DeviceRequestType.BaudRateFastDivisor, new ELM327Command{ IsUserFunction = false, Code = "ATBRDFF", Name = "Set device obd2 baud rate fast divisor", Description = "Set device obd2 baud rate fast divisor", RequestType = DeviceRequestType.BaudRateFastDivisor } },
            {DeviceRequestType.SetDefaultHeader, new ELM327Command{ IsUserFunction = false, Code = "ATSH7DF", Name = "Set device can header value (7DF)", Description = "Set device CAN header value (7DF)", RequestType = DeviceRequestType.SetDefaultHeader } },
            {DeviceRequestType.HeadersOn, new ELM327Command{ IsUserFunction = false, Code = "ATH1", Name = "Headers On", Description = "Headers On", RequestType = DeviceRequestType.HeadersOn } },
            {DeviceRequestType.HeadersOff, new ELM327Command{ IsUserFunction = false, Code = "ATH0", Name = "Headers Off", Description = "Headers Off", RequestType = DeviceRequestType.HeadersOff } },
            {DeviceRequestType.DisplayDataByteLengthOn, new ELM327Command{ IsUserFunction = false, Code = "ATD1", Name = "Display data length byte on", Description = "Display data length byte on", RequestType = DeviceRequestType.DisplayDataByteLengthOn } },
            {DeviceRequestType.DisplayDataByteLengthOff, new ELM327Command{ IsUserFunction = false, Code = "ATD0", Name = "Display data length byte off", Description = "Display data length byte off", RequestType = DeviceRequestType.DisplayDataByteLengthOff } },
            {DeviceRequestType.MemoryOff, new ELM327Command{ IsUserFunction = false, Code = "ATM0", Name = "Memory Off", Description = "Do Not store last used protocol to non-volitile memory", RequestType = DeviceRequestType.MemoryOff } },
            {DeviceRequestType.IdentifyIC, new ELM327Command{ IsUserFunction = false, Code = "ATI", Name = "Identify IC version", Description = "Identify IC version", RequestType = DeviceRequestType.IdentifyIC } },
            {DeviceRequestType.CAN_IFRH, new ELM327Command{ IsUserFunction = false, Code = "ATIFRH", Name = "Set CAN header", Description = "Set CAN header", RequestType = DeviceRequestType.CAN_IFRH } },
            {DeviceRequestType.SetDefaultCANRxAddress, new ELM327Command{ IsUserFunction = false, Code = "ATCRA7DF", Name = "Set Default CAN receive address (7DF)", Description = "Set Default CAN receive address (7DF)", RequestType = DeviceRequestType.SetDefaultCANRxAddress } },
            {DeviceRequestType.SetCANRxAddress, new ELM327Command{ IsUserFunction = false, Code = "ATCRA", Name = "Set CAN receive address", Description = "Set CAN receive address", RequestType = DeviceRequestType.SetCANRxAddress } },

            {DeviceRequestType.ISO_SlowInit, new ELM327Command{ IsUserFunction = false, Code = "ATSI", Name = "Set ISO Init Address", Description = "Set ISO Init Address", RequestType = DeviceRequestType.ISO_SlowInit } },
            {DeviceRequestType.ISO_SetSlowInitAddress, new ELM327Command{ IsUserFunction = false, Code = "ATIIA", Name = "Set ISO Slow Init Address", Description = "Set ISO Slow Init Address", RequestType = DeviceRequestType.ISO_SetSlowInitAddress } },
            {DeviceRequestType.SET_ISOInitAddress_13, new ELM327Command{ IsUserFunction = false, Code = "ATIIA13", Name = "Set ISO Init Address", Description = "Set ISO Init Address", RequestType = DeviceRequestType.SET_ISOInitAddress_13 } },
            {DeviceRequestType.SET_OBD1Wakeup, new ELM327Command{ IsUserFunction = false, Code = "ATSW", Name = "KWP Wakeup messages", Description = "KWP Wakeup messages", RequestType = DeviceRequestType.SET_OBD1Wakeup } },
            {DeviceRequestType.SET_OBD1WakeupOff, new ELM327Command{ IsUserFunction = false, Code = "ATSW00", Name = "KWP Wakeup messages off", Description = "KWP Wakeup messages off", RequestType = DeviceRequestType.SET_OBD1WakeupOff } },

            {DeviceRequestType.SET_Timeout, new ELM327Command{ IsUserFunction = false, Code = "ATST", Name = "Set timeouts", Description = "Set timeouts", RequestType = DeviceRequestType.SET_Timeout } },
            {DeviceRequestType.SETAdaptiveTiming, new ELM327Command{ IsUserFunction = false, Code = "ATAT", Name = "Set Adaptive timing", Description = "Set Adaptive timing", RequestType = DeviceRequestType.SETAdaptiveTiming } },
            {DeviceRequestType.CANAddressFilter, new ELM327Command{ IsUserFunction = false, Code = "ATCF", Name = "Set CAN address filter", Description = "Set CAN address filter", RequestType = DeviceRequestType.CANAddressFilter } },
            {DeviceRequestType.CANAddressMask, new ELM327Command{ IsUserFunction = false, Code = "ATCM", Name = "Set CAN address mask", Description = "Set CAN address mask", RequestType = DeviceRequestType.CANAddressMask } },
            {DeviceRequestType.CANAddressMaskReset, new ELM327Command{ IsUserFunction = false, Code = "ATCM00000000", Name = "Reset CAN address mask", Description = "Reset CAN address mask", RequestType = DeviceRequestType.CANAddressMaskReset } },
            {DeviceRequestType.CANAutoAddress, new ELM327Command{ IsUserFunction = false, Code = "ATAR", Name = "Set CAN Auto address", Description = "Set CAN Auto address", RequestType = DeviceRequestType.CANAutoAddress } },
            {DeviceRequestType.CANAutoFormatOn, new ELM327Command{ IsUserFunction = false, Code = "ATCAF1", Name = "Auto-format CAN messages on", Description = "Auto-format sent and received CAN messages on", RequestType = DeviceRequestType.CANAutoFormatOn } },
            {DeviceRequestType.CANAutoFormatOff, new ELM327Command{ IsUserFunction = false, Code = "ATCAF0", Name = "Auto-format CAN messages off", Description = "Auto-format sent and received CAN messages off", RequestType = DeviceRequestType.CANAutoFormatOff } },
            {DeviceRequestType.CANFlowControlAutoOn, new ELM327Command{ IsUserFunction = false, Code = "ATCFC1", Name = "Auto-Flow CAN messages on", Description = "Auto-flow CAN messages on", RequestType = DeviceRequestType.CANFlowControlAutoOn } },
            {DeviceRequestType.CANFlowControlAutoOff, new ELM327Command{ IsUserFunction = false, Code = "ATCFC0", Name = "Auto-Flow CAN messages off", Description = "Auto-flow CAN messages off", RequestType = DeviceRequestType.CANFlowControlAutoOff } },
            {DeviceRequestType.CANSilentMonitoringOn, new ELM327Command{ IsUserFunction = false, Code = "ATCSM1", Name = "CAN Passive read on", Description = "CAN Passive read on", RequestType = DeviceRequestType.CANSilentMonitoringOn } },
            {DeviceRequestType.CANSilentMonitoringOff, new ELM327Command{ IsUserFunction = false, Code = "ATCSM0", Name = "CAN Passive read off", Description = "CAN Passive read off", RequestType = DeviceRequestType.CANSilentMonitoringOff } },
            {DeviceRequestType.CANVariableLengthOn, new ELM327Command{ IsUserFunction = false, Code = "ATV1", Name = "CAN variable byte length on", Description = "CAN variable byte length on", RequestType = DeviceRequestType.CANVariableLengthOn } },
            {DeviceRequestType.CANVariableLengthOff, new ELM327Command{ IsUserFunction = false, Code = "ATV0", Name = "CAN variable byte length off", Description = "CAN variable byte length off", RequestType = DeviceRequestType.CANVariableLengthOff } },
            {DeviceRequestType.MonitorAll, new ELM327Command{IsUserFunction = false, Code = "ATMA", Name = "Start Bus Monitor", Description = "Monitors vehicle's data bus", RequestType = DeviceRequestType.MonitorAll } },
            {DeviceRequestType.GetLiveData, new ELM327Command{IsUserFunction = false, Code = "", Name = "Live Data", Description = "Live Data", RequestType = DeviceRequestType.GetLiveData } },
            {DeviceRequestType.OBD2_ClearDTCs, new ELM327Command{ Code = "04", Name = "Clear DTCs", Description = "Clear OBD2 DTCs", RequestType = DeviceRequestType.OBD2_ClearDTCs } },
            {DeviceRequestType.CAN_ClearDTCs, new ELM327Command{ Code = "14FF00", Name = "Clear CAN DTCs", Description = "Clear CAN DTCs", RequestType = DeviceRequestType.CAN_ClearDTCs } },
            {DeviceRequestType.CAN_ClearDTCs_2, new ELM327Command{ Code = "14FFFFFF", Name = "Clear CAN DTCs 2", Description = "Clear CAN DTCs 2", RequestType = DeviceRequestType.CAN_ClearDTCs_2 } },
            {DeviceRequestType.CAN_GetAllDTC, new ELM327Command{ Code = "13FF00", Name = "Get all CAN Fault Codes", Description = "Ask vehicle for error codes from CAN", RequestType = DeviceRequestType.CAN_GetAllDTC,  // } },
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.CAN_GetDTCByStatus, new ELM327Command{ Code = "1800FF00", Name = "Get Fault Codes by status", Description = "Ask vehicle for error codes by status", RequestType = DeviceRequestType.CAN_GetDTCByStatus,  // } },
                                function = OBD2Device.GetManufacturerDTCs } },
            {DeviceRequestType.CAN_GetDTCByStatus1, new ELM327Command{ Code = "18FFFF00", Name = "Get Fault Codes by status", Description = "Ask vehicle for error codes by status", RequestType = DeviceRequestType.CAN_GetDTCByStatus1,  // } },
                                function = OBD2Device.GetManufacturerDTCs } },
            {DeviceRequestType.OBD2_GetDTCs, new ELM327Command{ Code = "03", Name = "Get Fault Codes", Description = "Ask vehicle for error codes", RequestType = DeviceRequestType.OBD2_GetDTCs, //} },
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.OBD2_GetPermanentDTCs, new ELM327Command{ Code = "0A", Name = "Get Permanent Fault Codes", Description = "Ask vehicle for permanent error codes", RequestType = DeviceRequestType.OBD2_GetPermanentDTCs, //} },
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.OBD2_GetPendingDTCs, new ELM327Command{IsUserFunction = false, Code = "07", Name = "Get Pending Fault Codes", Description = "Ask vehicle for pending error codes", RequestType = DeviceRequestType.OBD2_GetPendingDTCs,
                                function = OBD2Device.GetDTCs } },
            {DeviceRequestType.OBD2_GetVIN, new ELM327Command{ Code = "0902", Name = "Get VIN", Description = "Ask vehicle for its Vehice Identification Number (VIN)", RequestType = DeviceRequestType.OBD2_GetVIN, function=GetVIN } },
            {DeviceRequestType.DeviceDescription, new ELM327Command{ Code = "AT@1", Name = "Get Description", Description = "Gets a description from the device", RequestType = DeviceRequestType.DeviceDescription } },
            {DeviceRequestType.SerialNumber, new ELM327Command{ Code = "AT@2", Name = "Get Serial Number", Description = "Gets the permanent serial number (if programmed)", RequestType = DeviceRequestType.SerialNumber } },
            {DeviceRequestType.SupplyVoltage, new ELM327Command{ Code = "ATRV", Name = "Get Voltage", Description = "Gets the voltage of the connected vehicle", RequestType = DeviceRequestType.SupplyVoltage } },
            // special case of non-AT command, used for inititalization and protocol detection
            {DeviceRequestType.OBD2_GetPIDS_00, new ELM327Command{ Code = "0100", Name = "Supported PIDs", Description = "Ask vehicle for supported PIDS", RequestType = DeviceRequestType.OBD2_GetPIDS_00 } },
             //{DeviceRequestType.OBD2_GetPIDS_00, new ELM327Command{ Code = "0100", Name = "Supported PIDs", Description = "Ask vehicle for supported PIDS", RequestType = DeviceRequestType.OBD2_GetPIDS_00,
           //                    function = (obj)=>
            //                    {
            //                        int i = 0;
            //                        var x = 0;
            //                        var dataIndex = 0;
            //                        string [] strArray = obj as string[];
            //                        int byteBufLen = 4;
            //                        byte[] byteBuf = new byte[byteBufLen];

            //                        // Identify which string has the response
            //                        for(i=0;i<strArray.Length;i++)
            //                        {
            //                            if(strArray[i].Length>4 && strArray[i].Substring(0,4) == "4100")
            //                            {
            //                                dataIndex = i;
            //                                break;
            //                            }
            //                        }


            //                        string[] socl = new string[byteBufLen];
            //                        for(i=0;i<byteBufLen;i++)
            //                        {
            //                            socl[i] = strArray[dataIndex].Substring(4+(i*2),2);
            //                        }

            //                        for(i = 0;i<byteBufLen;i++)
            //                        {
            //                            byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
            //                        }

            //                        x = 0;
            //                        uint mask = 0b10000000; // init to binary 10000000
            //                        ReadinessMonitor tempPIDCat = null;
            //                        StringBuilder sb = new StringBuilder($"Supported PIDs:{Environment.NewLine}");
            //                        for (i = 0; i < byteBufLen; i++)
            //                        {
            //                            mask = 0b10000000; // reset to binary 10000000
            //                            switch (i)
            //                            {
            //                                case 0:
            //                                    for (x = 1; x < 9; x++)
            //                                    {
            //                                        tempPIDCat = OBD2Device.OBD2PIDS[x];
            //                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
            //                                        {
            //                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
            //                                        }
            //                                        mask >>= 1;
            //                                    }
            //                                    break;
            //                                case 1:
            //                                    for (x = 9; x < 17; x++)
            //                                    {
            //                                        tempPIDCat = OBD2Device.OBD2PIDS[x];
            //                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
            //                                        {
            //                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
            //                                        }
            //                                        mask >>= 1;
            //                                    }
            //                                    break;
            //                                case 2:
            //                                    for (x = 17; x < 25; x++)
            //                                    {
            //                                        tempPIDCat = OBD2Device.OBD2PIDS[x];
            //                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
            //                                        {
            //                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
            //                                        }
            //                                        mask >>= 1;
            //                                    }
            //                                    break;
            //                                case 3:
            //                                    for (x = 25; x < 33; x++)
            //                                    {
            //                                        tempPIDCat = OBD2Device.OBD2PIDS[x];
            //                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
            //                                        {
            //                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
            //                                        }
            //                                        mask >>= 1;
            //                                    }
            //                                    break;
            //                            }
            //                        }
            //                        return sb.ToString();
            //                    } } },
            {DeviceRequestType.OBD2_StatusSinceCodesLastCleared, new ELM327Command{ Code = "0101", Name = "Status Since Last Fault Clearing", Description = "Gets status since codes last cleared", RequestType = DeviceRequestType.OBD2_StatusSinceCodesLastCleared,
                                function = (obj)=>
                                {

                                    Readiness readinessObject = new Readiness();

                                    string [] strArray = obj as string[];
                                    StringBuilder sb = new StringBuilder($"{Environment.NewLine}");
                                    foreach(string arrayString in strArray)
                                    {
                                        // skip the summary, get details...
                                        if(arrayString.Substring(8,4) == "0000") continue;
                                        int byteBufLen = 4;
                                        byte[] byteBuf = new byte[byteBufLen];

                                        string[] socl = new string[byteBufLen];
                                        int i = 0;
                                        var x = 0;
                                        for(i=0;i<byteBufLen;i++)
                                        {
                                            socl[i] = arrayString.Substring(4+(i*2),2);
                                        }

                                        for(i = 0;i<byteBufLen;i++)
                                        {
                                            byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                                        }

                                        x = 0;
                                        uint mask = 0b10000000; // init to binary 10000000
                                        ReadinessMonitor tempPIDCat = null;

                                        bool isGasolineEngine = (byteBuf[1] & 0x08) == 0x08; // bit 3 of byte 2 (spark vs compression monitors)
                                        int maxRng = 0;

                                        // for each byte...
                                        for (i = 0; i < byteBufLen; i++)
                                        {
                                            mask = 0b10000000; // reset to binary 10000000
                                            switch (i)
                                            {
                                                case 0:
                                                    // bit 7 (mil light)
                                                    tempPIDCat = OBD2Device.OBD2MONITORSTATUS[1];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        readinessObject.MonitorTestList.Add(tempPIDCat);
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }

                                                    readinessObject.DTCCount = SystemReport.DTCCount = byteBuf[i] & 0x7F; // everything but bit 7 (mil light)
                                                    sb.Append($"DTC Count: {readinessObject.DTCCount}{Environment.NewLine}");

                                                    break;
                                                case 1:

                                                    readinessObject.IsSparkSystem = isGasolineEngine = (byteBuf[i] & 0x08) == 0x00; // bit 3 of byte 2 (spark vs compression monitors)

                                                    // tests that are incomplete
                                                    mask = 0b01000000; // reset to binary 01000000 (bit 6)
                                                    for (x = 4; x < 7; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }

                                                    // tests that are available (complete)
                                                    mask = 0b00000100; // reset to binary 00000100 (bit 2)
                                                    for (x = 8; x < 0x0B; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 2:
                                                    // Tests that are available (complete)
                                                    if(isGasolineEngine) x = 0x0B;
                                                    else x = 0x1B;

                                                    maxRng = x + 8;

                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 3:
                                                    // Tests that are incomplete
                                                    if(isGasolineEngine) x = 0x13;
                                                    else x = 0x23;
                                                    maxRng = x + 8;
                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                    return readinessObject;
                                } } },
            {DeviceRequestType.OBD2_StatusThisDriveCycle, new ELM327Command{ Code = "0141", Name = "Status For This Drive Cycle", Description = "Gets monitor status For This Drive Cycle", RequestType = DeviceRequestType.OBD2_StatusThisDriveCycle,
                                function = (obj)=>
                                {

                                    Readiness readinessObject = new Readiness();

                                    string [] strArray = obj as string[];
                                    StringBuilder sb = new StringBuilder($"{Environment.NewLine}");
                                    foreach(string arrayString in strArray)
                                    {
                                        // skip the summary, get details...
                                        if(arrayString.Substring(8,4) == "0000") continue;
                                        int byteBufLen = 4;
                                        byte[] byteBuf = new byte[byteBufLen];

                                        string[] socl = new string[byteBufLen];
                                        int i = 0;
                                        var x = 0;
                                        for(i=0;i<byteBufLen;i++)
                                        {
                                            socl[i] = arrayString.Substring(4+(i*2),2);
                                        }

                                        for(i = 0;i<byteBufLen;i++)
                                        {
                                            byteBuf[i] = byte.Parse(socl[i], System.Globalization.NumberStyles.HexNumber);
                                        }

                                        x = 0;
                                        uint mask = 0b10000000; // init to binary 10000000
                                        ReadinessMonitor tempPIDCat = null;

                                        bool isGasolineEngine = (byteBuf[1] & 0x08) == 0x08; // bit 3 of byte 2 (spark vs compression monitors)
                                        int maxRng = 0;

                                        // for each byte...
                                        for (i = 0; i < byteBufLen; i++)
                                        {
                                            mask = 0b10000000; // reset to binary 10000000
                                            switch (i)
                                            {
                                                case 0:
                                                    // bit 7 (mil light)
                                                    tempPIDCat = OBD2Device.OBD2MONITORSTATUS[1];
                                                    if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                    {
                                                        readinessObject.MonitorTestList.Add(tempPIDCat);
                                                        sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                    }

                                                    readinessObject.DTCCount = SystemReport.DTCCount = byteBuf[i] & 0x7F; // everything but bit 7 (mil light)
                                                    sb.Append($"DTC Count: {readinessObject.DTCCount}{Environment.NewLine}");

                                                    break;
                                                case 1:

                                                    readinessObject.IsSparkSystem = isGasolineEngine = (byteBuf[i] & 0x08) == 0x00; // bit 3 of byte 2 (spark vs compression monitors)

                                                    // tests that are incomplete
                                                    mask = 0b01000000; // reset to binary 01000000 (bit 6)
                                                    for (x = 4; x < 7; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }

                                                    // tests that are available (complete)
                                                    mask = 0b00000100; // reset to binary 00000100 (bit 2)
                                                    for (x = 8; x < 0x0B; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 2:
                                                    // Tests that are available (complete)
                                                    if(isGasolineEngine) x = 0x0B;
                                                    else x = 0x1B;

                                                    maxRng = x + 8;

                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                                case 3:
                                                    // Tests that are incomplete
                                                    if(isGasolineEngine) x = 0x13;
                                                    else x = 0x23;
                                                    maxRng = x + 8;
                                                    for (; x < maxRng; x++)
                                                    {
                                                        tempPIDCat = OBD2Device.OBD2MONITORSTATUS[x];
                                                        if (tempPIDCat.IsSupported = !((mask & byteBuf[i]) == 0))
                                                        {
                                                            readinessObject.MonitorTestList.Add(tempPIDCat);
                                                            sb.Append($"{tempPIDCat.Description}{Environment.NewLine}");
                                                        }
                                                        mask >>= 1;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                    return readinessObject;
                                } } },
            {DeviceRequestType.OBD2_MAFRate, new ELM327Command{ Code = "0110", Name = "MAF Rate (grams/sec)", Description = "Mass Air Flow Rate in grams/second", RequestType = DeviceRequestType.OBD2_MAFRate,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    var t = int.Parse(str.Substring(0,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    t += int.Parse(str.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
                                    return Convert.ToDouble(t)/100;
                                } } },
            {DeviceRequestType.OBD2_FreezeFrameDTC, new ELM327Command{ Code = "01022", Name = "Freeze Frame Fault", Description = "Gets the fault code that caused a freeze frame to be save", RequestType = DeviceRequestType.OBD2_FreezeFrameDTC,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];

                                    return strArray[0];
                                } } },
            {DeviceRequestType.OBD2_FreezeFrameDTC1, new ELM327Command{ Code = "0202002", Name = "Freeze Frame Fault", Description = "Gets the fault code that caused a freeze frame to be save", RequestType = DeviceRequestType.OBD2_FreezeFrameDTC1,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];

                                    return strArray[0];
                                } } },
            {DeviceRequestType.OBD2_FreezeFrame22DTC, new ELM327Command{ Code = "2200022", Name = "Freeze Frame Fault", Description = "Gets the fault code that caused a freeze frame to be save", RequestType = DeviceRequestType.OBD2_FreezeFrameDTC,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];

                                    return strArray[0];
                                } } },
            {DeviceRequestType.OBD2_FuelSystemStatus, new ELM327Command{ Code = "0103", Name = "Fuel System Status", Description = "Gets vehicle fuel system status", RequestType = DeviceRequestType.OBD2_FuelSystemStatus,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];

                                    var fuelStatus = new FuelSystemStatus[2];


                                    fuelStatus[0].ApplyValue(uint.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber));
                                    fuelStatus[1].ApplyValue(uint.Parse(strArray[0].Substring(8,2), System.Globalization.NumberStyles.HexNumber));
                                    // (A*256)+B
                                    //var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    //return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);



                                    return fuelStatus;
                                } } },
            {DeviceRequestType.OBD2_EngineCoolantTemp, new ELM327Command{ Code = "0105", Name = "Get Coolant Temperature", Description = "Ask vehicle for coolant temperature", RequestType = DeviceRequestType.OBD2_EngineCoolantTemp,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (Convert.ToDouble((int.Parse(str, System.Globalization.NumberStyles.HexNumber)) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_GetAmbientTemp, new ELM327Command{ Code = "01461", Name = "Get Ambient Temperature", Description = "Ask vehicle for ambient air temperature", RequestType = DeviceRequestType.OBD2_GetAmbientTemp,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return ((int.Parse(str, System.Globalization.NumberStyles.HexNumber) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_FuelRailPressure, new ELM327Command{ Code = "01591", Name = "Fuel Rail Pressure", Description = "Vehicle fuel rail pressure", RequestType = DeviceRequestType.OBD2_FuelRailPressure,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 10 * 0.14503774;
                                } } },
            {DeviceRequestType.OBD2_OilTemperature, new ELM327Command{ Code = "015C1", Name = "Oil Temperature", Description = "Vehicle oil temperature", RequestType = DeviceRequestType.OBD2_OilTemperature,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return ((int.Parse(str, System.Globalization.NumberStyles.HexNumber) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_FuelInjectorTiming, new ELM327Command{ Code = "015D1", Name = "Fuel Injection Timing", Description = "Vehicle fuel injection timing", RequestType = DeviceRequestType.OBD2_FuelInjectorTiming,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber)/128) - 10;
                                } } },
            {DeviceRequestType.OBD2_FuelRate, new ELM327Command{ Code = "015E1", Name = "Fuel Rate", Description = "Vehicle fuel rate", RequestType = DeviceRequestType.OBD2_FuelRate,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 20;
                                } } },
            {DeviceRequestType.OBD2_ShortTermFuelTrimBank1, new ELM327Command{ Code = "01061", Name = "Short-Term FT B1 %", Description = "Short-term fuel trim for bank 1 (%)", RequestType = DeviceRequestType.OBD2_ShortTermFuelTrimBank1,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_LongTermFuelTrimBank1, new ELM327Command{ Code = "01071", Name = "Long-Term FT B1 %", Description = "Long-term fuel trim for bank 1 (%)", RequestType = DeviceRequestType.OBD2_LongTermFuelTrimBank1,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_ShortTermFuelTrimBank2, new ELM327Command{ Code = "01081", Name = "Short-Term FT B2 %", Description = "Short-term fuel trim for bank 2 (%)", RequestType = DeviceRequestType.OBD2_ShortTermFuelTrimBank2,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_LongTermFuelTrimBank2, new ELM327Command{ Code = "01091", Name = "Long-Term FT B2 %", Description = "Long-term fuel trim for bank 2 (%)", RequestType = DeviceRequestType.OBD2_LongTermFuelTrimBank2,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    //(A-128) * 100/128
                                    var t = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                                    return ((double)t-128) * 100/128;
                                } } },
            {DeviceRequestType.OBD2_IntakeMAP, new ELM327Command{ Code = "010B1", Name = "Intake Manifold Absolute Pressure", Description = "Vehicle intake MAP", RequestType = DeviceRequestType.OBD2_IntakeMAP,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 0.14503774;
                                } } },
            {DeviceRequestType.OBD2_GetEngineRPM, new ELM327Command{ Code = "010C1", Name = "Get Engine RPM", Description = "Ask vehicle for Engine RPM", RequestType = DeviceRequestType.OBD2_GetEngineRPM,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) / 4;
                                } } },
            {DeviceRequestType.OBD2_VehicleSpeed, new ELM327Command{ Code = "010D1", Name = "Get Vehicle Speed", Description = "Ask vehicle for the speed", RequestType = DeviceRequestType.OBD2_VehicleSpeed,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 0.6213712;
                                } } },
            {DeviceRequestType.OBD2_IntakeAirTemperature, new ELM327Command{ Code = "010F1", Name = "Intake Air Temperature", Description = "Vehicle intake air temperature", RequestType = DeviceRequestType.OBD2_IntakeAirTemperature,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (Convert.ToDouble((int.Parse(str, System.Globalization.NumberStyles.HexNumber)) - 40) * 1.8) + 32;
                                } } },
            {DeviceRequestType.OBD2_ThrottlePostion, new ELM327Command{ Code = "01111", Name = "Throttle Position", Description = "Ask vehicle for the throttle position", RequestType = DeviceRequestType.OBD2_ThrottlePostion,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_RelativeThrottlePostion, new ELM327Command{ Code = "01451", Name = "Relative Throttle Position", Description = "Ask vehicle for the relative throttle position", RequestType = DeviceRequestType.OBD2_RelativeThrottlePostion,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AbsoluteThrottlePostionB, new ELM327Command{ Code = "01471", Name = "Absolute Throttle Position B", Description = "Ask vehicle for the absolute throttle position B", RequestType = DeviceRequestType.OBD2_AbsoluteThrottlePostionB,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AbsoluteThrottlePostionC, new ELM327Command{ Code = "01481", Name = "Absolute Throttle Position C", Description = "Ask vehicle for the absolute throttle position C", RequestType = DeviceRequestType.OBD2_AbsoluteThrottlePostionC,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AcceleratorPedalPostionD, new ELM327Command{ Code = "01491", Name = "Accelerator Pedal Position D", Description = "Ask vehicle for the accelerator position D", RequestType = DeviceRequestType.OBD2_AcceleratorPedalPostionD,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AcceleratorPedalPostionE, new ELM327Command{ Code = "014A1", Name = "Accelerator Pedal Position E", Description = "Ask vehicle for the accelerator position E", RequestType = DeviceRequestType.OBD2_AcceleratorPedalPostionE,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_AcceleratorPedalPostionF, new ELM327Command{ Code = "014B1", Name = "Accelerator Pedal Position F", Description = "Ask vehicle for the accelerator position F", RequestType = DeviceRequestType.OBD2_AcceleratorPedalPostionF,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_CommandedThrottleActuator, new ELM327Command{ Code = "014C1", Name = "Commanded Throttle Actuator Position", Description = "Ask vehicle for the commanded throttle actuator position", RequestType = DeviceRequestType.OBD2_CommandedThrottleActuator,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_GetEngineLoad, new ELM327Command{ Code = "01041", Name = "Engine Load Calculated %", Description = "Gets vehicle engine calculated load %", RequestType = DeviceRequestType.OBD2_GetEngineLoad,
                                function = (obj)=>
                                {
                                    string str = obj as string;
                                    return (Convert.ToDouble(int.Parse(str, System.Globalization.NumberStyles.HexNumber)) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_FuelLevel, new ELM327Command{ Code = "012F1", Name = "Fuel Level %", Description = "Ask vehicle for fuel level in percent", RequestType = DeviceRequestType.OBD2_FuelLevel ,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    return (int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 100) / 256;
                                } } },
            {DeviceRequestType.OBD2_WarmUpsSinceDTCCleared, new ELM327Command{ Code = "01302", Name = "Warmups since DTCs Cleared", Description = "Ask vehicle for number of warmups since DTCs Cleared", RequestType = DeviceRequestType.OBD2_WarmUpsSinceDTCCleared,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    return int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_KmSinceDTCCleared, new ELM327Command{ Code = "01312", Name = "Distance since DTCs Cleared", Description = "Distance since DTCs Cleared", RequestType = DeviceRequestType.OBD2_KmSinceDTCCleared,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    // (A*256)+B
                                    var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_KmWithMilOn, new ELM327Command{ Code = "01212", Name = "Distance with MIL on", Description = "Distance With MIL on", RequestType = DeviceRequestType.OBD2_KmWithMilOn,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    // (A*256)+B
                                    var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                                } } },
            {DeviceRequestType.OBD2_Odometer, new ELM327Command{ Code = "01A64", Name = "Odometer", Description = "Odometer", RequestType = DeviceRequestType.OBD2_Odometer,
                                function = (obj)=>
                                {
                                    string [] strArray = obj as string[];
                                    //var A = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 16777216;
                                    //A += int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber) * 65536;
                                    //A += int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                                    return int.Parse(strArray[0].Substring(4,8), System.Globalization.NumberStyles.HexNumber)/10;
                                } } }

        };

        public static readonly Dictionary<int, ReadinessMonitor> OBD2MONITORSTATUS = new Dictionary<int, ReadinessMonitor>
        {
            {0x01,  new ReadinessMonitor {Code=0x01, BitMask = (ulong)0x80000000, IsFault=true, Description = "MIL Light On" } },
            {0x02,  new ReadinessMonitor {Code=0x02, BitMask = (ulong)0x7F000000, Description = "Number of Confirmed emissions DTCs" } },
            {0x03,  new ReadinessMonitor {Code=0x03, SiblingCode=0x00, BitMask = (ulong)0x00800000, Description = "Reserved (should be zero)" } },
            {0x04,  new ReadinessMonitor {Code=0x04, SiblingCode=0x08, BitMask = (ulong)0x00400000, Description = "Components Test Incomplete" } },
            {0x05,  new ReadinessMonitor {Code=0x05, SiblingCode=0x09, BitMask = (ulong)0x00200000, Description = "Fuel System Test Incomplete" } },
            {0x06,  new ReadinessMonitor {Code=0x06, SiblingCode=0x0A, BitMask = (ulong)0x00100000, Description = "Misfire Test Incomplete" } },
            {0x07,  new ReadinessMonitor {Code=0x07, SiblingCode=0x00, BitMask = (ulong)0x00080000, Description = "Compression vs. Spark monitors supported (spark = 0)" } },
            {0x08,  new ReadinessMonitor {Code=0x08, SiblingCode=0x04, BitMask = (ulong)0x00040000, IsCompleted=true, Description = "Components Test" } },
            {0x09,  new ReadinessMonitor {Code=0x09, SiblingCode=0x05, BitMask = (ulong)0x00020000, IsCompleted=true, Description = "Fuel System Test" } },
            {0x0A,  new ReadinessMonitor {Code=0x0A, SiblingCode=0x06, BitMask = (ulong)0x00010000, IsCompleted=true, Description = "Misfire Test" } },
            // For Spark systems (gasoline)
            {0x0B,  new ReadinessMonitor {Code=0x0B, SiblingCode=0x13, BitMask = (ulong)0x00008000, IsCompleted=true, Description = "EGR System Test" } },
            {0x0C,  new ReadinessMonitor {Code=0x0C, SiblingCode=0x14, BitMask = (ulong)0x00004000, IsCompleted=true, Description = "Oxygen Sensor Heater Test" } },
            {0x0D,  new ReadinessMonitor {Code=0x0D, SiblingCode=0x15, BitMask = (ulong)0x00002000, IsCompleted=true, Description = "Oxygen Sensor Test" } },
            {0x0E,  new ReadinessMonitor {Code=0x0E, SiblingCode=0x16, BitMask = (ulong)0x00001000, IsCompleted=true, Description = "A/C Refrigerant Test" } },
            {0x0F,  new ReadinessMonitor {Code=0x0F, SiblingCode=0x17, BitMask = (ulong)0x00000800, IsCompleted=true, Description = "Secondary Air System Test" } },
            {0x10,  new ReadinessMonitor {Code=0x10, SiblingCode=0x18, BitMask = (ulong)0x00000400, IsCompleted=true, Description = "Evaporative System Test" } },
            {0x11,  new ReadinessMonitor {Code=0x11, SiblingCode=0x19, BitMask = (ulong)0x00000200, IsCompleted=true, Description = "Heated Catalyst Test" } },
            {0x12,  new ReadinessMonitor {Code=0x12, SiblingCode=0x1A, BitMask = (ulong)0x00000100, IsCompleted=true, Description = "Catalyst Test" } },
            {0x13,  new ReadinessMonitor {Code=0x13, SiblingCode=0x0B, BitMask = (ulong)0x00000080, Description = "EGR System Test incomplete" } },
            {0x14,  new ReadinessMonitor {Code=0x14, SiblingCode=0x0C, BitMask = (ulong)0x00000040, Description = "Oxygen Sensor Heater Test incomplete" } },
            {0x15,  new ReadinessMonitor {Code=0x15, SiblingCode=0x0D, BitMask = (ulong)0x00000020, Description = "Oxygen Sensor Test incomplete" } },
            {0x16,  new ReadinessMonitor {Code=0x16, SiblingCode=0x0E, BitMask = (ulong)0x00000010, Description = "A/C Refrigerant Test incomplete" } },
            {0x17,  new ReadinessMonitor {Code=0x17, SiblingCode=0x0F, BitMask = (ulong)0x00000008, Description = "Secondary Air System Test incomplete" } },
            {0x18,  new ReadinessMonitor {Code=0x18, SiblingCode=0x10, BitMask = (ulong)0x00000004, Description = "Evaporative System Test incomplete" } },
            {0x19,  new ReadinessMonitor {Code=0x19, SiblingCode=0x11, BitMask = (ulong)0x00000002, Description = "Heated Catalyst Test incomplete" } },
            {0x1A,  new ReadinessMonitor {Code=0x1A, SiblingCode=0x12, BitMask = (ulong)0x00000001, Description = "Catalyst Test incomplete" } },
            // For compression systems (diesel)
            {0x1B,  new ReadinessMonitor {Code=0x1B, SiblingCode=0x23, BitMask = (ulong)0x00008000, IsCompleted=true, Description = "EGR and/or VVT System Test" } },
            {0x1C,  new ReadinessMonitor {Code=0x1C, SiblingCode=0x24, BitMask = (ulong)0x00004000, IsCompleted=true, Description = "PM filter monitoring Test" } },
            {0x1D,  new ReadinessMonitor {Code=0x1D, SiblingCode=0x25, BitMask = (ulong)0x00002000, IsCompleted=true, Description = "Exhaust Gas Sensor Test" } },
            {0x1E,  new ReadinessMonitor {Code=0x1E, SiblingCode=0x26, BitMask = (ulong)0x00001000, IsCompleted=true, Description = "- Reserved - Test" } },
            {0x1F,  new ReadinessMonitor {Code=0x1F, SiblingCode=0x27, BitMask = (ulong)0x00000800, IsCompleted=true, Description = "Boost Pressure Test" } },
            {0x20,  new ReadinessMonitor {Code=0x20, SiblingCode=0x28, BitMask = (ulong)0x00000400, IsCompleted=true, Description = "- Reserved - Test" } },
            {0x21,  new ReadinessMonitor {Code=0x21, SiblingCode=0x29, BitMask = (ulong)0x00000200, IsCompleted=true, Description = "NOx/SCR Monitor Test" } },
            {0x22,  new ReadinessMonitor {Code=0x22, SiblingCode=0x2A, BitMask = (ulong)0x00000100, IsCompleted=true, Description = "NMHC Catalyst Test" } },
            {0x23,  new ReadinessMonitor {Code=0x23, SiblingCode=0x1B, BitMask = (ulong)0x00000080, Description = "EGR and/or VVT System Test incomplete" } },
            {0x24,  new ReadinessMonitor {Code=0x24, SiblingCode=0x1C, BitMask = (ulong)0x00000040, Description = "PM filter monitoring Test incomplete" } },
            {0x25,  new ReadinessMonitor {Code=0x25, SiblingCode=0x1D, BitMask = (ulong)0x00000020, Description = "Exhaust Gas Sensor Test incomplete" } },
            {0x26,  new ReadinessMonitor {Code=0x26, SiblingCode=0x1E, BitMask = (ulong)0x00000010, Description = "- Reserved - Test incomplete" } },
            {0x27,  new ReadinessMonitor {Code=0x27, SiblingCode=0x1F, BitMask = (ulong)0x00000008, Description = "Boost Pressure Test incomplete" } },
            {0x28,  new ReadinessMonitor {Code=0x28, SiblingCode=0x20, BitMask = (ulong)0x00000004, Description = "- Reserved - Test incomplete" } },
            {0x29,  new ReadinessMonitor {Code=0x29, SiblingCode=0x21, BitMask = (ulong)0x00000002, Description = "NOx/SCR Monitor Test incomplete" } },
            {0x2A,  new ReadinessMonitor {Code=0x2A, SiblingCode=0x22, BitMask = (ulong)0x00000001, Description = "NMHC Catalyst Test incomplete" } },

        };


    }
}
