using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface ISeatingPrioritySetupModel
    {
        Area ReloadWithSeat(Area area, out IEnumerable<IArrangeSeatRule> availableOrganizations);

        void Save();

        Luna.Core.Tuple<IList<Employee>, Dictionary<ISeat, List<PriorityEmployee>>> GetPriorityEmployeeMisc(
            Entity area);

        void AddPriorityEmployee(PriorityEmployee entity);
        void DeletePriorityEmployee(PriorityEmployee entity);
   
    }


    
}
