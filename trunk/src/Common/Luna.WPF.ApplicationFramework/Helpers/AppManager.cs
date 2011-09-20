using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Luna.WPF.ApplicationFramework.Helpers
{
    public static class AppManager
    {
        private static readonly List<AppDomain> _usedChildDomains = new List<AppDomain>();

        public static void Run(string filePath)
        {
            Run(filePath, null);
        }

        public static void Run(string filePath, string parameter)
        {
            var childDomain = AppDomain.CreateDomain(string.Format("ChildAppDomain {0}", _usedChildDomains.Count));
            var childThread = new Thread(o =>
            {
                childDomain.ExecuteAssembly(filePath, AppDomain.CurrentDomain.Evidence, new[] { parameter });
                _usedChildDomains.Remove(childDomain);
                Application.Current.Dispatcher.Invoke(() => AppDomain.Unload(childDomain));
            });
            childThread.Name = string.Format("Child AppDomain Thread {0}", _usedChildDomains.Count);
            childThread.SetApartmentState(ApartmentState.STA);
            childThread.Start();
            _usedChildDomains.Add(childDomain);
        }

        public static void Shutdown()
        {
            if (_usedChildDomains.Count == 0)
                Application.Current.Shutdown();
            else
                Environment.Exit(0);
        }
    }
}
