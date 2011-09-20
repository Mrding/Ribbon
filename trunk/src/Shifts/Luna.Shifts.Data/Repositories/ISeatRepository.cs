using System.Collections.Generic;
using Luna.Data;
using Luna.Common;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface ISeatRepository : IRepository<Seat>
    {
        void MakePersistentWithSync(Seat entity);

        int GetMaxPriority(Entity area);

        IList<Seat> GetAllWithFullDetail();
    }

}