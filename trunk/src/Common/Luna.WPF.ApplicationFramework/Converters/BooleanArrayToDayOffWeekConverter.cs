using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using Caliburn.Core;
using Luna.Common;

namespace Luna.WPF.ApplicationFramework.Converters
{

    public class CheckableItem : PropertyChangedBase, ISelectable
    {
        public object Content { get; set; }
        private bool? _isSelected;
        public bool? IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyOfPropertyChange(() => IsSelected); }
        }
    }


    public class BooleanArrayToDayOffWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as DependencyObject;
            var itemsControl = System.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(item);
            var dayOfWeek = (DayOfWeek)itemsControl.ItemContainerGenerator.IndexFromContainer(item);
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(dayOfWeek);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    
}
