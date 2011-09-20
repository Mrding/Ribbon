using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class Skill : Entity
    {
        public virtual string Description { get; set; }
        public override string GetUniqueKey()
        {
            return Name;
        }
    }
}
