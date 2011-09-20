using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface ISeatBoxRepository : IRepository<SeatBox>
    {
        IList<SeatBox> Search(Entity[] areas, DateTime start, DateTime end, bool includedNotInUse);

        //IList<SeatBox> Search(Site site, DateTime start, DateTime end);

        //IList<SeatBox> GetSeatBoxesWithSeatingMisc(Site site, DateTime start, DateTime end);

        IList<SeatBox> Search(Entity area, DateTime watchPoint);

        //IList<SeatBox> GetSeatArrangement(Guid empId, DateTime start, DateTime end);

        void UpdateShiftOccupyStatus(ITerm shift);

        void MakePersistent(SeatBox seatBox, ITerm[] shifts);

        SeatBox GetByRagne(Guid seatId, DateTime start, DateTime end);

        IList<SeatBox> GetByRagne(Guid[] seatIds, DateTime start, DateTime end);

        void MakeTransient(Seat seat);

        //bool AnyOccupations(Guid seatId);

        SeatArrangement GetPlannedSeatArrangement(ISeat seat, DateTime watchPoint);

        void Search(Entity area, DateTime watchPoint, Action<string, Occupation> loopingDelegate);

    }
}
