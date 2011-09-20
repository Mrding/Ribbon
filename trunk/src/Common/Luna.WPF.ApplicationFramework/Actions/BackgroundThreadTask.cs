using System;
using System.Collections.Generic;
using System.Threading;
using Caliburn.Core.Threading;

namespace Luna.WPF.ApplicationFramework.Actions
{
    public class BackgroundThreadTask : AbstractBackgroundThreadTask
    {
        private readonly Thread _backgroundThread;

        public BackgroundThreadTask()
        {
            _backgroundThread = new Thread(new ParameterizedThreadStart(DoWork)) { IsBackground = true };
        }

        protected override void Start(object userState)
        {
            if ((_backgroundThread.ThreadState & ThreadState.Unstarted) > 0)
                _backgroundThread.Start(userState);
        }
    }
}
