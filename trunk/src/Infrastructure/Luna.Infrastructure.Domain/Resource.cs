using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class Resource:Entity
    {
        public virtual byte[] Value { get; set; }
    }
}
