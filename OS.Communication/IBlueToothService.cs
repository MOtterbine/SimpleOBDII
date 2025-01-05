using OS.Communication;

namespace OS.OBDII.Interfaces
{
    public interface IBlueToothService :IDevicesService, ICommunicationDevice
    {
        event BluetoothEvent DeviceEvent;
    }

}
