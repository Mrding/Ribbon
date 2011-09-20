using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate.Criterion;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
       
        //public IList<Attendance> GetAgents()
        //{
        //    return Session.CreateQuery(@"from Attendance a inner join fetch a.Agent")
        //                  .List<Attendance>();
        //}
        /// <summary>
        /// Fetch Employee immediately
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public IEnumerable<Attendance> GetAttendanceFrom(Schedule schedule)
        {
            return Session.CreateQuery(@"from Attendance a
                                         where a.Campaign =:campaign and a.Start >=:start and a.End <=:end")
                          .SetGuid("campaign", schedule.Campaign.Id)
                          .SetDateTime("start", schedule.Start)
                          .SetDateTime("end", schedule.End)
                          .List<Attendance>();
        }
       

        public int CountAttendance(Schedule schedule)
        {
            return Session.CreateCriteria<Attendance>()
                .Add(Restrictions.Eq("Campaign", schedule.Campaign))
                .Add(Restrictions.Ge("Start", schedule.Start))
                .Add(Restrictions.Le("End", schedule.End))
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();
        }
    }
}
