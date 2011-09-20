using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework.Converters
{
    public class BooleanInverseConverter : ValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, object parameter, System.Globalization.CultureInfo culture)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, object parameter, System.Globalization.CultureInfo culture)
        {
            return !value;
        }
    }
}
