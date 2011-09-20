using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface IMaintenanceScheduleModel
    {
        void Announce(Schedule schedule);

        void CloseAnnouncement(Schedule schedule);

        bool EmptyAgentTerms(IAgent agent, bool lockedTermIncluded);

        bool EmptyAgentTerms(IAgent agent, DateTime[] days, bool lockedTermIncluded);
    }
}
