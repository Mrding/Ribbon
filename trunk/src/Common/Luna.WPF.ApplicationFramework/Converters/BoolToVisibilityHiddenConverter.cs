using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class BoolToVisibilityHiddenConverter : IValueConverter
    {
        #region Fields

        private bool invertBoolean;

        #endregion

        #region Props

        public bool InvertBoolean
        {
            get { return invertBoolean; }
            set { invertBoolean = value; }
        }

        #endregion

        #region IValueConverter

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            var hidden = Visibility.Hidden;

            if (o is bool?)
            {
                var nullable = (bool?)o;

                if (nullable.Value ^ invertBoolean)
                    hidden = Visibility.Visible;

                return hidden;
            }

            if (o is bool && ((bool)o) ^ invertBoolean)
            {
                hidden = Visibility.Visible;
            }

            return hidden;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)o;
            return ((visibility == Visibility.Visible) ^ invertBoolean);
        }

        #endregion
    }
}