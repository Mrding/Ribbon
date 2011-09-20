using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface IBackupTermRepository : IRepository<BackupTerm>
    {
        IList<BackupTerm> GetAllEmployeeLevelOneTermWithTouchRange(Guid[] employeeIds, DateTime startTime, DateTime endTime);

        IList<BackupTerm> GetLevelOneTermWithTouchRange(Guid employeeId, DateTime startTime, DateTime endTime);

        IList<BackupTerm> GetAllTermsWithMidRange(Guid employeeId, DateTime startTime, DateTime endTime);

        void Delete(Guid[] employeeIds, ITerm range);
    }
}
