using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.Common;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Presenters.Interfaces
{
    public interface ISeatMonitoringPresenter : IPresenter
    {
        IEnumerable<AgentStatusType> AgentStatusTypes { get; }

        Dimension Dimension { get; }

        IEnumerable<IAgentSeatModel> AgentSeats { get; }

        DateTime CurrentTime { get; set; }
        int CurrentValue { get; set; }
        bool AutoRefresh { get; set; }
        int RefreshInterval { get; set; }
        bool ShowRtsaPanel { get; set; }
        bool ShowRtaaPanel { get; set; }
        bool ShowFilter { get; set; }

        event Action<DateTime> UpdateEvent;
    }
}