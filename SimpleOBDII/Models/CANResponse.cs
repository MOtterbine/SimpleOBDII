using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class CANResponse
    {
        public CANResponse(uint val, string code, string decription)
        {
            this.Value = val;
            this.Code = code;
            this.Description = decription;
        }
        public uint Value { get; }
        public string Code { get; }
        public string Description { get; }
    }
}
