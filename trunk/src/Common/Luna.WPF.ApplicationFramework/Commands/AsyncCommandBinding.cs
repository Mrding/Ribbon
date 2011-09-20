using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Luna.WPF.ApplicationFramework.Commands
{
    public class AsyncCommandBinding : CommandBinding
    {
        private readonly BackgroundWorker _task;

        public AsyncCommandBinding(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute, RunWorkerCompletedEventHandler runWokrerCompleted)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            _task = new BackgroundWorker();
            _task.DoWork += delegate
                                {
                                    executed(null, null);
                                };
            _task.RunWorkerCompleted += (s, e) =>
                                            {
                                                runWokrerCompleted(s, e);
                                                _task.Dispose();
                                            };

            Command = command;
            if (executed != null)
            {
                Executed += delegate
                                {
                                    if (!_task.IsBusy)
                                        _task.RunWorkerAsync();
                                };
            }
            if (canExecute != null)
            {
                CanExecute += canExecute;
            }
        }
    }
}
