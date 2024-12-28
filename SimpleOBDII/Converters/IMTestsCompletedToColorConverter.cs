using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OS.OBDII.Converters
{
    public class IMTestsCompletedToColorConverter : IValueConverter
    {
   //     private Styles st = new Styles();//.LoadFromXaml("Col";
        object v = null;
        
       public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)value)
            {
                //if (Application.Current.Resources.TryGetValue("Success", out v)) return v;
                if (Application.Current.Resources.TryGetValue("GreenText", out v)) return v;
                //return  st["GreenText"];
                return Colors.Green;
            }
            //if (Application.Current.Resources.TryGetValue("Failure", out v)) return v;
            if(Application.Current.Resources.TryGetValue("PrimaryText", out v)) return v;
            //return st["PrimaryText"];
            return Color.FromArgb("FF444444");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
