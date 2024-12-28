using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class Protocol
    {
        public Protocol(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        public string Name { get; set; }
        public int Id { get; set; }

    }
}
