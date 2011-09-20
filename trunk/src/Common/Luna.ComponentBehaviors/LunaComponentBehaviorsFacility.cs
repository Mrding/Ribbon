using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;

namespace Luna.ComponentBehaviors
{
    public class LunaComponentBehaviorsFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(Component.For<EditingBehavior>().LifeStyle.Transient);
            Kernel.Register(Component.For<IndexableBehavior>().LifeStyle.Transient);
            Kernel.Register(Component.For<SelectableBehavior>().LifeStyle.Transient);
            Kernel.Register(Component.For<AopBehavior>().LifeStyle.Transient);
            Kernel.Register(Component.For<AgentStatusBehavior>().LifeStyle.Transient);
            Kernel.Register(Component.For<DebugBehavior>().LifeStyle.Transient);
        }
    }
}
