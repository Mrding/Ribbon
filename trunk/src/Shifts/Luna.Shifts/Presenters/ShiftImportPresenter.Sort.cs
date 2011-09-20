using System.Collections;
using System.Collections.Generic;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework.Extensions;
namespace Luna.Shifts.Presenters
{
    public partial class ShiftImportPresenter
    {
        private readonly Dictionary<string, System.Func<PlanningAgent, PlanningAgent, int>> _sortingFuncs = new Dictionary<string, System.Func<PlanningAgent, PlanningAgent, int>>
                    {
                        {"Correctness",(x,y)=> (x.Tag != null || x.OperationFail == true).CompareTo((y.Tag != null || y.OperationFail == true)) }
                    };

        private string _sorting = "asc";
        /// <summary>
        /// asc, des
        /// </summary>
        public string Sorting
        {
            get { return _sorting; }
            set { _sorting = value; NotifyOfPropertyChange(() => Sorting); }
        }

        private int OrderByIndex(int compareResult, int x, int y)
        {
            if (compareResult == 0)
                compareResult = x.CompareTo(y);
            if (Sorting == "des")
                compareResult = -compareResult;

            return compareResult;
        }

        public void Sort() { }

        private string _lastSortingProperty = "Correctness";
        public void Sort(string property)
        {
            if (_lastSortingProperty != property)
                Sorting = "asc"; // 默認使用 asc
            else
                if (Sorting == "asc") // 反向給值 asc -> des
                    Sorting = "des";
                else
                    Sorting = "asc"; // des -> asc

            ((List<IEnumerable>)BindableAgents).Sort((x, y) =>
            {
                var a = x.As<PlanningAgent>();
                var b = y.As<PlanningAgent>();
                if (Sorting == "des")
                {
                    if (a == null && b == null)
                        return OrderByIndex(0, y.As<IIndexable>().Index, x.As<IIndexable>().Index);
                    if (a == null || b == null)
                    {
                        if (a == null) return -1;
                        return 1;
                    }
                    return OrderByIndex(_sortingFuncs[property](a, b), y.As<IIndexable>().Index,
                                           x.As<IIndexable>().Index);
                }
                return x.As<IIndexable>().Index.CompareTo(y.As<IIndexable>().Index);
            });

            _lastSortingProperty = property;
            this.QuietlyReload(ref _bindableAgents, "BindableAgents");
        }
    }
}
