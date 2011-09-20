using Luna.Common.Domain;
using Luna.Common.Model;
using Luna.Infrastructure.Data.Repositories;
using uNhAddIns.Adapters;

namespace Luna.Infrastructure.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit)]
    public class AuditLogModel : IAuditLogModel
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogModel(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Write(AuditLog log)
        {
            _repository.MakePersistent(log);
        }
    }
}
