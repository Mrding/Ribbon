using System;
using System.ComponentModel;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    [IgnoreRegister]
    public interface IAgentSeatModel : ISelectable, ICloze
    {
        ISeat Profile { get; set; }

        LastStatusDto GetLastShowOn(ISimpleEmployee employee);

        Func<Employee, DateTime, LastStatusDto> ShowOn { get; set; }

        Occupation Arrangement { get; set; }

        SeatEvent SeatEvent { get; set; }

        AgentStatus AgentStatus { get; set; }

        RtaaSlicedTerm CurrentActivity { get; set; }

        void UpdateStatus(DateTime watchPoint, Func<AgentStatusType, bool, int> getAlterTime);

        void UpdateSeatInfo(int columnIndex);

        void UpdateAgentInfo(int columnIndex);

        TimeBox CurrentAgent { get; set; }

        void StatusNotFound();

        string SeatingStatus { get; }

        bool? IsOvertime { get; }

        bool Highlight { get; set; }

        TimeSpan StatusTimeoutElapsed { get; set; }

        TimeSpan RtaaTimeoutElapsed { get; set; }

        string ShiftInfo { get; }

        string SeatInfo { get; }

        string AgentInfo { get; }

        int WarningLevel { get; }

       // bool IsActivated { get; }

        event Action<IAgentSeatModel> SeatingStatusChanged;

    }
}
