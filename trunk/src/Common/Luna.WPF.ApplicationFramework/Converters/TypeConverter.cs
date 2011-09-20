using System;
using System.Globalization;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class TypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Type.Missing;
            return value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
