using System.Collections.Generic;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface IAttendanceManagerModel
    {
        IList<Attendance> GetEmployees(Schedule schedule);

        void Enroll(IEnumerable<Attendance> attendances);

        void Evict(IEnumerable<Attendance> attendances);

        void AcceptAllChanges(IEnumerable<Attendance> attendances);

        Attendance CreateNewInstance(Attendance exist, Schedule schedule);
    }
}
