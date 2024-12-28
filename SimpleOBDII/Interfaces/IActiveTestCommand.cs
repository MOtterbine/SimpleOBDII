using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OS.OBDII.Interfaces
{
    public interface IActiveTestCommand
    {
        string CANID { get; set; }
        int ResponseByteCount { get; set; }
        int DecimalPlaces { get; set; }
        bool IsSelected { get; set; }
        byte[] QueryBytes { get; }
        string CommandString { get; set; }

    }

}
