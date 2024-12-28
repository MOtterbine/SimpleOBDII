using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OS.OBDII.Models
{
    public class Readiness
    {
        public int DTCCount { get; set; }
        public bool IsSparkSystem { get; set; }
        public ObservableCollection<ReadinessMonitor> MonitorTestList { get; set; } = new ObservableCollection<ReadinessMonitor>();

        public void Clear()
        {
            this.MonitorTestList.Clear();
            this.DTCCount = 0;
        }
        public override string ToString()
        {
            return $"{(IsSparkSystem?"Spark System":"Compression System")}";
        }
    }
}
