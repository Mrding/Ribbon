using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
       // IList<SchedulingPayload> GetAgents();

        IEnumerable<Attendance> GetAttendanceFrom(Schedule schedule);

        int CountAttendance(Schedule schedule);
    }
}
