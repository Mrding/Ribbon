using System;
using System.Globalization;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class RedeemValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = System.Convert.ToDouble(value);
            return d - System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RedeemHourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime && (DateTime)value != default(DateTime))
            {
                var newValue = ((DateTime)value).AddHours(System.Convert.ToInt32(parameter));
              
                return newValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime && (DateTime)value != default(DateTime))
            {
                return ((DateTime)value).AddHours(-System.Convert.ToInt32(parameter));
            }
            return value;
        }
    }
    
}