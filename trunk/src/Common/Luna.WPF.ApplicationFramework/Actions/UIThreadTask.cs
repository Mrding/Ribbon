using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Core.Threading;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public class UIThreadTask : IBackgroundThreadTask
    {
        protected DispatcherOperation _latestOperation;

        public UIThreadTask()
        {
        }

        public void Enqueue(object userState)
        {
            if (userState == null)
                throw new ArgumentNullException("userState");

            CurrentContext = userState as BackgroundActionContext;
            if (CurrentContext == null)
                throw new NotSupportedException("Not support non BackgroundActionContext type userState");

            _latestOperation = UIDispatcher.BeginInvoke(() => DoWork(userState), DispatcherPriority.Background);
        }

        public virtual void Cancel()
        {
            CancellationPending = true;
            if (_latestOperation.Status == DispatcherOperationStatus.Executing ||
                _latestOperation.Status == DispatcherOperationStatus.Pending)
                _latestOperation.Abort();
        }

        protected virtual void DoWork(object userState)
        {
            try
            {
                if (CancellationPending)
                {
                    Completed(this, new BackgroundTaskCompletedEventArgs(null, userState, true, null));
                }
                else
                {
                    BackgroundActionContext context = userState as BackgroundActionContext;
                    Func<object> task = context.Task;
                    if (task != null)
                    {
                        if (context.BackgourndEndMode != EndMode.Abort)
                        {
                            Starting(this, EventArgs.Empty);

                            IsBusy = true;
                            var result = task();
                            IsBusy = false;

                            Completed(this, new BackgroundTaskCompletedEventArgs(result, userState, false, null));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IsBusy = false;
                Completed(this, new BackgroundTaskCompletedEventArgs(null, userState, CancellationPending, e));
                throw;
            }
        }

        public Dispatcher UIDispatcher
        {
            get { return Application.Current.Dispatcher; }
        }

        public BackgroundActionContext CurrentContext
        { get; set; }

        public bool IsBusy
        { get; private set; }

        public bool CancellationPending
        { get; private set; }

        public event EventHandler Starting = delegate { };

        public event EventHandler<BackgroundTaskProgressChangedEventArgs> ProgressChanged = delegate { };

        public event EventHandler<BackgroundTaskCompletedEventArgs> Completed = delegate { };

        public void Dispose()
        {
            _latestOperation = null;
        }
    }
}
