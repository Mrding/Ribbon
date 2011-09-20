using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public static class ApplicationExtension
    {
       

        public static void OpenMainWindow(this Application app)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        public static void DirectClose(this Application app)
        {

        }

        public static string GetAppPath(this Application app)
        {
            return AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
        }

        public static void SetAutoRun(this Application app, bool bAutoRun)
        {
            RegistryKey currentUser = Registry.CurrentUser;
            string name = @"Software\Microsoft\Windows\CurrentVersion\Run";
            string appName = "Luna.GUI.exe";
            try
            {
                currentUser = currentUser.OpenSubKey(name, true);
                if (currentUser == null)
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
            try
            {
                if (bAutoRun)
                {
                    name = GetAppPath(app);
                    name = "\"" + name + "\" -AutoStart";
                    currentUser.SetValue(appName, name, RegistryValueKind.String);
                }
                else if (currentUser.GetValue(appName) != null)
                {
                    currentUser.DeleteValue(appName);
                }
                currentUser.Close();
            }
            catch (Exception exception)
            {

            }
        }

        public static bool IsAutoRun(this Application app)
        {
            return IsAutoRun(app, "Luna.GUI.exe");
        }

        public static bool IsAutoRun(this Application app, string appName)
        {
            RegistryKey currentUser = Registry.CurrentUser;
            string name = @"Software\Microsoft\Windows\CurrentVersion\Run";
            try
            {
                currentUser = currentUser.OpenSubKey(name, true);
                if (currentUser == null)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            name = currentUser.GetValue(appName) as string;
            currentUser.Close();
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            return true;
        }

        public static void ForceGC(this Application app)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        public static void DoEvents(this Application app)
        {
            // Create new nested message pump.
            var nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than the that of UI event messages.
            var exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, ExitFrameCallback, nestedFrame);

            // pump the nested message loop, the nested message loop will
            // immediately process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback doesn't get finished, Abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        static readonly DispatcherOperationCallback ExitFrameCallback = ExitFrame;

        static Object ExitFrame(Object state)
        {
            var frame = state as DispatcherFrame;

            // Exit the nested message loop.
            if(frame!=null)
                frame.Continue = false;

            return null;
        }

        public static void RunScreen(this Application app, Window window, Action action)
        {
            window.Show();
            Application.Current.DoEvents();
            action();
            window.Close();
        }

        public static bool HasFunction(this Application app, object functionKey)
        {
            var resource = app.TryFindResource(functionKey);
            if (resource is bool)
            {
                var shouldRegister = (bool)resource;
                return shouldRegister;
            }
            return false;
        }
    }
}
