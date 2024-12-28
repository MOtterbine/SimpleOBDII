using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using static OS.OBDII.Models.OBD2PIDS;

namespace OS.OBDII.Models
{
    public class OBD2MIDS
    {

        public enum MonitorIds
        {
            None = -1,
            OBDMID_ExhaustGasSensor_B1_S1 = 0x01,
            OBDMID_ExhaustGasSensor_B1_S2 = 0x02,
            OBDMID_ExhaustGasSensor_B1_S3 = 0x03,
            OBDMID_ExhaustGasSensor_B1_S4 = 0x04,
            OBDMID_ExhaustGasSensor_B2_S1 = 0x05,
            OBDMID_ExhaustGasSensor_B2_S2 = 0x06,
            OBDMID_ExhaustGasSensor_B2_S3 = 0x07,
            OBDMID_ExhaustGasSensor_B2_S4 = 0x08,
            OBDMID_ExhaustGasSensor_B3_S1 = 0x09,
            OBDMID_ExhaustGasSensor_B3_S2 = 0x0A,
            OBDMID_ExhaustGasSensor_B3_S3 = 0x0B,
            OBDMID_ExhaustGasSensor_B3_S4 = 0x0C,
            OBDMID_ExhaustGasSensor_B4_S1 = 0x0D,
            OBDMID_ExhaustGasSensor_B4_S2 = 0x0E,
            OBDMID_ExhaustGasSensor_B4_S3 = 0x0F,
            OBDMID_ExhaustGasSensor_B4_S4 = 0x10,

            // 20
            OBDMID_CatalystMonitor_B1 = 0x21,
            OBDMID_CatalystMonitor_B2 = 0x22,
            OBDMID_CatalystMonitor_B3 = 0x23,
            OBDMID_CatalystMonitor_B4 = 0x24,

            // 30
            OBDMID_EGRMonitor_B1 = 0x31,
            OBDMID_EGRMonitor_B2 = 0x32,
            OBDMID_EGRMonitor_B3 = 0x33,
            OBDMID_EGRMonitor_B4 = 0x34,
            OBDMID_VVTMonitor_B1 = 0x35,
            OBDMID_VVTMonitor_B2 = 0x36,
            OBDMID_VVTMonitor_B3 = 0x37,
            OBDMID_VVTMonitor_B4 = 0x38,
            OBDMID_EvapMonitor_15 = 0x39, // Cap off
            OBDMID_EvapMonitor_09 = 0x3A,
            OBDMID_EvapMonitor_04 = 0x3B,
            OBDMID_EvapMonitor_02 = 0x3C,
            OBDMID_PurgeFlowMonitor = 0x3D,


            // 40 - 60)
            OBDMID_ExhaustGasSensorHeater_B1_S1 = 0x41,
            OBDMID_ExhaustGasSensorHeater_B1_S2 = 0x42,
            OBDMID_ExhaustGasSensorHeater_B1_S3 = 0x43,
            OBDMID_ExhaustGasSensorHeater_B1_S4 = 0x44,
            OBDMID_ExhaustGasSensorHeater_B2_S1 = 0x45,
            OBDMID_ExhaustGasSensorHeater_B2_S2 = 0x46,
            OBDMID_ExhaustGasSensorHeater_B2_S3 = 0x47,
            OBDMID_ExhaustGasSensorHeater_B2_S4 = 0x48,
            OBDMID_ExhaustGasSensorHeater_B3_S1 = 0x49,
            OBDMID_ExhaustGasSensorHeater_B3_S2 = 0x4A,
            OBDMID_ExhaustGasSensorHeater_B3_S3 = 0x4B,
            OBDMID_ExhaustGasSensorHeater_B3_S4 = 0x4C,
            OBDMID_ExhaustGasSensorHeater_B4_S1 = 0x4D,
            OBDMID_ExhaustGasSensorHeater_B4_S2 = 0x4E,
            OBDMID_ExhaustGasSensorHeater_B4_S3 = 0x4F,
            OBDMID_ExhaustGasSensorHeater_B4_S4 = 0x50,



            // 60
            OBDMID_HeatedCatalyst_B1 = 0x61,
            OBDMID_HeatedCatalyst_B2 = 0x62,
            OBDMID_HeatedCatalyst_B3 = 0x63,
            OBDMID_HeatedCatalyst_B4 = 0x64,

            // 70
            OBDMID_SecondaryAir1 = 0x71,
            OBDMID_SecondaryAir2 = 0x72,
            OBDMID_SecondaryAir3 = 0x73,
            OBDMID_SecondaryAir4 = 0x74,

            // 80
            OBDMID_FuelSystem_B1 = 0x81,
            OBDMID_FuelSystem_B2 = 0x82,
            OBDMID_FuelSystem_B3 = 0x83,
            OBDMID_FuelSystem_B4 = 0x84,
            OBDMID_BoostPressureControl_B1 = 0x85,
            OBDMID_BoostPressureControl_B2 = 0x86,

            // 90
            OBDMID_NOxAdsorber_B1 = 0x91,
            OBDMID_NOxAdsorber_B2 = 0x92,
            OBDMID_NOxSCRCatalyst_B1 = 0x98,
            OBDMID_NOxSCRCatalyst_B2 = 0x99,

            // A0 - C0
            OBDMID_MisfireGeneralData = 0xA1,
            OBDMID_MisfireData_Cylinder1 = 0xA2,
            OBDMID_MisfireData_Cylinder2 = 0xA3,
            OBDMID_MisfireData_Cylinder3 = 0xA4,
            OBDMID_MisfireData_Cylinder4 = 0xA5,
            OBDMID_MisfireData_Cylinder5 = 0xA6,
            OBDMID_MisfireData_Cylinder6 = 0xA7,
            OBDMID_MisfireData_Cylinder7 = 0xA8,
            OBDMID_MisfireData_Cylinder8 = 0xA9,
            OBDMID_MisfireData_Cylinder9 = 0xAA,
            OBDMID_MisfireData_Cylinder10 = 0xAB,
            OBDMID_MisfireData_Cylinder11 = 0xAC,
            OBDMID_MisfireData_Cylinder12 = 0xAD,
            OBDMID_MisfireData_Cylinder13 = 0xAE,
            OBDMID_MisfireData_Cylinder14 = 0xAF,
            OBDMID_MisfireData_Cylinder15 = 0xB0,
            OBDMID_MisfireData_Cylinder16 = 0xB1,
            OBDMID_PMFilter_B1 = 0xB2,
            OBDMID_PMFilter_B2 = 0xB3,

            // B0

        }

        public PID this[MonitorIds key]
        {
            get
            {
                if (!MonitorIdsDictionary.ContainsKey(key)) return null;
                return MonitorIdsDictionary[key];
            }
        }

        public static int DTCCount = 0;


        public object GetFunction(DeviceRequestType devReq)
        {
            return OBD2Device.OPBD2PIDSDictionary[devReq].function;
        }

        /// <summary>
        /// J1979 Scale IDs
        /// </summary>
        public static readonly Dictionary<int, UnitMeasureScale> UnitScalingDictionary = new Dictionary<int, UnitMeasureScale>
        {
            {01, new UnitMeasureScale(01, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return (uint)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2) { Minimum_English = 0, Maximum_English = 0xFFFF, Minimum_Metric = 0, Maximum_Metric=0xFFFF } },
            {02, new UnitMeasureScale(02, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .1*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2) { Minimum_English = 0, Maximum_English = 6553.5, Minimum_Metric = 0, Maximum_Metric=6553.5 } },
            {03, new UnitMeasureScale(03, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2) { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric=655.35 } },
            {04, new UnitMeasureScale(04, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2) { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric=65.535 } },
            {05, new UnitMeasureScale(05, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .0000305*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2) { Minimum_English = 0, Maximum_English = 1.999, Minimum_Metric = 0, Maximum_Metric=1.999 } },
            {06, new UnitMeasureScale(06, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .000305*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2) { Minimum_English = 0, Maximum_English = 19.988, Minimum_Metric = 0, Maximum_Metric=19.988 } },
            {07, new UnitMeasureScale(07, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .25*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"rpm","rpm") { Minimum_English = 0, Maximum_English = 16383.75, Minimum_Metric = 0, Maximum_Metric=16383.75 } },
            {08, new UnitMeasureScale(08, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // km/h
                        return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // mph
                        return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]))/1.609344;
                    }
                },2,"mph","km/h") { Minimum_English = 0, Maximum_English = 407.21, Minimum_Metric = 0, Maximum_Metric= 655.35 } },
            {09, new UnitMeasureScale(09, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // km/h
                        return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // mph
                        return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]))/1.609344;
                    }
                },2,"mph","km/h") { Minimum_English = 0, Maximum_English = 40721, Minimum_Metric = 0, Maximum_Metric= 65535 } },
            {0x0A, new UnitMeasureScale(0x0A, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .000122*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"VS","V") { Minimum_English = 0, Maximum_English = 7.999, Minimum_Metric = 0, Maximum_Metric=7.999 } },
            {0x0B, new UnitMeasureScale(0x0B, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"V","V") { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric=65.535 } },
            {0x0C, new UnitMeasureScale(0x0C, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"V","V") { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric=655.35 } },
            {0x0D, new UnitMeasureScale(0x0D, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .00390625*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"mA","mA") { Minimum_English = 0, Maximum_English = 255.996, Minimum_Metric = 0, Maximum_Metric=255.996 } },
            {0x0E, new UnitMeasureScale(0x0E, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"A","A") { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric=65.535 } },
            {0x0F, new UnitMeasureScale(0x0F, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"A","A") { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric=655.35 } },
            {0x10, new UnitMeasureScale(0x10, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"s","s") { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric=65.535 } },
            {0x11, new UnitMeasureScale(0x11, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .1*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                },2,"s","s") { Minimum_English = 0, Maximum_English = 6553.5, Minimum_Metric = 0, Maximum_Metric=6553.5 } },
            {0x12, new UnitMeasureScale(0x12, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "s", "s") { Minimum_English = 0, Maximum_English = 65535, Minimum_Metric = 0, Maximum_Metric=65535 } },
            {0x13, new UnitMeasureScale(0x13, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "Ohm", "Ohm") { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric=65.535 } },
            {0x14, new UnitMeasureScale(0x14, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "kOhm", "kOhm") { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric=65.535 } },
            {0x15, new UnitMeasureScale(0x15, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "kOhm", "kOhm") { Minimum_English = 0, Maximum_English = 65535, Minimum_Metric = 0, Maximum_Metric=65535 } },
            {0x16, new UnitMeasureScale(0x16, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // ºC
                        return .1*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) - 40;
                    }
                    else
                    {
                        // ºF
                        return ((.1*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) - 40) * 1.8) + 32;
                    }
                }, 2, "ºF", "ºC") { Minimum_English = -40, Maximum_English = 11756.3, Minimum_Metric = -40, Maximum_Metric=6513.5 } },
            {0x17, new UnitMeasureScale(0x17, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // psi
                        return 0.1450377 * .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "psi", "kPa") { Minimum_English = 0, Maximum_English = 95.1, Minimum_Metric = 0, Maximum_Metric=655.35 } },
            {0x18, new UnitMeasureScale(0x18, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return .0117*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // psi
                        return 0.1450377 * .0117 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "psi", "kPa") { Minimum_English = 0, Maximum_English = 111.2, Minimum_Metric = 0, Maximum_Metric = 766.7595 } },
            {0x19, new UnitMeasureScale(0x19, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return .079*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // psi
                        return 0.1450377 * .079 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "psi", "kPa") { Minimum_English = 0, Maximum_English = 750.9, Minimum_Metric = 0, Maximum_Metric = 5177.265 } },
            {0x1A, new UnitMeasureScale(0x1A, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // psi
                        return 0.1450377 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "psi", "kPa") { Minimum_English = 0, Maximum_English = 9505.0, Minimum_Metric = 0, Maximum_Metric = 65535 } },
            {0x1B, new UnitMeasureScale(0x1B, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return 10*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // psi
                        return 0.1450377 * 10 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "psi", "kPa") { Minimum_English = 0, Maximum_English = 95050.5, Minimum_Metric = 0, Maximum_Metric = 655350  } },
            {0x1C, new UnitMeasureScale(0x1C, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "º", "º") { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric = 655.35  } },
            {0x1D, new UnitMeasureScale(0x1D, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .5*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "º", "º") { Minimum_English = 0, Maximum_English = 32767.5, Minimum_Metric = 0, Maximum_Metric = 32767.5  } },
            {0x1E, new UnitMeasureScale(0x1E, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .0000305*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2) { Minimum_English = 0, Maximum_English = 1.999, Minimum_Metric = 0, Maximum_Metric = 1.999  } },
            {0x1F, new UnitMeasureScale(0x1F, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .05*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2) { Minimum_English = 0, Maximum_English = 3276.75, Minimum_Metric = 0, Maximum_Metric = 3276.75  } },
            {0x20, new UnitMeasureScale(0x20, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return 0.00390625*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2) { Minimum_English = 0, Maximum_English = 255.996, Minimum_Metric = 0, Maximum_Metric = 255.996  } },
            {0x21, new UnitMeasureScale(0x21, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return 0.001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "Hz", "Hz") { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric = 65.535  } },
            {0x22, new UnitMeasureScale(0x22, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "Hz", "Hz") { Minimum_English = 0, Maximum_English = 65535, Minimum_Metric = 0, Maximum_Metric = 65535  } },
            {0x23, new UnitMeasureScale(0x23, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "MHz", "MHz") { Minimum_English = 0, Maximum_English = 65.535, Minimum_Metric = 0, Maximum_Metric = 65.535  } },
            {0x24, new UnitMeasureScale(0x24, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2) { Minimum_English = 0, Maximum_English = 65535, Minimum_Metric = 0, Maximum_Metric = 65535  } },
            {0x25, new UnitMeasureScale(0x25, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();

                    if(OBD2Device.UseMetricUnits)
                    {
                        // km/h
                        return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // mph
                        return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]))/1.609344;
                    }

                },2,"mi","km") { Minimum_English = 0, Maximum_English = 40721, Minimum_Metric = 0, Maximum_Metric= 65535 } },
            {0x26, new UnitMeasureScale(0x26, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .0001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "V/ms", "V/ms") { Minimum_English = 0, Maximum_English = 6.5535, Minimum_Metric = 0, Maximum_Metric = 6.5535  } },
            {0x27, new UnitMeasureScale(0x27, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.002204623 *.01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lb/s", "g/s") { Minimum_English = 0, Maximum_English = 1.445, Minimum_Metric = 0, Maximum_Metric = 655.35  } },
            {0x28, new UnitMeasureScale(0x28, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.002204623 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lb/s", "g/s") { Minimum_English = 0, Maximum_English = 144.48, Minimum_Metric = 0, Maximum_Metric = 65535  } },
            {0x29, new UnitMeasureScale(0x29, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .00025*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.0040146307866177 * .25 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "inH2O/s", "kPa/s") { Minimum_English = 0, Maximum_English = 65.775, Minimum_Metric = 0, Maximum_Metric = 16.3838  } },
            {0x2A, new UnitMeasureScale(0x2A, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.45359237 * .001 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lb/hr", "kg/hr") { Minimum_English = 0, Maximum_English = 29.726, Minimum_Metric = 0, Maximum_Metric = 65.535  } },
            {0x2B, new UnitMeasureScale(0x2B, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "switches", "switches") { Minimum_English = 0, Maximum_English = 65535, Minimum_Metric = 0, Maximum_Metric = 65535  } },
            {0x2C, new UnitMeasureScale(0x2B, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.002204623 * .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lb/cyl", "g/cyl") { Minimum_English = 0, Maximum_English = 1.4448, Minimum_Metric = 0, Maximum_Metric = 655.35  } },
            {0x2D, new UnitMeasureScale(0x2D, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.00003527 * .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "oz/stroke", "mg/stroke") { Minimum_English = 0, Maximum_English = .023, Minimum_Metric = 0, Maximum_Metric = 655.35  } },
            {0x2E, new UnitMeasureScale(0x2E, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return (Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) == 1);
                }, 2)  },
            {0x2F, new UnitMeasureScale(0x2F, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "%", "%") { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric = 655.35  } },
            {0x30, new UnitMeasureScale(0x30, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return 0.001526*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "%", "%") { Minimum_English = 0, Maximum_English = 100.00641, Minimum_Metric = 0, Maximum_Metric = 100.00641  } },
            {0x31, new UnitMeasureScale(0x31, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return 0.001*Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.26417205 * 0.001 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "gal", "L") { Minimum_English = 0, Maximum_English = 17.313, Minimum_Metric = 0, Maximum_Metric = 65.535  } },
            {0x32, new UnitMeasureScale(0x32, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return 25.4 * 0.0000305 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.0000305 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "inch", "mm") { Minimum_English = 0, Maximum_English = 1.999, Minimum_Metric = 0, Maximum_Metric = 50.799  } },
            {0x33, new UnitMeasureScale(0x33, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .00024414 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2) { Minimum_English = 0, Maximum_English = 16, Minimum_Metric = 0, Maximum_Metric = 16  } },
            {0x34, new UnitMeasureScale(0x34, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2) { Minimum_English = 0, Maximum_English = 65535, Minimum_Metric = 0, Maximum_Metric =  65535  } },
            {0x35, new UnitMeasureScale(0x34, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2) { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric =  655.35  } },
            {0x36, new UnitMeasureScale(0x36, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.00220462 * .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lbs", "g") { Minimum_English = 0, Maximum_English = 1.447, Minimum_Metric = 0, Maximum_Metric = 655.35  } },
            {0x37, new UnitMeasureScale(0x37, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .1 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.00220462 * .1 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lbs", "g") { Minimum_English = 0, Maximum_English = 14.467, Minimum_Metric = 0, Maximum_Metric = 6553.5  } },
            {0x38, new UnitMeasureScale(0x38, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.00220462 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lbs", "g") { Minimum_English = 0, Maximum_English = 144.67, Minimum_Metric = 0, Maximum_Metric = 65535  } },
            {0x39, new UnitMeasureScale(0x39, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (uint)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return .01 * (Convert.ToInt32(rawVal)-32768);




                   // return .01 * (Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) - 32768);
                }, 2, "%", "%") { Minimum_English = -327.68, Maximum_English = 327.67, Minimum_Metric = -327.68, Maximum_Metric =  327.67  } },
            {0x3A, new UnitMeasureScale(0x3A, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .001 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.00220462 * .001 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lbs", "g") { Minimum_English = 0, Maximum_English = .14467, Minimum_Metric = 0, Maximum_Metric = 65.535  } },
            {0x3B, new UnitMeasureScale(0x3A, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .0001 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.00220462 * .0001 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "lbs", "g") { Minimum_English = 0, Maximum_English = .014467, Minimum_Metric = 0, Maximum_Metric = 6.5535  } },
            {0x3C, new UnitMeasureScale(0x3C, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .1 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "µs", "µs") { Minimum_English = 0, Maximum_English = 6553.5, Minimum_Metric = 0, Maximum_Metric =  6553.5  } },
            {0x3D, new UnitMeasureScale(0x3D, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "mA", "mA") { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric =  655.35  } },
            {0x3E, new UnitMeasureScale(0x3E, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .00006103516 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        // sq mm to sq inch
                        return .00155 * .00006103516 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "in2", "mm2") { Minimum_English = 0, Maximum_English = .0062, Minimum_Metric = 0, Maximum_Metric =  4  } },
            {0x3F, new UnitMeasureScale(0x3F, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    if(OBD2Device.UseMetricUnits)
                    {
                        return .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                    else
                    {
                        return 0.26417205 * .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    }
                }, 2, "gal", "L") { Minimum_English = 0, Maximum_English = 173.1251, Minimum_Metric = 0, Maximum_Metric =  655.35  } },
            {0x40, new UnitMeasureScale(0x40, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "ppm", "ppm") { Minimum_English = 0, Maximum_English = 65535, Minimum_Metric = 0, Maximum_Metric =  65535  } },
            {0x41, new UnitMeasureScale(0x41, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return .01 * Convert.ToDouble((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                }, 2, "μA", "μA") { Minimum_English = 0, Maximum_English = 655.35, Minimum_Metric = 0, Maximum_Metric =  655.35  } },
            {0x81, new UnitMeasureScale(0x81, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                    // is the number positive?
                    if((rawVal & 0x8000) == 0) return Convert.ToInt32(rawVal);
                    // negative number, do 2's compliment...
                    rawVal &= 0x7FFF;
                    return Convert.ToInt32(rawVal) - 32768;
                }, 2) { Minimum_English = -32768, Maximum_English = 32767, Minimum_Metric = -32768, Maximum_Metric =  32767  } },
            {0x82, new UnitMeasureScale(0x82, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return .1 * rawVal;

                }, 2) { Minimum_English = -3276.8, Maximum_English = 3276.7, Minimum_Metric = -3276.8, Maximum_Metric =  3276.7  } },
            {0x83, new UnitMeasureScale(0x83, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return .01 * rawVal;
                }, 2) { Minimum_English = -327.68, Maximum_English = 327.67, Minimum_Metric = -327.68, Maximum_Metric =  327.67  } },
            {0x84, new UnitMeasureScale(0x84, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return .001 * rawVal;
                }, 2) { Minimum_English = -32.768, Maximum_English = 32.767, Minimum_Metric = -32.768, Maximum_Metric =  32.767  } },
            {0x85, new UnitMeasureScale(0x85, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return .0000305 * rawVal;

                }, 2) { Minimum_English = -.999, Maximum_English = .999, Minimum_Metric = -.999, Maximum_Metric =  .999  } },
            {0x86, new UnitMeasureScale(0x86, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .000305;
                    
                }, 2) { Minimum_English = -10, Maximum_English = 9.999, Minimum_Metric = -10, Maximum_Metric =  9.999  } },
            {0x87, new UnitMeasureScale(0x87, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    return (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));

                }, 2, "ppm", "ppm") { Minimum_English = -32768, Maximum_English = 32767, Minimum_Metric = -32768, Maximum_Metric =  32767  } },
            {0x8A, new UnitMeasureScale(0x8A, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .000122;

                }, 2, "V", "V") { Minimum_English = -3.999, Maximum_English = 3.999, Minimum_Metric = -3.999, Maximum_Metric =  3.999  } },
            {0x8B, new UnitMeasureScale(0x8B, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .001;

                }, 2, "V", "V") { Minimum_English = -32.768, Maximum_English = 32.767, Minimum_Metric = -32.768, Maximum_Metric =  32.767  } },
            {0x8C, new UnitMeasureScale(0x8C, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .01;

                }, 2, "V", "V") { Minimum_English = -327.68, Maximum_English = 327.67, Minimum_Metric = -327.68, Maximum_Metric =  327.67  } },
            {0x8D, new UnitMeasureScale(0x8D, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .00390625;

                }, 2, "mA", "mA") { Minimum_English = -128, Maximum_English = 127.996, Minimum_Metric = -128, Maximum_Metric =  127.996  } },
            {0x8E, new UnitMeasureScale(0x8E, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .001;

                }, 2, "A", "A") { Minimum_English = -32.768, Maximum_English = 32.767, Minimum_Metric = -32.768, Maximum_Metric =  32.767  } },
            {0x90, new UnitMeasureScale(0x90, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .001;

                }, 2, "s", "s") { Minimum_English = -32.768, Maximum_English = 32.767, Minimum_Metric = -32.768, Maximum_Metric =  32.767  } },
            {0x96, new UnitMeasureScale(0x96, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // ºC
                        return .1 * rawVal;
                    }
                    else
                    {
                        // ºF
                        return ((.1 * rawVal) * 1.8) + 32;
                    }
                }, 2, "ºF", "ºC") { Minimum_English = -5886.2, Maximum_English = 5930.1, Minimum_Metric = -3276.8, Maximum_Metric=3276.7 } },
            {0x99, new UnitMeasureScale(0x99, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return .1 * rawVal;
                    }
                    else
                    {
                        // psi
                        return 0.14503774 * .1 * rawVal;
                    }
                }, 2, "psi", "kPs") { Minimum_English = -475.26, Maximum_English = 475.25, Minimum_Metric = -3276.7, Maximum_Metric=3276.7 } },
            {0x9C, new UnitMeasureScale(0x9C, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .01;

                }, 2, "º", "º") { Minimum_English = -327.68, Maximum_English = 327.67, Minimum_Metric = -327.68, Maximum_Metric=327.67 } },
            {0x9D, new UnitMeasureScale(0x9D, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return rawVal * .5;

                }, 2, "º", "º") { Minimum_English = -16384, Maximum_English = 16383.5, Minimum_Metric = -16384, Maximum_Metric=16383.5 } },
            {0xA8, new UnitMeasureScale(0xA8, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return rawVal;
                    }
                    else
                    {
                        // psi
                        return 0.002204623 * rawVal;
                    }
                }, 2, "lb/s", "g/s") { Minimum_English = -72.24, Maximum_English = 72.24, Minimum_Metric = -32768, Maximum_Metric=32767 } },
            {0xA9, new UnitMeasureScale(0xA9, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // Pa/s
                        return .25 * rawVal;
                    }
                    else
                    {
                        // inH20/s
                        return 0.0040146309 * .25 * rawVal;
                    }
                }, 2, "inH20/s", "Pa/s") { Minimum_English = -32.888, Maximum_English = 32.887, Minimum_Metric = -8192, Maximum_Metric=8191.75 } },
            {0xAD, new UnitMeasureScale(0xAD, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // mg/stroke
                        return .01 * rawVal;
                    }
                    else
                    {
                        // oz/s
                        return 0.00003527 * .01 * rawVal;
                    }
                }, 2, "oz/stroke", "mg/stroke") { Minimum_English = -.0115, Maximum_English = .0115, Minimum_Metric = -327.68, Maximum_Metric = 327.67 } },
            {0xAE, new UnitMeasureScale(0xAE, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // mg/stroke
                        return .1 * rawVal;
                    }
                    else
                    {
                        // oz/s
                        return 0.00003527 * .1 * rawVal;
                    }
                }, 2, "oz/stroke", "mg/stroke") { Minimum_English = -.115, Maximum_English = .115, Minimum_Metric = -3276.8, Maximum_Metric = 3276.7 } },
            {0xAF, new UnitMeasureScale(0xAF, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return .01 * rawVal;
                    
                }, 2, "%", "%") { Minimum_English = -327.68, Maximum_English = 327.67, Minimum_Metric = -327.68, Maximum_Metric=327.67 } },
            {0xB0, new UnitMeasureScale(0xB0, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return .003052 * rawVal;

                }, 2, "%", "%") { Minimum_English = -100, Maximum_English = 100, Minimum_Metric = -100, Maximum_Metric=100 } },
            {0xB1, new UnitMeasureScale(0xB1, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    return 2 * rawVal;

                }, 2, "mV/s", "mV/s") { Minimum_English = -65536, Maximum_English = 65534, Minimum_Metric = -65536, Maximum_Metric=65534 } },
            {0xFC, new UnitMeasureScale(0xFC, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return .01 * rawVal;
                    }
                    else
                    {
                        // psi
                        return 0.14503774 * .01 * rawVal;
                    }
                }, 2, "psi", "kPa") { Minimum_English = -47.525, Maximum_English = 47.525, Minimum_Metric = -327.68, Maximum_Metric=327.67 } },
            {0xFD, new UnitMeasureScale(0xFD, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return .001 * rawVal;
                    }
                    else
                    {
                        // psi
                        return 0.14503774 * .001 * rawVal;
                    }
                }, 2, "psi", "kPa") { Minimum_English = -4.7525, Maximum_English = 4.7525, Minimum_Metric = -32.768, Maximum_Metric=32.767 } },
            {0xFE, new UnitMeasureScale(0xFE, (obj) =>
                {
                    char[] data = ((string)obj).ToCharArray();
                    var rawVal = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                    if(OBD2Device.UseMetricUnits)
                    {
                        // kPa
                        return .25 * rawVal;
                    }
                    else
                    {
                        // psi
                        return 0.0040146308 * .25 * rawVal;
                    }
                }, 2, "inH20", "Pa") { Minimum_English = -.131, Maximum_English = .131, Minimum_Metric = -32.768, Maximum_Metric=32.767 } },


        };

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

        public static readonly Dictionary<MonitorIds, PID> MonitorIdsDictionary = new Dictionary<MonitorIds, PID>
        {
            {MonitorIds.OBDMID_CatalystMonitor_B1, new PID((int)MonitorIds.OBDMID_CatalystMonitor_B1, "Catalyst Monitor B1 Data", "Catalyst Monitor B1 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_CatalystMonitor_B2, new PID((int)MonitorIds.OBDMID_CatalystMonitor_B2, "Catalyst Monitor B2", "Catalyst Monitor B2 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_CatalystMonitor_B3, new PID((int)MonitorIds.OBDMID_CatalystMonitor_B3, "Catalyst Monitor B3 Data", "Catalyst Monitor B3 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_CatalystMonitor_B4, new PID((int)MonitorIds.OBDMID_CatalystMonitor_B4, "Catalyst Monitor B4 Data", "Catalyst Monitor B4 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EGRMonitor_B1, new PID((int)MonitorIds.OBDMID_EGRMonitor_B1, "EGR Monitor B1 Data", "EGR Monitor B1 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EGRMonitor_B2, new PID((int)MonitorIds.OBDMID_EGRMonitor_B2, "EGR Monitor B2 Data", "EGR Monitor B2 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EGRMonitor_B3, new PID((int)MonitorIds.OBDMID_EGRMonitor_B3, "EGR Monitor B3 Data", "EGR Monitor B3 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EGRMonitor_B4, new PID((int)MonitorIds.OBDMID_EGRMonitor_B4, "EGR Monitor B4 Data", "EGR Monitor B4 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_VVTMonitor_B1, new PID((int)MonitorIds.OBDMID_VVTMonitor_B1, "VVT Monitor B1 Data", "VVT Monitor B1 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_VVTMonitor_B2, new PID((int)MonitorIds.OBDMID_VVTMonitor_B2, "VVT Monitor B2 Data", "VVT Monitor B2 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_VVTMonitor_B3, new PID((int)MonitorIds.OBDMID_VVTMonitor_B3, "VVT Monitor B3 Data", "VVT Monitor B3 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_VVTMonitor_B4, new PID((int)MonitorIds.OBDMID_VVTMonitor_B4, "VVT Monitor B4 Data", "VVT Monitor B4 Data", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EvapMonitor_15, new PID((int)MonitorIds.OBDMID_EvapMonitor_15, "Evap Monitor .150\"/Cap Off", "Evap Monitor .150\"/Cap Off", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EvapMonitor_09, new PID((int)MonitorIds.OBDMID_EvapMonitor_09, "Evap Monitor .90\"", "Evap Monitor .90\"", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EvapMonitor_04, new PID((int)MonitorIds.OBDMID_EvapMonitor_04, "Evap Monitor .40\"", "Evap Monitor .40\"", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_EvapMonitor_02, new PID((int)MonitorIds.OBDMID_EvapMonitor_02, "Evap Monitor .20\"", "Evap Monitor .20\"", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_PurgeFlowMonitor, new PID((int)MonitorIds.OBDMID_PurgeFlowMonitor, "Purge Flow Monitor", "Purge Flow Monitor", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S1, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S1, "Exhaust Gas Sensor Heater B1 S1", "Exhaust Gas Sensor Heater B1 S1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S2, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S2, "Exhaust Gas Sensor Heater B1 S2", "Exhaust Gas Sensor Heater B1 S2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S3, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S3, "Exhaust Gas Sensor Heater B1 S3", "Exhaust Gas Sensor Heater B1 S3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S4, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B1_S4, "Exhaust Gas Sensor Heater B1 S4", "Exhaust Gas Sensor Heater B1 S4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S1, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S1, "Exhaust Gas Sensor Heater B2 S1", "Exhaust Gas Sensor Heater B2 S1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S2, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S2, "Exhaust Gas Sensor Heater B2 S2", "Exhaust Gas Sensor Heater B2 S2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S3, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S3, "Exhaust Gas Sensor Heater B2 S3", "Exhaust Gas Sensor Heater B2 S3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S4, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B2_S4, "Exhaust Gas Sensor Heater B2 S4", "Exhaust Gas Sensor Heater B2 S4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S1, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S1, "Exhaust Gas Sensor Heater B3 S1", "Exhaust Gas Sensor Heater B3 S1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S2, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S2, "Exhaust Gas Sensor Heater B3 S2", "Exhaust Gas Sensor Heater B3 S2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S3, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S3, "Exhaust Gas Sensor Heater B3 S3", "Exhaust Gas Sensor Heater B3 S3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S4, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B3_S4, "Exhaust Gas Sensor Heater B3 S4", "Exhaust Gas Sensor Heater B3 S4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S1, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S1, "Exhaust Gas Sensor Heater B4 S1", "Exhaust Gas Sensor Heater B4 S1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S2, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S2, "Exhaust Gas Sensor Heater B4 S2", "Exhaust Gas Sensor Heater B4 S2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S3, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S3, "Exhaust Gas Sensor Heater B4 S3", "Exhaust Gas Sensor Heater B4 S3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S4, new PID((int)MonitorIds.OBDMID_ExhaustGasSensorHeater_B4_S4, "Exhaust Gas Sensor Heater B4 S4", "Exhaust Gas Sensor Heater B4 S4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_HeatedCatalyst_B1, new PID((int)MonitorIds.OBDMID_HeatedCatalyst_B1, "Heated Catalyst B1", "Heated Catalyst B1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_HeatedCatalyst_B2, new PID((int)MonitorIds.OBDMID_HeatedCatalyst_B2, "Heated Catalyst B2", "Heated Catalyst B2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_HeatedCatalyst_B3, new PID((int)MonitorIds.OBDMID_HeatedCatalyst_B3, "Heated Catalyst B3", "Heated Catalyst B3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_HeatedCatalyst_B4, new PID((int)MonitorIds.OBDMID_HeatedCatalyst_B4, "Heated Catalyst B4", "Heated Catalyst B4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_SecondaryAir1, new PID((int)MonitorIds.OBDMID_SecondaryAir1, "Secondary Air Monitor 1", "Secondary Air Monitor 1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_SecondaryAir2, new PID((int)MonitorIds.OBDMID_SecondaryAir2, "Secondary Air Monitor 2", "Secondary Air Monitor 2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_SecondaryAir3, new PID((int)MonitorIds.OBDMID_SecondaryAir3, "Secondary Air Monitor 3", "Secondary Air Monitor 3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_SecondaryAir4, new PID((int)MonitorIds.OBDMID_SecondaryAir4, "Secondary Air Monitor 4", "Secondary Air Monitor 4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_FuelSystem_B1, new PID((int)MonitorIds.OBDMID_FuelSystem_B1, "Fuel System Monitor B1", "Fuel System Monitor B1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_FuelSystem_B2, new PID((int)MonitorIds.OBDMID_FuelSystem_B2, "Fuel System Monitor B2", "Fuel System Monitor B2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_FuelSystem_B3, new PID((int)MonitorIds.OBDMID_FuelSystem_B3, "Fuel System Monitor B3", "Fuel System Monitor B3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_FuelSystem_B4, new PID((int)MonitorIds.OBDMID_FuelSystem_B4, "Fuel System Monitor B4", "Fuel System Monitor B4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_BoostPressureControl_B1, new PID((int)MonitorIds.OBDMID_BoostPressureControl_B1, "Boost Pressure Control Monitor B1", "Boost Pressure Control Monitor B1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_BoostPressureControl_B2, new PID((int)MonitorIds.OBDMID_BoostPressureControl_B2, "Boost Pressure Control Monitor B2", "Boost Pressure Control Monitor B2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_NOxAdsorber_B1, new PID((int)MonitorIds.OBDMID_NOxAdsorber_B1, "NOx Adsorber Monitor B1", "NOx Adsorber Monitor B1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_NOxAdsorber_B2, new PID((int)MonitorIds.OBDMID_NOxAdsorber_B2, "NOx Adsorber Monitor B2", "NOx Adsorber Monitor B2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_NOxSCRCatalyst_B1, new PID((int)MonitorIds.OBDMID_NOxSCRCatalyst_B1, "NOx/SCR Catalyst B1", "NOx/SCR Catalyst B1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_NOxSCRCatalyst_B2, new PID((int)MonitorIds.OBDMID_NOxSCRCatalyst_B2, "NOx/SCR Catalyst B2", "NOx/SCR Catalyst B2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireGeneralData, new PID((int)MonitorIds.OBDMID_MisfireGeneralData, "Misfire Monitor Data", "Misfire Monitor", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder1, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder1, "Misfires Cylinder 1", "Misfires Cylinder 1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder2, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder2, "Misfires Cylinder 2", "Misfires Cylinder 2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder3, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder3, "Misfires Cylinder 3", "Misfires Cylinder 3", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder4, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder4, "Misfires Cylinder 4", "Misfires Cylinder 4", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder5, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder5, "Misfires Cylinder 5", "Misfires Cylinder 5", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder6, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder6, "Misfires Cylinder 6", "Misfires Cylinder 6", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder7, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder7, "Misfires Cylinder 7", "Misfires Cylinder 7", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder8, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder8, "Misfires Cylinder 8", "Misfires Cylinder 8", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder9, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder9, "Misfires Cylinder 9", "Misfires Cylinder 9", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder10, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder10, "Misfire Cylinder 10", "Misfire Cylinder 10", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder11, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder11, "Misfire Cylinder 11", "Misfire Cylinder 11", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder12, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder12, "Misfire Cylinder 12", "Misfire Cylinder 12", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder13, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder13, "Misfire Cylinder 13", "Misfire Cylinder 13", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder14, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder14, "Misfire Cylinder 14", "Misfire Cylinder 14", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder15, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder15, "Misfire Cylinder 15", "Misfire Cylinder 15", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_MisfireData_Cylinder16, new PID((int)MonitorIds.OBDMID_MisfireData_Cylinder16, "Misfire Cylinder 16", "Misfire Cylinder 16", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_PMFilter_B1, new PID((int)MonitorIds.OBDMID_PMFilter_B1, "PM Filter B1", "PM Filter B1", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },
            {MonitorIds.OBDMID_PMFilter_B2, new PID((int)MonitorIds.OBDMID_PMFilter_B2, "PM Filter B2", "PM Filter B2", typeof(string), UnitMeasuresDictionary[UnitMeasure.MonitorTestResult] ) },

        };

    }
}
