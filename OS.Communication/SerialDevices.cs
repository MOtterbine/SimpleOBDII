
using OS.Localization;

namespace OS.Communication
{
    public  class SerialDevices 
    {
        public static List<Tuple<string,int>> DeviceTypes => sTypes;

        private static readonly List<Tuple<string, int>> sTypes = new List<Tuple<string, int>>()
        {
            new Tuple<string,int>((string)LocalizationResourceManager.Instance["BLUETOOTH_USB"], 0),
            new Tuple<string,int>((string)LocalizationResourceManager.Instance["WIFI_TYPE_DESCRIPTOR"], 1),
        };

    }
}
