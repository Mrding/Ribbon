using System;
using Caliburn.Core.Metadata;
using Caliburn.Core.Threading;

namespace Luna.WPF.ApplicationFramework.Actions
{
    /// <summary>
    /// 声明后台线程任务，整个生命周期中都使用同一个线程
    /// </summary>
    public interface IBackgroundThreadTask : IBackgroundTask, IMetadata, IDisposable
    {
        /// <summary>
        /// Gets or sets the current context.
        /// </summary>
        /// <value>The current context.</value>
        BackgroundActionContext CurrentContext { get; set; }
    }
}
