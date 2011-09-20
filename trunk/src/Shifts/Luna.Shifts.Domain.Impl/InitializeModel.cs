using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class InitializeModel : IInitializeModel
    {
        private readonly ITermStyleRepository _termStyleRepository;
        public InitializeModel(ITermStyleRepository termStyleRepository)
        {
            _termStyleRepository = termStyleRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Initialize()
        {
            _termStyleRepository.GetAssignmentTypesWithInsertRules();
            _termStyleRepository.GetEventTypes();
        }
    }
}
