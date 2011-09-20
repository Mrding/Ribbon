using System;
using System.Collections.Generic;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Luna.Common.Interfaces;
using uNhAddIns.Adapters;

namespace Luna.Basic.GuyWire
{
    public abstract class GeneralGuyWire : IGuyWire, IContainerAccessor, IInitializeNotification
    {
        protected readonly List<IConfigurator> Configurators = new List<IConfigurator>(16);
        private IWindsorContainer _container;
        public event EventHandler Initialized;

        public bool IsInitialized
        {
            get;
            private set;
        }

        public IWindsorContainer Container
        {
            get
            {
                if (_container == null)
                    _container = new WindsorContainer(new XmlInterpreter());
                return _container;
            }
        }

        protected abstract void OnWire();

        public virtual void Wire()
        {
            if (!IsInitialized)
            {
                OnWire();
                
                Configurators.ForEach(c => c.Configure(Container));
                IsInitialized = true;
                if (Initialized != null)
                    Initialized(this, EventArgs.Empty);
            }
        }

        public void Dewire()
        {
            if (Container != null)
                Container.Dispose();
        }

        protected void AddConfigurator(IConfigurator configurator)
        {
            Configurators.Add(configurator);
        }

        protected void RemoveConfigurator(IConfigurator configurator)
        {
            Configurators.Remove(configurator);
        }
    }
}
