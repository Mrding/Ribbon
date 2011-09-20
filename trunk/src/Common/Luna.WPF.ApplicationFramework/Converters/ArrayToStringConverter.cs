using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var array = value as string[];

            if(value is IEnumerable)
                array = (value as IEnumerable).OfType<object>().Select(o => o.ToString()).ToArray();

            if (array == null)
                return null;
            return String.Join(", ", array);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
