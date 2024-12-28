using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OS.OBDII.Manufacturers;

namespace OS.OBDII.Models
{
    [Serializable]
    public class DTCGroup : ObservableCollection<OBDIIFaultCode>
    {
        public DTCGroup() { }
        public DTCGroup(string groupName)
        {
            this.Name = groupName;
        }

        private ObservableCollection<OBDIIFaultCode> dtcGroups = new ObservableCollection<OBDIIFaultCode>();

        public DTCGroup(string groupName, ObservableCollection<OBDIIFaultCode> group)
        {
        //    this.CollectionChanged += OnCollectionChanged;
            this.Name = groupName;
            this.dtcGroups.Clear();
            group.ToList().ForEach(g => {
                this.dtcGroups.Add(g);
            });
            

        }

        //[JsonIgnore]
        public bool IsEmpty => this.Count < 1;
        
        public string Name { get; set; }
    }
}
