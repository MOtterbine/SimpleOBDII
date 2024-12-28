using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public interface IRange
    {
        string MetricMaxRange { get; set; }
        string EnglishMaxRange { get; set; }
        string MetricMinRange { get; set; }
        string EnglishMinRange { get; set; }
    }

}
