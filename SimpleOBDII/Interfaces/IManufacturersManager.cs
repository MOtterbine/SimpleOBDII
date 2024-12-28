using System;
using System.Collections.Generic;
using System.Text;
using OS.OBDII.Models;

namespace OS.OBDII.Interfaces
{
    public interface IManufacturersManager
    {
        bool ManufacturersAreLoaded { get; }
        event EventHandler ManufacturersLoaded;
        List<IVehicleModel> VehicleModels { get; set; }
    }
}
