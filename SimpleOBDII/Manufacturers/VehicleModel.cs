
using OS.OBDII.Interfaces;


namespace OS.OBDII.Manufacturers
{
    public class VehicleModel : IVehicleModel
    {
        public Dictionary<uint, OBDIIFaultCode> FaultCodes { get; set; } = new Dictionary<uint, OBDIIFaultCode>();

        public virtual string Name { get; set; }

        public override string ToString() => this.Name;

        public VehicleModel()
        {

        }

    }

}
