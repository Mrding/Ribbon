using System;
using System.Collections;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using System.Reflection;
using System.Collections.Generic;

namespace Luna.Common
{
    public class EntityFactory : IEntityFactory
    {
        private readonly IWindsorContainer _serviceLocator;

        public EntityFactory(IWindsorContainer serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public T Create<T>(Type type)
        {
            object entity = null;
            try
            {
                entity = _serviceLocator.Resolve(type);
            }
            catch (ActivationException)
            {
               entity = Activator.CreateInstance(type);
            }
            return entity is T ? (T)entity : default(T);
        }

        public T Create<T>()
        {
            try
            {
                return _serviceLocator.Resolve<T>();
            }
            catch (ActivationException)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public T Create<T>(IDictionary args)
        {
            try
            {
                return _serviceLocator.Resolve<T>(args);
            }
            catch (ActivationException)
            {
                return Activator.CreateInstance<T>();
            }
        }
        
        public T Create<T>(object source)
        {
            try
            {
                var args = new Dictionary<string, object>();
                foreach (var pi1 in source.GetType().GetProperties())
                {
                    if (!pi1.CanRead) continue;
                    args[pi1.Name] = pi1.GetValue(source, null);
                }

                return _serviceLocator.Resolve<T>(args);
            }
            catch (ActivationException)
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}