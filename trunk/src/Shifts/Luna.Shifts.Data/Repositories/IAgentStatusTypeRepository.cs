using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface IAgentStatusTypeRepository : IRepository<AgentStatusType>
    {
        bool HasRelation(AgentStatusType agentStatusType);

        IList<AgentStatusAlertTime> GetAreaCustomAgentStatusAlters(Entity area);

        AgentStatusAlertTime MakePersistent(AgentStatusAlertTime entity);
    }
}