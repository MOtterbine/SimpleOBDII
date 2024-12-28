
namespace OS.OBDII.Communication
{
    /// <summary>
    /// Provides functionality specific to serial device communication
    /// </summary>
    public interface ISerialDevice
    {
        uint BaudRate { get; set; }
    }
}
