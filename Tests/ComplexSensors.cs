using Xunit.Abstractions;
using Xunit.Sdk;
using OS.OBDII.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace PrimaryTests
{
    public class ComplexSensors
    {


        private readonly ITestOutputHelper output;

        public ComplexSensors(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void O2Data()
        {
            O2PID o2Data = null;
            // Max O2 Voltage and Trim
            o2Data = (O2PID)OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.O2PIDVTData].ValueFromInput("FFFF");
            Assert.NotNull(o2Data);
            Assert.True(o2Data.Volts > 1.27 && o2Data.Volts < 1.28, $"O2 volts should max out at 1.275V, was {o2Data.Volts:F2}");
            Assert.True(o2Data.Trim >= 99 && o2Data.Trim <= 100, $"O2 trim should be maxed out at 100%, was {o2Data.Trim:F2}%");
            this.output.WriteLine("***Max O2 Voltage and Trim***");
            this.output.WriteLine($"Voltage: {o2Data.Volts:F3} V{Environment.NewLine}Trim: {o2Data.Trim} %{Environment.NewLine}");


            // Mid O2 Voltage and Trim
            o2Data = (O2PID)OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.O2PIDVTData].ValueFromInput("8080");
            Assert.NotNull(o2Data);
            Assert.True(o2Data.Volts == .64, $"O2 volts should be at mid point of .64V, was {o2Data.Volts:F2}");
            Assert.True(o2Data.Trim == 0, $"O2 trim should be at mid point of 0%, was {o2Data.Trim:F2}%");
            this.output.WriteLine("***Mid/Neutral O2 Voltage and Trim***");
            this.output.WriteLine($"Voltage: {o2Data.Volts:F3} V{Environment.NewLine}Trim: {o2Data.Trim} %{Environment.NewLine}");

            // Min O2 Voltage and Trim
            o2Data = (O2PID)OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.O2PIDVTData].ValueFromInput("0000");
            Assert.NotNull(o2Data);
            Assert.True(o2Data.Volts == 0, $"O2 volts minimum should be 0V, was {o2Data.Volts:F2}");
            Assert.True(o2Data.Trim == -100, $"O2 trim should be at minimum of -100%, was {o2Data.Trim:F2}%");
            this.output.WriteLine("***Min O2 Voltage and Trim***");
            this.output.WriteLine($"Voltage: {o2Data.Volts:F3} V{Environment.NewLine}Trim: {o2Data.Trim} %");

        }


        [Fact]
        public void O2WideBandData()
        {

            string inputData = "FFFFFFFF";

            // Wideband Current
            PID<O2WidePIDmA> p = new PID<O2WidePIDmA>(0x0000, "New Test PID", "PID for testing", OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.O2PIDECData]);

            // Max A/F Eq. Ratio and current (mA)
            O2WidePIDmA tmpVal = p.Parse(inputData);
            Assert.NotNull(tmpVal);
            Assert.True(tmpVal.mA > 127 && tmpVal.mA <= 128, $"A/F Ratio current should max out at 127.996mA, was {tmpVal.mA}");
            Assert.True(tmpVal.EqRatio >= 1.99 && tmpVal.EqRatio <=1.999, $"A/F Equivalence Ratio should be maxed out at 1.999, was {tmpVal.EqRatio}%");
            this.output.WriteLine("***Max A/F Eq. Ratio and current (mA)***");
            this.output.WriteLine($"Equivalence Ratio: {tmpVal.EqRatio:F3} V{Environment.NewLine}Current(mA): {tmpVal.mA} mA{Environment.NewLine}");

            // Mid A/F Eq. Ratio and current (mA)
            inputData = "80008000";
            tmpVal = p.Parse(inputData);
            Assert.NotNull(tmpVal);
            Assert.True(tmpVal.mA == 0, $"A/F Ratio current mid point should be at 0mA, was {tmpVal.mA}");
            Assert.True(tmpVal.EqRatio >= 0.99 && tmpVal.EqRatio <= 1, $"A/F Equivalence Ratio mid point should be at 1, was {tmpVal.EqRatio}%");
            this.output.WriteLine("***Mid A/F Eq. Ratio and current (mA)***");
            this.output.WriteLine($"Equivalence Ratio: {tmpVal.EqRatio} V{Environment.NewLine}Current(mA): {tmpVal.mA} mA{Environment.NewLine}");

            // Min A/F Eq. Ratio and current (mA)
            inputData = "00000000";
            tmpVal = p.Parse(inputData);
            Assert.NotNull(tmpVal);
            Assert.True(tmpVal.mA == -128, $"A/F Ratio current minimum should be at -128mA, was {tmpVal.mA}");
            Assert.True(tmpVal.EqRatio == 0, $"A/F Equivalence Ratio minimum should be at 0, was {tmpVal.EqRatio}%");
            this.output.WriteLine("***Min A/F Eq. Ratio and current (mA)***");
            this.output.WriteLine($"Equivalence Ratio: {tmpVal.EqRatio} V{Environment.NewLine}Current(mA): {tmpVal.mA} mA{Environment.NewLine}");

            // Wideband Voltage
            PID<O2WidePIDVolts> d = new PID<O2WidePIDVolts>(0x0000, "New Test PID", "PID for testing", OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.O2PIDEVData]);

            // Max A/F Eq. Ratio and Volts
            inputData = "FFFFFFFF";
            O2WidePIDVolts wideVoltageData = d.Parse(inputData);
            Assert.NotNull(wideVoltageData);
            Assert.True(wideVoltageData.Volts > 7.99 && wideVoltageData.Volts < 8, $"A/F Ratio current should max out at 7.99V, was {wideVoltageData.Volts}");
            Assert.True(wideVoltageData.EqRatio >= 1.99 && wideVoltageData.EqRatio <= 1.999, $"A/F Equivalence Ratio should be maxed out at 1.999, was {wideVoltageData.EqRatio} %");
            this.output.WriteLine("***Max A/F Eq. Ratio Volts***");
            this.output.WriteLine($"Equivalence Ratio: {wideVoltageData.EqRatio} V{Environment.NewLine}Volts: {wideVoltageData.Volts} V{Environment.NewLine}");

            // Mid A/F Eq. Ratio and Volts
            inputData = "80008000";
            wideVoltageData = d.Parse(inputData);
            Assert.NotNull(wideVoltageData);
            Assert.True(wideVoltageData.Volts > 3.99 && wideVoltageData.Volts < 4, $"A/F Ratio voltage midpoint should be at 3.9V, was {wideVoltageData.Volts}");
            Assert.True(wideVoltageData.EqRatio >= 0.99 && wideVoltageData.EqRatio < 1, $"A/F Equivalence Ratio minimum should be at 0, was {wideVoltageData.EqRatio}%");
            this.output.WriteLine("***Mid A/F Eq. Ratio and Volts***");
            this.output.WriteLine($"Equivalence Ratio: {wideVoltageData.EqRatio} V{Environment.NewLine}Volts: {wideVoltageData.Volts} V{Environment.NewLine}");

            // Min A/F Eq. Ratio and Volts
            inputData = "00000000";
            wideVoltageData = d.Parse(inputData);
            Assert.NotNull(wideVoltageData);
            Assert.True(wideVoltageData.Volts  == 0, $"A/F Ratio voltage minimum should be at 0V, was {wideVoltageData.Volts}");
            Assert.True(wideVoltageData.EqRatio == 0, $"A/F Equivalence Ratio minimum should be at 0, was {wideVoltageData.EqRatio}%");
            this.output.WriteLine("***Min A/F Eq. Ratio and Volts***");
            this.output.WriteLine($"Equivalence Ratio: {wideVoltageData.EqRatio} V{Environment.NewLine}Volts: {wideVoltageData.Volts} V{Environment.NewLine}");

        }


        [Fact]
        public void O2Info()
        {

            List<string> inputs = new List<string>()
            {
                "8080",
                "5A80",
                "5AFF",
                "1070"
            };
            var l = new OBD2PIDS();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // For Each set of inputs, scenarios
            inputs.ForEach(strArray => {
                this.output.WriteLine("_________________");
                try
                {
                    PID<O2PID> p = new PID<O2PID>(0x0000, "New Test PID", "PID for testing", OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.O2PIDVTData]);
                    var o2PIDData = p.Parse(strArray);

                    Assert.NotNull(o2PIDData);

                    this.output.WriteLine($"{Environment.NewLine}Voltage: {o2PIDData.Volts} V{Environment.NewLine}Trim: {o2PIDData.Trim} %");

                }
                catch (Exception ex)
                {
                    this.output.WriteLine($"Error: {ex.Message}");
                    Assert.Fail();

                }
                finally
                {

                }

            });

        }





    }
}