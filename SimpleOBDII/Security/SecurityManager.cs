using OS.OBDII;
using Force.Crc32;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace OS.OBDII
{
    public class SecurityManager
    {

        private string userUnit = String.Empty;
        private const string tosup1 = "50EF5D0A-F009-4E99-AAE7-2C25A4317D88";
        private const string tosup2 = "F0FE389E-A91E-4FE9-BC67-55E331E26BFF";

        public SecurityManager(String input)
        {
            if(string.IsNullOrEmpty(input))
            {
                throw new NullReferenceException("input cannot be empty or null");
            }
            this.userUnit = input;
        }
        /// <summary>
        /// Determines whether a license has been applied
        /// </summary>
        /// <returns></returns>
        public bool ValidateApplication()
        {
            // Is there any hash key?
            if(!Preferences.Default.ContainsKey(Constants.PREFS_KEY_APPLICATION_REGISTRATION_ANSWER))
            {
                return false;
            }
            
            // Get the  App's stored guid and the stored hash
            var storedHash = Preferences.Get(Constants.PREFS_KEY_APPLICATION_REGISTRATION_ANSWER, string.Empty);

            // basic validation
            if(userUnit.Length < 12 || storedHash.Length < 8)
            {
                return false;
            }

            // Calculate the stored hash
            var calcHash = GetCRCStringFromString(userUnit);

            // Return true if calculated hash is the same as the stored value
            var r = string.Compare(storedHash, calcHash) == 0;
            return r;

        }


        public string GetAppId() => userUnit;
        public string GetStoredHash() => Preferences.Get(OS.OBDII.Constants.PREFS_KEY_APPLICATION_REGISTRATION_ANSWER, string.Empty);

        public static string ComputeChecksum(string s)
        {
            uint ch = 0x0;
            var b = Encoding.ASCII.GetBytes(s);
            foreach (byte bt in b)
            {
                ch += bt;
            }
            return Convert.ToString(ch, 16).ToUpper();

        }

        public static string GetCRCStringFromString(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            uint r = 0;
            int len = data.Length;
            r = ComputeCRC(Encoding.ASCII.GetBytes(data), 0, len);
            return Convert.ToString(r, 16).ToUpper();
        }

        public static uint ComputeCRC(byte[] data, int offset, int length)
        {
            // original data
            uint s1 = Crc32Algorithm.Append(0, data, offset, length);
            // first iteration
            uint s2 = Crc32Algorithm.Append(s1, Encoding.ASCII.GetBytes(tosup1));
            // second iteration
            return Crc32Algorithm.Append(s2, Encoding.ASCII.GetBytes(tosup2));
        }

        public string CRCForFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            uint r = 0;
            using (var s = file.OpenRead())
            {
                byte[] buff = new byte[10240];
                int len = s.Read(buff, 0, buff.Length);
                r = Crc32Algorithm.Compute(buff, 0, len);
            }
            return Convert.ToString(r, 16).ToUpper();
        }

    }
}
