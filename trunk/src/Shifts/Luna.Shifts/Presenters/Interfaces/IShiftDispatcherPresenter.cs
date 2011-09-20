using System;
using System.Collections;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common.Interfaces;
using System.Collections.Generic;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IShiftDispatcherPresenter : IDockablePresenter
    {
        IList BindableAgents { get; set; }

        void ReloadAgents(IList affectedAgents, Exception ex);

        //IEnumerable GetSelectedAgent(bool? filterWithIsSelected);

        Dictionary<string, object> GetOperationParams();

        bool EnableRtaa { get; set; }


    }
}
