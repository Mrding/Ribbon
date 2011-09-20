using System;
using System.Collections.Generic;
using Luna.Data;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Repositories
{
    public interface IAssignmentTypeRepository : IRepository<AssignmentType>
    {
        void MakePersistentWithSync(Action<AssignmentType, Dictionary<string, TermStyle>> updateWithSubEventInsertRules, AssignmentType entity);

    }
}
