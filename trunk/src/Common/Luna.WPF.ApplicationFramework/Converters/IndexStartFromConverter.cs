using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class IndexStartFromConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var incressValue = System.Convert.ToInt32(parameter);
            var index = System.Convert.ToInt32(value);
            if (index == -1)
                return value;
            return index + incressValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
