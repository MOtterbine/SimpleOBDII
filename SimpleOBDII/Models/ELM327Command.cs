using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
//using System.Threading.Tasks;
//using OS.Communication;
//using System.Xml.Serialization;


namespace OS.OBDII.Models
{

	public class ELM327Command
	{
		public DeviceRequestType RequestType { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsUserFunction { get; set; } = true;
		public Func<object, object> function { get; set; }
        public override string ToString()
        {
            return $"{Name} - {Description}";
        }

    }

}
