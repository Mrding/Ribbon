using System;
using Castle.Windsor;

namespace Luna.Basic.GuyWire
{
    public interface IConfigurator
    {
        event EventHandler AfterConfiguration;
        void Configure(IWindsorContainer container);
    }
}