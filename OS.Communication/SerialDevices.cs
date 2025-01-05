
namespace OS.Communication
{
    public  class SerialDevices 
    {
        public static List<string> DeviceTypes => sTypes;

        private static readonly List<string> sTypes = new List<string>()
        {
           Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR,
            Constants.PREFS_WIFI_TYPE_DESCRIPTOR
        };

    }
}
