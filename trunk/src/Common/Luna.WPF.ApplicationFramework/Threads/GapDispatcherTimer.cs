using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Luna.WPF.ApplicationFramework.Threads
{
    public class GapDispatcherTimer
    {
        private DispatcherTimer _gapValidateTimer;

        public void Invoke(double second, Action action)
        {
            if (_gapValidateTimer == null && second > 0)
            {
                _gapValidateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(second) };
                _gapValidateTimer.Tick += delegate
                {
                    action();
                    _gapValidateTimer.Stop();
                };
            }
            if (_gapValidateTimer != null)
                _gapValidateTimer.Start();
        }
    }
}
