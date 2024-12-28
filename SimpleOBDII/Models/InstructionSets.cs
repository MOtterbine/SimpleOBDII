using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Models
{
    public class InstructionSets
    {

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
          //    DeviceRequestType.SETAdaptiveTiming,
             // DeviceRequestType.SET_Timeout,
          //    DeviceRequestType.ProtocolSearch,
              //DeviceRequestType.OBD2_GetPIDS_00,
            //  DeviceRequestType.GetCurrentProtocolDescription,
              DeviceRequestType.SetHeader,// argument provided in comm event filter
        };
        public static readonly List<DeviceRequestType> InitializeForUserPIDS = new List<DeviceRequestType>()
        {
              DeviceRequestType.DeviceReset,
              DeviceRequestType.SetProtocol,// argument provided in comm event filter
              DeviceRequestType.EchoOff,
              DeviceRequestType.MemoryOff,
              DeviceRequestType.HeadersOff,
              DeviceRequestType.SpacesOff,
              DeviceRequestType.LineFeedsOff,
              //DeviceRequestType.CANAutoAddress,
              DeviceRequestType.ForgetEvents,
          //    DeviceRequestType.SETAdaptiveTiming,
             // DeviceRequestType.SET_Timeout,
          //    DeviceRequestType.ProtocolSearch,
              //DeviceRequestType.OBD2_GetPIDS_00,
            //  DeviceRequestType.GetCurrentProtocolDescription,
              //DeviceRequestType.SetHeader,// argument provided in comm event filter
              DeviceRequestType.OBD2_GetVIN
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
            new OBD2Command("Get System Protocol ID", OBD2Device.ServiceModes[0], DeviceRequestType.GetSystemProtocolID)

        };
        //public static readonly List<OBD2Command> GetSystemProtocolID = new List<OBD2Command>()
        //{
        //    new OBD2Command("Get System Protocol ID", OBD2Device.ServiceModes[0], DeviceRequestType.GetSystemProtocolID)
        //};

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
            new OBD2Command("Get O2 Sensor Locations", OBD2Device.ServiceModes[11], DeviceRequestType.OBD2_GetPIDINFO_13)
        };

        public static readonly List<OBD2Command> GetFreezeFrameLocations1D_Mode22 = new List<OBD2Command>()
        {
            new OBD2Command("Get O2 Sensor Locations", OBD2Device.ServiceModes[11], DeviceRequestType.OBD2_GetPIDINFO_1D)
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
            new OBD2Command("Get Current Protocol ID", OBD2Device.ServiceModes[0], DeviceRequestType.GetSystemProtocolID),
            new OBD2Command("Get Current Protocol Description", OBD2Device.ServiceModes[0], DeviceRequestType.GetCurrentProtocolDescription)
       };


        public static readonly List<OBD2Command> VINReport = new List<OBD2Command>()
        {
            new OBD2Command("Get VIN", OBD2Device.ServiceModes[9], DeviceRequestType.OBD2_GetVIN),
        };


        public static readonly List<DeviceRequestType> DTCReport = new List<DeviceRequestType>()
        {
              DeviceRequestType.OBD2_GetVIN,
              DeviceRequestType.OBD2_StatusSinceCodesLastCleared,
              // J1979
              DeviceRequestType.OBD2_GetDTCs,
              // J2190
              DeviceRequestType.CAN_GetAllDTC,
              DeviceRequestType.OBD2_GetPendingDTCs,
              DeviceRequestType.OBD2_GetPermanentDTCs,
              DeviceRequestType.CAN_GetDTCByStatus,//,
              DeviceRequestType.CAN_GetDTCByStatus1,//,
              DeviceRequestType.OBD2_FreezeFrameDTC,
              DeviceRequestType.OBD2_FreezeFrameDTC1,
              DeviceRequestType.OBD2_FreezeFrame22DTC

           //   DeviceRequestType.OBD2_StatusThisDriveCycle
          //     DeviceRequestType.OBD2_Odometer
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
