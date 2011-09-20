using Caliburn.Core.Invocation;

namespace Luna.WPF.ApplicationFramework.Actions
{
    /// <summary>
    /// 后台任务信息
    /// </summary>
    public interface IBackgroundActionInfo
    {
        /// <summary>
        /// Gets or sets the background end mode.
        /// </summary>
        /// <value>The background end mode.</value>
        EndMode BackgroundEndMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [block if busy].
        /// </summary>
        /// <value><c>true</c> if [block if busy]; otherwise, <c>false</c>.</value>
        bool BlockIfBusy { get; set; }

        /// <summary>
        /// Gets or sets the task mode.
        /// </summary>
        /// <value>The task mode.</value>
        TaskImplMode TaskMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show busy cursor].
        /// </summary>
        /// <value><c>true</c> if [show busy cursor]; otherwise, <c>false</c>.</value>
        bool ShowBusyCursor { get; set; }

        /// <summary>
        /// Gets or sets the before method.
        /// </summary>
        /// <value>The before method.</value>
        IMethod BeforeMethod { get; set; }
    }
}
