using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class CANDevice
    {

        public static int bytesExpected = 0;
        public static int bytesToRead = 0;
        /// <summary>
        /// This method assumes headers are on
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ParseMultiLineResponse(object obj)
        {
            string[] strArray = (string[])obj;
            int objectByteSize = 6;
            var byteCnt = 0;
            var tmpByteCnt = 0;
            var lineStatus = 0;

            string sBuf = "";
            string str;
            int objectCount = 0;
            int tmpObjectCount = 0;
            int objectByteCount = 0;
            int tmpObjectByteCount = 0;
            int objectBytesRead = 0;

            // Make one long string...
            foreach (string s in strArray)
            {
                // multi-line
                if (s.Length >= OBD2Device.DataPositionOffset)
                {

                    int.TryParse(s.Substring(OBD2Device.DataPositionOffset - 2, 2), System.Globalization.NumberStyles.HexNumber, null, out lineStatus);
                    switch (lineStatus)
                    {
                        case 0x10: // first line - get total bytes
                            int.TryParse(s.Substring(OBD2Device.DataPositionOffset, 2), System.Globalization.NumberStyles.HexNumber, null, out bytesToRead);
                            break;
                        default:
                            if (bytesToRead >= s.Length - OBD2Device.DataPositionOffset)
                            {
                                sBuf += s.Substring(OBD2Device.DataPositionOffset);
                                byteCnt += s.Length - OBD2Device.DataPositionOffset;
                                bytesToRead -= s.Length - OBD2Device.DataPositionOffset;
                            }
                            else
                            {
                                sBuf += s.Substring(OBD2Device.DataPositionOffset, bytesToRead);
                                byteCnt += bytesToRead;
                                 return true;
                            }
                            break;

                    }
                }
            }

            return false;
        }
    }
}
