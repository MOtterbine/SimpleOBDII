using Newtonsoft.Json;
using OS.OBDII.Interfaces;
using OS.OBDII.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OS.OBDII.Models
{
    public class UserPID : BasePID<double>, ICloneable, IPid
    {

        public UserPID(string descr, string unitDesc, int decPlaces, string query, string expression) : base()
        {
            this.Code = 0;
            //if (string.IsNullOrEmpty(query))
            //{
            //    this.QueryBytes = new byte[0] { };
            //}
            this.QueryBytes = new byte[query.Length];
            query.ToCharArray().CopyTo(this.QueryBytes, 0);

            base.DecimalPlaces = decPlaces;
            base.Name = this.Description = descr;
            this.dt.TableName = descr;
            this.UnitDescriptor = unitDesc;
            base.EnglishUnitDescriptor = unitDesc;
            base.MetricUnitDescriptor = unitDesc;

            this.CalcExpression = expression;
        }

        public override string ToString()
        {
            return $"{Name}, Code:{Code}, {CalcExpression}";
        }
        public UserPID(string descr, string unitDesc, int decPlaces, byte[] queryBytes, string expression) : base()
        {
            this.Code = 0;
            if (queryBytes == null)
            {
                queryBytes = new byte[0] { };
            }
            this.QueryBytes = new byte[queryBytes.Length];
            queryBytes.CopyTo(this.QueryBytes, 0);

            base.DecimalPlaces = decPlaces;
            base.Name = this.Description = descr;
            this.dt.TableName = descr;
            this.UnitDescriptor = unitDesc;
            base.EnglishUnitDescriptor = unitDesc;
            base.MetricUnitDescriptor = unitDesc;

            this.CalcExpression = expression;
        }

        public UserPID(uint code, UserPID uPid) : base()
        {
            this.Code = code;
            this.QueryBytes = new byte[uPid.QueryBytes.Length];
            uPid.QueryBytes.CopyTo(this.QueryBytes, 0);

            base.DecimalPlaces = uPid.DecimalPlaces;
            base.Name = this.Description = uPid.Description;
            this.dt.TableName = uPid.Description;
            this.UnitDescriptor = uPid.UnitDescriptor;
            base.EnglishUnitDescriptor = uPid.EnglishUnitDescriptor;
            base.MetricUnitDescriptor = uPid.MetricUnitDescriptor;

            base.ResponseByteCount = uPid.ResponseByteCount;

            base.IsBroadcast = uPid.IsBroadcast;

            this.CalcExpression = uPid.CalcExpression;
            this.CANID = uPid.CANID;
        }
        public UserPID(uint code)
        {
            this.Code = code;
            this.Description = this.Name = "New PID";
            this.CalcExpression = "A";
            this.ResponseByteCount = 1;
        }
        public UserPID()
        {
            this.EnglishUnitDescriptor = this.MetricUnitDescriptor = this.UnitDescriptor;
        }
        /// <summary>
        /// UserPID objects (user-defined pids) don't support metric/english selection, so they are all the same
        /// </summary>
        /// 



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

        //private void DropGestureRecognizer_Drop_Collection(System.Object sender, Xamarin.Forms.DropEventArgs e)
        //{
        //    e.Handled = true;
        //}






        public override string UnitDescriptor
        {
            get => base.unitDescriptor;
            set
            {
                SetProperty(ref unitDescriptor, value);
                base.MetricUnitDescriptor = value;
                base.EnglishUnitDescriptor = value;
            }
        }

        public string CommandString
        {
            get => Encoding.ASCII.GetString(this.QueryBytes);
            set
            {
                base.QueryBytes = Encoding.ASCII.GetBytes(value);
                OnPropertyChanged("CommandString");
            }
        }
        private string commandString = string.Empty;




        [JsonIgnore]
        public override object Value
        {
            get
            {
                try
                {
                    return dt.Rows[0]["exp"] ?? 0.0;
                    // return Convert.ToDouble(dt.Rows[0]["exp"]);
                }
                catch (Exception)
                {
                    return 0.0;
                }
            }
        }

        [JsonIgnore]
        public override string OutputString
        {
            get
            {
                try
                {
                    
                    return $"{Math.Round((double)(Value??0), this.DecimalPlaces)} {this.UnitDescriptor}";
                }
                catch(Exception)
                {
                    return string.Empty;
                }
            }
        }

        public object Clone()
        {
            return new UserPID()
            {
                Code = Code,
                Name = this.Name,
                Description = this.Description,
                CANID = this.CANID,
                CalcExpression = this.CalcExpression,
                CommandString = this.CommandString,
                DecimalPlaces = this.DecimalPlaces,
                UnitDescriptor = this.UnitDescriptor,
                EnglishUnitDescriptor = base.EnglishUnitDescriptor,
                MetricUnitDescriptor = base.MetricUnitDescriptor,

                ResponseByteCount = this.ResponseByteCount,
                ValueType = this.ValueType,
                Value = (((this.Value as ICloneable) == null) ? this.Value : (this.Value as ICloneable).Clone())
            };

        }
    }

}
