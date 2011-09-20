using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface IAdherenceEventRepository : IRepository<AdherenceEvent>
    {
        IList<string> GetAbsenceTypes();

        void Search(Guid[] employeeIds, DateTime start, DateTime end, Action<AdherenceEvent> loopingDelegate);
    }
}
