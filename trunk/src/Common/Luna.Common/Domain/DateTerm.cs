using System;

namespace Luna.Common.Domain
{
    public class DateTerm : IVisibleTerm, IStyledTerm, IIndexable
    {
        private DateTime _date;
        protected readonly string _format;

        public DateTerm() { }

        public DateTerm(DateTime date, string format)
        {
            _date = date;
            _format = format;
        }

        public virtual DateTime Date
        {
            get { return _date; }
            protected set { _date = value; }
        }

        public virtual bool IsWeekend { get { return _date.DayOfWeek == DayOfWeek.Sunday || _date.DayOfWeek == DayOfWeek.Saturday; } }

        public virtual bool IsHoliday { get; set; }

        public virtual bool IsDaylightSaving { get; set; }

        public virtual bool IsDirty { get; set; }

        DateTime ITerm.Start
        {
            get { return Date; }
        }

        DateTime ITerm.End
        {
            get { return Date.AddDays(1); }
        }

        public virtual string Text
        {
            get
            {
                //return Index == 0 ? string.Format(_format, Date) : Date.Day.ToString();
                return string.Format(_format, Date);
            }
        }

        public virtual string Remark { get; set; }

        public virtual string Background
        {
            get { return "Transparent"; }
        }

        public virtual int Index { get; set; }

        public override string ToString()
        {
            return Text;
        }

        //public override bool Equals(object obj)
        //{
        //    var other = obj as DateTerm;

        //    if (other == null) return false;
        //    return other.Date == Date;
        //}

        //public override int GetHashCode()
        //{
        //    return Date.GetHashCode();
        //}
    }
}