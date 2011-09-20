using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luna.WPF.ApplicationFramework
{
    public class ActionCondition
    {
        public ActionCondition() { }

        public ActionCondition(Action action, Func<bool> condition)
        {
            Action = action;
            Condition = condition;
        }

        public Action Action { get; set; }

        public Func<bool> Condition { get; set; }
    }
}
