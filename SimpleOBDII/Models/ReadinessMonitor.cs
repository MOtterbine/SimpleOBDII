using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class ReadinessMonitor
    {
        public ReadinessMonitor()
        {
            this.IsCompleted = false;
            this.IsSupported = false;
            this.Description = this.Code.ToString();
        }
        public int Code { get; set; }
        public int SiblingCode { get; set; }
        public ulong BitMask { get; set; }
        public string Description { get; set; }
        public bool IsSupported { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFault { get; set; }
        public String Status
        {
            get
            {
                return IsCompleted ? "Complete" : (IsFault ? "Fault" : "Not Ready");
            }
        }
    }
}
