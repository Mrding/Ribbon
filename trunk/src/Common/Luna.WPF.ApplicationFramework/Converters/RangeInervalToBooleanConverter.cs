using System;
using System.Globalization;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class RangeInervalToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return value.Equals(parameter.ToString());
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter.ToString();
        }
    }
}