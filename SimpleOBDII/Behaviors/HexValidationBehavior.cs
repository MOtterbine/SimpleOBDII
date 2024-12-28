using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OS.OBDII.Behaviors
{
    public class HexValidationBehavior : Behavior<Entry>
    {

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }


        public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached(
                "AttachBehavior",
                typeof(bool),
                typeof(HexValidationBehavior),
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


        public static readonly BindableProperty DefaultTextColorProperty =
            BindableProperty.CreateAttached(
                "DefaultTextColor",
                typeof(Color),
                typeof(HexValidationBehavior),
                new Color(0, 0, 0));

        public static Color GetDefaultTextColor(BindableObject view)
        {
            return (Color)view.GetValue(DefaultTextColorProperty);
        }

        public static void SetDefaultTextColor(BindableObject view, Color value)
        {
            view.SetValue(DefaultTextColorProperty, value);
        }

        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            var entry = view as Entry;
            if (entry == null) return;

            bool attachBehavior = (bool)newValue;
            if (attachBehavior)
            {
                entry.Behaviors.Add(new HexValidationBehavior());
            }
            else
            {
                var toRemove = entry.Behaviors.FirstOrDefault(b => b is HexValidationBehavior);
                if (toRemove != null)
                {
                    entry.Behaviors.Remove(toRemove);
                }
            }
        }

        static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {

            var entry = sender as Entry;
            if (entry == null) return;
            var cursorPos = entry.CursorPosition;
            if (string.IsNullOrEmpty(args.NewTextValue)) return;


            // looking to match anything not in the string
            string sPattern = "^[a-fA-F0-9]*$";
            bool isValid = Regex.IsMatch(args.NewTextValue, sPattern);

            if (!isValid)// || r == oldVal)
            {
               // entry.TextColor = Color.FromArgb("CC2222");
                if (string.IsNullOrEmpty(args.OldTextValue))
                {
                    entry.Text = string.Empty;
                    entry.CursorPosition = 0;
                    return;
                }
                entry.Text = args.OldTextValue.ToUpper();
            }
            else
            {
               // entry.TextColor = (Color)entry.GetValue(DefaultTextColorProperty);

                //entry.SetValue(entry.Text, entry.Text.ToUpper());
                //  entry.Text = args.OldTextValue.ToUpper();
            }
            entry.CursorPosition = cursorPos;
        }
    }
}
