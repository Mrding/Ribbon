using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class DateTimeLengthConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(values[0] is DateTime))
                return string.Empty;

            DateTime start = (DateTime) values[0];
            DateTime end = (DateTime)values[1];

            TimeSpan timeSpan = end.Subtract(start);

            return string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.Hours, 
                                 (int)timeSpan.Minutes,
                                 (int)timeSpan.Seconds);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
