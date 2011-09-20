using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class ValueRangeToStatusConveter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3) return null;

            var status = System.Convert.ToString(parameter).Split(',');

            if (status.Length != 3) return null;

            if (values[0] == DependencyProperty.UnsetValue) 
                return null;

            var min = System.Convert.ToDouble(values[0]);
            var current = System.Convert.ToDouble(values[1]);
            var max = System.Convert.ToDouble(values[2]);

            if (current < min)
                return new BitmapImage(new Uri(string.Format("pack://application:,,,/{0}", status[0]))); // under
            if (max < current)
                return new BitmapImage(new Uri(string.Format("pack://application:,,,/{0}", status[2]))); // over

            return null; // in the ragne
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}