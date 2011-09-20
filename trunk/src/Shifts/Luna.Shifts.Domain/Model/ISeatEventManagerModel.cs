using System.Collections;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    public interface ISeatEventManagerModel
    {
        IList<SeatEvent> Searh(Entity site, string category, DateRange range, out IList seatEventGroup);

        //IEnumerable<Occupation> FindConflict(SeatEvent seatEvent);

        //void SyncShiftOccupation(IList<AssignmentBase> effectShifts);

        //void AcceptChanged();

        //void Refresh();

    }
}