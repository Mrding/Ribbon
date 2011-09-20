using System;
using System.Diagnostics;
using System.Threading;
using Luna.Basic.GuyWire;
using Luna.Common.Interfaces;
using Luna.Infrastructure.Domain;
using Luna.WPF.GuyWire.Configurators;
using NHibernate;
using uNhAddIns.SessionEasier;

namespace Luna.WPF.GuyWire
{
    public class WpfGuyWire : GeneralGuyWire, INotifyNhBuildComplete
    {
        protected override void OnWire()
        {
            AddConfigurator(new NHibernateConfigurator());
            AddConfigurator(new EntitiesConfigurator());
            AddConfigurator(new RepositoriesConfigurator());
            AddConfigurator(new ModelsConfigurator());
            AddConfigurator(new ViewsConfigurator());
            AddConfigurator(new ComponentsConfigurator());
            Prefetch();
        }

        private void Prefetch()
        {
            //After NHibernateConfigurator
            Configurators[1].AfterConfiguration += OnAfterConfiguration;

            //After EntitiesConfigurator
            Configurators[2].AfterConfiguration += (s, e) =>  Container.Resolve<Employee>();
        }

        private void OnAfterConfiguration(object s, EventArgs e)
        {
            //ThreadPool.QueueUserWorkItem(o =>
           //                                  {
#if DEBUG
                                                 var sw = Stopwatch.StartNew();
#endif
                                                 var session = Container.Resolve<ISessionFactory>().OpenSession();
                                                 using (Container.Resolve<ISessionWrapper>().Wrap(session, null, null))
                                                 {
                                                     IsBuildCompleted = true;
                                                     if (NHibernateBuildCompleted != null)
                                                         NHibernateBuildCompleted(this, EventArgs.Empty);
                                                 }
#if DEBUG
                                                 sw.Stop();
                                                 Debug.Print(string.Format("firt time open session elapsed: {0}s",sw.Elapsed.TotalSeconds));
#endif
                                             //});
        }

        public event EventHandler<EventArgs> NHibernateBuildCompleted;

        public bool IsBuildCompleted { get; private set; }

        public void AfterBuildComplete(Action action)
        {
            if (IsBuildCompleted)
                action();
            else
                NHibernateBuildCompleted += (sender, e) => action();
        }
    }
}
