using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class AuthRoleFunction:AbstractEntity<int>
    {
        public override int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual AuthRole AuthRole { get; set; }
    }
}