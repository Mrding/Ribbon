using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class DateTimeToDayConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode)
            {
                return string.Empty;
            }
            var zoomValue = (double)values[0];
            var dateTime = System.Convert.ToDateTime(values[1]);
            var returnString = string.Empty;
            var dateFormat = "MM/dd(ddd)";
            if (parameter != null)
            {
                if (dateTime.Day == 1 && dateTime.Hour == 0 && dateTime.Minute == 0)
                {
                    returnString = string.Empty;
                    return returnString;
                }
                if (dateTime.Day == 2 && dateTime.Hour == 0 && dateTime.Minute == 0)
                {
                    returnString = string.Empty;
                    return returnString;
                }
            }
            if (zoomValue == 1)
            {
                if (dateTime.Hour == 0)
                    returnString = dateTime.ToString(dateFormat);
            }
            else if (zoomValue == 2)
            {
                if (dateTime.Hour == 0 && dateTime.Minute == 0)
                    returnString = dateTime.ToString(dateFormat);
                for (int i = 1; i < 24; i++)
                {
                    if (dateTime.Hour == i && dateTime.Minute == 0)
                        returnString = dateTime.ToString("HH");
                }
            }

            else if (zoomValue == 4)
            {
                if (dateTime.Hour == 0 && dateTime.Minute == 0)
                    returnString = dateTime.ToString(dateFormat);
                for (int i = 1; i < 24; i++)
                {
                    if (dateTime.Hour == i && dateTime.Minute == 0)
                        returnString = dateTime.ToString("HH");
                }
            }
            else if (zoomValue == 12)
            {
                if (dateTime.Hour == 0 && dateTime.Minute == 0)
                    returnString = dateTime.ToString(dateFormat);
                for (int i = 1; i < 24; i++)
                {
                    if (dateTime.Hour == i && dateTime.Minute == 0)
                        returnString = dateTime.ToString("HH");
                }
            }
            else if (zoomValue == 60)
            {
                if (dateTime.Hour == 0 && dateTime.Minute == 0)
                    returnString = dateTime.ToString(dateFormat);
                for (int i = 0; i < 24; i++)
                {
                    if (dateTime.Hour == i && dateTime.Minute == 0 && i > 0)
                    {
                        returnString = dateTime.ToString("HH");
                    }
                }
            }

            return returnString;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
