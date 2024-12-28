using Xunit.Abstractions;
using Xunit.Sdk;
using OS.OBDII.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace PrimaryTests
{
    public class MonitorTests
    {
        private readonly ITestOutputHelper output;

        public MonitorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AllMonitorScaleDefinitions()
        {

            List<string> inputs = new List<string>()
            {
                "8000",
                "D8F0",
                "FFFF",
                "0000",
                "0001",
                "2710",
                "7FFF"
            };

            OBD2Device.UseMetricUnits = true;

            var l = new OBD2MIDS();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            OBD2MIDS.UnitScalingDictionary.ToList().ForEach(scaleEntry => {
                this.output.WriteLine($"***************** Code:${scaleEntry.Key:X2} *****************");
                inputs.ForEach(strArray => {
                    try
                    {
                        var ump = scaleEntry.Value.ValueFromInput(strArray);
                        this.output.WriteLine($"{strArray} : {ump} {scaleEntry.Value.Descriptor}");
                       // this.output.WriteLine($"{strArray} : {scaleEntry}");

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
            });

        }

        [Fact]
        public void SupportedPIDS()
        {
            //Func<object, object> pidFunction = null;
            List<ECU> ecus = new List<ECU>();
            List<uint> supportedPidsList = null;
            var l = new OBD2PIDS();

            List<string[]> inputs = new List<string[]>()
            {
              //  new string[] { "7E8064100BF9FB993","7E80641208F9FC975","7E8064140BF9FB993", "7E8064160BF9FB993", "7E8064180BF9FB993", "7E80641A0BF9FB993", "7E80641C0BF9FB993", "7E80641E0BF9FB993", },
                new string[] { "7E8064100FFFFFFFF", "7E8064120FFFFFFFF", "7E8064140FFFFFFFF", "7E8064160FFFFFFFF", "7E8064180FFFFFFFF", "7E80641A0FFFFFFFF", "7E80641C0FFFFFFFF", "7E80641E0FFFFFFFF", },
             //  new string[] { "7E8064100FFFFFFFF", "7E8064120FFFFFFFF", "7E0064140FFFFFFFF", "7E8064160FFFFFFFF", "7E8064180FFFFFFFF", "7E80641A0FFFFFFFF", "7E80641C0FFFFFFFF", "7E80641E0FFFFFFFF", },
               //new string[] { "7E806410000100000" },
            };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // For Each set of inputs, scenarios
            inputs.ForEach(strArray => {
                this.output.WriteLine("_________________");
                try
                {
                    var pidFunction = (Func<object, object>)l.GetFunction(DeviceRequestType.OBD2_GetPIDS_00);
                    var ecuList = (List<ECU>)pidFunction(strArray);
                    ecus.AddRange(ecuList);

                    sb.Clear();

                    ecus.ForEach(ecu =>
                    {

                        sb.Append($"\nECU {ecu.Id.ToString("X3")}");
                        var supPids = ecu.SupportedPIDS;

                        ecu.SupportedPIDS.ForEach(pid =>
                        {
                            sb.Append($"{Environment.NewLine}  {pid}");
                        });

                    });

                    this.output.WriteLine(sb.ToString());

                }
                catch (Exception ex)
                {
                    this.output.WriteLine($"Error: {ex.Message}");
                    Assert.Fail();

                }
                finally
                {
                    ecus.Clear();
                }

            });

        }

        [Fact]
        public void SupportedMonitorIds()
        {
            //Func<object, object> pidFunction = null;
            List<ECU> ecus = new List<ECU>();
            List<uint> supportedMonitorIds = null;
            var l = new OBD2MIDS();

            List<string[]> inputs = new List<string[]>()
            {
              //  new string[] { "7E8064100BF9FB993","7E80641208F9FC975","7E8064140BF9FB993", "7E8064160BF9FB993", "7E8064180BF9FB993", "7E80641A0BF9FB993", "7E80641C0BF9FB993", "7E80641E0BF9FB993", },
             //   new string[] { "7E8064600FFFFFFFF", "7E8064620FFFFFFFF", "7E8064640FFFFFFFF", "7E8064660FFFFFFFF", "7E8064680FFFFFFFF", "7E80646A0FFFFFFFF", "7E80646C0FFFFFFFF", "7E80646E0FFFFFFFF", },
               new string[] { "7E8064100FFFFFFFF", "7E8064120FFFFFFFF", "7E0064140FFFFFFFF", "7E8064160FFFFFFFF", "7E8064180FFFFFFFF", "7E80641A0FFFFFFFF", "7E80641C0FFFFFFFF", "7E80641E0FFFFFFFF", },
             //  new string[] { "7E806410000100000" },
            };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // For Each set of inputs, scenarios
            inputs.ForEach(strArray => {
                this.output.WriteLine("_________________");
                try
                {
                    var ecuList = (List<ECU>)OBD2Device.GetSupportedMonitorIds(strArray);
                    ecus.AddRange(ecuList);

                    Assert.True(ecus.Count() > 1, "Must have found at least two ECU ids in the array");

                    sb.Clear();

                    ecus.ForEach(ecu =>
                    {

                        sb.Append($"\nECU {ecu.Id.ToString("X3")}");
                        var supPids = ecu.SupportedPIDS;

                        ecu.SupportedPIDS.ForEach(pid =>
                        {
                            sb.Append($"{Environment.NewLine}  {pid},{pid.Value}");
                        });

                    });

                    this.output.WriteLine(sb.ToString());

                }
                catch (Exception ex)
                {
                    this.output.WriteLine($"Error: {ex.Message}");
                    Assert.Fail();

                }
                finally
                {
                    ecus.Clear();
                }


            });


        }


        //[Fact]
        public void MisfireData()
        {

            List<string> inputs = new List<string>()
            {
                "A10C000015000000FF",
                "A10C00F0150000FFFF"
            };
            var l = new OBD2MIDS();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // For Each set of inputs, scenarios
            this.output.WriteLine("_________________");
            inputs.ForEach(strArray => {
                try
                {

                    PID p = new PID((int)OBD2MIDS.MonitorIds.OBDMID_MisfireGeneralData, "New Test PID", "PID for testing", typeof(string), OBD2MIDS.UnitScalingDictionary[(int)UnitMeasure.MonitorTestResult]);
                    var c = (MonitorTestResult)p.Parse(strArray);

                    Assert.NotNull(c);

                    this.output.WriteLine($"{Environment.NewLine}{c}");
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