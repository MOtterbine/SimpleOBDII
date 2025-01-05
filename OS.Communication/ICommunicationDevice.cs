
namespace OS.Communication;

public interface ICommunicationDevice
{

    #region Events

    event DeviceEvent CommunicationEvent;

    #endregion Events


    bool IsConnected { get; }

    /// <summary>
    /// General description
    /// </summary>
    string Description
    {
        get;
    }
    /// <summary>
    /// Device Name or Model...
    /// </summary>
    string DeviceName
    {
        get;
        set;
    }

    bool Open(string channel);
    Task<bool> Send(string text);
    Task<bool> Send(byte[] buffer, int offset, int count);
    /// <summary>
    /// Method is used to close the entire channel (client or listener)
    /// </summary>
    /// <returns></returns>
    bool Close();

}
