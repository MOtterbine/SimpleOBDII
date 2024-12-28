using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class RangeDefinition
    {
        public double Maximum_Metric { get; set; } = double.NaN;
        public double Maximum_English { get; set; } = double.NaN;

        public double Minimum_Metric { get; set; } = double.NaN;
        public double Minimum_English { get; set; } = double.NaN;


    }
}
