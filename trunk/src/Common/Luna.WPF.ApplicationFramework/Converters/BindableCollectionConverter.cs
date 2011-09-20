using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Caliburn.PresentationFramework;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class BindableCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as IEnumerable == null) return null;

            Type constructed =  typeof (BindableCollection<>).MakeGenericType((Type)parameter);

            var x = constructed.GetConstructors()[1];
            var collection = x.Invoke(new[] { value });

            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}