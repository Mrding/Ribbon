namespace Luna.Infrastructure.Domain
{
    using System;
    using Luna.Common.Extensions;

    public class DaylightSavingTime : CalendarEvent
    {
        public override string ToString()
        {
            return string.Format("{0:MM/dd} - {1:MM/dd}", Start,End.AddDays(-1));
        }

        public virtual TimeZoneInfo TimeZone
        {
            get
            {
                return TimeZoneInfo.FindSystemTimeZoneById(_timeZone);
            }
            set
            {
                _timeZone = value.Id;
            }
        }

        private string _timeZone;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;
            var other = obj as DaylightSavingTime;
            if (other == null)
                return false;
            if (other._timeZone ==_timeZone && this.AnyOverlap(other))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
