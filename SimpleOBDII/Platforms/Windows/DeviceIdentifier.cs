using Microsoft.Maui.Controls.PlatformConfiguration;
using OS.OBDII.Interfaces;
using System.Net.NetworkInformation;

namespace OS.OBDII.PartialClasses;
/// <summary>
/// For Windows
/// </summary>
public partial class DeviceIdentifier : IDeviceId
{

    public DeviceIdentifier()
    {
        try
        {
            UID = this.GetDefaultMacAddress().ToUpper();
        }
        catch (Exception e)
        {
            // couldn't resolve a hardware id, so we create a per-installation id
            UID = Guid.NewGuid().ToString();
        }
        finally
        {
            if(string.IsNullOrEmpty(UID)) UID = Guid.NewGuid().ToString();
        }
    }

    public string GetDefaultMacAddress()
    {
        Dictionary<string, long> macAddresses = new Dictionary<string, long>();
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus == OperationalStatus.Up)
                macAddresses[nic.GetPhysicalAddress().ToString()] = nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived;
        }
        long maxValue = 0;
        string mac = "";
        foreach (KeyValuePair<string, long> pair in macAddresses)
        {
            if (pair.Value > maxValue)
            {
                mac = pair.Key;
                maxValue = pair.Value;
            }
        }
        return mac;
    }
}
