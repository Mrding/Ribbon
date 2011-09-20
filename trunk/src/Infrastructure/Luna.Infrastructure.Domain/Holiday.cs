namespace Luna.Infrastructure.Domain
{
    using Luna.Common.Extensions;

    public class Holiday : CalendarEvent
    {
        public virtual string Country { get; set; }

        public override string ToString()
        {
            if (1 < End.Subtract(Start).Days)
                return string.Format("{0:MM/dd}-{1:MM/dd}", Start, End.AddDays(-1));
            return Start.ToString("MM/dd");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;

            var other = obj as Holiday;

            if (other == null) return false;

            if (other.EventName == EventName && this.AnyOverlap(other) && other.Country == Country)
                return true;

            return false;
        }
    }
}
