using Caliburn.PresentationFramework.ApplicationModel;
using System.Collections;
using Luna.Common;
using System;

namespace Luna.WPF.ApplicationFramework
{
    
    public interface IShiftViewerPresenter : ILifecycleNotifier,IPresenter
    {
        void RegisterRefreshDelegate(object source);

        void UnregisterDelegate(object source);

        IList BindableAgents { get; set; }

        DateRange ScheduleRange { get; }
        
    }
}
