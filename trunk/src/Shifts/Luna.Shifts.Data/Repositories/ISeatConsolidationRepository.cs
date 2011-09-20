using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface ISeatConsolidationRepository : IRepository<SeatConsolidationRule>
    {
        IList<SeatConsolidationRule> GetByStie(Entity site);

        IList<SeatConsolidationRule> GetByStieAndCampaign(Entity site, Entity campaign);
    }
}
