using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ActiproSoftware.Products.Docking;
using ActiproSoftware.Windows.Themes;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common.Constants;
using Luna.Core.Extensions;
using Luna.Globalization;
using Luna.Common;
using Luna.Infrastructure.Domain;
using System.Collections.Generic;
using Luna.WPF.ApplicationFramework.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Luna.GUI
{
    internal class AppStartManager
    {
        #region Fields

        private const string ActriproLanguageKey = "Actripro_{0}_{1}";

        #endregion Fields

        #region Methods

        public static void AfterInit()
        {
            //AttachGc(TimeSpan.FromMinutes(0.1));
            //Dispatcher.CurrentDispatcher.BeginInvoke(() =>
            //{
            //    //LoadTrayIconMenu();
            //    //LoadTheme();
            //    AttachGc(TimeSpan.FromMinutes(0.1));
            //    //WeakReferenceManager.ScheduleCleanup();
            //    //ThreadPool.UnsafeQueueUserWorkItem(o => { Console.WriteLine("Current server time: is {0}", Server.CurrentTime); }, null);
            //}, DispatcherPriority.Background);
        }

        public static void BeforeInit()
        {
            AttachUnhandledException();
        }

        //private static void LoadDefaultTrayIcon()
        //{
        //    var bi = new BitmapImage();
        //    bi.BeginInit();
        //    bi.UriSource = new Uri(@"pack://application:,,,/Resources/Images/WMSLogo.ico", UriKind.RelativeOrAbsolute);
        //    bi.EndInit();
        //    App.Current.TrayIcon.Icon = bi;
        //}

        public static void LoadCultureInfo()
        {
            var language = ConfigurationManager.AppSettings["Language"];

            LanguageReader.Load(language);

            Country.Local = new Country(Application.Current.Dispatcher.Thread.CurrentCulture, new string[0]).ToString();
            ApplicationCache.Set(Global.GlobalCalendar, new Dictionary<DateTime, Dictionary<string, bool>>(365));
        }

        private static void AttachGc(TimeSpan interval)
        {
            var timer = new DispatcherTimer(DispatcherPriority.SystemIdle) {Interval = interval};
            timer.Tick += (sender, args) => ThreadPool.UnsafeQueueUserWorkItem(state =>
                                                                                   {
                                                                                       GC.Collect(GC.MaxGeneration,GCCollectionMode.Optimized);
                                                                                       GC.WaitForPendingFinalizers();
                                                                                       GC.Collect(GC.MaxGeneration,GCCollectionMode.Optimized);
                                                                                   }, null);
            timer.Start();
        }

        private static void AttachUnhandledException()
        {
            //AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            //{
            //    //var ex =
            //    //    new Exception(String.Format(LanguageReader.GetValue("ApplicationMultiThreadError"), args.ExceptionObject));
            //    HandleException(e.ExceptionObject as Exception);
            //};

            App.Current.DispatcherUnhandledException += (sender, args) =>
                                                            {
                                                                args.Handled = true;
                                                                HandleException(args.Exception);
                                                            };
        }

        //private static void LoadTrayIconMenu()
        //{
        //    Dispatcher.CurrentDispatcher.BeginInvoke(() =>
        //    {
        //        App.Current.TrayIcon.ContextMenu = App.Current.FindResource("TrayIconMenu") as ContextMenu;
        //        Caliburn.PresentationFramework.Actions.Action.SetTargetWithoutContext(
        //            App.Current.TrayIcon.ContextMenu,
        //            ServiceLocator.Current.GetInstance<object>("TrayIconPresenter"));
        //    }, DispatcherPriority.ApplicationIdle);
        //}

        private static void HandleException(Exception exception)
        {
            //加这一句，是因为FileDialog会改变Directory的当前目录，这样会找不到EnterpriseLibrary.config。
            //Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            //ex.HelpLink = string.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            //ExceptionPolicy.HandleException(ex, "Unhandled Exception Policy");

            ServiceLocator.Current.GetInstance<IMessagePresenter>()
                    .Self(q =>
                    {
                        //q.DisplayName = LanguageReader.GetValue(Title);
                        q.Text = "Exception";
                        q.Details = string.Format("{0}:{1}\r\n{2}", exception.GetType(), exception.Message, exception.StackTrace);
                        ServiceLocator.Current.GetInstance<IWindowManager>().ShowDialog(q);
                        
                    });

            //string errorMessage = ex.GetExceptionMessage();
            //var result = MsgBoxWindow.Show(LanguageReader.GetValue("ApplicationErrorInfo"),
            //                             LanguageReader.GetValue("ApplicationErrorTitle"),
            //                             errorMessage,
            //                             MessageBoxButton.YesNo, MessageBoxImage.Error);
            //if (result == MessageBoxResult.Yes)
            //{
            //    if (Thread.CurrentThread.ManagedThreadId == Application.Current.Dispatcher.Thread.ManagedThreadId)
            //        App.Current.Shutdown();
            //    else
            //        Process.GetCurrentProcess().Kill();
            //}
        }

        //private static void LoadApplcationStyle()
        //{

        //}

        //private static void LoadAppResource()
        //{
        //    ////Load default resource
        //    //LanguageReader.LoadResource(RESOURCE_FILE);
        //}

        private static void LoadGlobalResource()
        {
            //LoadApplcationStyle();
            //SetActriproResource();
            //LoadAppResource();

            //Dispatcher.CurrentDispatcher.BeginInvoke(LoadDefaultTrayIcon, DispatcherPriority.ApplicationIdle);
        }

        private static void SetActriproResource()
        {
            Application.Current.Dispatcher.BeginInvoke(
                () => ThemeManager.CurrentTheme = CommonThemeName.AeroNormalColor.ToString(),
                DispatcherPriority.ApplicationIdle);


            if (!ConfigurationManager.AppSettings["Language"].Contains("en"))
            {
                //Docking
                Resources.Culture = CultureInfo.CurrentCulture;

                string[] names = Enum.GetNames(typeof (SRName));
                foreach (string name in names)
                {
                    string fullKeyName = string.Format(ActriproLanguageKey, "Docking", name);
                    if (LanguageReader.ContainsKey(fullKeyName))
                    {
                        SR.SetCustomString(name, LanguageReader.GetValue(fullKeyName));
                    }
                }

                //Editors
                ActiproSoftware.Products.Editors.Resources.Culture = CultureInfo.CurrentCulture;

                names = Enum.GetNames(typeof (ActiproSoftware.Products.Editors.SRName));
                foreach (string name in names)
                {
                    string fullKeyName = string.Format(ActriproLanguageKey, "Editors", name);
                    if (LanguageReader.ContainsKey(fullKeyName))
                    {
                        ActiproSoftware.Products.Editors.SR.SetCustomString(name, LanguageReader.GetValue(fullKeyName));
                    }
                }
            }
        }

        #endregion Methods
    }
}