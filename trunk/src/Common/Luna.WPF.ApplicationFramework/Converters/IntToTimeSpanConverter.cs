using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class IntToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromMinutes(System.Convert.ToInt32(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan;
            return TimeSpan.TryParse(value.ToString(), out timeSpan) ? timeSpan.TotalMinutes : 0;
        }
    }
}
