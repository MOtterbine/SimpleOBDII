using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class ECU
    {
        public List<PID> SupportedPIDS { get; private set; } = new List<PID>();
        public List<PID> SupportedTests { get; private set; } = new List<PID>();
        public uint Id { get; private set; }

        public ECU(uint id)
        {
            this.Id = id;
        }

        public bool AddSupportedPID(PID pid)
        {
            if (this.SupportedPIDS.Contains(pid) || !pid.IsVisible)
            {
                return false;
            }
            this.SupportedPIDS.Add(pid);
            return true;
        }

        public bool AddSupportedTest(PID pid)
        {
            if (this.SupportedTests.Contains(pid) || !pid.IsVisible)
            {
                return false;
            }
            this.SupportedTests.Add(pid);
            return true;
        }

    }
}
