using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Data;
namespace Luna.Basic.GuyWire.Configurators
{
    public class RepositoriesConfigurator : AbstractConfigurator
    {
        static protected void Register(string moduleName, IWindsorContainer container)
        {
            var repositoryService = Assembly.Load(new AssemblyName(string.Format("{0}.Data", moduleName))).GetTypes()
                                            .Where(t => t.IsInterface).ToList();
            

            foreach (var type in  Assembly.Load(new AssemblyName(string.Format("{0}.Data.Impl", moduleName))).GetTypes())
            {
                if(!type.Name.EndsWith("Repository")) continue;
                var impl = type;
                var service = repositoryService.FirstOrDefault(s => s.IsAssignableFrom(impl));
                if (service == null) continue;

                container.Register(Component.For(service).ImplementedBy(impl).LifeStyle.Transient); // ioc Register
                repositoryService.Remove(service);
                if (repositoryService.Count == 0)
                    break;
            }

            /*foreach (var t in Assembly.Load(new AssemblyName(string.Format("{0}.Domain", moduleName))).GetTypes())
            {
                //if (t.IsDefined(typeof(IgnoreRegisterAttribute), false))
                //    continue;

                if (t.IsClass && t.BaseTypeIsEntity())
                    RegisterGenericRepository(container, t);
            }*/
        }

        private static void RegisterGenericRepository(IWindsorContainer container, Type type)
        {
            var typeArgs = new[] { type };
            var service = typeof(IRepository<>).MakeGenericType(typeArgs);
            var impl = typeof(Repository<>).MakeGenericType(typeArgs);
            container.Register(Component.For(service).ImplementedBy(impl).LifeStyle.Transient);
        }
    }
}