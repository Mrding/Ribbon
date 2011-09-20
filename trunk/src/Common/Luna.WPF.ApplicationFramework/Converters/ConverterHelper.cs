using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Caliburn.PresentationFramework;
using System.Collections;

namespace Luna.WPF.ApplicationFramework.Converters
{
   public class ConverterHelper
    {
       public static object ArrayToStringConvert(object value, Type targetType, object parameter, CultureInfo culture)
       {
           var array = value as string[];
           if (array == null)
               return null;
           return String.Join(", ", array);
       }

       public object BindableCollectionConvert(object value, Type targetType, object parameter, CultureInfo culture)
       {
           if (value as IEnumerable == null) return null;

           Type constructed = typeof(BindableCollection<>).MakeGenericType((Type)parameter);

           var x = constructed.GetConstructors()[1];
           var collection = x.Invoke(new[] { value });

           return collection;
       }

       public object BooleanStringConvert(object value, Type targetType, object parameter, CultureInfo culture)
       {
           var boolean = System.Convert.ToBoolean(value);

           if (parameter == null) return value;

           var stringArray = parameter.ToString().Split(';');

           if (stringArray.Length <= 1) return value;

           return stringArray[System.Convert.ToInt32(boolean)];
       }

    }
}
