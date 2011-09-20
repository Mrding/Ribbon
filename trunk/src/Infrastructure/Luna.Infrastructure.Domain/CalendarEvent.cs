namespace Luna.Infrastructure.Domain
{
    using System;

    using Luna.Common;

    public class CalendarEvent : ITerm
    {
        private bool? _isWeekendHoliday;

        public virtual Type CalendarEventType
        {
            get { return this.GetType(); }
        }

        public virtual bool IsWeekendHoliday
        {
            get
            {
                if(_isWeekendHoliday == null)
                {
                    _isWeekendHoliday = (EventName == DayOfWeek.Sunday.ToString() || EventName == DayOfWeek.Saturday.ToString()) &&
                                (Start.DayOfWeek == DayOfWeek.Saturday || Start.DayOfWeek == DayOfWeek.Sunday);

                    
                }
                //if (_isWeekendHoliday == null && !string.IsNullOrEmpty(EventName) && Start != default(DateTime))

                return _isWeekendHoliday == true;
            }
        }

        public virtual string EventName { get; set; }

        public virtual DateTime Start { get; set; }

        public virtual DateTime End { get; set; }


    }
}