using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Luna.Core.Extensions;


namespace Luna.WPF.ApplicationFramework.Converters
{
    public class ArrayToCheckItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var type = parameter as Type;
            if (type == null) return null;
            var enumArray = Enum.GetValues(type);
            var array = value as bool[];
            if (array == null || array.Length != enumArray.Length) return null;

            var items = new CheckableItem[enumArray.Length];

            enumArray.ForEach<Enum>((e, i) =>
                                                   {
                                                       var content = default(string);
                                                       e.SaftyInvoke<DayOfWeek>(d => content = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(d));
                                                       if (string.IsNullOrEmpty(content))
                                                           content = type.GetDescription(e);
                                                       items[i] = new CheckableItem {Content = content, IsSelected = array[i]};
                                                   });
            return items;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}