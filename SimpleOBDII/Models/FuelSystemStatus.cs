using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class FuelSystemStatus
    {
        public string Description { get; private set; }
        private uint RawValue = 0;
        public int FuelSystem { get; set; } = 0;
        public bool Active { get; private set; } = false;
        public bool MotorIsOff { get; private set; } = false;
        public bool OpenLoopDueToLowTemperature { get; private set; } = false;
        public bool ClosedLoop { get; private set; } = false;
        public bool OpenLoopDueToEngineLoadOrFuelCutFromDecel { get; private set; } = false;
        public bool OpenLoopDueToSystemFailure { get; private set; } = false;
        public bool ClosedLoopWithFeedbackFault { get; private set; } = false;

        public override string ToString()
        {
            return this.Description;
        }

        public string ApplyValue(uint statusValue)
        {
            this.RawValue = statusValue;
            
            switch (statusValue)
            {
                case 1:
                    this.Active = true;
                    this.OpenLoopDueToLowTemperature = true;
                    this.Description = "Open Loop";
                    break;
                case 2:
                    this.Active = true;
                    this.ClosedLoop = true;
                    this.Description = "Closed Loop";
                    break;
                case 4:
                    this.Active = true;
                    this.OpenLoopDueToEngineLoadOrFuelCutFromDecel = true;
                    this.Description = "Open Loop -Drive";
                    break;
                case 8:
                    this.Active = true;
                    this.OpenLoopDueToSystemFailure = true;
                    this.Description = "Open Loop - Fault";
                    break;
                case 16:
                    this.Active = true;
                    this.ClosedLoopWithFeedbackFault = true;
                    this.Description = "Closed Loop - Fault";
                    break;
                default:
                    this.Active = false;
                    this.Description = "N/A";
                    break;
            }
            return this.Description;

        }
    }
}
