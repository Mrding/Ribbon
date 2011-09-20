using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public static class ExecuteManager
    {
        public static void BackgroundAction(Action backgroundAction)
        {
            BackgroundAction(backgroundAction, EndMode.End);
        }

        public static void BackgroundAction(Action backgroundAction, EndMode endMode)
        {
            BackgroundAction(backgroundAction, endMode, TaskImplMode.UIThread);
        }

        public static void BackgroundAction(Action backgroundAction, EndMode endMode, TaskImplMode taskMode)
        {
            BackgroundAction(backgroundAction, default(Action), endMode, taskMode);
        }

        public static void BackgroundAction(Action backgroundAction, Action callback, EndMode endMode, TaskImplMode taskMode)
        {
            BackgroundAction(null, backgroundAction, default(Action), callback, endMode, taskMode);
        }

        public static void BackgroundAction(Action backgroundAction, Action before, Action callback)
        {
            BackgroundAction(null, backgroundAction, before, callback, EndMode.Continue, TaskImplMode.UIThread);
        }

        public static void BackgroundAction(object target, Action backgroundAction, Action before, Action callback)
        {
            BackgroundAction(target, backgroundAction, before, callback, EndMode.Continue, TaskImplMode.UIThread);
        }

        public static void BackgroundAction(object target, Action backgroundAction, Action befoe, Action callback, EndMode endMode, TaskImplMode taskMode)
        {
            var context = new BackgroundActionContext(endMode, () => { backgroundAction(); return null; });
            var container = target == null ? backgroundAction.Target.As<IExtendedPresenter>() : target.As<IExtendedPresenter>();
            var task = container.GetMetadata<IBackgroundThreadTask>();
            if (task == null)
            {
                task = TaskImplFactory.CreateTask(taskMode);
                if (taskMode != TaskImplMode.UIThread)
                {
                    container.WasShutdown += (s, e) => task.Dispose();
                    container.AddMetadata(task);
                }
            }

            //Before
            if (!befoe.IsNull())
                befoe();

            //Action
            task.Enqueue(context);

            //After
            if (!callback.IsNull())
                Application.Current.Dispatcher.BeginInvoke(callback, DispatcherPriority.ContextIdle);
        }
    }
}
