using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public static class VinUtils
    {
        private static readonly Dictionary<char, double> VinCharacterValueMap = new Dictionary<char, double>();
        private static readonly Dictionary<double, char> VinCheckDigitValueMap = new Dictionary<double, char>();

        private static readonly int[] VINWeightFactors = { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };

        static VinUtils()
        {

            if (VinCharacterValueMap.Count > 0) return;

            VinCheckDigitValueMap.Add(0.0, '0');
            VinCheckDigitValueMap.Add(0.091, '1');
            VinCheckDigitValueMap.Add(0.182, '2');
            VinCheckDigitValueMap.Add(0.273, '3');
            VinCheckDigitValueMap.Add(0.364, '4');
            VinCheckDigitValueMap.Add(0.455, '5');
            VinCheckDigitValueMap.Add(0.545, '6');
            VinCheckDigitValueMap.Add(0.634, '7');
            VinCheckDigitValueMap.Add(0.727, '8');
            VinCheckDigitValueMap.Add(0.818, '9');
            VinCheckDigitValueMap.Add(0.909, 'X');

            //map = new Map<string, number>();
            VinCharacterValueMap.Add('0', 0);
            VinCharacterValueMap.Add('1', 1);
            VinCharacterValueMap.Add('2', 2);
            VinCharacterValueMap.Add('3', 3);
            VinCharacterValueMap.Add('4', 4);
            VinCharacterValueMap.Add('5', 5);
            VinCharacterValueMap.Add('6', 6);
            VinCharacterValueMap.Add('7', 7);
            VinCharacterValueMap.Add('8', 8);
            VinCharacterValueMap.Add('9', 9);
            VinCharacterValueMap.Add('A', 1);
            VinCharacterValueMap.Add('B', 2);
            VinCharacterValueMap.Add('C', 3);
            VinCharacterValueMap.Add('D', 4);
            VinCharacterValueMap.Add('E', 5);
            VinCharacterValueMap.Add('F', 6);
            VinCharacterValueMap.Add('G', 7);
            VinCharacterValueMap.Add('H', 8);
            // Shitaki.map.Add('I', 9);
            VinCharacterValueMap.Add('J', 1);
            VinCharacterValueMap.Add('K', 2);
            VinCharacterValueMap.Add('L', 3);
            VinCharacterValueMap.Add('M', 4);
            VinCharacterValueMap.Add('N', 5);
            VinCharacterValueMap.Add('O', 6);
            VinCharacterValueMap.Add('P', 7);
            //  Shitaki.map.Add('Q', 8);
            VinCharacterValueMap.Add('R', 9);
            VinCharacterValueMap.Add('S', 2);
            VinCharacterValueMap.Add('T', 3);
            VinCharacterValueMap.Add('U', 4);
            VinCharacterValueMap.Add('V', 5);
            VinCharacterValueMap.Add('W', 6);
            VinCharacterValueMap.Add('X', 7);
            VinCharacterValueMap.Add('Y', 8);
            VinCharacterValueMap.Add('Z', 9);

        }


        public static string ValidateVIN(string Vin)
        {
            double sum = 0;
            try
            {

                if (Vin.Length < 1) 
                {
                    return "{vincheck: true, reason: 'empty data'}";
                }
                var r = new System.Text.RegularExpressions.Regex("^[0-9a-zA-Z]{17}");
                if (!r.IsMatch(Vin))
                {
                    return "{vincheck: true, reason: 'pattern'}";
                }

                // var vUtil = this;// new VinUtils();

                //    console.log("Validating Vin: " + Vin)

                char c;
                int i = 0;

                // VIN HAS A CHECKSUM defined by DOT - 9th DIGIT IS CHECKSUM
                // National Highway Traffic Safety Administration - (49 CFR Part 565) - VIN Requrements
                for (i = 0; i < 17; i++)
                {
                    if (i == 8) i++;
                    c = Vin.ToCharArray()[i];
                    var calcVal = VinUtils.VinCharacterValueMap[c];
                    if (calcVal != float.NaN)
                    {
                        //console.log("char: " + c + ", Val: " + calcVal + ", Weight: " + vUtil.VINWeightFactors[i])
                        calcVal *= VinUtils.VINWeightFactors[i];
                        sum += calcVal;
                    }
                }
                //console.log("Total SUM: " + sum)
            }
            catch (System.Exception e)
            {
                return "{ vincheck: true, reason: 'Exception '" + e.Message + "' }";
            }

            var chkDigit = sum % 11;
            string cs = chkDigit == 10 ? "X" : chkDigit.ToString();

            try
            {
                if (Vin.Substring(8, 1) == cs)
                {
                    ///.WriteLine("Checksum Good");
                    return null;
                }
            }
            catch (System.Exception e)
            {
                return "{ vincheck: true, reason: 'Exception ' + e.message}";
            }

            return "{ vincheck: true, reason: 'Checksum Mismatch'}";

        }

    }
}
