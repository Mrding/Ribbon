using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Luna.Basic.GuyWire;
using Luna.Common;
using Luna.Common.Interfaces;
using Luna.Infrastructure.Data.Impl.Repositories;
using Luna.Statistic.Domain;
using Luna.Statistic.Domain.Service;
using Luna.Statistic.Services;
using Luna.WPF.ApplicationFramework;
using Luna.ComponentBehaviors;
using uNhAddIns.ComponentBehaviors.Castle.Configuration;

namespace Luna.WPF.GuyWire.Configurators
{
    public class ComponentsConfigurator : AbstractConfigurator
    {
        public override void Configure(IWindsorContainer container)
        {
            container.Register(Component.For(typeof(IOpenFileService)).ImplementedBy(typeof(DefaultOpenFileService)).LifeStyle.Transient);

            container.Register(Component.For(typeof(IStaffingCalculatorService)).ImplementedBy(typeof(StaffingCalculatorService)).LifeStyle.Transient);

            container.Register(Component.For<IExpandMethod>()
                                    .Named("AgentStatus")
                                    .ImplementedBy<EmployeeRepository>()
                                    .LifeStyle.Singleton);

            container.Register(Component.For(typeof(IVisibleLinerData)).ImplementedBy(typeof(VisibleData)).AddNotificableBehavior().AddSelectableBehavior().LifeStyle.Transient);

            //container.Register(Component.For(typeof(Luna.Core.Tuple<string,IList>)).AddNotificableBehavior().LifeStyle.Transient);
            //container.Register(Component.For<QueueStatistic>().AddNotificableBehavior().AddSelectableBehavior().LifeStyle.Transient);)
         

            base.Configure(container);
        }
    }
}