using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Models
{
    public class WifiTypes
    {
        public static readonly List<WifiInfoRecord> WifiTypesList = new List<WifiInfoRecord> {
            new WifiInfoRecord(WifiTypeDescriptions.AccessPoint, new WifiTypeInfo(){ Description ="Acccess Point", WifiMode = WifiTypeDescriptions.AccessPoint }),
            new WifiInfoRecord(WifiTypeDescriptions.NetworkClient, new WifiTypeInfo(){Description ="Network Client", WifiMode = WifiTypeDescriptions.NetworkClient })
            //new WifiInfoRecord(WifiTypeDescriptions.AccessPoint, new WifiTypeInfo(){Description ="Internet of Things", WifiMode = WifiTypeDescriptions.Iot })
        };
    }

    public class WifiTypeInfo
    {
        public String Description {get; set;}
        public WifiTypeDescriptions WifiMode { get; set; }
        public override string ToString() => Description;
    }

    public enum WifiTypeDescriptions
    {
        NetworkClient = 0x00,
        AccessPoint = 0x01,
        Iot = 0x03
    }
}
