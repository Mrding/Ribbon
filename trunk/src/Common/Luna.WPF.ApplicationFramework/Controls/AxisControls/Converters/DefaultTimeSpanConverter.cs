using System;

namespace Luna.WPF.ApplicationFramework.Controls.Converters
{
    internal class DefaultTimeSpanConverter : IFormatProvider, ICustomFormatter
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
            TimeSpan timeSpan = (TimeSpan)arg;
            var hour = (int)timeSpan.TotalHours;
            var minute = (int)Math.Round(timeSpan.TotalMinutes % 60d);
            if (minute == 60)
            {
                hour++;
                minute = 0;
            }
            return string.Format("{0:D2}:{1:D2}", hour, minute);
        }
    }
}