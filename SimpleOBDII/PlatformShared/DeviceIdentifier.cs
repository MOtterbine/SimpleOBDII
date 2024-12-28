using OS.OBDII.Interfaces;
using System.Net.NetworkInformation;

namespace OS.OBDII.PartialClasses;
/// <summary>
/// For Windows
/// </summary>
public partial class DeviceIdentifier : IDeviceId
{
    public string UID { get; private set; } = String.Empty;
}
