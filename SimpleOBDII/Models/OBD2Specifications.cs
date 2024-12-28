using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
//using static Android.Net.LocalSocketAddress;

namespace OS.OBDII.Models
{
    public class OBD2Specification
    {
        public override string ToString()
        {
            return this.Description;
        }
        public OBD2Specification(uint value, string description)
        {
            this.Value = value;
            this.Description = description;
        }
        public string Description { get; }
        public uint Value { get; }

        public static OBD2Specification GetSpecification(uint value)
        {
            return OBDSpecifications.ToList().FirstOrDefault(spec => spec.Value == value);
        }
        //  
        private static readonly List<OBD2Specification> OBDSpecifications = new List<OBD2Specification>()
        {
            new OBD2Specification(0x01, "OBD II"),
            new OBD2Specification(0x02, "OBD"),
            new OBD2Specification(0x03, "OBD I and OBD II"),
            new OBD2Specification(0x04, "OBD I"),
            new OBD2Specification(0x05, "No OBD Support"),
            new OBD2Specification(0x06, "EOBD"),
            new OBD2Specification(0x07, "EOBD and OBD II"),
            new OBD2Specification(0x08, "EOBD and OBD"),
            new OBD2Specification(0x09, "EOBD, OBD and OBD II"),
            new OBD2Specification(0x0A, "JOBD"),
            new OBD2Specification(0x0B, "JOBD and OBD II"),
            new OBD2Specification(0x0C, "JOBD and EOBD"),
            new OBD2Specification(0x0D, "JOBD, EOBD and OBD II"),
            new OBD2Specification(0x0E, "Heavy Duty Vehicles (EURO IV) B1"),
            new OBD2Specification(0x0F, "Heavy Duty Vehicles (EURO IV) B1"),
            new OBD2Specification(0x10, "Heavy Duty Vehicles (EURO EEV) C"),
            new OBD2Specification(0x11, "EMD"),
            new OBD2Specification(0x12, "EMD+"),
            new OBD2Specification(0x13, "HD OBD-C"),
            new OBD2Specification(0x14, "HD OBD"),
            new OBD2Specification(0x15, "WWH OBD"),
            new OBD2Specification(0x16, "SAE/ISO reserved"),
            new OBD2Specification(0x17, "HD EOBD-I"),
            new OBD2Specification(0x18, "HD EOBD-I N"),
            new OBD2Specification(0x19, "HD EOBD-II"),
            new OBD2Specification(0x1A, "HD EOBD-II N"),
            new OBD2Specification(0x1B, "ISO/SAE reserved"),
            new OBD2Specification(0x1C, "OBDBr-1"),
            new OBD2Specification(0x1D, "OBDBr-2")
        };
    }


}
