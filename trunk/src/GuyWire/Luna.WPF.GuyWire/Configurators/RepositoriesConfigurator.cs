using Castle.Windsor;

namespace Luna.WPF.GuyWire.Configurators
{
    public class RepositoriesConfigurator : Luna.Basic.GuyWire.Configurators.RepositoriesConfigurator
    {
        public override void Configure(IWindsorContainer container)
        {
            Register("Luna.Shifts",container);
            Register("Luna.Infrastructure", container);
            Register("Luna.Statistic",container);
        }
    }
}
