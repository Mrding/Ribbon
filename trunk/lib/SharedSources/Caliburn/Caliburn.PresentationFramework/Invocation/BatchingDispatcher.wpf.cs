﻿#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Invocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;
    using Core.Invocation;
    using Core.Threading;

    /// <summary>
    /// An impelementation of <see cref="IDispatcher"/> that efficiently batches updates to the UI thread.
    /// </summary>
    public class BatchingDispatcher : IDispatcher
    {
        private readonly Dispatcher _dispatcher;
        private readonly List<Action> queuedActions = new List<Action>();
        private readonly object locker = new object();
        private readonly IThreadPool _threadPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherImplementation"/> class.
        /// </summary>
        public BatchingDispatcher(IThreadPool threadPool)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _threadPool = threadPool;
            DefaultPriority = DispatcherPriority.Send;

            new Thread(SendBatchOfUpdates)
            {
                IsBackground = true
            }.Start();
        }

        /// <summary>
        /// Gets or sets the default dispatcher priority.
        /// </summary>
        /// <value>The default priority.</value>
        public DispatcherPriority DefaultPriority { get; set; }

        /// <summary>
        /// Executes code on the background thread.
        /// </summary>
        /// <param name="backgroundAction">The background action.</param>
        /// <param name="uiCallback">The UI callback.</param>
        /// <param name="progressChanged">The progress change callback.</param>
        public IBackgroundTask ExecuteOnBackgroundThread(Action backgroundAction, Action<BackgroundTaskCompletedEventArgs> uiCallback, Action<BackgroundTaskProgressChangedEventArgs> progressChanged)
        {
            var task = new BackgroundTask(
                _threadPool,
                () =>
                {
                    backgroundAction();
                    return null;
                });

            if (uiCallback != null)
                task.Completed += (s, e) => ExecuteOnUIThread(() => uiCallback(e));

            if (progressChanged != null)
                task.ProgressChanged += (s, e) => ExecuteOnUIThread(() => progressChanged(e));

            task.Enqueue(null);

            return task;
        }

        /// <summary>
        /// Executes code on the UI thread.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public void ExecuteOnUIThread(Action uiAction)
        {
            if (_dispatcher.CheckAccess())
                uiAction();
            else
            {
                lock (locker)
                    queuedActions.Add(uiAction);
                return;
            }
        }

        /// <summary>
        /// Executes code on the UI thread asynchronously.
        /// </summary>
        /// <param name="uiAction">The UI action.</param>
        public IDispatcherOperation BeginExecuteOnUIThread(Action uiAction)
        {
            var operation = _dispatcher.BeginInvoke(
                uiAction,
                DefaultPriority
                );

            return new DispatcherOperationProxy(operation);
        }

        private void SendBatchOfUpdates()
        {
            while (true)
            {
                Action[] actions;
                lock (locker)
                {
                    actions = queuedActions
                        .Take(100)
                        .ToArray();

                    for (int i = 0; i < actions.Length; i++)
                    {
                        queuedActions.RemoveAt(0);
                    }
                }

                if (actions.Length == 0)
                {
                    Thread.Sleep(500);
                    continue;
                }

                _dispatcher.Invoke(
                    (Action)delegate
                    {
                        using (_dispatcher.DisableProcessing())
                        {
                            foreach (var action in actions)
                            {
                                action();
                            }
                        }
                    });
            }
        }
    }
}

#endif