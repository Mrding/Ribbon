namespace Luna.WPF.ApplicationFramework.Actions
{
    /// <summary>
    /// 定义后台任务状态
    /// </summary>
    public enum EndMode
    {
        /// <summary>
        /// 未分配，表示同步
        /// </summary>
        Unspecified,

        /// <summary>
        /// 继续使用后台线程完成任务
        /// </summary>
        Continue,

        /// <summary>
        /// 结束后台线程任务
        /// </summary>
        End,

        /// <summary>
        /// 取消后台线程任务
        /// </summary>
        Abort
    }
}
