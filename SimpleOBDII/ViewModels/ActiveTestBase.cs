using Newtonsoft.Json;
using OS.OBDII.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OS.OBDII.ViewModels
{
    public abstract class ActiveTestBase<T> : BaseViewModel
    {
        protected DataTable dt = new DataTable() { Columns = {
            new DataColumn("A", typeof(double)),
            new DataColumn("B", typeof(double)),
            new DataColumn("C", typeof(double)),
            new DataColumn("D", typeof(double)),
            new DataColumn("E", typeof(double)),
            new DataColumn("F", typeof(double)),
            new DataColumn("exp", typeof(double))
            },
            Rows = { { 0, 0, 0, 0, 0, 0, 0 } }

        };
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
        public string CommandString
        {
            get => Encoding.ASCII.GetString(this.QueryBytes);
            set
            {
                QueryBytes = Encoding.ASCII.GetBytes(value);
                SetProperty(ref commandString, value);
            }
        }
        private string commandString = string.Empty;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private string name = string.Empty;
        public string UnitDescriptor
        {
            get => OBD2Device.UseMetricUnits ? this.MetricUnitDescriptor : this.EnglishUnitDescriptor;
        }
        public string MetricUnitDescriptor
        {
            get => this.metricUnitDescriptor;
            set
            {
                this.metricUnitDescriptor = value;
            }
        }
        private string metricUnitDescriptor = string.Empty;

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

        public void Parse(string input)
        {
            this.Parse(input.ToCharArray());
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
            OnPropertyChanged("Value");
            OnPropertyChanged("OutputString");
        }

    }


}
