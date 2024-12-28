using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OS.OBDII.Converters
{
    public class StdStringCtrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(String.IsNullOrEmpty((string)value))
            {
                return "<Not Set>";
            }
            return ((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
