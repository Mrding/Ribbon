using System;
using System.Globalization;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class BooleanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolean = System.Convert.ToBoolean(value);

            if (parameter == null) return value;

            var stringArray = parameter.ToString().Split(';');

            if (stringArray.Length <= 1) return value;

            return stringArray[System.Convert.ToInt32(boolean)];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        } 
    }
}