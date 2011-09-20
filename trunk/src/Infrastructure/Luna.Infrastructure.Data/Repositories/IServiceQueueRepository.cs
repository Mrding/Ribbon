using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface IServiceQueueRepository : IRepository<ServiceQueue>
    {
        void MakePersistentWithSync(ServiceQueue entity);

        IEnumerable<ServiceQueue> GetServiceQueueByCampaign(ICampaign campaign);
    }
}
