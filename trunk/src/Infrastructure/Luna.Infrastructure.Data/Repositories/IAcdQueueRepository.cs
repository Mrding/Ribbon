using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface IAcdQueueRepository : IRepository<AcdQueue>
    {
        void DeleteAcdQueueRelation(ServiceQueue serviceQueue);
    }
}
