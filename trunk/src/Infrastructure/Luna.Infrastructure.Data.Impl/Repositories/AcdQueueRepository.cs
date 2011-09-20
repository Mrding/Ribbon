using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class AcdQueueRepository : Repository<AcdQueue>, IAcdQueueRepository
    {
        public void DeleteAcdQueueRelation(ServiceQueue serviceQueue)
        {
            Session.CreateSQLQuery("Update AcdQueue set ServiceQueueId = null where ServiceQueueId =:sqId")
                .SetGuid("sqId",serviceQueue.Id)
                .ExecuteUpdate();
        }

        public override bool HaveAnyRelationships(AcdQueue entity)
        {
            using (var session = Factory.OpenStatelessSession())
            {
                const string acdQueueTraffic = "select count(*) from ACDQueueTraffic where AcdQueueId=:acdqId ";
                const string acdQueue = "select count(*) from AcdQueue where AcdQueueId=:acdqId and ServiceQueueId is not null ";
                var sql = string.Format("select ({0})+({1})", acdQueueTraffic, acdQueue);

                var results = session.CreateSQLQuery(sql).SetGuid("acdqId", entity.Id).UniqueResult<int>();

                return results > 0;
            }
        }
    }
}