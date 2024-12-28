using Xunit.Abstractions;
using Xunit.Sdk;
using OS.OBDII.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.IO;
using OS.OBDII.Interfaces;

namespace PrimaryTests;

public class UserPIDs
{


    private readonly ITestOutputHelper output;

    public UserPIDs(ITestOutputHelper output)
    {
        this.output = output;
    }



    public class TestPIDManager : IPIDManager
    {
        private const string JsonFileName = "userpids.json";
        private const string userTestsFileName = "usertests.json";
        public void SaveUserPIDs(IList<UserPID> pids)
        {
            // not implemented...
        }
        public IList<UserPID> LoadUserPIDs()
        {
            string jsonData = File.ReadAllText(JsonFileName);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<IList<UserPID>>(jsonData);
        }

        public IList<ActiveTest> LoadActiveTests()
        {

            string jsonData = File.ReadAllText(userTestsFileName);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<IList<ActiveTest>>(jsonData);
        }

        public void SaveActiveTests(IList<ActiveTest> tests)
        {
            // not implemented...
        }


    }

    [Fact]
    public void LoadUserPIDs()
    {
        int TotalPIDCount = 10; // the test file 'userpids.json' json pid entry count
        OBD2Device.SystemReport = new SystemReport(new TestPIDManager());

        Assert.NotNull(OBD2Device.SystemReport);
        var report = OBD2Device.SystemReport;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        try
        {
            Assert.True(report.LoadUserPIDs(), "Call without parameters should return all");
            Assert.Equal(TotalPIDCount, report.UserPIDs.Count);
            this.output.WriteLine($"Load all pids by default (no parameters) - Loaded {report.UserPIDs.Count} User Pids.");

            uint tstOffset = 0;
            int tstCount = 3;
            Assert.True(report.LoadUserPIDs(tstOffset, tstCount), "Call to load pids must succeed");
            Assert.Equal(tstCount, report.UserPIDs.Count);
            this.output.WriteLine($"Call to load pids - Loaded {report.UserPIDs.Count} User Pids from offset {tstOffset}.");

            tstCount = 1;
            Assert.True(report.LoadUserPIDs(tstOffset, tstCount), "Call to load single, first pid must succeed");
            Assert.Equal(tstCount, report.UserPIDs.Count);
            this.output.WriteLine($"Load the first pid - Loaded {report.UserPIDs.Count} User Pids from offset {tstOffset}.");


            // Beyond the limit...but should return all
            tstOffset = 0;
            tstCount = 100;
            Assert.True(report.LoadUserPIDs(tstOffset, tstCount), "Call for too many pids must succeed returning all");
            Assert.Equal(TotalPIDCount, report.UserPIDs.Count);
            this.output.WriteLine($"Request for more pids than exist - Loaded {report.UserPIDs.Count} User Pids from offset {tstOffset}.");

            // Anything negative should return None

            tstOffset = 0;
            tstCount = -1;
            Assert.False(report.LoadUserPIDs(tstOffset, tstCount), "Call to load -1 count of pids must return false");
            //Assert.Equal(0, report.UserPIDs.Count, "Specified pid count not returned");
            //this.output.WriteLine($"Call -1 amount of pids - Loaded {report.UserPIDs.Count} User Pids from offset {tstOffset}.");

            tstCount = -481; // random negative value
            Assert.False(report.LoadUserPIDs(tstOffset, tstCount), "Call to load any negative count of pids must return false");
            //Assert.Equal(0, report.UserPIDs.Count, "Specified pid count not returned");
            //this.output.WriteLine($"Call negative amount of pids - Loaded {report.UserPIDs.Count} User Pids from offset {tstOffset}.");


            tstCount = -1;
            Assert.False(report.LoadUserPIDs(tstOffset, tstCount), "Call to load -1 count of pids must return false");
            //Assert.Equal(0, report.UserPIDs.Count, "Specified pid count not returned");
            //this.output.WriteLine($"Call -1 amount of pids - Loaded {report.UserPIDs.Count} User Pids from offset {tstOffset}.");

            tstCount = -481; // random negative value
            Assert.False(report.LoadUserPIDs(tstOffset, tstCount), "Call to load any negative count of pids must return false");
            //Assert.Equal(0, report.UserPIDs.Count, "Specified pid count not returned");
            //this.output.WriteLine($"Call negative amount of pids - Loaded {report.UserPIDs.Count} User Pids from offset {tstOffset}.");

            // internal renumbering test, take a section , but codes should increment from zero
            tstOffset = 3;
            tstCount = 5;
            Assert.True(report.LoadUserPIDs(tstOffset, tstCount), "Call to load pids must succeed");
            Assert.Equal(1u, report.UserPIDs[1].Code);
            this.output.WriteLine($"Offset code value test - Loaded {report.UserPIDs.Count} pids starting at offset {tstOffset}");

            tstOffset = 1;
            tstCount = 7;
            Assert.True(report.LoadUserPIDs(tstOffset, tstCount), "Call to load pids must succeed");
            Assert.Equal(3u, report.UserPIDs[3].Code);
            this.output.WriteLine($"Offset code value test - Loaded {report.UserPIDs.Count} pids starting at offset {tstOffset}");

        }
        catch (Exception ex)
        {
            this.output.WriteLine($"Error: {ex.Message}");
            Assert.Fail();
        }
        finally
        {
        }

    }

}