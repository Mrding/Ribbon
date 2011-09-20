namespace Luna.WPF.ApplicationFramework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Luna.Core.Extensions;

    using Luna.Common.Interfaces;

    public class IndexToDateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2) return null;
            
            var index = values[1];

            return values[0].SaftyGetProperty<object, IIndexer>(o => o.GetItem(index, (Type)parameter));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
