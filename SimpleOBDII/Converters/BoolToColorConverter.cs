using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace OS.OBDII.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        object v = null;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                if (Application.Current.Resources.TryGetValue("Success", out v)) return v;
                return Color.FromArgb("#FF6fff6f");
            }
            if (Application.Current.Resources.TryGetValue("Failure", out v)) return v;
            return Color.FromArgb("#FFff85a5");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
