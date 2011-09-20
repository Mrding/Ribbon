using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public class AcdQueue:Entity
    {
        //對應CMS中的queue
        public virtual string SplitId { get; set; }
        public virtual string Description { get; set; }
        public virtual ServiceQueue ServiceQueue { get; set; }
        public virtual Acd Acd { get; set; }
    }
}
