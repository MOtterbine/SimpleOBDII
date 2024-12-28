using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OS.OBDII.Behaviors
{
    public static class NumbersOnlyValidationBehavior
    {
        public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached(
                "AttachBehavior",
                typeof(bool),
                typeof(NumbersOnlyValidationBehavior),
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
            string sPattern = "^[0-9]*$";
            bool isValid = Regex.IsMatch(args.NewTextValue, sPattern);

            if (args.NewTextValue.Length > 3) isValid = false;

            //int val;
            //if(int.TryParse(args.NewTextValue, out val))
            //{
            //    if ((val > 300) || (val < 75)) isValid = false;
            //}


            if (!isValid)
            {
                ((Entry)sender).Text = args.OldTextValue;
            }
            //else
            //{
            //    ((Entry)sender).Text = ((Entry)sender).Text;
            //}

        }
    }
}
