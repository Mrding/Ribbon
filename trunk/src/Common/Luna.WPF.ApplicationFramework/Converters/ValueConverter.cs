using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class ValueConverter<TSource, TTarget> : IValueConverter
    {
        protected virtual TTarget Convert(TSource value, object parameter, CultureInfo culture)
        {
            return Convert(value,parameter,culture);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert((TSource)value, parameter, culture);
        }

        protected virtual TSource ConvertBack(TTarget value, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConvertBack((TTarget)value, parameter, culture);
        }
    }
}
