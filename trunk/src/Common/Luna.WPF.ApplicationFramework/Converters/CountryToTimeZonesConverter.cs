using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    public class CultureCountryConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var array = value as Array;
            if (array == null) return value;

            var countries = new List<string>();
            foreach (string v in array)
            {
                var cultureInfo = CultureInfo.GetCultureInfo(v);
                if (cultureInfo.DisplayName.Contains("("))
                {
                    var name = cultureInfo.DisplayName.Split('(')[1];
                    countries.Add(name.Substring(0, name.Length - 1));
                }
                else 
                    countries.Add(cultureInfo.DisplayName);
            }
            return countries;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
