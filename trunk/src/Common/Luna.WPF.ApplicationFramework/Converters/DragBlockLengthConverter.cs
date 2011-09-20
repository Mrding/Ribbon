using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class DragBlockLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var hourWidth = System.Convert.ToDouble(Application.Current.FindResource(parameter));
            var minLength = System.Convert.ToDouble(value);

            return minLength /60 * hourWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
