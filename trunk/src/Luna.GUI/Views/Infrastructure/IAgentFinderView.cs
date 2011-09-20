using System.Collections;

namespace Luna.GUI.Views.Infrastructure
{
    public interface IAgentFinderView
    {
        /// <summary>
        /// 源数据
        /// </summary>
        IList Source { get; set; }

        /// <summary>
        /// 用于过滤筛选
        /// </summary>
        IList FilterItems { get; set; }

        /// <summary>
        /// 表示过滤项
        /// </summary>
        IEnumerable FilteredItems { get; set; }

        /// <summary>
        /// 表示当前选中项
        /// </summary>
        IList SelectedItems { get; }

        /// <summary>
        /// 表示过滤关键字
        /// </summary>
        string SearchText { get; set; }
    }
}
