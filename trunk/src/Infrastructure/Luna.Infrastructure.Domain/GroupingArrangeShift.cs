namespace Luna.Infrastructure.Domain
{
    public class GroupingArrangeShift
    {
        public GroupingArrangeShift()
        {
            IsGrouping = false;
            IsMappingEvent = false;
        }
        public virtual bool IsGrouping { get; set; }

        public virtual bool IsMappingEvent { get; set; }
    }
}