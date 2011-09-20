using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class MultiplicationConveter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = default(double?);

            if (values == null) return 0.0;

            foreach (var d in values.Select(System.Convert.ToDouble))
            {
                if (result == null)
                    result = d;
                else
                    result = result*d;
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}