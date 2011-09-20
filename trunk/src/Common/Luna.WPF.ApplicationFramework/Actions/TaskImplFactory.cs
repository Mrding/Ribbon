using Caliburn.Core.Threading;
using Microsoft.Practices.ServiceLocation;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public class TaskImplFactory
    {
        public static IBackgroundThreadTask CreateTask(TaskImplMode mode)
        {
            switch (mode)
            {
                case TaskImplMode.UIThread:
                    return new UIThreadTask();
                case TaskImplMode.ThreadPool:
                    return new ThreadPoolTask(ServiceLocator.Current.GetInstance<IThreadPool>());
                case TaskImplMode.BackgroundThread:
                    return new BackgroundThreadTask();
                case TaskImplMode.BackgroundWorker:
                    return new BackgroundWorkerTask();
                default:
                    return new UIThreadTask();
            }
        }
    }
}
