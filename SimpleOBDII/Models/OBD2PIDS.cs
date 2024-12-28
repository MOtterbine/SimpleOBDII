using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class OBD2PIDS
    {

        public enum PIDS
        {
            None = -1,
            OBD2_Status = 0x01,     // Status since DTCs cleared
            OBD2_DTCFRZF = 0x02,    // DTC that caused freeze frame data
            OBD2_FuelSystemStatus = 0x03,    // fuel system status
            OBD2_CalcEngineLoad = 0x04,   // calculated load
            OBD2_EngineCoolantTemp = 0x05,        // coolant temp
            OBD2_SHRTFT13 = 0x06,   // Fuel Trims
            OBD2_LONGFT13 = 0x07,
            OBD2_SHRTFT24 = 0x08,
            OBD2_LONGFT24 = 0x09,
            OBD2_FRP = 0x0A,        // fuel rail pressue
            OBD2_MAP = 0x0B,
            OBD2_RPM = 0x0C,    // Engine rpm
            OBD2_VehicleSpeed = 0x0D,    // Vehicle speed sensor
            OBD2_SPARKADV = 0x0E,
            OBD2_IAT = 0x0F,    // intake air temp - .01g/sec per bit
            OBD2_MAF = 0x10,
            OBD2_ThrottlePosition = 0x11,     //AbsoluteThrottlePosition
            OBD2_AirStatus = 0x12,     //Commanded secondary air status
            /// <summary>
            /// Oxygen sensors location alternate (only 0x13 or 0x1D will be supported by vehicle)
            /// </summary>
            OBD2_OS2LOC = 0x13, 
            OBD2_O2SPID14 = 0x14,
            OBD2_O2SPID15 = 0x15,
            OBD2_O2SPID16 = 0x16,
            OBD2_O2SPID17 = 0x17,
            OBD2_O2SPID18 = 0x18,
            OBD2_O2SPID19 = 0x19,
            OBD2_O2SPID1A = 0x1A,
            OBD2_O2SPID1B = 0x1B,
            OBD2_VehicleOBD2Specification = 0x1C,
            OBD2_PTO_STAT = 0x1E, // PTO, auxiliary input status
            /// <summary>
            /// Oxygen sensors location alternate (only 0x1D or 0x13 will be supported by vehicle)
            /// </summary>
            OBD2_OS2LOC2 = 0x1D, 
            OBD2_TimeSinceEngineStart = 0x1F,

            // 20
            OBD2_KmWithMilOn = 0x21,
            OBD2_FuelRailPressureRelativeToMAP = 0x22,
            OBD2_FuelRailPressure = 0x23,
            OBD2_O2S11V = 0x24,
            OBD2_O2S12V = 0x25,
            OBD2_O2S13V = 0x26,
            OBD2_O2S14V = 0x27,
            OBD2_O2S21V = 0x28,
            OBD2_O2S22V = 0x29,
            OBD2_O2S23V = 0x2A,
            OBD2_O2S24V = 0x2B,
            OBD2_EGR_PCT = 0x2C,  // commanded egr
            OBD2_EGR_ERR = 0x2D,
            OBD2_EVAP_PCT = 0x2E, // commanded evap
            OBD2_FuelLevelInput = 0x2F,        //fuel level input

            // 30
            OBD2_WARM_UPS = 0x30,   // since dtc cleared
            OBD2_KmSinceDTCCleared = 0x31,   // km since cleard
            OBD2_EVAP_VP = 0x32,    // Evap vapor pressure
            OBD2_BARO = 0x33,
            OBD2_O2S11A = 0x34,
            OBD2_O2S12A = 0x35,
            OBD2_O2S13A = 0x36,
            OBD2_O2S14A = 0x37,
            OBD2_O2S21A = 0x38,
            OBD2_O2S22A = 0x39,
            OBD2_O2S23A = 0x3A,
            OBD2_O2S24A = 0x3B,
            OBD2_CATTEMP11 = 0x3C,
            OBD2_CATTEMP21 = 0x3D,  // bank 2, sensor 1
            OBD2_CATTEMP12 = 0x3E,
            OBD2_CATTEMP22 = 0x3F,

            // 40
            OBD2_StatusDriveCycle = 0x41,           // status this monitoring cycle
            OBD2_VPWR = 0x42,                       // Module voltage
            OBD2_LOAD_ABS = 0x43,
            OBD2_LAMBDA = 0x44,                     // Fuel/Air Commanded Equivalence Ratio
            OBD2_RelativeThrottlePosition = 0x45,       // relative throttle pos %
            OBD2_AmbientAirTemp = 0x46,        // ambient air temp
            OBD2_TP_B = 0x47,
            OBD2_TP_C = 0x48,
            OBD2_APP_D = 0x49,
            OBD2_APP_E = 0x4A,
            OBD2_APP_F = 0x4B,
            OBD2_TAC_PCT = 0x4C,    // commanded throttle actuator control
            OBD2_MIL_TIME = 0x4D,   // Run time with MIL on
            OBD2_CLR_TIME = 0x4E,   // Run time since DTCs cleard
            OBD2_ExternalEquipTestConfig1 = 0x4F,

            // 50
            OBD2_ALCH_PCT = 0x52,       // alcohol %
            OBD2_EVAP_VPA = 0x53,       // abs evap pressure
            OBD2_EVAP_VPW = 0x54,      // evap pressure wide - opposed to 0x32 a narrower range
            OBD2_STSO2FT13 = 0x55,      // secondary fuel trims
            OBD2_LGSO2FT13 = 0x56,
            OBD2_STSO2FT24 = 0x57,      // secondary fuel trims
            OBD2_LGSO2FT24 = 0x58,
            OBD2_FRP_ABS = 0x59,        // absolute fuel rail pressure
            OBD2_APP_R = 0x5A,      // relative accelerator pedal pos
            OBD2_BAT_PWR = 0x5B,    // hybride battery life
            OBD2_EOT = 0x5C,        // Engine Oil temp
            OBD2_FUEL_TIMING = 0x5D,    // fuel injection timing
            OBD2_FUEL_RATE = 0x5E,      // engine fuel rate
            OBD2_EMIS_SUP = 0x5F,       // emission requirement support

            // 60
            OBD2_TQ_DD = 0x61,          // Driver's demand engine percent torque %
            OBD2_TQ_ACT = 0x62,         // Actual engine torque %
            OBD2_TQ_REF = 0x63,         // reference torque
            OBD2_TQ_DATA = 0x64,        // various data
            OBD2_PTO_SUPPORT = 0x65,
            OBD2_MAF_DETAIL = 0x66,
            OBD2_ECT_DETAIL = 0x67,     // coolant
            OBD2_IAT_DETAIL = 0x68,
            OBD2_EGR_DETAIL = 0x69,
            OBD2_DIESEL_INTAKE_DETAIL = 0x6A,
            OBD2_EGR_TEMP_DETAIL = 0x6B,
            OBD2_THROTTLE_DETAIL = 0x6C,
            OBD2_FUEL_PRESSURE_CONTROL_SYSTEM = 0x6D,
            OBD2_INJECTION_PRESSURE_CONTROL_SYSTEM = 0x6E,
            OBD2_TURBOCHARGER_COMPRESSOR_INLET_PRESSURE = 0x6F,

            // 70
            OBD2_BoostPressureCtrl = 0x70,
            OBD2_VGT_CTRL = 0x71, // variable geometry turbo
            OBD2_WasteGateCtrl = 0x72,
            OBD2_ExhaustPressure = 0x73,
            OBD2_TurboChargerRPM = 0x74,
            OBD2_TurboChargerATemp = 0x75,
            OBD2_TurboChargerBTemp = 0x76,
            OBD2_ChargeAirCoolerTemp = 0x77,
            OBD2_ExhaustGasTemperatureBank1 = 0x78,
            OBD2_ExhaustGasTemperatureBank2 = 0x79,
            OBD2_DieselParticulateFilterBank1 = 0x7A,
            OBD2_DieselParticulateFilterBank2 = 0x7B,
            OBD2_DieselParticulateFilterTemp = 0x7C,
            OBD2_NOxNTEControlAreaStatus = 0x7D,
            OBD2_PM_NTEControlAreaStatus = 0x7E,
            OBD2_EngineRunTime = 0x7F,

            // 80
            OBD2_EngineRunTimeAECD15 = 0x81,
            OBD2_OBD2_EngineRunTimeAECD610 = 0x82,
            OBD2_NOxSensor = 0x83,
            OBD2_ManifoldSurfaceTemp = 0x84,
            OBD2_NOxControlSystem = 0x85,
            OBD2_ParticulateMatterSensor = 0x86
        }

        public PID this[PIDS key]
        {
            get
            {
                if (!PIDSDictionary.ContainsKey(key)) return null;
                return PIDSDictionary[key];
            }
        }

        public static int DTCCount = 0;


        public object GetFunction(DeviceRequestType devReq)
        {
            return OBD2Device.OPBD2PIDSDictionary[devReq].function;
        }

        public static readonly Dictionary<UnitMeasure, UnitMeasureScale> UnitMeasuresDictionary = new Dictionary<UnitMeasure, UnitMeasureScale>
        {
            {UnitMeasure.Pressure1, new UnitMeasureScale(UnitMeasure.Pressure1, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        int dt = (Int16)((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));

                        if(OBD2Device.UseMetricUnits)
                        {
                            // Pa
                            return .25*dt;
                        }
                        else
                        {
                            // water inches (inH2O)
                            return 0.0040146307866177 * (.25*dt);
                       }

                    },2, "inH2O","Pa") },
            {UnitMeasure.kPa, new UnitMeasureScale(UnitMeasure.kPa, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // kPa
                            return ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) / 100;
                        }
                        else
                        {
                            // PSI
                            return 0.14503774*((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) / 100;
                       }

                    },2,"psi", "kPa") },
            {UnitMeasure.AirFlow1, new UnitMeasureScale(UnitMeasure.AirFlow1, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        var t = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);


                        if(OBD2Device.UseMetricUnits)
                        {
                            // g/s
                            return Convert.ToDouble(t)/100;
                        }
                        else
                        {
                            // lb/min
                            return 0.002204623 * 60 * (Convert.ToDouble(t)/100);
                        }

                   }, 2,"lb/min", "g/s") },
            {UnitMeasure.Temperature1, new UnitMeasureScale(UnitMeasure.Temperature1, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        var dt = ((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));


                        if(OBD2Device.UseMetricUnits)
                        {
                            // Celcius
                            return (Convert.ToDouble(dt) - 40);
                        }
                        else
                        {
                            // Farenheight
                            return ((Convert.ToDouble(dt) - 40) * 1.8) + 32;
                        }

                        //return ((Convert.ToDouble(dt) - 40) * 1.8) + 32;
                    }, 1, "°F", "°C") },
            //{UnitMeasure.degC, new UnitMeasureScale(UnitMeasure.degC, (obj) =>
            //        {
            //            char[] data = ((string)obj).ToCharArray();
            //            var dt = ((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
            //            return Convert.ToDouble(dt) - 40;
            //        }, 1,"°C") },
            {UnitMeasure.CatalystTemp, new UnitMeasureScale(UnitMeasure.CatalystTemp, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        var dt = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);


                        if(OBD2Device.UseMetricUnits)
                        {
                            // Celcius
                            return ((dt*.1) - 40);
                        }
                        else
                        {
                            // Farenheight
                            return (((dt*.1) - 40) * 1.8) + 32;
                        }



                    },2,"°F", "°C") },
            {UnitMeasure.Rpm, new UnitMeasureScale(UnitMeasure.Rpm, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        return ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) / 4;
                    }, 2, string.Empty, string.Empty) },
            {UnitMeasure.Percent, new UnitMeasureScale(UnitMeasure.Percent, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        return (int)((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1])) * 100/256;
                    },1 ,"%", "%") },
            {UnitMeasure.TorquePercent, new UnitMeasureScale(UnitMeasure.TorquePercent, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        return ((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1])) - 125;
                    },1 ,"%", "%") { Maximum_Metric = 100, Maximum_English = 100, Minimum_Metric = 0, Minimum_English = 0 } },
            {UnitMeasure.Distance, new UnitMeasureScale(UnitMeasure.Distance, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // km
                            return ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                        }
                        else
                        {
                            // mi
                            return Convert.ToInt32((((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]))/1.609344));
                        }

                    },2 ,"mi", "km") },
            {UnitMeasure.Count, new UnitMeasureScale(UnitMeasure.Count, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        return (double)((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                    },1 ) },
            {UnitMeasure.Count16, new UnitMeasureScale(UnitMeasure.Count, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        return (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                    },2 ) },
            {UnitMeasure.psi, new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                    {
                        // .079 kPa/.1450377 PSI - per bit
                        char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // kPa
                            return (((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * .079);
                        }
                        else
                        {
                            // psi
                            return (((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * .079) * 0.1450377;
                        }

                    },2 ,"psi", "kPa"){ DecimalPlaces = 2} },
            {UnitMeasure.Pressure2, new UnitMeasureScale(UnitMeasure.Pressure2, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // kPa
                            return (double)((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                        }
                        else
                        {
                            // inches of mercury (inHg)
                            return  0.2953006999 * (double)((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                        }

                    },1 ,"inHg","kPa") },
           // Standard O2 (narrow band) voltage
           {UnitMeasure.O2PIDVTData, new UnitMeasureScale(UnitMeasure.O2PIDVTData, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        var v = (HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]);
                        var tVal = (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                        int jack = ((int)tVal - 128);
                        double t = jack * 100/128;

                        return new O2PID()
                        {
                            Volts = v*.005,
                            Trim = t
                        };
                    },2)  { Maximum_Metric = 1.3, Maximum_English = 1.3, Minimum_Metric = 0, Minimum_English = 0 }},
            // AF Sensor (wide-band) current
            {UnitMeasure.O2PIDECData, new UnitMeasureScale(UnitMeasure.O2PIDECData, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();

                        var v = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8);
                        v += (HexTable.FromHex(data[2])<<4) | (HexTable.FromHex(data[3]));

                        var t = (HexTable.FromHex(data[4])<<12) | (HexTable.FromHex(data[5])<<8);
                        t += (HexTable.FromHex(data[6])<<4) | HexTable.FromHex(data[7]);

                        return new O2WidePIDmA()
                        {
                            EqRatio = v*.0000305,
                            mA = (t*.00390625)-128
                        };

                    },4)  { Maximum_Metric = 130, Maximum_English = 130, Minimum_Metric = -130, Minimum_English = -130 }},
            // AF Sensor (wide-band) voltage
            {UnitMeasure.O2PIDEVData, new UnitMeasureScale(UnitMeasure.O2PIDEVData, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();

                        var v = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8);
                        v += (HexTable.FromHex(data[2])<<4) | (HexTable.FromHex(data[3]));

                        var t = (HexTable.FromHex(data[4])<<12) | (HexTable.FromHex(data[5])<<8);
                        t += (HexTable.FromHex(data[6])<<4) | HexTable.FromHex(data[7]);

                        return new O2WidePIDVolts()
                        {
                            EqRatio = v*.0000305,
                            Volts = (t*.000122)
                        };

                    },4)  { Maximum_Metric = 8, Maximum_English = 8, Minimum_Metric = 0, Minimum_English = 0 }},
            {UnitMeasure.Volts32, new UnitMeasureScale(UnitMeasure.Volts32, (obj) =>
                    {
                        char[] data = ((string)obj).ToCharArray();
                        var v = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8);
                        v += (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                        return ((double)v)/1000;
                    },2 ,"V","V") },
            {UnitMeasure.OffsetPercent, new UnitMeasureScale(UnitMeasure.OffsetPercent, (obj) =>
                    {
                        // fuel trims...
                        char[] data = ((string)obj).ToCharArray();
                        return ((double)((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]))-128) * 100/128;
                    },1 ,"%","%") { Maximum_Metric = 25, Maximum_English = 25, Minimum_Metric = -25, Minimum_English = -25 }},
        };

        public static readonly Dictionary<PIDS, PID> PIDSDictionary = new Dictionary<PIDS, PID>
        {
            {PIDS.OBD2_DTCFRZF, new PID(0x02, "DTCFRZF", "DTC that caused required freeze frame data storage", typeof(string),
                        (obj)=>
                        {
                            string str = obj as string;
                            return OBD2Device.TranslateToString(uint.Parse(str.Substring(0,4), System.Globalization.NumberStyles.AllowHexSpecifier));
                        }, false ) },
            {PIDS.OBD2_FuelSystemStatus, new PID(0x03, "FUELSYS", "Fuel system status", typeof(string),
                        (obj)=>
                        {
                            string str = obj as string;
                            var fuelStatus = new FuelSystemStatus[]{ new FuelSystemStatus(), new FuelSystemStatus() };

                            uint f = uint.Parse(str.Substring(0,2));
                            fuelStatus[0].ApplyValue(f);

                            f = uint.Parse(str.Substring(2,2));
                            fuelStatus[1].ApplyValue(f);


                            // (A*256)+B
                            //var km = int.Parse(strArray[0].Substring(4,2), System.Globalization.NumberStyles.HexNumber) * 256;
                            //return km + int.Parse(strArray[0].Substring(6,2), System.Globalization.NumberStyles.HexNumber);

                            return fuelStatus;
                        } ) { CanPlot = false } },
            {PIDS.OBD2_CalcEngineLoad, new PID(0x04, "LOAD_PCT", "Calc. Engine Load %", typeof(int),UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_EngineCoolantTemp, new PID(0x05, "ECT", "Engine Coolant Temp", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Temperature1]) {DecimalPlaces=1 }},
            {PIDS.OBD2_SHRTFT13, new PID(0x06, "SHRTFT", "Short-term fuel trim for bank 1/3 (%)", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.OffsetPercent]){DecimalPlaces=1 } },
            {PIDS.OBD2_LONGFT13, new PID(0x07, "LONGFT", "Long-term fuel trim for bank 1/3 (%)", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.OffsetPercent]){DecimalPlaces=1 } },
            {PIDS.OBD2_SHRTFT24, new PID(0x08, "SHRTFT", "Short-term fuel trim for bank 2/4 (%)", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.OffsetPercent]){DecimalPlaces=1 } },
            {PIDS.OBD2_LONGFT24, new PID(0x09, "LONGFT", "Long-term fuel trim for bank 2/4 (%)", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.OffsetPercent]){DecimalPlaces=1 } },
            {PIDS.OBD2_FRP, new PID(0x0A, "FRP", "Fuel Rail Pressure (gauge)", typeof(decimal),
                        new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                        {
                            //string str = obj as string;
                            //var t = int.Parse(str.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
                            //return 3*t; // 3kPa per bit

                            char[] data = ((string)obj).ToCharArray();


                        if(OBD2Device.UseMetricUnits)
                        {
                            // kPa
                            return 3*((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                        }
                        else
                        {
                            // PSI
                            return 3*0.14503774*((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                        }


                        },1 ,"psi","kPa") ) {DecimalPlaces=1 } },
            {PIDS.OBD2_MAP, new PID(0x0B, "MAP", "Intake Manifold Air Pressure", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Pressure2]){DecimalPlaces=1 } },
            {PIDS.OBD2_RPM, new PID(0x0C, "RPM", "Engine RPM", typeof(int), UnitMeasuresDictionary[UnitMeasure.Rpm]) },
            {PIDS.OBD2_VehicleSpeed, new PID(0x0D, "VSS", "Vehicle speed sensor", typeof(decimal),
                        new UnitMeasureScale(UnitMeasure.MPH, (obj) =>
                        {
                            char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // kph
                            return ((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                        }
                        else
                        {
                            // mph
                            return ((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]))/1.609344;
                        }

                        },1 ,"mph", "kph") ) {DecimalPlaces=0 } },
            {PIDS.OBD2_SPARKADV, new PID(0x0E, "SPARKADV", "Spark advance (non mechanical)", typeof(decimal),
                        new UnitMeasureScale(UnitMeasure.AngleDeg, (obj) =>
                        {
                            //string str = obj as string;
                            //return (int.Parse(str.Substring(0,2), System.Globalization.NumberStyles.HexNumber) / 2) - 64;

                            char[] data = ((string)obj).ToCharArray();
                            return (((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1])) / 2) - 64;


                        },1 ,"°BTDC","°BTDC") ) {DecimalPlaces=1 } },
            {PIDS.OBD2_IAT, new PID(0x0F, "IAT", "Intake air temperature", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Temperature1] ){DecimalPlaces=1 } },
            {PIDS.OBD2_MAF, new PID(0x10, "MAF", "Mass Air Flow Rate", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.AirFlow1]){DecimalPlaces=1 } },
            {PIDS.OBD2_ThrottlePosition, new PID(0x11, "TP", "Throttle position", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent] ) },
            {PIDS.OBD2_OS2LOC, new PID(0x13, "O2SLOC", "O2 Sensor Location", typeof(byte),
                        new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                        {
                            char[] data = ((string)obj).ToCharArray();
                            return data[0];
                        },1 ,"O2SLOC","O2SLOC"), false ){ IsVisible = false, CanPlot = false}  },
            
            // WHEN PID 0x13 is supported
            {PIDS.OBD2_O2SPID14, new PID(0x14, "O2S11", "O2 Sensor 1 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            {PIDS.OBD2_O2SPID15, new PID(0x15, "O2S12", "O2 Sensor 2 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            {PIDS.OBD2_O2SPID16, new PID(0x16, "O2S13", "O2 Sensor 3 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            {PIDS.OBD2_O2SPID17, new PID(0x17, "O2S14", "O2 Sensor 4 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            {PIDS.OBD2_O2SPID18, new PID(0x18, "O2S21", "O2 Sensor 1 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            {PIDS.OBD2_O2SPID19, new PID(0x19, "O2S22", "O2 Sensor 2 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            {PIDS.OBD2_O2SPID1A, new PID(0x1A, "O2S23", "O2 Sensor 3 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            {PIDS.OBD2_O2SPID1B, new PID(0x1B, "O2S24", "O2 Sensor 4 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
           
            // WHEN PID 0x1D is supported
            //{PIDS.OBD2_O2SPID14, new PID(0x14, "O2S11", "Bank1 Sensor1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            //{PIDS.OBD2_O2SPID15, new PID(0x15, "O2S12", "Bank1 Sensor2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            //{PIDS.OBD2_O2SPID16, new PID(0x16, "O2S21", "Bank2 Sensor1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            //{PIDS.OBD2_O2SPID17, new PID(0x17, "O2S22", "Bank2 Sensor2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            //{PIDS.OBD2_O2SPID18, new PID(0x18, "O2S31", "Bank3 Sensor1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            //{PIDS.OBD2_O2SPID19, new PID(0x19, "O2S32", "Bank3 Sensor2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            //{PIDS.OBD2_O2SPID1A, new PID(0x1A, "O2S41", "Bank4 Sensor1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },
            //{PIDS.OBD2_O2SPID1B, new PID(0x1B, "O2S42", "Bank4 Sensor2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]) },

            {PIDS.OBD2_VehicleOBD2Specification, new PID(0x1C, "OBDSUP", "OBD Requirements Supported", typeof(OBD2Specification),
                        (obj)=>
                        {
                            char[] data = ((string)obj).ToCharArray();
                            return OBD2Specification.GetSpecification((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                        } ) { ResponseByteCount=1, CanPlot=false } },
            {PIDS.OBD2_OS2LOC2, new PID(0x1D, "O2SLOC", "O2 Sensor Location 2", typeof(byte),
                        new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                        {
                            char[] data = ((string)obj).ToCharArray();
                            return data[0];
                        },1 ,"O2SLOC"), false ){ IsVisible = false, CanPlot = false }  },
            {PIDS.OBD2_PTO_STAT, new PID(0x1E, "PTO_STAT", "Power Take Off Status", typeof(bool),
                        (obj)=>
                        {
                            char[] data = ((string)obj).ToCharArray();
                            return Convert.ToBoolean((HexTable.FromHex(data[0])<<4) | HexTable.FromHex(data[1]));
                        }) { ResponseByteCount=1, CanPlot = false } },
            {PIDS.OBD2_TimeSinceEngineStart, new PID(0x1F, "RUNTM", "Time Since Engine Start", typeof(TimeSpan),
                        (obj)=>
                        {
                            char[] data = ((string)obj).ToCharArray();
                            double value = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                            return TimeSpan.FromSeconds(value);
                        } )  { CanPlot = false }},
            {PIDS.OBD2_KmWithMilOn, new PID(0x21, "MIL_DIST", "Distance With MIL on", typeof(int), UnitMeasuresDictionary[UnitMeasure.Distance] ){DecimalPlaces=1, CanPlot = false } },
            {PIDS.OBD2_FuelRailPressureRelativeToMAP, new PID(0x22, "FRP", "Fuel Rail Pressure Relative to Manifold Vacuum", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.psi]) },
            {PIDS.OBD2_FuelRailPressure, new PID(0x23, "FRP", "Fuel Rail Pressure", typeof(decimal),
                    new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                    {
                        // .079 kPa/.1450377 PSI - per bit
                        //string str = obj as string;
                        //return (int.Parse(str, System.Globalization.NumberStyles.HexNumber) * 10) * 0.1450377;

                        char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // kPa
                            return (((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * 10);
                        }
                        else
                        {
                            // PSI
                            return (((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * 10) * 0.1450377;
                        }

                    },2 ,"psi", "kPa") )  },
            {PIDS.OBD2_EGR_PCT, new PID(0x2C, "EGR_PCT", "Commanded EGR %", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_EGR_ERR, new PID(0x2D, "EGR_PCT", "EGR Error %", typeof(int), UnitMeasuresDictionary[UnitMeasure.OffsetPercent]) },
            {PIDS.OBD2_EVAP_PCT, new PID(0x2E, "EVAP_PCT", "Commanded Evap. Purge %", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_FuelLevelInput, new PID(0x2F, "FLI", "Fuel level %", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_WARM_UPS, new PID(0x30, "WARM_UPS", "Warmups since DTCs Cleared", typeof(int), UnitMeasuresDictionary[UnitMeasure.Count])  { CanPlot = false }},
            {PIDS.OBD2_KmSinceDTCCleared, new PID(0x31, "CLR_DIST", "Distance Since DTCs Cleared", typeof(int), UnitMeasuresDictionary[UnitMeasure.Distance] ){DecimalPlaces=0, CanPlot = false }},
            {PIDS.OBD2_EVAP_VP, new PID(0x32, "EVAP_VP", "Evap Pressure", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Pressure1]){DecimalPlaces=1 } },
            {PIDS.OBD2_BARO, new PID(0x33, "BARO", "Barometric Pressure", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Pressure2]){DecimalPlaces=1 } },

            // WHEN PID 0x13 is supported
            //{PIDS.OBD2_O2SPID14, new PID(0x34, "O2S11", "O2 Sensor 1 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            //{PIDS.OBD2_O2SPID15, new PID(0x35, "O2S12", "O2 Sensor 2 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            //{PIDS.OBD2_O2SPID16, new PID(0x36, "O2S13", "O2 Sensor 3 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            //{PIDS.OBD2_O2SPID17, new PID(0x37, "O2S14", "O2 Sensor 4 Bank 1", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            //{PIDS.OBD2_O2SPID18, new PID(0x38, "O2S21", "O2 Sensor 1 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            //{PIDS.OBD2_O2SPID19, new PID(0x39, "O2S22", "O2 Sensor 2 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            //{PIDS.OBD2_O2SPID1A, new PID(0x3A, "O2S23", "O2 Sensor 3 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            //{PIDS.OBD2_O2SPID1B, new PID(0x3B, "O2S24", "O2 Sensor 4 Bank 2", typeof(O2PID), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },

            {PIDS.OBD2_O2S11A, new PID(0x34, "O2S11", "O2 Sensor 1 Bank 1", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            {PIDS.OBD2_O2S12A, new PID(0x35, "O2S12", "O2 Sensor 2 Bank 1", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            {PIDS.OBD2_O2S13A, new PID(0x36, "O2S13", "O2 Sensor 3 Bank 1", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            {PIDS.OBD2_O2S14A, new PID(0x37, "O2S14", "O2 Sensor 4 Bank 1", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            {PIDS.OBD2_O2S21A, new PID(0x38, "O2S21", "O2 Sensor 1 Bank 2", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            {PIDS.OBD2_O2S22A, new PID(0x39, "O2S22", "O2 Sensor 2 Bank 2", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            {PIDS.OBD2_O2S23A, new PID(0x3A, "O2S23", "O2 Sensor 3 Bank 2", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },
            {PIDS.OBD2_O2S24A, new PID(0x3B, "O2S24", "O2 Sensor 4 Bank 2", typeof(O2WidePIDmA), UnitMeasuresDictionary[UnitMeasure.O2PIDECData]) },

            {PIDS.OBD2_CATTEMP11, new PID(0x3C, "CATEMP11:", "Cat Temp Bank 1 Sensor 1", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.CatalystTemp]) },
            {PIDS.OBD2_CATTEMP21, new PID(0x3D, "CATEMP21:", "Cat Temp Bank 2 Sensor 1", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.CatalystTemp]) },
            {PIDS.OBD2_CATTEMP12, new PID(0x3E, "CATEMP12:", "Cat Temp Bank 1 Sensor 2", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.CatalystTemp]) },
            {PIDS.OBD2_CATTEMP22, new PID(0x3F, "CATEMP22:", "Cat Temp Bank 2 Sensor 2", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.CatalystTemp]) },


            {PIDS.OBD2_StatusDriveCycle, new PID(0x41, "Drive Cycle Readiness", "Readiness status For This Drive Cycle", typeof(string),
                        (obj)=>
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

                                            readinessObject.DTCCount = DTCCount = byteBuf[i] & 0x7F; // everything but bit 7 (mil light)
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
                        }, false ) { CanPlot = false } },
            {PIDS.OBD2_VPWR, new PID(0x42, "VPWR", "System Volts", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Volts32]){DecimalPlaces=2 } },
            {PIDS.OBD2_LOAD_ABS, new PID(0x43, "LOAD_ABS:", "Absolute Load", typeof(decimal),
                        new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                        {

                            char[] data = ((string)obj).ToCharArray();
                            return (((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * 100) / 256;

                        },2 ,"%","%") )  },
            {PIDS.OBD2_LAMBDA, new PID(0x44, "LAMBDA", "Fuel/Air Commanded Equiv. Ratio", typeof(decimal),
                        new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                        {

                            char[] data = ((string)obj).ToCharArray();
                            return ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * .0000305;

                        },2 ,string.Empty, string.Empty) ){ DecimalPlaces = 3}  },
            {PIDS.OBD2_RelativeThrottlePosition, new PID(0x45, "TP_R", "Relative Throttle Position", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]){DecimalPlaces=1 } },
            {PIDS.OBD2_AmbientAirTemp, new PID(0x46, "AAT", "Ambient Air Temperature", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Temperature1]){DecimalPlaces=0 } },
            {PIDS.OBD2_TP_B, new PID(0x47, "TP_B", "Absolute Throttle Position B", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_TP_C, new PID(0x48, "TP_C", "Absolute Throttle Position C", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_APP_D, new PID(0x49, "APP_D", "Accelerator Position D", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_APP_E, new PID(0x4A, "APP_E", "Accelerator Position E", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_APP_F, new PID(0x4B, "APP_F", "Accelerator Position F", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_TAC_PCT, new PID(0x4C, "TAC_PCT", "Commanded Throttle Actuator Position", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]){DecimalPlaces=1 } },

            {PIDS.OBD2_MIL_TIME, new PID(0x4D, "MIL_TIME", "Engine Run Time - MIL On", typeof(TimeSpan),
                        (obj)=>
                        {
                            char[] data = ((string)obj).ToCharArray();
                            double value = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                            return TimeSpan.FromMinutes(value);
                        } )  { CanPlot = false }},
            {PIDS.OBD2_CLR_TIME, new PID(0x4E, "CLR_TIME", "Run Time Since DTCs Cleared", typeof(TimeSpan),
                        (obj)=>
                        {
                            char[] data = ((string)obj).ToCharArray();
                            double value = (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                            return TimeSpan.FromMinutes(value);
                        } )  { CanPlot = false }},
            {PIDS.OBD2_EVAP_VPA, new PID(0x53, "EVAP_VPA", "Absolute Evap System Vapor Pressure", typeof(decimal),
                        new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                        {
                            // .005 kPa/bit...
                            char[] data = ((string)obj).ToCharArray();
                            return ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * .005;

                            if(OBD2Device.UseMetricUnits)
                            {
                                // kPa
                                return ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * .005;
                            }
                            else
                            {
                                // inH2O
                                return 4.014742 * ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * .005;
                            }

                        },2, "inH2O","kPa") ){DecimalPlaces=3 } },

            {PIDS.OBD2_EVAP_VPW, new PID(0x54, "EVAP_VPW", "Evap System Vapor Pressure", typeof(int),
                        new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                        {
                            //  1 Pa/bit...signed
                            char[] data = ((string)obj).ToCharArray();

                            if(OBD2Device.UseMetricUnits)
                            {
                                // kPa
                                return ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                            }
                            else
                            {
                                // PSI
                                return 0.1450377 *((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                            }

                        },2, "psi","kPa") ){DecimalPlaces=3 } },

            {PIDS.OBD2_FRP_ABS, new PID(0x59, "FRP", "Fuel Rail Pressure (absolute)", typeof(decimal),
                    new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                    {
                        //  1 Pa/bit...signed
                        char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // kPa
                            return (((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * 10);
                        }
                        else
                        {
                            // PSI
                            return  0.1450377 * (((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3])) * 10);
                        }

                    },2 ,"psi", "kPa") ){DecimalPlaces=3 } },
            {PIDS.OBD2_APP_R, new PID(0x5A, "APP_R", "Relative Accelerator Pedal Position", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_BAT_PWR, new PID(0x5B, "BAT_PWR", "Hybrid/EV Battery Pack Remaining Charge", typeof(int), UnitMeasuresDictionary[UnitMeasure.Percent]) },
            {PIDS.OBD2_EOT, new PID(0x5C, "Oil Temperature", "Vehicle Oil Temperature", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.Temperature1]){DecimalPlaces=1 } },
            {PIDS.OBD2_FUEL_TIMING, new PID(0x5D, "FUEL_TIMING", "Fuel Injection Timing", typeof(decimal),
                    (obj)=>
                    {
                        string str = obj as string;
                        return (double)(int.Parse(str, System.Globalization.NumberStyles.HexNumber)/128);

                    } ){DecimalPlaces=1, EnglishUnitDescriptor="°", MetricUnitDescriptor = "°" } },
            {PIDS.OBD2_FUEL_RATE, new PID(0x5E, "FUEL_RATE", "Fuel Rate", typeof(decimal), // liters per hour
                    new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                    {
                        string str = obj as string;

                        if(OBD2Device.UseMetricUnits)
                        {
                            // Liter/hr
                            return int.Parse(str, System.Globalization.NumberStyles.HexNumber) * .05;
                        }
                        else
                        {
                            // Gallon/hour
                            return 0.2641729*(int.Parse(str, System.Globalization.NumberStyles.HexNumber) * .05);
                        }

                    },2,"G/h", "L/h") ) {DecimalPlaces=1 }},
            {PIDS.OBD2_TQ_DD, new PID(0x61, "TQ_DD:", "Driver's Demand Engine Percent Torque", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.TorquePercent]) },
            {PIDS.OBD2_TQ_ACT, new PID(0x62, "TQ_ACT:", "Actual Engine Torque Percent", typeof(decimal), UnitMeasuresDictionary[UnitMeasure.TorquePercent]) },
            {PIDS.OBD2_TQ_REF, new PID(0x63, "TQ_REF:", "Engine Reference Torque", typeof(decimal),
                    new UnitMeasureScale(UnitMeasure.psi, (obj) =>
                    {
                        // 1Nm/bit...
                        char[] data = ((string)obj).ToCharArray();

                        if(OBD2Device.UseMetricUnits)
                        {
                            // nM
                            return (HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]);
                        }
                        else
                        {
                            // foot-lbs
                            return 0.7375621493 * ((HexTable.FromHex(data[0])<<12) | (HexTable.FromHex(data[1])<<8) | (HexTable.FromHex(data[2])<<4) | HexTable.FromHex(data[3]));
                        }

                    },2, "ft-lb" ,"Nm") ){DecimalPlaces=1 } },
            //{PIDS.OBD2_TQ_REF, new PID(0x64, "TQ_REF:", "Engine reference torque", typeof(List<double>),
            //            new UnitMeasureScale(UnitMeasure.psi, (obj) =>
            //            {

            //                char[] data = ((string)obj).ToCharArray();

            //                double[] tqs = new double[5];
                            
            //                for(int i=0;i<5;i++)
            //                {
            //                    tqs[i] = (HexTable.FromHex(data[i*2])<<4) | HexTable.FromHex(data[(i*2)+1]);
            //                }
            //                return tqs;
            //            },2 ,"Nm") )  },
            {PIDS.OBD2_ExhaustPressure, new PID(0x73, "Exhaust Pressure", "Exhaust Pressure", typeof(decimal[]), new UnitMeasureScale(UnitMeasure.kPa, (obj) =>
                    {
                        double[] d = new double[2];
                        string str = obj as string;
                        var t = str.ToArray();//.Substring(0,8), System.Globalization.NumberStyles.HexNumber))/100;
                        if(t.Length < 10) return null;
                        if((t[0] & 0x01) > 0)
                        {
                            // .079 kPa/.1450377 PSI - per bit
                            //string str = obj as string;

                            if(OBD2Device.UseMetricUnits)
                            {
                                // kPa
                                d[0] = ((Int16)t[1] * .079);
                            }
                            else
                            {
                                // psi
                                d[0] = 0.1450377 * ((Int16)t[1] * .079);
                            }
                        }
                        if((t[0] & 0x02) > 0)
                        {

                            if(OBD2Device.UseMetricUnits)
                            {
                                // kPa
                                d[1] = ((Int16)t[3] * .079);
                            }
                            else
                            {
                                // psi
                                d[1] = 0.1450377 * ((Int16)t[3] * .079);
                            }
                       }


                        return d;

                    },5,"psi", "kPa")){DecimalPlaces=1 }}

        };



    }
}
