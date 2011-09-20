using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class EntityFillConvertor : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var container = value as IClozeContainer;
            if (container == null) return value;

            var enumerator = container.GetEnumerator();
            if (enumerator == null) return value;

            var items = new Dictionary<int, ICloze>();

            while (enumerator.MoveNext())
            {
                var item = (ICloze)enumerator.Current;
                items.Add(item.LocationIndex, item);
            }

            var results = new ICloze[container.Capacity];

            for (var i = 0; i < container.Capacity; i++)
            {
                results[i] = items.ContainsKey(i) ? items[i] : container.NewItem(i);
            }

            return new ObservableCollection<ICloze>(results);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
