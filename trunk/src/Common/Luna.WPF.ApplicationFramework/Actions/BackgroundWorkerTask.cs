using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public class BackgroundWorkerTask : AbstractBackgroundThreadTask
    {
        private readonly BackgroundWorker _worker;

        public BackgroundWorkerTask()
        {
            _worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            _worker.DoWork += (s, e) => DoWork(e.Argument);
        }

        protected override void Start(object userState)
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync(userState);
        }
    }
}
