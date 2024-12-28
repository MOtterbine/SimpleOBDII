using System;
using System.Collections.Generic;
using System.Text;
using OS.OBDII.Models;
using OS.OBDII.Manufacturers;

namespace OS.OBDII.Interfaces
{
    public interface IVehicleModel
    {
        string Name { get; }
        Dictionary<uint, OBDIIFaultCode> FaultCodes { get; }

    }
}
