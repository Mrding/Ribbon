using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Core;
using Caliburn.Core.Invocation;

namespace Luna.Common.Extensions
{
    public static class NotifyPropertyChangedExtension
    {
        private const string PropertyChangedEventName = "PropertyChanged";
        private const BindingFlags BindingAttr = BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance;

        public static void NotifyOfPropertyChange<T, TProperty>(this T target, Expression<Func<T, TProperty>> expression)
            where T : INotifyPropertyChanged
        {
            var propertyName = expression.GetMemberInfo().Name;
            var propertyChangedBase = target as PropertyChangedBase;
            if (propertyChangedBase != null)
            {
                propertyChangedBase.NotifyOfPropertyChange(propertyName);
                return;
            }

            FieldInfo eventField;
            var type = target.GetType();
            do
            {
                eventField = type.GetField(PropertyChangedEventName, BindingAttr);
                type = type.BaseType;

            } while (type != typeof(object) && eventField == null);


            var eventHandlers = eventField.GetValue(target) as MulticastDelegate;

            if (eventHandlers != null)
            {
                foreach (var handler in eventHandlers.GetInvocationList())
                {
                    var @delegate = handler;
                    Execute.OnUIThread(() => @delegate.DynamicInvoke(target, new PropertyChangedEventArgs(propertyName)));
                }
            }
        }

        //public static void NotifyPropertyChanged<T>(this T target, string propertyName)
        //    where T : INotifyPropertyChanged
        //{
        //    var eventField = target.GetType().GetField(StrPropertyChangedEventName, BindingFlags.Instance | BindingFlags.NonPublic);
        //    var eventHandlers = eventField.GetValue(target) as MulticastDelegate;

        //    if (eventHandlers != null)
        //    {
        //        foreach (var handler in eventHandlers.GetInvocationList())
        //        {
        //            handler.DynamicInvoke(target, new PropertyChangedEventArgs(propertyName));
        //        }
        //    }
        //}
    }
}
