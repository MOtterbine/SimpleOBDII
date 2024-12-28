using Xunit.Abstractions;
using Xunit.Sdk;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using OS.OBDII.Models;
using OS.OBDII.Interfaces;

namespace PrimaryTests
{
    public class ScalarSensors
    {
        private readonly ITestOutputHelper output;

        public ScalarSensors(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void VINParse()
        {
            string VIN = "";
            List<string[]> vinTypes = new List<string[]>()
            {
                //new string[] { "014\r0:490201314434\r1:47503030523535\r2:42313233343536\r" },
                new string[] { "014", "0:490201314434", "1:47503030523535", "2:42313233343536" },
                new string[] { "49020100000031", "49020244344750", "49020330305235", "49020435423132", "49020533343536" },
                new string[] { "014", "0:490201324731", "1:57533538315836", "2:39333633323635" },
                new string[] { "49020100000032", "49020247315753", "49020335383158", "49020436393336", "49020533323635" }
            };

            vinTypes.ForEach(vin => {
                try
                {
                    VIN = (string)OBD2Device.ELM327CommandDictionary[DeviceRequestType.OBD2_GetVIN].function(vin);
                    this.output.WriteLine($"VIN: {VIN}");
                }
                catch (Exception ex)
                {
                    this.output.WriteLine($"Error: {ex.Message}");
                    Assert.Fail();

                }

                Assert.True(!string.IsNullOrEmpty(VIN), "VIN cannot be null or empty.");
                Assert.Equal(17, VIN.Length);


            });

        }


        [Fact]
        public void CustomPIDs()
        {

            ObservableCollection<IPid> inputs = new ObservableCollection<IPid>()
            {
                new UserPID("Temperature", "°F", 1, new byte[]{ 0x00}, "((A-40)*1.8)+32"),
                new UserPID("Temperature", "°F", 1, new byte[]{ 0x00}, "((c-40)*1.8)+32"),
                new UserPID("Offset Percent", "%", 0, new byte[]{ 0x00}, "((b-128)*100)/255"),
                new UserPID("Offset Percent", "%", 0, new byte[]{ 0x00}, "((c-128)*100)/255"),
                new UserPID("Percent", "%", 0, new byte[]{ 0x00}, "(e*100)/255"),
                new UserPID("Count", "Count", 0, new byte[]{ 0x00}, "a*256 + b"),
                new UserPID("Percent", "%", 0, new byte[]{ 0x00}, "(a*256*100 + b)/65535"),
                new UserPID("True/False", string.Empty, 0, new byte[]{ 0x00}, "a"),
                //new UserPID("True/False", "", 0, new byte[]{ 0x00}, "(a>=64)")
            };

            //object inputByte = "0080FF403788";
            object[] inputByte = new object[]
            {
                "4880FF24303a",
                "4880FF24303a",
                "75154e4A303a",
                "4880FF24303a",
                "4880FF24303a",
                "FFFF",
                "3F4F",
                "B880FF24303a",
                "B880FF24303a",

            };

            int inputIdx = 0;
            this.output.WriteLine($"______________________{Environment.NewLine}");
            inputs.ToList().ForEach(customPID =>
            {
                //customPID.Parse((string)inputByte[inputIdx++]);
                this.output.WriteLine($"{customPID.Name}: {customPID.OutputString}");
            });
        }



        [Fact]
        public void CATtemp()
        {

            List<string> inputs = new List<string>()
            {
                "0000",
                "08FF",
                "0580",
                "026F"
            };
            var l = new OBD2PIDS();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // For Each set of inputs, scenarios
            inputs.ForEach(strArray => {
                this.output.WriteLine("_________________");
                try
                {
                    PID<double> p = new PID<double>(0x0000, "New Test PID", "PID for testing", OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.CatalystTemp]);
                    var c = p.Parse(strArray);

                    Assert.NotNull(c);
                    
                    this.output.WriteLine($"{Environment.NewLine}Cat temp: {c:F2} °C");
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


        [Fact]
        public void OffsetPercent()
        {

            List<string> inputs = new List<string>()
            {
                "00",
                "79",
                "85",
                "FE"
            };
                     var l = new OBD2PIDS();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // For Each set of inputs, scenarios
            inputs.ForEach(strArray => {
                try
                {

                    var ump =  (double)OBD2PIDS.UnitMeasuresDictionary[UnitMeasure.OffsetPercent].ValueFromInput(strArray);
                    this.output.WriteLine($"{strArray} - {ump:f1} %");

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


        [Fact]
        public void IntakeAirTempParse()
        {
            //Func<object, object> pidFunction = null;
            var l = new OBD2PIDS();

            List<string> inputs = new List<string>()
            {
                "00",
                "FF",
                "50",
                "90"
            };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // For Each set of inputs, scenarios
            inputs.ForEach(str => {
                try
                {
                    PID p = OBD2PIDS.PIDSDictionary[OBD2PIDS.PIDS.OBD2_IAT];

                    var temperature = p.Parse(str);

                    Assert.NotNull(temperature);

                    this.output.WriteLine($"{temperature:F1}F°");

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


        [Fact]
        public void RPMParse()
        {
            //Func<object, object> pidFunction = null;
            var l = new OBD2PIDS();

            List<string> inputs = new List<string>()
            {
                "0000",
                "7FFF",
                "2450",
                "00A2",
                "0890"
            };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // For Each set of inputs, scenarios
            inputs.ForEach(str => {
                try
                {
                    var rpm = l[OBD2PIDS.PIDS.OBD2_RPM].function(str);

                    Assert.NotNull(rpm);


                    //var a = Decimal.Round(122.5m, 1);
                    //var b = 122m;
                    //Assert.IsTrue(a == b, $"*** RPM ({a}) should be an integer.");

                    //Assert.IsTrue(a == b, $"*** RPM ({Decimal.Round((uint)rpm, 0)}) should be an integer.");


                    this.output.WriteLine($"{rpm} rpm");

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