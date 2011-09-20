using System;
using System.Globalization;
using System.Windows.Data;
using Luna.Common;

namespace Luna.GUI.Views.Shifts
{
    public class DimensionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dimension)
                return value.ToString();

            throw new InvalidOperationException("The value must be a Dimension type");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pairValues = value.ToString().Split(new[] { ',' });
            int rows = 0;
            int columns = 0;

            if (pairValues.Length == 2)
            {
                int.TryParse(pairValues[0], out rows);
                int.TryParse(pairValues[1], out columns);
            }
            if (rows == 0 || columns == 0)
                return Dimension.Invalid;
            return new Dimension(rows, columns);
        }

        #endregion
    }
}
