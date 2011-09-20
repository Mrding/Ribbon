using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface ISeatDispatcherModel
    {
        IList<SeatBox> GetSeats(Schedule schedule, Guid[] excludedEmployeeIds, out IList<TimeBox> timeBoxes, out IList<Area> areas);

        void SubmitChanges();

        void Abort();
    }

}
