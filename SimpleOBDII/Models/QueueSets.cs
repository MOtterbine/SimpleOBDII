
namespace OS.OBDII.Models
{
    public enum QueueSets
    {
        None,
        DeviceReset,
        GeneralSnapshot,
        DetectSystemProtocolID,
        GetSupportedPids,
        GetSupportedMonitorIds,
        GetO2SensorLocations13,
        GetO2SensorLocations1D,
        //GetFFPids,
        //GetFFO2SensorLocations13,
        //GetFFO2SensorLocations1D,
        //GetFFPids_Mode22,
        //GetFFO2SensorLocations13_Mode22,
        //GetFFO2SensorLocations1D_Mode22,
        DTCReport,
        ClearDTCs,
        IMMonitors,
        IMMonitorsDriveCyle,
        FreezeFrameData,
        Live,
        Initialize,
        InitializeForUserPIDS,
        InitializeWithHeaders,
        InitializeCAN,
        InitializeCANMonitor,
        SetCANID,
        SetCANIDRange,
        SendCANMessage,
        CANMonitor,
        Dynamic,
        User
    }

}
