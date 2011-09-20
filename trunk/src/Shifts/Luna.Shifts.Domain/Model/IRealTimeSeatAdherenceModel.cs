using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface IRealTimeSeatAdherenceModel
    {
        IEnumerable<TermStyle> GetAllEventTypes();
        void BuildSeats(Entity area, Action<IEnumerable<ISeat>> buildSeats);
        IList<TimeBox> Save(IList<TimeBox> timeBoxs);
        IList<AgentStatus> GetAgentStatusHistory(string extNo, DateTime start, DateTime end);
        LastStatusDto GetLastStatus(Employee agent, DateTime currentTime);
        void SetMonitoringArea(Entity area, DateTime watchPoint, Dictionary<string, AgentStatusType> agentStatusTypes);
        void LoadData(IAgentSeatModel agentSeat);
    }
}
