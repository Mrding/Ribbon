using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework
{
    public class ScheduleGridService
    {
        public static GridLength GetLeftWidth(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(LeftWidthProperty);
        }

        public static void SetLeftWidth(DependencyObject obj, GridLength value)
        {
            obj.SetValue(LeftWidthProperty, value);
        }

        public static readonly DependencyProperty LeftWidthProperty =
            DependencyProperty.RegisterAttached("LeftWidth", typeof(GridLength), typeof(ScheduleGridService),
            new UIPropertyMetadata(new GridLength(0, GridUnitType.Pixel)));
    }
}
