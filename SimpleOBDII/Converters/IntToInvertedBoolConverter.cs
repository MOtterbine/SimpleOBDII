using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OS.OBDII.Converters
{
    public class IntToInvertedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (int)value != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)value) return 0;
            return 1;
        }
    }
}
