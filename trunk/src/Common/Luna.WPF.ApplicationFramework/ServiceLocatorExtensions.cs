using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework
{
    public static class ServiceLocatorExtensions
    {
        public static object TryResolve(this IServiceLocator locator, Type type)
        {
            try
            {
                return locator.GetInstance(type);
            }
            catch (ActivationException)
            {
                return null;
            }
        }

        public static T TryResolve<T>(this IServiceLocator locator) where T : class
        {
            return locator.TryResolve(typeof(T)) as T;
        }
    }
}
