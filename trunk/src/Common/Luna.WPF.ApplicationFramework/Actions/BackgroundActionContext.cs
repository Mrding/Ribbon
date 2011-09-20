using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Core.Invocation;

namespace Luna.WPF.ApplicationFramework.Actions
{
    /// <summary>
    /// 后台任务上下文
    /// </summary>
    public class BackgroundActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundActionContext"/> class.
        /// </summary>
        /// <param name="endMode">The end mode.</param>
        /// <param name="task">The task.</param>
        public BackgroundActionContext(EndMode endMode, Func<object> task)
        {
            BackgourndEndMode = endMode;
            Task = task;
        }
        /// <summary>
        /// Gets or sets the backgournd end mode.
        /// </summary>
        /// <value>The backgournd end mode.</value>
        public EndMode BackgourndEndMode
        { get; set; }

        /// <summary>
        /// Gets or sets the task.
        /// </summary>
        /// <value>The task.</value>
        public Func<object> Task
        { get; set; }
    }
}
