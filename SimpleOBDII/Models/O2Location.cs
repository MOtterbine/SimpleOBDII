using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class O2Location
    {
        public O2Location(string sName, string lName = null)
        {
            this.ShortName = sName;
            if (string.IsNullOrEmpty(lName)) this.LongName = sName;
        }
        public string ShortName { get; set; }
        public string LongName { get; set; }


        // List items are in a specific order based on PIDs 0x13 (bits 0-7) and 0x1D (bits 8-15)
        public static List<O2Location> Locations => new List<O2Location>()
        {
            new O2Location("O2S11", "O2 Bank 1 Sensor 1"),
            new O2Location("O2S12", "O2 Bank 1 Sensor 2"),
            new O2Location("O2S13", "O2 Bank 1 Sensor 3"),
            new O2Location("O2S14", "O2 Bank 1 Sensor 4"),
            new O2Location("O2S21", "O2 Bank 2 Sensor 1"),
            new O2Location("O2S22", "O2 Bank 2 Sensor 2"),
            new O2Location("O2S23", "O2 Bank 2 Sensor 3"),
            new O2Location("O2S24", "O2 Bank 2 Sensor 4"),
            new O2Location("O2S11", "O2 Bank 1 Sensor 1"),
            new O2Location("O2S12", "O2 Bank 1 Sensor 2"),
            new O2Location("O2S21", "O2 Bank 2 Sensor 1"),
            new O2Location("O2S22", "O2 Bank 2 Sensor 2"),
            new O2Location("O2S31", "O2 Bank 3 Sensor 1"),
            new O2Location("O2S32", "O2 Bank 3 Sensor 2"),
            new O2Location("O2S41", "O2 Bank 4 Sensor 1"),
            new O2Location("O2S42", "O2 Bank 4 Sensor 2")
        };
    }


}
