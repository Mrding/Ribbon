using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;
using Luna.Shifts.Domain;
using Luna.WPF.ApplicationFramework;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface IRTAAPrefixPresenter : IDockablePresenter
    {
        Func<IVisibleTerm, AdherenceEvent> WhenAdding { get; }

        Action<Luna.Shifts.Domain.AdherenceEvent> WhenRemoving { get; }

        Action<AdherenceEvent> WhenChanged { get; }

        void Refresh();
    }
}
