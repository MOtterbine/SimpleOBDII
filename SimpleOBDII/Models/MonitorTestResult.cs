using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class MonitorTestResult
    {
        private char[] data = new char[18];
        public static bool TryParse(string inputString, out MonitorTestResult result)
        {
            result = null;
            if (string.IsNullOrEmpty(inputString) || inputString.Length < 18) return false;
            var inData = inputString.ToCharArray();


            MonitorTestResult tr = new MonitorTestResult();
            try
            {
                for (int i = 0; i < 18; i++)
                {
                    tr.data[i] = inData[i];
                }
                result = tr;

                var scaleEntry = OBD2MIDS.UnitScalingDictionary[result.ScalingID];
                //var xul = (uint)scaleEntry.ValueFromInput((HexTable.FromHex(inData[6]) << 12) | (HexTable.FromHex(inData[7]) << 8) | (HexTable.FromHex(inData[8]) << 4) | (HexTable.FromHex(inData[9])));
                //tr.Value = (uint)scaleEntry.ValueFromInput(inputString.Substring(6, 4));
                //tr.MaxLimit = (uint)scaleEntry.ValueFromInput(inputString.Substring(10, 4));
                //tr.MinLimit = (uint)scaleEntry.ValueFromInput(inputString.Substring(14, 4));

                return true;
            }
            catch (Exception e)
            {
            }
            result = null;
            return false;
        }
        public uint MonitorID => (HexTable.FromHex(data[0]) << 4) | (HexTable.FromHex(data[1]));
        public uint TestID => (HexTable.FromHex(data[2]) << 4) | (HexTable.FromHex(data[3]));
        /// <summary>
        /// Calculation method
        /// </summary>
        public int ScalingID => Convert.ToInt32((HexTable.FromHex(data[4]) << 4) | (HexTable.FromHex(data[5])));
        public uint Value {get; set;}// (HexTable.FromHex(data[6]) << 12) | (HexTable.FromHex(data[7]) << 8) | (HexTable.FromHex(data[8]) << 4) | (HexTable.FromHex(data[9]));
        public uint MinLimit => (HexTable.FromHex(data[10]) << 12) | (HexTable.FromHex(data[11]) << 8) | (HexTable.FromHex(data[12]) << 4) | (HexTable.FromHex(data[13]));
        public uint MaxLimit => (HexTable.FromHex(data[14]) << 12) | (HexTable.FromHex(data[15]) << 8) | (HexTable.FromHex(data[16]) << 4) | (HexTable.FromHex(data[17]));

        public override string ToString()
        {
            return $"{MonitorID:X2}:{TestID:X2} - val:{Value}, max:{MaxLimit}, min:{MinLimit}";
        }

    }
}
