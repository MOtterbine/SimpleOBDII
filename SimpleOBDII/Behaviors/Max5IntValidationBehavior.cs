using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OS.OBDII.Behaviors
{
    public static class Max5IntValidationBehavior
    {
        public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached(
                "AttachBehavior",
                typeof(bool),
                typeof(Max5IntValidationBehavior),
                false,
                propertyChanged: OnAttachBehaviorChanged);

        public static bool GetAttachBehavior(BindableObject view)
        {
            return (bool)view.GetValue(AttachBehaviorProperty);
        }

        public static void SetAttachBehavior(BindableObject view, bool value)
        {
            view.SetValue(AttachBehaviorProperty, value);
        }

        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            var entry = view as Entry;
            if (entry == null)
            {
                return;
            }

            bool attachBehavior = (bool)newValue;
            if (attachBehavior)
            {
                entry.TextChanged += OnEntryTextChanged;
            }
            else
            {
                entry.TextChanged -= OnEntryTextChanged;
            }
        }

        static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.NewTextValue)) return;


            // looking to match anything not in the string
            string sPattern = "^[0-5]*$";
            bool isValid = Regex.IsMatch(args.NewTextValue, sPattern);




            //int r = 0;
            //   bool isValid = int.TryParse(args.NewTextValue, System.Globalization.NumberStyles.HexNumber, null, out r);
            //isValid |= args.NewTextValue == "X";
            if (!isValid)// || r == oldVal)
            {
                ((Entry)sender).Text = args.OldTextValue;
            }

        }
    }
}
