using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OS.OBDII;
using OS.OBDII.Models;
using OS.OBDII.PartialClasses;

namespace OS.OBDII.Behaviors;

public class CursorBehavior : Behavior
{
    public static readonly BindableProperty CursorProperty = BindableProperty.CreateAttached("Cursor", typeof(CursorIcon), typeof(CursorBehavior), CursorIcon.Arrow, propertyChanged: CursorChanged);

    private static void CursorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is VisualElement visualElement)
        {

#if WINDOWS

            var ctx = Application.Current?.MainPage?.Handler?.MauiContext;

            visualElement.SetCustomCursor((CursorIcon)newvalue, ctx);

#endif

        }
    }

    public static CursorIcon GetCursor(BindableObject view) => (CursorIcon)view.GetValue(CursorProperty);

    public static void SetCursor(BindableObject view, CursorIcon value) => view.SetValue(CursorProperty, value);
}
