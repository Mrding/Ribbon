using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class ZoomToTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double zoomValue = (double)values[0];
           
            if (values[1] == null) 
                return string.Empty;

            string text;
            var dateTime = (DateTime)values[1];

            if (zoomValue == 1)
            {
                text = string.Format("{0:HH}", dateTime);
            }
            else if (zoomValue == 2)
                text = dateTime.ToString("mm");
            else if (zoomValue == 4)
                text = dateTime.ToString("mm");
            else if (zoomValue == 12)
                text = dateTime.ToString("mm");
            else
                text = dateTime.ToString("mm");

            return text == "00" ? "" : text[0] == '0' ? text.Remove(0, 1) : text;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
