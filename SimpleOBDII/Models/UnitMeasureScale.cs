using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{

    public class UnitMeasureScale : ICloneable
    {
        public UnitMeasureScale(UnitMeasure measure, Func<object, object> func, int expectedResponseByteCount = 2, string englishDescriptor = null, string metricDescriptor = null)
        {
            this.MetricUnitDescriptor = metricDescriptor;
            this.EnglishUnitDescriptor = englishDescriptor;
            //this.Descriptor = descriptor ?? measure.ToString();
            this.Unit = measure;
            this.Scale = func;
            this.ResponseByteCount = expectedResponseByteCount;
        }
        public UnitMeasureScale(uint scalingId, Func<object, object> func, int expectedResponseByteCount = 2, string englishDescriptor = null, string metricDescriptor = null)
        {
            this.MetricUnitDescriptor = metricDescriptor;
            this.EnglishUnitDescriptor = englishDescriptor;
            //this.Descriptor = descriptor ?? measure.ToString();
            this.Unit = UnitMeasure.None;
            this.Scale = func;
            this.ResponseByteCount = expectedResponseByteCount;
        }

        
        public int ResponseByteCount { get; private set; }
        public string Descriptor 
        {
            get
            {
                return OBD2Device.UseMetricUnits ? this.metricUnitDescriptor : this.englishUnitDescriptor;
            } 
           // private set; 
        }

        public int DecimalPlaces
        {
            get => this.decimalPLaces;
            set => this.decimalPLaces = value;
        }
        private int decimalPLaces = 0;

        public double Maximum_English
        {
            get => this.Range.Maximum_English;
            set => this.Range.Maximum_English = value;
        }

        public double Minimum_English
        {
            get => this.Range.Minimum_English;
            set => this.Range.Minimum_English = value;
        }

        public double Maximum_Metric
        {
            get => this.Range.Maximum_Metric;
            set => this.Range.Maximum_Metric = value;
        }

        public double Minimum_Metric
        {
            get => this.Range.Minimum_Metric;
            set => this.Range.Minimum_Metric = value;
        }
        public RangeDefinition Range { get; } = new RangeDefinition();


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


        public string UnitDescriptor
        {
            get => OBD2Device.UseMetricUnits?this.MetricUnitDescriptor:this.EnglishUnitDescriptor;
        }

        public UnitMeasure Unit {get; private set;}
        private Func<object, object> Scale = null;

        public object ValueFromInput(object input)
        {
            return this.Scale(input);
        }

        public override string ToString()
        {
            return $"{Unit} ({Descriptor})";
        }

        public object Clone()
        {
            return new UnitMeasureScale(this.Unit, this.Scale, this.ResponseByteCount, this.englishUnitDescriptor, this.metricUnitDescriptor) 
                                            { Maximum_English = this.Range.Maximum_English,
                                                Minimum_English = this.Range.Minimum_English,
                                                Maximum_Metric = this.Range.Maximum_Metric,
                                                Minimum_Metric = this.Minimum_Metric
                                            };
        }
    }
}
