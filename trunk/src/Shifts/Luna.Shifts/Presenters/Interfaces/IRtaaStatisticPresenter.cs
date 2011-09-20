using System;
using System.Collections;
using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.WPF.ApplicationFramework;
using Luna.Core;
using Luna.Common;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IRtaaStatisticPresenter : IPresenter
    {
        void UpdateData(IList<IEnumerable> values);

        ICanSupportAgentFinder ShiftViewerPresenter { get; set; }


        DateTime MonitoringTime { get; set; }
    }
}
