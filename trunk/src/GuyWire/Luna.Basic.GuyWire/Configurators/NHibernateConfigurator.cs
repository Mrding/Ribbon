using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Castle.Facilities.FactorySupport;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Listeners;
using NHibernate;
using NHibernate.Caches.SysCache;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using uNhAddIns.CastleAdapters;
using uNhAddIns.CastleAdapters.AutomaticConversationManagement;
using uNhAddIns.CastleAdapters.EnhancedBytecodeProvider;
using uNhAddIns.ComponentBehaviors.Castle.ProxyFactory;
using uNhAddIns.NHibernateTypeResolver;
using uNhAddIns.SessionEasier;
using uNhAddIns.SessionEasier.Conversations;

namespace Luna.Basic.GuyWire.Configurators
{
    public class NHibernateConfigurator : AbstractConfigurator
    {
        public override void Configure(IWindsorContainer container)
        {
            container.AddFacility<PersistenceConversationFacility>();
            container.AddFacility<FactorySupportFacility>();

            NHibernate.Cfg.Environment.BytecodeProvider = new EnhancedBytecode(container);
            var nhConfigurator = new DefaultSessionFactoryConfigurationProvider();

            //sessionFactory part
            var sessionFactoryProvider = new SessionFactoryProvider(nhConfigurator);
            container.Register(Component.For<ISessionFactory>().UsingFactoryMethod(() => sessionFactoryProvider.GetFactory(null)));
            container.Register(Component.For<ISessionFactoryImplementor>().UsingFactoryMethod(() => (ISessionFactoryImplementor)sessionFactoryProvider.GetFactory(null)));

            //uNhAddIns part
            container.Register(Component.For<ISessionFactoryProvider>().Instance(sessionFactoryProvider));
            container.Register(Component.For<ISessionWrapper>().ImplementedBy<SessionWrapper>());
            container.Register(Component.For<IConversationFactory>().ImplementedBy<DefaultConversationFactory>());
            container.Register(Component.For<IConversationsContainerAccessor>().ImplementedBy<NhConversationsContainerAccessor>());

            nhConfigurator.BeforeConfigure += (sender, e) =>
            {
                var nhCfg = e.Configuration;
                nhCfg.RegisterEntityNameResolver();
                RegisterProperties(nhCfg);
                RegisterMappings(nhCfg);
                RegisterListeners(nhCfg);
                RegisterEntityCaches(nhCfg);
                SchemaMetadataUpdater.QuoteTableAndColumns(nhCfg);
                e.Configured = true; // prevent read from Nhibernate.cfg
            };


            nhConfigurator.AfterConfigure += (sender, e) =>
            {
                var nhCfg = e.Configuration;

                //TryUpdateSchema
                var value = ConfigurationManager.AppSettings["SchemaUpdate"];
                if (value.IsNotNullOrEmpty() && value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
                    new SchemaUpdate(nhCfg).Execute(false, true);
            };

           

            base.Configure(container);
        }

        protected virtual void RegisterProperties(NHibernate.Cfg.Configuration configuration)
        {
            configuration.Proxy(p => p.ProxyFactoryFactory<ComponentProxyFactoryFactory>());
            configuration.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2005Dialect>();
                db.Driver<SqlClientDriver>();
                //db.ConnectionStringName = "ConnectionString";
                db.ConnectionString = Common.Constants.AppConfig.ConnectionString;
                db.BatchSize = 10;
            });
            configuration.CurrentSessionContext<ThreadLocalConversationalSessionContext>();

            configuration.Cache(cp =>
            {
                cp.UseQueryCache = true;
                cp.Provider<SysCacheProvider>();
            });
        }

        protected void RegisterMappings(NHibernate.Cfg.Configuration configuration)
        {
            //configuration.AddAssembly(Assembly.Load("Luna.Config"));//花费 1.306 秒
            //var timer2 = Stopwatch.StartNew();
            configuration.AddAssembly(Assembly.Load("Luna.Infrastructure.Data.Impl"));
            configuration.AddAssembly(Assembly.Load("Luna.Shifts.Data.Impl"));
            configuration.AddAssembly(Assembly.Load("Luna.Statistic.Data.Impl"));
            //timer2.Stop();
            //var timer3 = Stopwatch.StartNew();
            //configuration.AddDeserializedMapping(GetMapping(), "WFM8200_Domain");
            //timer3.Stop();
            //Console.WriteLine("读取映射花费 {0}.{1} 秒.", timer2.Elapsed.Seconds, timer2.Elapsed.Milliseconds);
            //Console.WriteLine("映射花费 {0}.{1} 秒.", timer3.Elapsed.Seconds, timer3.Elapsed.Milliseconds);

        }
        protected void RegisterEntityCaches(NHibernate.Cfg.Configuration configuration)
        {
           //xconfiguration.EntityCache<AgentStatusType>(eccp => eccp.Strategy = EntityCacheUsage.ReadWrite);
            //configuration.EntityCache<TermStyle>(eccp => eccp.Strategy = EntityCacheUsage.ReadWrite);
        }

        protected void RegisterListeners(NHibernate.Cfg.Configuration configuration)
        {
            var listeners = configuration.EventListeners;
            var alteringListener = new TermAlteringListener();
            var backupListener = new TermBackupListener();
            //xvar arrangementListener = new SearArrangementListener();
            listeners.PostUpdateEventListeners =
                new[] { alteringListener }.Concat(listeners.PostUpdateEventListeners).ToArray();
            listeners.PostDeleteEventListeners =
                new[] { alteringListener }.Concat(listeners.PostDeleteEventListeners).ToArray();
            //xlisteners.PreDeleteEventListeners = new[] { arrangementListener }.Concat(listeners.PreDeleteEventListeners).ToArray();
            listeners.PreDeleteEventListeners =
                new[] { backupListener }.Concat(listeners.PreDeleteEventListeners).ToArray();
            listeners.PreUpdateEventListeners =
                new[] { alteringListener }.Concat(listeners.PreUpdateEventListeners).ToArray();
            listeners.PostInsertEventListeners =
                new[] { alteringListener }.Concat(listeners.PostInsertEventListeners).ToArray();
            listeners.PostInsertEventListeners =
                new[] { backupListener }.Concat(listeners.PostInsertEventListeners).ToArray();
        }

        
    }
}