using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Media;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class NameToBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return StaticObjectConverter.GetObjectByPropertyName<SolidColorBrush>(typeof(Brushes), value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return StaticObjectConverter.GetNameByObj(typeof(Brushes), value);
        }

        #endregion
    }

    public class NameToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return StaticObjectConverter.GetObjectByPropertyName<SolidColorBrush>(typeof(Brushes), value.ToString()).Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return StaticObjectConverter.GetNameByObj(typeof(Brushes), value, "Color");
        }

        #endregion
    }

  
}
