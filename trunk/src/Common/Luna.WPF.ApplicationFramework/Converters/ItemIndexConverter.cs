using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class ItemIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as DependencyObject;
            if (item == null) return 0;

            var itemsControl = System.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(item);
            return itemsControl.ItemContainerGenerator.IndexFromContainer(item);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}