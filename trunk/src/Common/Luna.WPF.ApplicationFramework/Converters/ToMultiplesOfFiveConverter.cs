using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ActiproSoftware.Windows.Controls.Editors.Primitives;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class ToMultiplesOfFiveConverter : IStringValueConverter<DateTime?>
    {
      

        #region Implementation of IStringValueConverter<DateTime>

        public bool CanConvertFromString(string text)
        {
            return true;
        }

        public bool CanConvertToString(DateTime? value)
        {
            return true;
        }

        public DateTime? ConvertFromString(string text)
        {
            var datetimeValue = DateTime.Parse(text);

            var remainder = datetimeValue.Minute % 5;

            if (remainder > 0)
                datetimeValue = datetimeValue.AddMinutes(-remainder);

            return new DateTime?(datetimeValue);
        }

        public string ConvertToString(DateTime? dateTime)
        {
            var remainder = dateTime.Value.Minute % 5;

            if (remainder > 0)
                dateTime = new DateTime?(dateTime.Value.AddMinutes(-remainder));

            return dateTime.ToString();
        }

        #endregion
    }
}
