using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using Caliburn.PresentationFramework;
using Luna.Common;
using System.Collections;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class EnumerableToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = value as IEnumerable;

            if (items == null)
                return value;

            var list = items.Cast<object>().ToList();
            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
