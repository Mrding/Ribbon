using System;

namespace Luna.WPF.ApplicationFramework.Controls.Converters
{
    public class DefaultDateTimeConverter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var dateTime = (DateTime)arg;
            var hour = dateTime.Hour;
            var minute = (int)Math.Round(dateTime.TimeOfDay.TotalMinutes % 60d);
            if (minute == 60)
            {
                hour++;
                minute = 0;
            }
            return string.Format("{0:D2}:{1:D2}", hour, minute);
        }
    }
}