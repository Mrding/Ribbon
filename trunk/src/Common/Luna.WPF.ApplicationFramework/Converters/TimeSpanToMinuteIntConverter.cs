using System;
using System.Globalization;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class TimeSpanToMinuteIntConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var timespan = (TimeSpan) value;

                return System.Convert.ToInt32(timespan.TotalMinutes);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromMinutes(System.Convert.ToInt32(value));
        }

        #endregion
    }
}