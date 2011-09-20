using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg.Loquacious;
using uNhAddIns.WPF.Collections;

namespace Luna.WPF.GuyWire.Configurators
{
    public class NHibernateConfigurator : Basic.GuyWire.Configurators.NHibernateConfigurator
    {
        protected override void RegisterProperties(NHibernate.Cfg.Configuration configuration)
        {
            base.RegisterProperties(configuration);
            configuration.CollectionTypeFactory<WpfCollectionTypeFactory>();
        }
    }
}
