
namespace OS.OBDII.Communication
{
    public class BaudRates : List<UInt32>
    {
        public new UInt32 this[int key] => Items[key];
            
        public static List<UInt32> Items { get; } = new List<UInt32>()
        { 1200, 2400, 4800, 9600, 19200, 28800, 38400, 57600, 76800, 115200, 230400 };

    }
}
