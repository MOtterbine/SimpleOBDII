using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class OBD2ServiceMode
    {
        public OBD2ServiceMode(int mode, string value, string description)
        {
            this.Mode = mode;
            this.Value = value;
            this.Description = description;
        }
        public int Mode { get; }
        public string Value { get; }
        public string Description { get; }

    }
}
