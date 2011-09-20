using System.Collections;
using System;
using System.Collections.Generic;

namespace Luna.Common
{
    public interface ICanSupportAgentFinder
    {
        IEnumerable Agents { get; set; }

        IList BindableAgents { get; set; }

        Func<bool> FullyRefresh { get; set; }

        //void LoadAgents();

        TResult Transform<TResult>(object item) where TResult : class;

        DateTime GetWatchPoint();

        void Sort();

        void Refresh(IEnumerable list);

        bool CanAddTo(object item);

        bool UnselectedAfterUpateQueryResult { get;}
    }


    public interface IBlockMatrixContainer
    {
        void NavigateTo(object item);

        int CurrentIndex { get; }
    }



    public interface ICanSupportCustomGroup
    {
        IEnumerable Agents { get; }
    }

    //public interface IShiftViewerPresenter : ILifecycleNotifier
    //{
    //    void RegisterRefreshDelegate(object source);

    //    void UnregisterDelegate(object source);

    //    IList BindableAgents { get; set; }

    //    DateRange ScheduleRange { get; }
    //}
}