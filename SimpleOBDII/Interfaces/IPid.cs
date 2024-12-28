using OS.OBDII.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OS.OBDII.Interfaces
{
    public interface IPid
    {

        string CANID { get; set; }
        uint Code { get; set; }
        string OutputString { get; }
        object Value { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string UnitDescriptor { get; }// set; }
        string MetricUnitDescriptor { get; set; }
        string EnglishUnitDescriptor { get; set; }
        RangeDefinition Range { get; }
        Type ValueType { get; set; }
        int ResponseByteCount { get; set; }
        int DecimalPlaces { get; set; }
        bool IsSelected { get; set; }
        bool IsBroadcast { get; set; }
        bool CanPlot { get; set; }
        byte[] QueryBytes { get; }
        string CalcExpression { get; set; }
        object Parse(string input);

        bool IsBeingDragged { get; set; }

        bool IsBeingDraggedOver { get; set; }


        event PropertyChangedEventHandler PropertyChanged;

    }


    public interface IPid<T>
    {
        string CANID { get; set; }
        uint Code { get; set; }
        string OutputString { get; }
        T Value { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string UnitDescriptor { get; }// set; }
        string MetricUnitDescriptor { get; set; }
        string EnglishUnitDescriptor { get; set; }
        Type ValueType { get; }
        int ResponseByteCount { get; set; }
        int DecimalPlaces { get; set; }
        bool IsSelected { get; set; }
        bool IsBroadcast { get; set; }
        byte[] QueryBytes { get; }
        string CalcExpression { get; set; }
        T Parse(string input);

        event PropertyChangedEventHandler PropertyChanged;
    }

}
