//using OS.Communication;
using OS.OBDII.Communication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Interfaces
{
    public interface IBlueToothService :IDevicesService, ICommunicationDevice
    {
        event BluetoothEvent DeviceEvent;
    }



}
