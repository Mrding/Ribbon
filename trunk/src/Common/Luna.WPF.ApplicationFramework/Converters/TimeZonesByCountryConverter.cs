using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework.Converters
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class CountryToTimeZonesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timezones = new List<TimeZoneInfo>();

            if (value == null || !Application.Current.Resources.Contains(value)) return timezones;

            var timezoneIds = Application.Current.Resources[value] as Array;

            if (timezoneIds != null)
            {
                timezones = (from string timeZoneId in timezoneIds
                                   select TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)).ToList();
            }
            return timezones;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
