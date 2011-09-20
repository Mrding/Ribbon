using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Caliburn.Core.Threading;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public abstract class AbstractBackgroundThreadTask : IBackgroundThreadTask
    {
        protected readonly AutoResetEvent _autoReset;
        protected readonly Queue<Func<object>> _theDelegateQueue;

        public AbstractBackgroundThreadTask()
        {
            _autoReset = new AutoResetEvent(false);
            _theDelegateQueue = new Queue<Func<object>>();
        }

        public BackgroundActionContext CurrentContext { get; set; }

        public virtual void Enqueue(object userState)
        {
            if (userState == null)
                throw new ArgumentNullException("userState");

            CurrentContext = userState as BackgroundActionContext;
            if (CurrentContext == null)
                throw new NotSupportedException("Not support non BackgroundActionContext type userState");

            lock (_theDelegateQueue)
            {
                _theDelegateQueue.Enqueue(CurrentContext.Task);
            }

            Start(userState);

            if (CurrentContext.BackgourndEndMode == EndMode.Abort)
                Cancel();
            else
                NotifyBackgroundThread();
        }

        protected abstract void Start(object userState);

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
                    Func<object> task = null;
                    lock (_theDelegateQueue)
                    {
                        if (_theDelegateQueue.Count > 0)
                            task = _theDelegateQueue.Dequeue();
                    }
                    if (task != null)
                    {
                        Starting(this, EventArgs.Empty);

                        IsBusy = true;
                        var result = task();
                        IsBusy = false;

                        Completed(this, new BackgroundTaskCompletedEventArgs(result, userState, false, null));

                        if (!CancellationPending)
                        {
                            IsWaiting = false;
                            if (context.BackgourndEndMode == EndMode.Continue)
                            {
                                if (_theDelegateQueue.Count == 0)
                                {
                                    IsWaiting = true;
                                    _autoReset.WaitOne();
                                }
                            }
                            else if (context.BackgourndEndMode == EndMode.End || context.BackgourndEndMode == EndMode.Abort)
                                return;
                            DoWork(CurrentContext);
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

        public virtual void Cancel()
        {
            CancellationPending = true;
        }

        protected void NotifyBackgroundThread()
        {
            if (IsWaiting)
                _autoReset.Set();
        }

        public bool IsBusy
        { get; private set; }

        public bool CancellationPending
        { get; private set; }

        public bool IsWaiting
        { get; set; }

        public event EventHandler Starting = delegate { };

        public event EventHandler<BackgroundTaskProgressChangedEventArgs> ProgressChanged = delegate { };

        public event EventHandler<BackgroundTaskCompletedEventArgs> Completed = delegate { };

        public void Dispose()
        {
            if (CurrentContext.BackgourndEndMode == EndMode.End)
            {
                Completed += (s, e) => Clean();
            }
            else
            {
                Cancel();
                NotifyBackgroundThread();
                Clean();
            }
        }

        protected void Clean()
        {
            _theDelegateQueue.Clear();
            _autoReset.Close();
        }
    }
}
