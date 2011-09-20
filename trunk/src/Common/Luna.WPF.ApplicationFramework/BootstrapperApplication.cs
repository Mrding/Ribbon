using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Caliburn.Core;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.WPF.ApplicationFramework.Actions;
using Luna.WPF.ApplicationFramework.Controls;
using Luna.WPF.ApplicationFramework.Helpers;
using Luna.WPF.ApplicationFramework.Interfaces;
using Luna.Core.Extensions;
using Caliburn.PresentationFramework.Metadata;

namespace Luna.WPF.ApplicationFramework
{
    public class BootstrapperApplication : CaliburnApplication
    {

        static BootstrapperApplication()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Process.GetCurrentProcess().PriorityBoostEnabled = true;
            //Since I have 4 tasks when start, so I set 4 min threads to do task in the same time.
            if (Environment.ProcessorCount < 4)
                ThreadPool.SetMinThreads(4, 4);
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected virtual void InitializeModules()
        {
            //var manager = Container.GetInstance<IModuleManager>();
            //manager.Run();
        }

        protected IEnumerable<Assembly> GetAllModuleAssemblies()
        {
            return new[] { Assembly.Load(new AssemblyName("Luna.Shifts")), 
                           Assembly.Load(new AssemblyName("Luna.Infrastructure")), 
                           Assembly.Load(new AssemblyName("Luna.Statistic")) };
        }

        protected override void ConfigurePresentationFramework(PresentationFrameworkModule module)
        {
            module.UsingViewStrategy<ViewStrategy>();
            module.UsingActionFactory<ActionFactory>();
        }

        protected override object CreateRootModel()
        {
            var binder = (DefaultBinder)Container.GetInstance<IBinder>();
            binder.EnableMessageConventions();
            binder.EnableBindingConventions();

            return Container.GetInstance<IShellPresenter>();
        }

        protected override void ShowMainWindow(object rootModel)
        {
            base.ShowMainWindow(rootModel);
            rootModel.SaftyInvoke<IMetadataContainer>(p =>
                                                          {
                                                              MainWindow = p.GetView<Window>(null);
                                                              MainWindow.Owner = null;
                                                              MainWindow.Closed += (s, e) => AppManager.Shutdown();
                                                          });
        }

        public virtual void LoadModuleAssemblies()
        {
            var assemblySource = Container.GetInstance<IAssemblySource>();
            GetAllModuleAssemblies().Apply(assemblySource.Add);
        }

        private void AddResourceDictionary(ResourceDictionary dictionary)
        {
            if(dictionary == null) return;
            EnsureMergedDictionaries();
            Resources.MergedDictionaries.Add(dictionary);
        }

        protected void AddResourceDictionary(Uri uri)
        {
            AddResourceDictionary(new ResourceDictionary { Source = uri });
        }

        protected void AddResourceDictionary(string fileName)
        {
            if (!File.Exists(fileName)) return;

            using (var fs = File.OpenRead(fileName))
            {
                AddResourceDictionary(XamlReader.Load(fs) as ResourceDictionary);
            }
        }

        protected void RemoveResourceDictionary(ResourceDictionary dictionary)
        {
            EnsureMergedDictionaries();
            var index = Resources.MergedDictionaries.IndexOf(dictionary);

            if (index == -1) return;
            Resources.MergedDictionaries[index] = new ResourceDictionary();
        }

        protected void EnsureMergedDictionaries()
        {
            if (Resources == null)
            {
                Resources = new ResourceDictionary();
            }

            if (Resources.MergedDictionaries.Count == 0)
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary());
            }
        }

        //public void DoEvents()
        //{
        //    var frame = new DispatcherFrame();
        //    Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DoEventsDelegate(DoEventsCore), frame);
        //    Dispatcher.PushFrame(frame);
        //}

        private static object DoEventsCore(object arg)
        {
            var frame = arg as DispatcherFrame;
            if (frame != null)
                frame.Continue = false;

            return null;
        }

        //private ITrayIcon _trayIcon;
        //public ITrayIcon TrayIcon
        //{
        //    get
        //    {
        //        if (_trayIcon == null) _trayIcon = Controls.TrayIcon.CreateTrayIcon();
        //        return _trayIcon;
        //    }
        //    set
        //    {
        //        _trayIcon = value;
        //    }
        //}
    }
}