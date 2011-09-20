using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Luna.WPF.ApplicationFramework.Controls.Converters
{
    public class LengthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var intervalLength = (double)values[0];
            var target = (FrameworkElement)values[1];
            var angle = (double)values[2];
            var height = (double)values[3];
            if (target == null)
                return 0;

            Size maxSize = new Size(15, 30);

            //height = height / 2 - maxSize.Height / 2;

            if (angle < 0)
                angle += 360;
            var temp = intervalLength - maxSize.Width;
            if (angle > 90 && angle < 270)
            {

                if (angle < 180)
                {
                    double sub = target.ActualWidth * (angle - 90) / 90;
                    temp -= sub;
                }
                else
                {
                    double sub = target.ActualWidth * (angle - 180) / 90;
                    temp -= target.ActualWidth - sub;
                }

            }

            temp -= 10;
            //if (angle > 0 && angle < 180)
            //    temp -= height;
            //else
            //    temp += height;

            return temp > 0 ? temp : 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
