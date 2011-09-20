using System;
using System.Collections.Generic;
using Luna.Data;
using Luna.Shifts.Domain;
using Luna.Shifts.Data.Repositories;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class AdherenceEventRepository : Repository<AdherenceEvent>, IAdherenceEventRepository
    {

        public IList<string> GetAbsenceTypes()
        {
            return Session.CreateSQLQuery("SELECT Name FROM  AbsenceType WHERE (isDeleted = 0)").List<string>();
        }

        public void Search(Guid[] employeeIds, DateTime start, DateTime end, Action<AdherenceEvent> loopingDelegate)
        {
         

            Session.EnableFilter("AdherenceEventCoveredFilter").SetParameter("start", start)
                          .SetParameter("end", end);

             Session.CreateQuery("from AdherenceEvent a where a.EmployeeId in (:employeeIds) order by a.Start")
                .SetParameterList("employeeIds", employeeIds)
                .List(new ActionableList<AdherenceEvent>(loopingDelegate));

            Session.DisableFilter("AdherenceEventCoveredFilter");
        }
    }
}
