
namespace OS.OBDII.Models
{
    public class OBD2Command
    {
        private string name;
        private string commandstring;
        private OBD2ServiceMode servicemode = null;
        private DeviceRequestType requesttype = DeviceRequestType.None;
        public OBD2Command(string name, OBD2ServiceMode svcMode, DeviceRequestType requestType, string arguments = null)
        {
            this.name = name;
            this.servicemode = svcMode;
            this.requesttype = requestType;
            // var svcMode = ServiceModes.FirstOrDefault(sm => sm.Mode == serviceMode)?.Value; // should be the actual 2-character command string
            this.commandstring = OBD2Device.CreateRequest(svcMode, requestType, true, arguments);
        }
        
        public string Name => this.name;
        public string CommandString => this.commandstring;
        public DeviceRequestType RequestType => this.requesttype;
        public OBD2ServiceMode ServiceMode => this.servicemode;
    }
}
