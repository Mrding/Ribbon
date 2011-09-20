using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Luna.Common.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Converters
{
    //public class DayBackgroundConverter : IValueConverter
    //{
    //    private static readonly SolidColorBrush[] Brushes = new[] { new SolidColorBrush("#FFF4F9FF".ToColor()), new SolidColorBrush("#FFF9FFF6".ToColor()) };

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var currentCount = System.Convert.ToInt32(parameter);
    //        return Brushes[currentCount % 2];
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    public class TodayToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush NotTodayBrush = new SolidColorBrush("#FFE7E7E7".ToColor());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;
            if (date.Date == DateTime.Today)
                return null;
            //if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            //    return Brushes.SeaShell;
            return NotTodayBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class HolidayToBurshConverter : IValueConverter
    //{
    //    private static readonly SolidColorBrush NotTodayBrush = new SolidColorBrush(Colors.White);//
    //    private static readonly SolidColorBrush WeekendBrush = new SolidColorBrush(Colors.White);

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var date = (DateTime)value;
    //        //if (date.Date == DateTime.Today)
    //        //    return null;
    //        if (date.IsHoliday())
    //            return WeekendBrush;
    //        return NotTodayBrush;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
