using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Shifts.Domain;
using System.Collections;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IAddingEventPresenter : IAddingTermPresenter, IDockablePresenter
    {
        //DateTime Start { get; set; }
        //DateTime End { get; set; }

        DateTime EventStart { get; set; }

        Action<IAddingEventPresenter> OnActivateDelegate { get; set; }
    }

    public interface IAddingTermPresenter : IPresenter
    {
        //IList<TimeBox> SelectedAgents { get; set; }

        Func<bool?,IEnumerable> GetSelectedAgents { get; set; }

        IEnumerable<TermStyle> AvailableTypes { get; set; }

        //TimeBox CurrentAgent { get; set; }



        Action<IEnumerable, bool> RefreshDelegate { get; set; }

        Action<IAddingTermPresenter, Exception> WhenClosed { get; set; }

        //Func<IList<TimeBox>> ReloadAgentDelegate { get; set; }
    }
}
