using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface IAgentStatusRepository : IRepository<AgentStatus>
    {
        Dictionary<string, AgentStatus> Search(Entity area, DateTime time, Dictionary<string, AgentStatusType> agentStatusTypes);

        IEnumerable<AgentStatus> Search(string[] agentAcdIds, DateTime start, DateTime end);

        void FastSearch(string[] agentAcdIds, DateTime start, DateTime end, Action<IAgentStatus> loopingDelegate);

        IList<AgentStatus> Search(string extNo, DateTime start, DateTime end);

        LastStatusDto GetLastStatus(Employee agent, DateTime currentTime);

    }
}
