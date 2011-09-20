using System.Linq;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Data.Repositories;
using NHibernate.Linq;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class AcdRepository : Repository<Acd>, IAcdRepository
    {
        public override bool HaveAnyRelationships(Acd entity)
        {
            var acdQueueCount = Session.Query<AcdQueue>().Count(o => o.Acd == entity);
            return acdQueueCount > 0;
        }
    }
}
