using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public enum UnitMeasure
    {
        Rpm,
        None,
        Pressure1,
        kPa,
        Pressure2,
        psi,
        Temperature1,
        Volts32,
        Volts005,
        Amperes,
        Percent,
        OffsetPercent,
        TorquePercent,
        Distance,
        MPH,
        Count,
        Count16,
        km,
        AngleDeg,
        AirFlow1,
        /// <summary>
        /// Oxygen PID voltage and trim data
        /// </summary>
        O2PIDVTData,
        /// <summary>
        /// Wide-Range O2 PID equivalence ratio and current
        /// </summary>
        O2PIDECData,
        /// <summary>
        /// Wide-Range O2 PID equivalence ratio and voltage
        /// </summary>
        O2PIDEVData,
        CatalystTemp,
        //CatalystTempC,
        MonitorTestResult
    }

}
