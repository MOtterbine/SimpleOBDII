using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Models;

public class WifiInfoRecord
{
    public WifiTypeDescriptions WifiType { get; private set; }
    public WifiTypeInfo WifiInfo { get; private set; }

    public WifiInfoRecord(WifiTypeDescriptions wifiType,  WifiTypeInfo wifiInfo)
    {
        // validate the command
        //if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
       // if (string.IsNullOrEmpty(wifiInfo)) throw new ArgumentNullException("cmdString");
        this.WifiType = wifiType;
        this.WifiInfo = wifiInfo;
    }


    // what a listview will show
    public override string ToString() => this.WifiInfo.ToString();

}
