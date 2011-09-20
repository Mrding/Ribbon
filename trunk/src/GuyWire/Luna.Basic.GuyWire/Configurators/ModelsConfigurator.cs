using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Common.Model;
using Luna.Data;
using Luna.Infrastructure.Domain.Impl;
using System.Collections.Generic;

namespace Luna.Basic.GuyWire.Configurators
{
    public abstract class ModelsConfigurator : AbstractConfigurator
    {
        private const string Model = "Model";
        protected void Register(string moduleName, IWindsorContainer container)
        {
            var domainTypes = Assembly.Load(new AssemblyName(string.Format("{0}.Domain", moduleName))).GetTypes();

            var modelService = new List<Type>(domainTypes.Length);

            foreach (var t in domainTypes)
            {
                if (t.IsDefined(typeof(IgnoreRegisterAttribute), false))
                    continue;

                if (t.IsInterface && t.Namespace != null && t.Namespace.EndsWith(Model))
                    modelService.Add(t);
                //xelse if (t.IsClass && t.BaseTypeIsEntity())
                    //xRegisterGenericModel(container, t);
            }

            var modelImpl = Assembly.Load(new AssemblyName(string.Format("{0}.Domain.Impl", moduleName))).GetTypes();

            foreach (var item in modelImpl)
            {
                if (!item.Name.EndsWith(Model)) continue;
                
                var impl = item;
                var service = modelService.FirstOrDefault(s => s.IsAssignableFrom(impl));
                if (service == null) continue;

                ComponentRegistration(impl, service, container);
                modelService.Remove(service);
                if (modelService.Count == 0)
                    break;
            }
        }

        protected virtual void ComponentRegistration(Type impl, Type service, IWindsorContainer container)
        {
            var registration = Component.For(service).ImplementedBy(impl).LifeStyle.Transient;
            container.Register(registration);
        }

        private static void RegisterGenericModel(IWindsorContainer container, Type type)
        {
            //var repositoryType = typeof(IRepository<>).MakeGenericType(type);
            //var typeArgs = new[] { type, repositoryType };
            //var service = typeof(IModelBase<>).MakeGenericType(type);
            //var impl = typeof(ModelBase<,>).MakeGenericType(typeArgs);
            //container.Register(Component.For(service).ImplementedBy(impl).LifeStyle.Transient);
        }
    }
}