using Luna.Common.Domain;

namespace Luna.Common.Model
{
    [IgnoreRegister]
    public interface IAuditLogModel
    {
        void Write(AuditLog log);
    }
}
