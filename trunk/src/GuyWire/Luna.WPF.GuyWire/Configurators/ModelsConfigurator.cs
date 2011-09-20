using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Luna.Common;
using Luna.Common.Attributes;
using Luna.Common.Model;
using Luna.ComponentBehaviors;
using Luna.Infrastructure.Domain.Impl;
using Luna.Infrastructure.Domain.Model;
using uNhAddIns.ComponentBehaviors.Castle.Configuration;

namespace Luna.WPF.GuyWire.Configurators
{
    public class ModelsConfigurator : Basic.GuyWire.Configurators.ModelsConfigurator
    {
        public override void Configure(IWindsorContainer container)
        {
            Register("Luna.Shifts", container);
            Register("Luna.Infrastructure", container);
            Register("Luna.Statistic", container);

            container.Register(Component.For(typeof(IBackendModel)).ImplementedBy(typeof(BackendModel)).LifeStyle.Singleton);
            container.Register(Component.For(typeof(IAuditLogModel)).ImplementedBy(typeof(AuditLogModel)).LifeStyle.Singleton);
        }

        protected override void ComponentRegistration(Type impl, Type service, IWindsorContainer container)
        {
            var registration = Component.For(service).ImplementedBy(impl);

            if (impl.IsDefined(typeof(NotificableAttribute), true))
            {
                var notificableAttribute = impl.GetCustomAttributes(typeof(NotificableAttribute), true).OfType<NotificableAttribute>().FirstOrDefault();

                if (notificableAttribute.AdditionalType != null)
                    registration = registration.AddNotificableBehavior()
                                            .Proxy.AdditionalInterfaces(typeof(IDisposable), notificableAttribute.AdditionalType);
                else
                    registration = registration.AddNotificableBehavior()
                                            .Proxy.AdditionalInterfaces(typeof(IDisposable));
            }
            else
                registration = registration.Proxy.AdditionalInterfaces(typeof(IDisposable));

            if (impl.IsDefined(typeof(AopAttribute), true))
                    registration = registration.AddAopBehavior();

            container.Register(registration.LifeStyle.Transient);
        }
    }
}
