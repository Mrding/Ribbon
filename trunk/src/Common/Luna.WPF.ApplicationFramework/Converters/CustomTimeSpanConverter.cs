using System;
using System.Windows.Data;
using Luna.Globalization;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class CustomTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan timeSpan = (TimeSpan)value;
            if (timeSpan.TotalMinutes < 60)
                return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);

            return string.Format("{0:00}{1}", (int)timeSpan.TotalMinutes, LanguageReader.GetValue("Minute"));
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
