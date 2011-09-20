using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate.Transform;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class BackupTermRepository : Repository<BackupTerm>, IBackupTermRepository
    {
      
        public IList<BackupTerm> GetAllEmployeeLevelOneTermWithTouchRange(Guid[] employeeIds, DateTime startTime, DateTime endTime)
        {
            var results = Session.CreateQuery("from BackupTerm t where t.EmployeeId in (:employeeIds) and t.Level=:level and t.Start >= :start and t.Start < :end")
               .SetParameterList("employeeIds", employeeIds)
               .SetInt16("level", 0)
               .SetDateTime("start", startTime)
               .SetDateTime("end", endTime)
               .SetResultTransformer(new DistinctRootEntityResultTransformer())
               .List<BackupTerm>();
            return results;
        }

        public IList<BackupTerm> GetLevelOneTermWithTouchRange(Guid employeeId, DateTime startTime, DateTime endTime)
        {
            var results = Session.CreateQuery("from BackupTerm t where t.EmployeeId =:employeeId and t.Level=:level and t.Start >= :start and t.Start < :end")
               .SetGuid("employeeId", employeeId)
               .SetInt16("level", 0)
               .SetDateTime("start", startTime)
               .SetDateTime("end", endTime)
               .SetResultTransformer(new DistinctRootEntityResultTransformer())
               .List<BackupTerm>();
            return results;
        }

        public IList<BackupTerm> GetAllTermsWithMidRange(Guid employeeId, DateTime startTime, DateTime endTime)
        {
            var results = Session.CreateQuery("from BackupTerm t where t.EmployeeId= :employeeId and t.Start >= :start and t.End <=:end")
               .SetGuid("employeeId", employeeId)
               .SetDateTime("start", startTime)
               .SetDateTime("end", endTime)
               .SetResultTransformer(new DistinctRootEntityResultTransformer())
               .List<BackupTerm>();
            return results;
        }

        public void Delete(Guid[] employeeIds, ITerm range)
        {
            Session.CreateQuery("delete from BackupTerm b where b.EmployeeId in (:employeeIds) and :start <= b.HrDate and b.HrDate < :end")
                .SetParameterList("employeeIds", employeeIds)
                .SetDateTime("start", range.Start)
                .SetDateTime("end", range.End)
                .ExecuteUpdate();
        }
    }
}
