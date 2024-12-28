using OS.OBDII.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class PID : PIDBase, IPid, ICloneable
    {
        public PID() { }
        public PID(uint code, string name, string description, Type pidValueType, Func<object, object> func = null, bool isUserFunction = true)
        {
            this.Scaling = new UnitMeasureScale(UnitMeasure.None, func);// { Descriptor = string.Empty };
            this.Code = code;
            this.Name = name;
            this.Description = description;
            this.IsSupportable = isUserFunction;
            this.function = func;
            this.ValueType = pidValueType;
           // this.IsVisible = true;
        }
        public PID(uint code, string name, string description, Type pidValueType, UnitMeasureScale measureScale = null, bool isUserFunction = true)
        {
            this.Scaling = (UnitMeasureScale)measureScale.Clone();
            this.Code = code;
            this.Name = name;
            this.Description = description;
            this.IsSupportable = isUserFunction;
            this.function = measureScale.ValueFromInput;
            this.ValueType = pidValueType;
            this.ResponseByteCount = measureScale.ResponseByteCount;
           // this.IsVisible = true;
        }
        private bool isBeingDragged;
        public bool IsBeingDragged
        {
            get { return isBeingDragged; }
            set { SetProperty(ref isBeingDragged, value); }
        }

        private bool isBeingDraggedOver;
        public bool IsBeingDraggedOver
        {
            get { return isBeingDraggedOver; }
            set { SetProperty(ref isBeingDraggedOver, value); }
        }

        public override string ToString()
        {
            return $"{Code:X2} - {Description}: {this.OutputString}";
        }

        object ICloneable.Clone()
        {
            PID np = null;
            try
            {
                np = new PID(this.Code, this.Name, this.Description, this.ValueType, this.Scaling, this.IsSupportable) { CANID = this.CANID, CanPlot = this.CanPlot};
            }
            catch(Exception ex)
            {
                var l = ex.Message;
            }
            return np;
        }
    }

    public class PIDBase : ViewModels.BaseViewModel
    { 
       // public RangeDefinition Range { get; } = new RangeDefinition();
        public int DecimalPlaces
        {
            get => this.Scaling.DecimalPlaces;
            set => this.Scaling.DecimalPlaces = value;
        }
        protected object pidValue = 0;
        public object Value
        {
            get { return pidValue; }
            set
            {
                SetProperty(ref pidValue, value);
                OnPropertyChanged("OutputString");
            }
        }
        public string OutputString => $"{this.Value} {this.Scaling.Descriptor}";
        public Type ValueType { get; set; }

        public byte[] QueryBytes { get; set; }

        public RangeDefinition Range => this.Scaling.Range;
        public string CANID
        {
            get { return cANID; }
            set { SetProperty(ref cANID, value); }
        }
        private string cANID = Constants.DEFAULT_DIAG_FUNCADDR_CAN_ID_11.ToString("X");

        public int ResponseByteCount { get; set; } = 2;
        public string CalcExpression { get; set; }

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }
        private string description = string.Empty;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private string name = string.Empty;

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }
        private bool isSelected = false;

        public bool IsBroadcast
        {
            get { return isBroadcast; }
            set { SetProperty(ref isBroadcast, value); }
        }
        private bool isBroadcast = false;

        public bool CanPlot
        {
            get { return canPlot; }
            set { SetProperty(ref canPlot, value); }
        }
        private bool canPlot = true;

        public uint Code { get; set; }
        public bool IsSupportable { get; protected set; }
        public bool IsVisible { get; set; } = true;
        public Func<object, object> function { get; protected set; }
        public virtual object Parse(string input)
        {
            return this.function(input);
        }

        public string UnitDescriptor
        {
            get => this.Scaling.UnitDescriptor;
        }
        public string MetricUnitDescriptor
        {
            get => this.Scaling.MetricUnitDescriptor;
            set
            {
                this.Scaling.MetricUnitDescriptor = value;
            }
        }
        public string EnglishUnitDescriptor
        {
            get => this.Scaling.EnglishUnitDescriptor;
            set
            {
                this.Scaling.EnglishUnitDescriptor = value;
            }
        }
        public UnitMeasure Unit => this.Scaling.Unit;
        protected UnitMeasureScale Scaling = null;


    }



    public class PID<T> : PIDBase, IPid<T>, ICloneable 
    {
        public PID(uint code, string name, string description, Func<object, object> func = null, bool isUserFunction = true)
        {
            this.Scaling = new UnitMeasureScale(UnitMeasure.None, func);// { Descriptor = string.Empty };
            this.Code = code;
            this.Name = name;
            this.Description = description;
            this.IsSupportable = isUserFunction;
            this.function = func;
            //this.ValueType = pidValueType;
            // this.IsVisible = true;
        }
        public PID(uint code, string name, string description, UnitMeasureScale measureScale = null, bool isUserFunction = true)
        {
            this.Scaling = (UnitMeasureScale)measureScale.Clone();
            this.Code = code;
            this.Name = name;
            this.Description = description;
            this.IsSupportable = isUserFunction;
            this.function = measureScale.ValueFromInput;
            //this.ValueType = pidValueType;
            this.ResponseByteCount = measureScale.ResponseByteCount;
            // this.IsVisible = true;
        }


        private new T pidValue;
        public new T Value
        {
            get { return pidValue; }
            set
            {
                SetProperty(ref pidValue, value);
                OnPropertyChanged("OutputString");
            }
        }
        public new Type ValueType => typeof(T);


        public new T Parse(string input)
        {
            return (T)this.function(input);
        }

        public override string ToString()
        {
            return $"{Code:X2} - {Description}: {this.OutputString}";
        }

        object ICloneable.Clone()
        {
            PID np = null;
            try
            {
                np = new PID(this.Code, this.Name, this.Description, this.ValueType, this.Scaling, this.IsSupportable) { CANID = this.CANID, CanPlot = this.CanPlot };
            }
            catch (Exception ex)
            {
                var l = ex.Message;
            }
            return np;
        }
    }




    public class PID13
    {

        public PID13(DeviceRequestType requestType)
        {
            this.SetupObject(requestType);
        }
        /// <summary>
        /// Loads up properties and methods for sending and receiving the selected request type (PID)
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        public PID13 Switch(DeviceRequestType requestType)
        {
            this.SetupObject(requestType);
            return this;
        }

        private void SetupObject(DeviceRequestType requestType)
        {
            this.RequestType = requestType;
            this.Code = OBD2Device.ELM327CommandDictionary[requestType].Code;
            this.Name = OBD2Device.ELM327CommandDictionary[requestType].Name;
            this.Description = OBD2Device.ELM327CommandDictionary[requestType].Description;
            this.Parse = OBD2Device.ELM327CommandDictionary[requestType].function;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Code { get; private set; } 
        public DeviceRequestType RequestType { get; private set; }
        public Func<object, object> Parse { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
