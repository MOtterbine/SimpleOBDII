using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class HexTable
    {
        public static uint FromHex(byte byteIn)
        {
            return Table[byteIn];
        }
        public static uint FromHex(char charIn)
        {
            try
            {
                return Table[Convert.ToByte(charIn)];
            }
            catch (Exception) { return 0; }
        }
        private static Dictionary<byte, uint> Table = new Dictionary<byte, uint>()
        {
            {0x30, 0x00},
            {0x31, 0x01},
            {0x32, 0x02},
            {0x33, 0x03},
            {0x34, 0x04},
            {0x35, 0x05},
            {0x36, 0x06},
            {0x37, 0x07},
            {0x38, 0x08},
            {0x39, 0x09},
            {0x41, 0x0A},
            {0x42, 0x0B},
            {0x43, 0x0C},
            {0x44, 0x0D},
            {0x45, 0x0E},
            {0x46, 0x0F},
            {0x61, 0x0A},
            {0x62, 0x0B},
            {0x63, 0x0C},
            {0x64, 0x0D},
            {0x65, 0x0E},
            {0x66, 0x0F}
        };
    }
}
