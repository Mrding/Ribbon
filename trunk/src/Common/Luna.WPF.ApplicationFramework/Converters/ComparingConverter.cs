using System;
using System.Globalization;
using System.Windows.Data;
using System.Linq;
using System.Collections;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class TypeIsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || string.IsNullOrEmpty(parameter.ToString()))
                return false;
            return value.GetType().Name.Contains(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ComparingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (1 < values.Length)
            {
                return Equals(values[0], values[1]);
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ContainsConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="values">Source of list is value[0], Comparing obj is value[1]</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Type of list</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var listType = parameter as Type;

            if (1 < values.Length && listType != null)
            {
                var found = false;

                if (listType == typeof(string))
                {
                    var charList = System.Convert.ToString(values[1]);
                    var item = System.Convert.ToString(values[0]);
                    found = charList.Contains(item);
                }
                else if (listType == typeof(Type))
                {
                    var list = values[0] as IEnumerable;
                   
                    if (list != null)
                    {
                        foreach (var o in list)
                        {
                            if (o.GetType() == values[1])
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
                return found;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}