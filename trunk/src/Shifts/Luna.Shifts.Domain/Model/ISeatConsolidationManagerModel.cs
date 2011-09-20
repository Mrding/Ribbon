using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;


namespace Luna.Shifts.Domain.Model
{
    using Luna.Common.Model;

    public interface ISeatConsolidationManagerModel : IModel<SeatConsolidationRule>
    {
        IEnumerable<Skill> GetSkills();

        IEnumerable<AssignmentType> GetAssignmentTypes();

        IEnumerable<SeatConsolidationRule> GetAll(Entity site, Entity campaign);

        //void ReloadSeatingSiteSeats(ISeatingSite<IAreaOrganizations> site);
        SeatConsolidationRule Reload(SeatConsolidationRule rule);

        void SaveAll(IEnumerable<SeatConsolidationRule> rules);

        void SaveAllIndex(IEnumerable<SeatConsolidationRule> rules);

        Luna.Core.Tuple<IEnumerable<Organization>, IEnumerable<Area>> GetDetails(Entity site, Entity campaign);
    }
}
