
namespace OS.Communication;

public interface ICommunicationDevice
{

    event DeviceEvent CommunicationEvent;
    bool IsConnected { get; }
    string Description { get; }
    string DeviceName { get; set; }
    bool Open(string channel);
    Task<bool> Send(string text);
    Task<bool> Send(byte[] buffer, int offset, int count);
    bool Close();

}
