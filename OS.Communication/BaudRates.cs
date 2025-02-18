
namespace OS.Communication
{
    public class BaudRates : List<UInt32>
    {
        public new UInt32 this[int key] => Items[key];

        public static List<UInt32> Items { get; } = new List<UInt32>()
        { 1200, 2400, 4800, 9600, 19200, 28800, 38400, 57600, 76800, 115200, 230400 };

    }
    public class ISOBaudRates : Dictionary<UInt32, string>
    {
        public new string this[int key] => Items[(uint)key];

        /// <summary>
        /// Use of a dictionary to look up ELM327 AT Command by ISO baud rate
        /// </summary>
        public static Dictionary<UInt32, string> Items { get; } = new Dictionary<UInt32, string>()
        {  { 4800 , "48"},
           { 9600 , "96"}, 
           { 10400 , "10" },
           { 12500 , "12" },
           { 15625 , "15" }
        };

    }
}
