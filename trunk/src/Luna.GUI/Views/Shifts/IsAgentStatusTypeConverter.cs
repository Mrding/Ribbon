using System;
using System.Windows.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;

namespace Luna.GUI.Views.Shifts
{
    public class IsAgentStatusTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is AgentStatusType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
