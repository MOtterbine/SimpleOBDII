using Newtonsoft.Json;
using OS.OBDII.Interfaces;
using OS.OBDII.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OS.OBDII.ViewModels
{
    public class ActiveTestCommand : ActiveTestBase<double>, IActiveTestCommand, ICloneable
    {

        public ActiveTestCommand(string descr, string englishUnitDesc, string metricUnitDesc, int decPlaces, string query, string expression) : base()
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
            base.EnglishUnitDescriptor = englishUnitDesc;
            base.MetricUnitDescriptor = metricUnitDesc;

            this.CalcExpression = expression;
        }
        public ActiveTestCommand(string descr, string englishUnitDesc, string metricUnitDesc, int decPlaces, byte[] queryBytes, string expression) : base()
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
            base.EnglishUnitDescriptor = englishUnitDesc;
            base.MetricUnitDescriptor = metricUnitDesc;

            this.CalcExpression = expression;
        }

        public ActiveTestCommand(uint code, UserPID uPid) : base()
        {
            this.Code = code;
            this.QueryBytes = new byte[uPid.QueryBytes.Length];
            uPid.QueryBytes.CopyTo(this.QueryBytes, 0);

            base.DecimalPlaces = uPid.DecimalPlaces;
            base.Name = this.Description = uPid.Description;
            this.dt.TableName = uPid.Description;

            base.EnglishUnitDescriptor = uPid.EnglishUnitDescriptor;
            base.MetricUnitDescriptor = uPid.MetricUnitDescriptor;



            base.ResponseByteCount = uPid.ResponseByteCount;

            this.CalcExpression = uPid.CalcExpression;
            this.CANID = uPid.CANID;
        }
        public ActiveTestCommand(uint code)
        {
            this.Code = code;
            this.Name = "New PID";
            this.Description = "A new pid";
            this.CalcExpression = "A";
        }
        public ActiveTestCommand()
        {

        }


        //public string CommandString
        //{
        //    get => Encoding.ASCII.GetString(this.QueryBytes);
        //    set
        //    {
        //        base.QueryBytes = Encoding.ASCII.GetBytes(value);
        //        OnPropertyChanged("CommandString");
        //    }
        //}
        //private string commandString = string.Empty;




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
                //UnitDescriptor = this.UnitDescriptor,
                EnglishUnitDescriptor = base.EnglishUnitDescriptor,
                MetricUnitDescriptor = base.MetricUnitDescriptor,
                ResponseByteCount = this.ResponseByteCount,
                ValueType = this.ValueType,
                Value = (((this.Value as ICloneable) == null) ? this.Value : (this.Value as ICloneable).Clone())
            };

        }
    }
}
