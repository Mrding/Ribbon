using System;
using Castle.Windsor;

namespace Luna.Basic.GuyWire
{
    public abstract class AbstractConfigurator : IConfigurator
    {
        public event EventHandler AfterConfiguration;

        protected virtual void Customize(IWindsorContainer container)
        {

        }

        public virtual void Configure(IWindsorContainer container)
        {
            if (AfterConfiguration != null)
                AfterConfiguration(this, EventArgs.Empty);
        }
    }
}
