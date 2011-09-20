using System;
using System.Collections.Generic;
using System.Threading;
using Caliburn.Core.Threading;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public class ThreadPoolTask : AbstractBackgroundThreadTask
    {
        private readonly IThreadPool _threadPool;

        public ThreadPoolTask(IThreadPool threadPool)
        {
            _threadPool = threadPool;
        }

        protected override void Start(object userState)
        {
            _threadPool.QueueUserWorkItem(state => DoWork(userState), userState);
        }
    }
}
