using System;

namespace Luna.GUI.UserControls
{
    public class HourFormat : ICustomFormatter, IFormatProvider
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
            var tsp = TimeSpan.FromMinutes(Convert.ToInt32(arg));

            if (format.Contains("mm"))
                return format.Replace("mm", string.Format("{0:00}", tsp.Minutes));

            if (tsp.Days == 0)
                format = format.Remove(0, format.IndexOf("hh"));

            format = format.Replace("hh", string.Format("{0}", (int)tsp.TotalHours ));

            return format;
        }
    }

    public class HourFormat2 : ICustomFormatter, IFormatProvider
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
            var tsp = TimeSpan.FromMinutes(Convert.ToInt32(arg));

            if (format.Contains("mm"))
                return format.Replace("mm", string.Format("{0:00}", tsp.Minutes));

            if (tsp.Days == 0)
                format = format.Remove(0, format.IndexOf("hh"));

            format = format.Replace("hh", string.Format("{0}", (int)tsp.Hours));

            return format;
        }
    }
}