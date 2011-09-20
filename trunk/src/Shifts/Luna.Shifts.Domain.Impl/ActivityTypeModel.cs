using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class ActivityTypeModel : IActivityTypeModel
    {
        private readonly ITermStyleRepository _termStyleRepository;
        public ActivityTypeModel(ITermStyleRepository termStyleRepository)
        {
            _termStyleRepository = termStyleRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Save(TermStyle entity)
        {
            _termStyleRepository.MakePersistent(entity);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void Delete(TermStyle entity)
        {
            _termStyleRepository.MakeTransient(entity);
        }

        public IEnumerable<TermStyle> GetSubEventTypes()
        {
            return _termStyleRepository.GetWholeEventTypes();
        }

        public IEnumerable<TermStyle> GetAbsentEventTypes()
        {
            return _termStyleRepository.GetAbsentTypes();
        }

        public void Reload(ref TermStyle entity)
        {
            if (entity.SaftyGetProperty<bool, IEditingObject>(o => o.IsNew))
            {
                _termStyleRepository.Evict(entity);
            }
            else
            {
                _termStyleRepository.Refresh(ref entity);
            }
            
        }

        public bool DuplicationChecking(Entity entity, string name)
        {
            var example = Activator.CreateInstance(entity.GetType()).As<TermStyle>();
            example.Name = name;
            var count = _termStyleRepository.Count(example);

            if (entity.IsNew())
                return 0 < count;
            else
            {
                if( entity.Name == name && 1 == count)
                    return false;
                return 0 < count;
            }
        }

        public void Release()
        {
            _termStyleRepository.Clear();
        }
    }
}
