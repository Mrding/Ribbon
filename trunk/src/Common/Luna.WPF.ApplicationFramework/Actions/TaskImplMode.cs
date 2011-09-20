namespace Luna.WPF.ApplicationFramework.Actions
{
    /// <summary>
    /// 后台任务实现方式
    /// </summary>
    public enum TaskImplMode
    {
        /// <summary>
        /// 前台线程
        /// </summary>
        UIThread,
        /// <summary>
        /// 后台线程
        /// </summary>
        BackgroundThread,
        /// <summary>
        /// 后台工作组件
        /// </summary>
        BackgroundWorker,
        /// <summary>
        /// 线程池
        /// </summary>
        ThreadPool        
    }
}
