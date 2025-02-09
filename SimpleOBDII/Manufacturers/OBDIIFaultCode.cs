using OS.OBDII.Interfaces;
using OS.OBDII.ViewModels;
using OS.OBDII.Models;
using Newtonsoft.Json;

namespace OS.OBDII.Manufacturers
{
    public class OBDIIFaultCode : BaseViewModel
    {
        public OBDIIFaultCode() { }


        public OBDIIFaultCode(uint dtcValue, IVehicleModel manufacturer, string codeNotFoundMessage = "Reported by ECU")
        {
            if (manufacturer == null) throw new ArgumentNullException("manufacturer cannot be null");
           
            OBDIIFaultCode code = null;
            if (manufacturer.FaultCodes.TryGetValue(dtcValue, out code))
            {
                this.Description = code.Description;
            }
            else
            {
                this.Description = $"{OBD2Device.TranslateToString(dtcValue)} {codeNotFoundMessage}";
            }
            this.PIDValue = dtcValue;
            this.PIDName = OBD2Device.TranslateToString(dtcValue);
            this.Prefix = this.PIDName.ToCharArray()[0];

        }

        public OBDIIFaultCode(uint pidValue)
        {
            this.PIDValue = pidValue;
            this.PIDName = OBD2Device.TranslateToString(pidValue);
            this.Description = $"{this.PIDName}";
            this.Prefix = this.PIDName.ToCharArray()[0];
        }
        public OBDIIFaultCode(OBDIIFaultCode fault)
        {
            this.Description = fault.Description;
            this.PIDName = fault.PIDName;
            this.PIDValue = fault.PIDValue;
        }
        public string Description { get; set; }
        public uint PIDValue { get; set; }
        public string PIDName { get; set; }
        public char Prefix { get; set; }
        [JsonIgnore]
        public bool IsFreezeFramePID
        {
            get => this.isFreezeFramePID;
            set => SetProperty(ref isFreezeFramePID, value);
        }
        private bool isFreezeFramePID = false;

        public override string ToString()
        {
            return this.PIDName;
        }

    }
}
