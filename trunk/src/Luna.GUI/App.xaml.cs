using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Caliburn.Castle;
using Castle.Windsor;
using Luna.Adapters.Login;
using Luna.Common.Interfaces;
using Luna.Core.Extensions;
using Luna.Shifts.Presenters;
using Luna.WPF.ApplicationFramework;
using Luna.WPF.ApplicationFramework.Threads;
using Microsoft.Practices.ServiceLocation;
using uNhAddIns.Adapters;

namespace Luna.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : BootstrapperApplication
    {
        /// <summary>
        /// Current Application
        /// </summary>
        public static new App Current
        {
            get { return Application.Current as App; }
        }

        private IGuyWire _guyWire;

        protected override IServiceLocator CreateContainer()
        {
            _guyWire = ApplicationConfiguration.GetGuyWire();


            // Dispatcher.BeginInvoke(()=>{  });

            var adp = new WindsorAdapter(_guyWire.SaftyGetProperty<IWindsorContainer, IContainerAccessor>(g =>
                                                                                                              {
                                                                                                                  g.Container.Register(Castle.MicroKernel.Registration.Component.For(typeof(INotifyNhBuildComplete))
                                                                                                                      .Instance(_guyWire).LifeStyle.Singleton);
                                                                                                                  return g.Container;
                                                                                                              }));

          

            return adp;
        }

        protected override void CoreStarted()
        {
            new Thread(() =>
            {
                var sw = Stopwatch.StartNew();
                _guyWire.Wire();

                sw.Stop();
                Debug.Print("GuyWire.Wire spend {0} seconds.", sw.Elapsed.TotalSeconds);
            }) { IsBackground = true }.Start();

            BeforeFrameworkInitialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _guyWire.Dewire();
        }

        protected override void BeforeFrameworkInitialize()
        {
            AppStartManager.LoadCultureInfo();
            new LoginManager().Login(AttachLoginSuccess);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            
        }

        private void AttachLoginSuccess()
        {
            _guyWire.SaftyInvoke<IInitializeNotification>(o =>
            {
                if (o.IsInitialized)
                    LoginSuccess();
                else
                    o.Initialized += (s, e) => LoginSuccess();
            });
        }

        private void LoginSuccess()
        {
            var rootModel = CreateRootModel();
            if (rootModel == null) return;

            //xAppStartManager.BeforeInit();
            //ThreadPool.UnsafeQueueUserWorkItem(o => Current.LoadModuleAssemblies(), null);
            Current.LoadModuleAssemblies();

            //xUIThread.BeginInvoke(LoadResource);
            UIThread.Invoke(() =>
                                {
                                    LoadResource();
                                    ShowMainWindow(rootModel);
                                });

            //xAppStartManager.AfterInit();

            AttachInitializeModules();
            //Luna.Infrastructure.Presenters.VersionHelper.CheckVersion();
        }

        private void AttachInitializeModules()
        {
            _guyWire.SaftyInvoke<INotifyNhBuildComplete>(o =>
            {
                if (o.IsBuildCompleted)
                    InitializeModules();//Dispatcher.Invoke(InitializeModules, DispatcherPriority.Send);
                else
                    o.NHibernateBuildCompleted += (s, e) => InitializeModules();
            });
        }

        private void LoadResource()
        {
            AddResourceDictionary(@".\Resources\ApplicationResource.xaml");
            AddResourceDictionary(new Uri("/Luna.GUI;component/Resources/ApplicationStyle.xaml", UriKind.Relative));
            AddResourceDictionary(new Uri("/Luna.GUI;component/Resources/DomainResource.xaml", UriKind.Relative));

            ActiproSoftware.Products.Editors.SR.SetCustomString(ActiproSoftware.Products.Editors.SRName.UICalendarTodayButtonFormat.ToString(), "{0:M/dd}");

            var standardFont = new FontFamily(Luna.Common.Constants.Global.FamilyFonts);
            TextElement.FontFamilyProperty.OverrideMetadata(typeof(TextElement), new FrameworkPropertyMetadata(standardFont));
            TextBlock.FontFamilyProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(standardFont));
            TextElement.FontSizeProperty.OverrideMetadata(typeof(TextElement), new FrameworkPropertyMetadata(12.0d));
            TextBlock.FontSizeProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(12.0d));
        }
    }
}