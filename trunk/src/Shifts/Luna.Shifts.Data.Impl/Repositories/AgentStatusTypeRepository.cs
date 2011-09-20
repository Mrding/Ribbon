using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate.Criterion;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class AgentStatusTypeRepository : Repository<AgentStatusType>, IAgentStatusTypeRepository
    {
        public bool HasRelation(AgentStatusType agentStatusType)
        {
            IList<AgentStatus> agentStatuses = Session.CreateCriteria<AgentStatus>()
                .Add(Restrictions.Eq("AgentStatusType", agentStatusType)).List<AgentStatus>();

            return agentStatuses.Count() != 0;
        }

        public IList<AgentStatusAlertTime> GetAreaCustomAgentStatusAlters(Entity area)
        {
            Session.Clear();
            return Session.CreateQuery(@"from AgentStatusAlertTime a inner join fetch a.Area inner join fetch a.Type
                                         where a.Area.Id =:areaId")
                          .SetGuid("areaId", area.Id)
                          .List<AgentStatusAlertTime>();
        }

        public AgentStatusAlertTime MakePersistent(AgentStatusAlertTime entity)
        {
            Session.SaveOrUpdate(entity);
            return entity;
        }
    }
}