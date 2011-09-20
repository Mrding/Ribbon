using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class InversionOfBooleanToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var booleanValue = System.Convert.ToBoolean(value);
            if (booleanValue)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibilityValue = (Visibility) Enum.Parse(typeof (Visibility), value.ToString(), true);
            return visibilityValue != Visibility.Visible;
        }

        #endregion
    }
}