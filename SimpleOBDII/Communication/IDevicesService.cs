
namespace OS.OBDII.Communication
{
    /// <summary>
    /// Provides a list of available devices
    /// </summary>
    public interface IDevicesService : ICommunicationDevice
    {
        IList<string> GetDeviceList();
        bool IsEnabled { get; }
    }



}
