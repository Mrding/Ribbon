using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Controls.Brick
{
    public class ZoomPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //parameter Percentage|ToolTip|ZoomValue
            int val = System.Convert.ToInt32(value);
            int result = 1;
            string operation = (string)parameter;
            if (operation == "Percentage")
            {

                switch (val)
                {
                    case 1:
                        result = 1;
                        break;
                    case 2:
                        result = 2;
                        break;
                    case 3:
                        result = 4;
                        break;
                    case 4:
                        result = 12;
                        break;
                    case 5:
                        result = 60;
                        break;
                    default:
                        break;
                }

                return string.Format("{0}%", result * 100);
            }
            if (operation == "ToolTip")
            {
                switch (val)
                {
                    case 1:
                        result = 60;
                        break;
                    case 2:
                        result = 30;
                        break;
                    case 3:
                        result = 15;
                        break;
                    case 4:
                        result = 5;
                        break;
                    case 5:
                        result = 1;
                        break;
                    default:
                        break;
                }

                return string.Format(Luna.Globalization.LanguageReader.GetValue("ZoomUnit"), result);
            }
            if (operation == "ZoomValue")
            {
                switch (val)
                {
                    case 1:
                        result = 1;
                        break;
                    case 2:
                        result = 2;
                        break;
                    case 4:
                        result = 3;
                        break;
                    case 12:
                        result = 4;
                        break;
                    case 60:
                        result = 5;
                        break;
                    default:
                        break;
                }
                return result;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = System.Convert.ToInt32(value);
            int result = 1;

            string operation = (string)parameter;
            if (operation == "ZoomValue")
            {
                switch (val)
                {
                    case 1:
                        result = 1;
                        break;
                    case 2:
                        result = 2;
                        break;
                    case 3:
                        result = 4;
                        break;
                    case 4:
                        result = 12;
                        break;
                    case 5:
                        result = 60;
                        break;
                    default:
                        break;
                }
                return result;
            }
            return value;
        }
    }
}
