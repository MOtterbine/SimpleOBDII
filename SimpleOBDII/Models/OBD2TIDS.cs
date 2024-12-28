using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using static OS.OBDII.Models.OBD2PIDS;
using static OS.OBDII.Models.OBD2TIDS;
using static OS.OBDII.Models.OBD2MIDS;

namespace OS.OBDII.Models
{
    public class OBD2TIDS
    {

        public enum TestIds
        {
            None = -1,
            RichToLeanSensorThresholdVolts = 0x01,
            LeanToRichSensorThresholdVolts = 0x02,
            LowSensorVoltForSwitchTimeCalculation = 0x03,
            HighSensorVoltForSwitchTimeCalculation = 0x04,
            RichToLeanSensorSwitchTime = 0x05,
            LeanToRichSensorSwitchTime = 0x06,
            MinimumSensorVoltageForTestCycle = 0x07,
            MaximumSensorVoltageForTestCycle = 0x08,
            TimeBetweenSensorTransitions = 0x09,
            SensorPeriod = 0x0A,
            MisfiresAverage = 0x0B,
            MisfiresCount = 0x0C
        }

        Dictionary<TestIds, int> scaleMap = new Dictionary<TestIds, int>()
        {
            { TestIds.RichToLeanSensorSwitchTime, 0x0A},
            { TestIds.LeanToRichSensorSwitchTime, 0x0A},
            { TestIds.LowSensorVoltForSwitchTimeCalculation, 0x0A},
            { TestIds.HighSensorVoltForSwitchTimeCalculation, 0x0A},
            { TestIds.RichToLeanSensorSwitchTime, 0x1D},
            { TestIds.LeanToRichSensorSwitchTime, 0x1D},
            { TestIds.MinimumSensorVoltageForTestCycle, 0x0A},
            { TestIds.MaximumSensorVoltageForTestCycle, 0x0A},
            { TestIds.TimeBetweenSensorTransitions, 0x1D},
            { TestIds.SensorPeriod, 0x1D},
            { TestIds.MisfiresAverage, 0x24},
            { TestIds.MisfiresCount, 0x24},
        };

        //public PID this[TestIds key]
        //{
        //    get
        //    {
        //        if (!TestIdsDictionary.ContainsKey(key)) return null;
        //        return TestIdsDictionary[key];
        //    }
        //}
        //public UnitMeasureScale this[TestIds key]
        //{
        //    get
        //    {
        //        return OBD2MIDS.UnitScalingDictionary[scaleMap[key]];
        //    }
        //}

   //     public static int DTCCount = 0;


        //public object GetFunction(DeviceRequestType devReq)
        //{
        //    return OBD2Device.OPBD2PIDSDictionary[devReq].function;
        //}

        private static readonly Dictionary<UnitMeasure, UnitMeasureScale> UnitMeasuresDictionary = new Dictionary<UnitMeasure, UnitMeasureScale>
        {
            {UnitMeasure.MonitorTestResult, new UnitMeasureScale(UnitMeasure.MonitorTestResult, (obj) =>
                    {
                            MonitorTestResult res = null;
                            if(MonitorTestResult.TryParse(obj as string, out res))
                            {
                                return res.Value.ToString();
                            }
                            return string.Empty;
                    },1 ) { }},
        };

        public static readonly Dictionary<TestIds, PID> TestIdsDictionary = new Dictionary<TestIds, PID>
        {
            {TestIds.RichToLeanSensorThresholdVolts, new PID((int)TestIds.RichToLeanSensorThresholdVolts, "Rich To Lean Sensor Threshold Volts", "Rich To Lean Sensor Threshold Volts", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.LeanToRichSensorThresholdVolts, new PID((int)TestIds.LeanToRichSensorThresholdVolts, "Lean To Rich Sensor Threshold Volts", "Lean To Rich Sensor Threshold Volts", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.LowSensorVoltForSwitchTimeCalculation, new PID((int)TestIds.LowSensorVoltForSwitchTimeCalculation, "Low Sensor Volt For Switch Time Calculation", "Low Sensor Volt For Switch Time Calculation", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.HighSensorVoltForSwitchTimeCalculation, new PID((int)TestIds.HighSensorVoltForSwitchTimeCalculation, "High Sensor Volt For Switch Time Calculation", "HIgh Sensor Volt For Switch Time Calculation", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.RichToLeanSensorSwitchTime, new PID((int)TestIds.RichToLeanSensorSwitchTime, "Rich To Lean Sensor Switch Time","Rich To Lean Sensor Switch Time", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.LeanToRichSensorSwitchTime, new PID((int)TestIds.LeanToRichSensorSwitchTime, "Lean To Rich Sensor Switch Time", "Lean To Rich Sensor Switch Time", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.MinimumSensorVoltageForTestCycle, new PID((int)TestIds.MinimumSensorVoltageForTestCycle, "Minimum Sensor Voltage For Test Cycle", "Minimum Sensor Voltage For Test Cycle", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.MaximumSensorVoltageForTestCycle, new PID((int)TestIds.MaximumSensorVoltageForTestCycle, "Maximum Sensor Voltage For Test Cycle", "Maximum Sensor Voltage For Test Cycle", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.SensorPeriod, new PID((int)TestIds.SensorPeriod, "Sensor Period", "Sensor Period", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.MisfiresAverage, new PID((int)TestIds.MisfiresAverage, "Misfires Average", "Misfires Average", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {TestIds.MisfiresCount, new PID((int)TestIds.MisfiresCount, "Misfires Count", "Misfires Count", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },

        };

    }
}
