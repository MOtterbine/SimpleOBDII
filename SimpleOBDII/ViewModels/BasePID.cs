using Newtonsoft.Json;
using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OS.OBDII.ViewModels
{
    public abstract class BasePID<T> : BaseViewModel, IPid
    {
        protected DataTable dt = new DataTable() { Columns = {
            new DataColumn("A", typeof(double)),
            new DataColumn("B", typeof(double)),
            new DataColumn("C", typeof(double)),
            new DataColumn("D", typeof(double)),
            new DataColumn("E", typeof(double)),
            new DataColumn("F", typeof(double)),
            new DataColumn("G", typeof(double)),
            new DataColumn("H", typeof(double)),
            new DataColumn("exp", typeof(double))
            },
            Rows = { { 0, 0, 0, 0, 0, 0, 0, 0, 0 } }

        };


        // public RangeDefinition Range { get; } = new RangeDefinition();
        private bool isBeingDragged;
        [JsonIgnore]
        public bool IsBeingDragged
        {
            get { return isBeingDragged; }
            set { SetProperty(ref isBeingDragged, value); }
        }

        private bool isBeingDraggedOver;
        [JsonIgnore]
        public bool IsBeingDraggedOver
        {
            get { return isBeingDraggedOver; }
            set { SetProperty(ref isBeingDraggedOver, value); }
        }


        [JsonIgnore]
        public byte[] QueryBytes { get; protected set; } = { 0x30, 0x31, 0x30, 0x30 };

        [JsonIgnore]
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            { 
                SetProperty(ref isSelected, value);
            }
        }
        private bool isSelected = false;
        [JsonIgnore]

        // Nothing implemented for this yet
        public RangeDefinition Range => null;


        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }
        private string description = string.Empty;

        public string CANID
        {
            get { return cANID; }
            set { SetProperty(ref cANID, value); }
        }
        private string cANID = Constants.DEFAULT_DIAG_FUNCADDR_CAN_ID_11.ToString("X");
        [JsonIgnore]
        public bool IsBroadcast
        {
            get { return isBroadcast; }
            set
            {
                SetProperty(ref isBroadcast, value);
            }
        }
        private bool isBroadcast = false;

        public bool CanPlot
        {
            get { return canPlot; }
            set
            {
                SetProperty(ref canPlot, value);
            }
        }
        private bool canPlot = true;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private string name = string.Empty;

        public virtual string UnitDescriptor
        {
            get => OBD2Device.UseMetricUnits?this.MetricUnitDescriptor:this.EnglishUnitDescriptor;
            set 
            { 
                // used in derived classes such as UserPID where there is no metric/english mode supported
                SetProperty(ref unitDescriptor, value); 
            }
        }
        protected string unitDescriptor;

        [JsonIgnore]
        public string MetricUnitDescriptor
        {
            get => this.metricUnitDescriptor;
            set
            {
                this.metricUnitDescriptor = value;
            }
        }
        private string metricUnitDescriptor = string.Empty;

        [JsonIgnore]
        public string EnglishUnitDescriptor
        {
            get => this.englishUnitDescriptor;
            set
            {
                this.englishUnitDescriptor = value;
            }
        }
        private string englishUnitDescriptor = string.Empty;

        public virtual object Value { get; set; }

        [JsonIgnore]
        public Type ValueType
        {
            get => typeof(T);
            set { }
        }
        public int DecimalPlaces { get; set; }
        public int ResponseByteCount { get; set; }
        [JsonIgnore]
        public uint Code { get; set; }
        public virtual string OutputString => $"{Value}";
        public string CalcExpression
        {
            get => dt.Columns["exp"]?.Expression;
            set
            {
                if (value == null)
                {
                    dt.Columns["exp"].Expression = "";
                    return;
                }
                dt.Columns["exp"].Expression = value.ToUpper();
            }
        } 

        public object Parse(string input)
        {
            this.Parse(input.ToCharArray());
            return this.OutputString;
        }

        public void Parse(char[] data)
        {

            if (data.Length < 2) return;
            else if (data.Length < 4)
            {
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
            }
            else if (data.Length < 6)
            {
                // Bytes from system
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
                dt.Columns["B"].Expression = ((HexTable.FromHex(data[2]) << 4) | HexTable.FromHex(data[3])).ToString();
            }
            else if (data.Length < 8)
            {
                // Bytes from system
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
                dt.Columns["B"].Expression = ((HexTable.FromHex(data[2]) << 4) | HexTable.FromHex(data[3])).ToString();
                dt.Columns["C"].Expression = ((HexTable.FromHex(data[4]) << 4) | HexTable.FromHex(data[5])).ToString();
            }
            else if (data.Length < 10)
            {
                // Bytes from system
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
                dt.Columns["B"].Expression = ((HexTable.FromHex(data[2]) << 4) | HexTable.FromHex(data[3])).ToString();
                dt.Columns["C"].Expression = ((HexTable.FromHex(data[4]) << 4) | HexTable.FromHex(data[5])).ToString();
                dt.Columns["D"].Expression = ((HexTable.FromHex(data[6]) << 4) | HexTable.FromHex(data[7])).ToString();
            }
            else if (data.Length < 12)
            {
                // Bytes from system
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
                dt.Columns["B"].Expression = ((HexTable.FromHex(data[2]) << 4) | HexTable.FromHex(data[3])).ToString();
                dt.Columns["C"].Expression = ((HexTable.FromHex(data[4]) << 4) | HexTable.FromHex(data[5])).ToString();
                dt.Columns["D"].Expression = ((HexTable.FromHex(data[6]) << 4) | HexTable.FromHex(data[7])).ToString();
                dt.Columns["E"].Expression = ((HexTable.FromHex(data[8]) << 4) | HexTable.FromHex(data[9])).ToString();
            }
            else if (data.Length < 14)
            {
                // Bytes from system
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
                dt.Columns["B"].Expression = ((HexTable.FromHex(data[2]) << 4) | HexTable.FromHex(data[3])).ToString();
                dt.Columns["C"].Expression = ((HexTable.FromHex(data[4]) << 4) | HexTable.FromHex(data[5])).ToString();
                dt.Columns["D"].Expression = ((HexTable.FromHex(data[6]) << 4) | HexTable.FromHex(data[7])).ToString();
                dt.Columns["E"].Expression = ((HexTable.FromHex(data[8]) << 4) | HexTable.FromHex(data[9])).ToString();
                dt.Columns["F"].Expression = ((HexTable.FromHex(data[10]) << 4) | HexTable.FromHex(data[11])).ToString();
            }
            else if (data.Length < 16)
            {
                // Bytes from system
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
                dt.Columns["B"].Expression = ((HexTable.FromHex(data[2]) << 4) | HexTable.FromHex(data[3])).ToString();
                dt.Columns["C"].Expression = ((HexTable.FromHex(data[4]) << 4) | HexTable.FromHex(data[5])).ToString();
                dt.Columns["D"].Expression = ((HexTable.FromHex(data[6]) << 4) | HexTable.FromHex(data[7])).ToString();
                dt.Columns["E"].Expression = ((HexTable.FromHex(data[8]) << 4) | HexTable.FromHex(data[9])).ToString();
                dt.Columns["F"].Expression = ((HexTable.FromHex(data[10]) << 4) | HexTable.FromHex(data[11])).ToString();
                dt.Columns["G"].Expression = ((HexTable.FromHex(data[12]) << 4) | HexTable.FromHex(data[13])).ToString();
            }
            else if (data.Length < 18)
            {
                // Bytes from system
                dt.Columns["A"].Expression = ((HexTable.FromHex(data[0]) << 4) | HexTable.FromHex(data[1])).ToString();
                dt.Columns["B"].Expression = ((HexTable.FromHex(data[2]) << 4) | HexTable.FromHex(data[3])).ToString();
                dt.Columns["C"].Expression = ((HexTable.FromHex(data[4]) << 4) | HexTable.FromHex(data[5])).ToString();
                dt.Columns["D"].Expression = ((HexTable.FromHex(data[6]) << 4) | HexTable.FromHex(data[7])).ToString();
                dt.Columns["E"].Expression = ((HexTable.FromHex(data[8]) << 4) | HexTable.FromHex(data[9])).ToString();
                dt.Columns["F"].Expression = ((HexTable.FromHex(data[10]) << 4) | HexTable.FromHex(data[11])).ToString();
                dt.Columns["G"].Expression = ((HexTable.FromHex(data[12]) << 4) | HexTable.FromHex(data[13])).ToString();
                dt.Columns["H"].Expression = ((HexTable.FromHex(data[14]) << 4) | HexTable.FromHex(data[15])).ToString();
            }
            OnPropertyChanged("Value");
            OnPropertyChanged("OutputString");
        }

    }

}
