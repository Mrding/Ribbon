using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class AssignmentTypeRepository : Repository<AssignmentType> , IAssignmentTypeRepository
    {
        private readonly ITermStyleRepository _termStyleRepository;
        private Dictionary<string, TermStyle> _subEventTypes;


        public AssignmentTypeRepository(ITermStyleRepository termStyleRepository)
        {
            _termStyleRepository = termStyleRepository;
        }

        public override void LoadRelatedEntities()
        {
           _subEventTypes = _termStyleRepository.GetEventTypes().ToDictionary(o=>o.GetUniqueKey());
        }

        public void MakePersistentWithSync(Action<AssignmentType, Dictionary<string, TermStyle>> updateWithSubEventInsertRules, AssignmentType entity)
        {
            MakePersistent(entity);
            updateWithSubEventInsertRules(entity, _subEventTypes);
        }
    }
}
