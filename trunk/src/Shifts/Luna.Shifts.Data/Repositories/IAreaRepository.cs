using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface IAreaRepository : IRepository<Area>
    {
        void MakePersistentWithSync(Area entity);

        IList<Area> GetAreaByCampaign(Entity campaign);

        IEnumerable<Area> GetBySite(Entity site);

        IEnumerable<Area> GetWithSeats(Entity site);

        Area GetWithSeats(Guid areaId);

        IEnumerable<Area> GetWithSeatPriorityDetails(Entity site, Entity campaign);

        IEnumerable<OrganizationSeatingArea> GetOrganizationSeatingArea(Guid areaId);

        OrganizationSeatingArea MakePersistent(OrganizationSeatingArea obj);

        IEnumerable<PriorityEmployee> GetPriorityEmployees(Entity[] areas);
    }

    public static class AreaRepositoryExt
    {
        public static Dictionary<ISeat, List<PriorityEmployee>> ToUsefulDictionary(this IEnumerable<PriorityEmployee> items)
        {
           return (from x in items
                        group x by x.Seat
                            into g
                            select g)
                        .ToDictionary(o => o.Key, o => o.OrderBy(t => t.Index).ToList());
            
        }
    }
}