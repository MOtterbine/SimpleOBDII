using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Models
{
    public class InstructionSets
    {

        public static readonly List<OBD2Command> Initialize0 = new List<OBD2Command>()
        {
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.DeviceReset),
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.EchoOff),
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.SetProtocol),// argument provided in comm event filter
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.MemoryOff),
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.HeadersOff),
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.SpacesOff),
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.LineFeedsOff),
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.CANAutoAddress),
            new OBD2Command("", OBD2Device.ServiceModes[0], DeviceRequestType.ForgetEvents),
          //    DeviceRequestType.SETAdaptiveTiming,
             // DeviceRequestType.SET_Timeout,
          //    DeviceRequestType.ProtocolSearch,
              //DeviceRequestType.OBD2_GetPIDS_00,
            //  DeviceRequestType.GetCurrentProtocolDescription,
            new OBD2Command("Set Header", OBD2Device.ServiceModes[0], DeviceRequestType.SetHeader),
        };


        public static readonly List<DeviceRequestType> Initialize = new List<DeviceRequestType>()
        {
              DeviceRequestType.DeviceReset,
              DeviceRequestType.EchoOff,
              DeviceRequestType.SetProtocol,// argument provided in comm event filter
              DeviceRequestType.MemoryOff,
              DeviceRequestType.HeadersOff,
              DeviceRequestType.SpacesOff,
              DeviceRequestType.LineFeedsOff,
              DeviceRequestType.CANAutoAddress,
              DeviceRequestType.ForgetEvents,
              DeviceRequestType.ISO_SetSlowInitAddress,      // argument provided in comm event filter
              DeviceRequestType.SetISOBaudRate,                 // argument provided in comm event filter
             // DeviceRequestType.SET_OBD1WakeupOff,
          //    DeviceRequestType.SETAdaptiveTiming,
             // DeviceRequestType.SET_Timeout,
            //  DeviceRequestType.GetCurrentProtocolDescription,
              DeviceRequestType.OBD2_GetPIDS_00, // forces a protocol detection
              DeviceRequestType.GetSystemProtocolID,
              DeviceRequestType.SetHeader,//  argument provided in comm event filter 
        };

        public static readonly List<OBD2Command> InitializeForUserPIDS = new List<OBD2Command>()
        {
            new OBD2Command("Device Reset", OBD2Device.ServiceModes[0], DeviceRequestType.DeviceReset),
            new OBD2Command("Echo Off", OBD2Device.ServiceModes[0], DeviceRequestType.EchoOff),
            new OBD2Command("Set Protocol", OBD2Device.ServiceModes[0], DeviceRequestType.SetProtocol),// argument provided in comm event filter
            new OBD2Command("Memory Off", OBD2Device.ServiceModes[0], DeviceRequestType.MemoryOff),
            new OBD2Command("Headers Off", OBD2Device.ServiceModes[0], DeviceRequestType.HeadersOff),
            new OBD2Command("Spaces Off", OBD2Device.ServiceModes[0], DeviceRequestType.SpacesOff),
            new OBD2Command("Line Feeds Off", OBD2Device.ServiceModes[0], DeviceRequestType.LineFeedsOff),
            new OBD2Command("Forget Events", OBD2Device.ServiceModes[0], DeviceRequestType.ForgetEvents),

            new OBD2Command("Set ISO Baud Rate", OBD2Device.ServiceModes[0], DeviceRequestType.SetISOBaudRate),// argument provided in comm event filter
            new OBD2Command("Set OBDI Wakeup", OBD2Device.ServiceModes[0], DeviceRequestType.SET_OBD1Wakeup),// argument provided in comm event filter

            //new OBD2Command("Get VIN", OBD2Device.ServiceModes[9], DeviceRequestType.OBD2_GetVIN),
            // Forces the hardware to search of the vehicle's protocol
            new OBD2Command("Get Supported PIDs 00", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_00),
      };


        public static readonly List<DeviceRequestType> InitializeCAN = new List<DeviceRequestType>()
        {
              DeviceRequestType.DeviceReset,
              //DeviceRequestType.WarmStart,
              DeviceRequestType.EchoOff,
              DeviceRequestType.MemoryOff,
              DeviceRequestType.HeadersOn,
              //DeviceRequestType.DisplayDataByteLengthOff,
              DeviceRequestType.SpacesOff,
              DeviceRequestType.LineFeedsOff,
              //DeviceRequestType.SetDefaultHeader,
              //DeviceRequestType.BaudRateFastTimeout,
            //  DeviceRequestType.SetDefaultCANRxAddress,
              DeviceRequestType.CANAutoAddress,
              DeviceRequestType.ForgetEvents,
             // DeviceRequestType.CANSilentMonitoringOff,
              DeviceRequestType.AllowLongMessages,// optional
              DeviceRequestType.CANAutoFormatOff, // optional
              DeviceRequestType.CANVariableLengthOn, // optional
              DeviceRequestType.SET_Timeout,
              DeviceRequestType.SetProtocol//,// arguement provided in comm event filter
             // DeviceRequestType.CAN_GetDTCByStatus
        };

        public static readonly List<DeviceRequestType> InitializeCANMonitor = new List<DeviceRequestType>()
        {
              DeviceRequestType.DeviceReset,
              DeviceRequestType.EchoOff,
              DeviceRequestType.MemoryOff,
              DeviceRequestType.HeadersOn,
              DeviceRequestType.DisplayDataByteLengthOn,
              DeviceRequestType.SpacesOff,
              DeviceRequestType.LineFeedsOff,
              DeviceRequestType.CANAutoFormatOff,
              DeviceRequestType.CANVariableLengthOff, // optional
              DeviceRequestType.IdentifyIC,
              DeviceRequestType.ForgetEvents,
              DeviceRequestType.SetProtocol,// arguement provided in comm event filter
              DeviceRequestType.CANAddressMask,
              DeviceRequestType.CANAddressFilter,
             // DeviceRequestType.BaudRateFastTimeout,
              DeviceRequestType.SET_Timeout,
              //DeviceRequestType.SETAdaptiveTiming
             // DeviceRequestType.CANSilentMonitoringOn
             // DeviceRequestType.SetProtocol// arguement provided in comm event filter
        };

        public static readonly List<DeviceRequestType> InitializeHeadersOn = new List<DeviceRequestType>()
        {
              DeviceRequestType.DeviceReset,
              //DeviceRequestType.WarmStart,
              DeviceRequestType.EchoOff,
              DeviceRequestType.MemoryOff,
              DeviceRequestType.HeadersOn,
              DeviceRequestType.SpacesOff,
              DeviceRequestType.LineFeedsOff,
              DeviceRequestType.SetProtocol,// arguement provided in comm event filter
              DeviceRequestType.ForgetEvents,
              DeviceRequestType.CANAutoAddress,
              //DeviceRequestType.BaudRateFastTimeout,
              DeviceRequestType.SetHeader// argument provided in comm event filter

              //DeviceRequestType.BaudRateFastDivisor
        };


        public static readonly List<OBD2Command> DetectSystemProtocol = new List<OBD2Command>()
        {
            new OBD2Command("Get Supported PIDs 00", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_00),
            new OBD2Command("Get System Protocol ID", OBD2Device.ServiceModes[0], DeviceRequestType.GetSystemProtocolID),
            new OBD2Command("Set System ISO Wakeup", OBD2Device.ServiceModes[0], DeviceRequestType.SET_OBD1Wakeup)

        };

        public static readonly List<OBD2Command> GetSupportedPids = new List<OBD2Command>()
        {
            new OBD2Command("Get Supported PIDs 00", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_00),
            new OBD2Command("Get Supported PIDs 20", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_20),
            new OBD2Command("Get Supported PIDs 40", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_40),
            new OBD2Command("Get Supported PIDs 60", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_60),
            new OBD2Command("Get Supported PIDs 80", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_80),
            new OBD2Command("Get Supported PIDs A0", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_A0),
            new OBD2Command("Get Supported PIDs C0", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_C0),
            new OBD2Command("Get Supported PIDs E0", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDS_E0)
        };
        public static readonly List<OBD2Command> GetSupportedMonitorIds = new List<OBD2Command>()
        {
            new OBD2Command("Get Supported MonitorIds 00", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_00),
            new OBD2Command("Get Supported MonitorIds 20", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_20),
            new OBD2Command("Get Supported MonitorIds 40", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_40),
            new OBD2Command("Get Supported MonitorIds 60", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_60),
            new OBD2Command("Get Supported MonitorIds 80", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_80),
            new OBD2Command("Get Supported MonitorIds A0", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_A0),
            new OBD2Command("Get Supported MonitorIds C0", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_C0),
            new OBD2Command("Get Supported MonitorIds E0", OBD2Device.ServiceModes[6], DeviceRequestType.OBD2_GetPIDS_E0)
        };
        public static readonly List<OBD2Command> GetLocations13 = new List<OBD2Command>()
        {
            new OBD2Command("Get O2 Sensor Locations", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDINFO_13)
        };

        public static readonly List<OBD2Command> GetLocations1D = new List<OBD2Command>()
        {
            new OBD2Command("Get O2 Sensor Locations", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_GetPIDINFO_1D)
        };

        public static readonly List<OBD2Command> GetFreezeFrameLocations13_Mode22 = new List<OBD2Command>()
        {
            new OBD2Command("Get O2 Sensor Locations", OBD2Device.ServiceModes[0x22], DeviceRequestType.OBD2_GetPIDINFO_13)
        };

        public static readonly List<OBD2Command> GetFreezeFrameLocations1D_Mode22 = new List<OBD2Command>()
        {
            new OBD2Command("Get O2 Sensor Locations", OBD2Device.ServiceModes[0x22], DeviceRequestType.OBD2_GetPIDINFO_1D)
        };

        public static readonly List<DeviceRequestType> DeviceReset = new List<DeviceRequestType>()
        {
              DeviceRequestType.DeviceReset
        };

        public static readonly List<DeviceRequestType> SetCANID = new List<DeviceRequestType>()
        {
              DeviceRequestType.SetHeader//,
              //DeviceRequestType.SetCANRxAddress
        };

        public static readonly List<OBD2Command> SendCANData = new List<OBD2Command>()
        {
            new OBD2Command("Set CAN Auto Rx Address", OBD2Device.ServiceModes[0], DeviceRequestType.CANAutoAddress),
            new OBD2Command("Set CAN Header", OBD2Device.ServiceModes[0], DeviceRequestType.SetHeader)
        };



        

        public static readonly List<DeviceRequestType> StartCANMonitor = new List<DeviceRequestType>()
        {
              DeviceRequestType.MonitorAll
        };


        public static readonly List<OBD2Command> GeneralSnapshot1 = new List<OBD2Command>()
        {
            new OBD2Command("Identify IC", OBD2Device.ServiceModes[0], DeviceRequestType.IdentifyIC),
            new OBD2Command("Get VIN", OBD2Device.ServiceModes[9], DeviceRequestType.OBD2_GetVIN),
            new OBD2Command("Distance Since Cleared", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_KmSinceDTCCleared),
            new OBD2Command("Distance with MIL light on", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_KmWithMilOn),
            new OBD2Command("I/M since codes cleared", OBD2Device.ServiceModes[1], DeviceRequestType.OBD2_StatusSinceCodesLastCleared),
            new OBD2Command("Get Current Protocol Description", OBD2Device.ServiceModes[0], DeviceRequestType.GetCurrentProtocolDescription),
       };


        public static readonly List<OBD2Command> VINReport = new List<OBD2Command>()
        {
            new OBD2Command("Get VIN", OBD2Device.ServiceModes[9], DeviceRequestType.OBD2_GetVIN),
        };

        public static readonly List<OBD2Command> DTCReport = new List<OBD2Command>()
        {
           // new OBD2Command("Set KWP Wakeup Off,\r\n", OBD2Device.ServiceModes[0], DeviceRequestType.SET_OBD1WakeupOff),
            new OBD2Command("Get Vin", OBD2Device.ServiceModes[0x09], DeviceRequestType.OBD2_GetVIN),
            new OBD2Command("Get Status Since DTCs Cleared", OBD2Device.ServiceModes[0x01], DeviceRequestType.OBD2_StatusSinceCodesLastCleared),
            new OBD2Command("Get Current Protocol ID", OBD2Device.ServiceModes[0], DeviceRequestType.GetSystemProtocolID),
            //new OBD2Command("Slow Init", OBD2Device.ServiceModes[0], DeviceRequestType.ISO_SlowInit),

             // J1979
            new OBD2Command("Get DTCs", OBD2Device.ServiceModes[-1], DeviceRequestType.OBD2_GetDTCs),
            new OBD2Command("Get Pending DTCs", OBD2Device.ServiceModes[-1], DeviceRequestType.OBD2_GetPendingDTCs),
            new OBD2Command("Get Permanent DTCs", OBD2Device.ServiceModes[-1], DeviceRequestType.OBD2_GetPermanentDTCs),

              // J2190
            new OBD2Command("Get All DTCs", OBD2Device.ServiceModes[0x13], DeviceRequestType.CAN_GetAllDTC),
            new OBD2Command("Get DTC by Status", OBD2Device.ServiceModes[0x18], DeviceRequestType.CAN_GetDTCByStatus),
            new OBD2Command("Get DTC by Status 1", OBD2Device.ServiceModes[0x18], DeviceRequestType.CAN_GetDTCByStatus1),
            new OBD2Command("Get Freeze Frame", OBD2Device.ServiceModes[0x01], DeviceRequestType.OBD2_FreezeFrameDTC),
            new OBD2Command("Get Freeze Frame 1", OBD2Device.ServiceModes[0x2], DeviceRequestType.OBD2_FreezeFrameDTC1),
            new OBD2Command("Get Freeze Frame mode 22", OBD2Device.ServiceModes[0x22], DeviceRequestType.OBD2_FreezeFrame22DTC)
       };

        public static readonly List<DeviceRequestType> ClearDTCs = new List<DeviceRequestType>()
        {
              DeviceRequestType.OBD2_ClearDTCs,
              DeviceRequestType.CAN_ClearDTCs,
              DeviceRequestType.CAN_ClearDTCs_2
        };

        public static readonly List<DeviceRequestType> IMMonitors = new List<DeviceRequestType>()
        {
              DeviceRequestType.OBD2_StatusSinceCodesLastCleared
        };

        public static readonly List<DeviceRequestType> IMMonitorsDriveCyle = new List<DeviceRequestType>()
        {
              DeviceRequestType.OBD2_StatusThisDriveCycle
        };


    }



}
