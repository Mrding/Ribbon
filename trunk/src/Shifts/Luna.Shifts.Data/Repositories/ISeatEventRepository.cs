using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface ISeatEventRepository : IRepository<SeatEvent>
    {
        IList<SeatEvent> Searh(Entity site, string category, DateRange range);
    }
}