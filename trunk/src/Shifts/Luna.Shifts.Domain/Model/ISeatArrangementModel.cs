using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface ISeatArrangementModel
    {
        IList<SeatBox> GetSeatBoxesWithSeatArrangment(Guid[] seatIds, DateTime start, DateTime end);

        Dictionary<ISimpleEmployee, ICollection<SeatArrangement>>  GetAgentsWithSeatArrangement(Guid[] employeeIds, DateTime start, DateTime end,
                                  IDictionary<string, SeatBox> seatBoxes);

        IDictionary<string, SeatBox> GetAllSeat();
    }
}
