
namespace OS.Communication;

/// <summary>
/// Provides functionality specific to serial device communication
/// </summary>
public interface ISerialDevice
{
    uint BaudRate { get; set; }
    // Data Terminal Ready
    bool DTR { get; set; }
    // Request To Send
    bool RTS { get; set; }
}
