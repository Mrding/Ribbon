using System;
using Luna.Common;
using System.Linq;

namespace Luna.Shifts.Domain
{
    public static class TermStyleExt
    {
        private static readonly Type[] SubEventKinds = new[] { typeof(RegularSubEvent), typeof(UnlaboredSubEvent), typeof(OvertimeSubEvent) };

        public static bool IsSubEventKind(this TermStyle obj)
        {
            return SubEventKinds.Contains(obj.Type);
        }
    }

    public class TermStyle : Entity, ICanConvertToValueTerm, IComparable, IComparable<ICanConvertToValueTerm>, IVisibleTerm, IStyledTerm
    {
        protected TimeValueRange _timeRange;

        public virtual TimeValueRange TimeRange
        {
            get { return _timeRange; }
            set
            {
                if(value.Invalid) 
                    return;
                _timeRange = value;
            }
        }


        private DateTime _start;
        public virtual DateTime Start
        {
            get { return _start; }
            protected set
            {
                _start = value;
                End = Start.AddMinutes(TimeRange.Length);
            }
        }

        public virtual DateTime End { get; protected set; }

        public virtual bool HasChanged(bool compareWithDateOnly, out DateTime oldStart, out DateTime oldEnd)
        {
            throw new NotImplementedException();
        }

        public virtual int Level { get { return 2; } }

        public virtual bool Occupied { get; set; }

        protected bool _onService;
        public virtual bool OnService
        {
            get { return _onService; }
            set { _onService = value; }
        }

        private bool _inUse = true;
        public virtual bool InUse
        {
            get { return _inUse; }
            set { _inUse = value; }
        }

        public virtual string Foreground { get; set; }

        protected string _background;
        public virtual string Background
        {
            get { return _background; }
            set { _background = value; }
        }

        public virtual string Tag { get; set; }

        public virtual Type Type { get; set; }

        public virtual bool AsARest { get; set; }

        public override string GetUniqueKey()
        {
            return Name;
        }

        public virtual string Text { get { return Name; } }

        public virtual string Remark { get; set; }

        public virtual double TimespanWidth { get; set; }

        public virtual int CustomLength { get; set; }

        public virtual bool SetNewTime(DateTime start, DateTime end)
        {
            if (end.Equals(default(DateTime)))
            {
                Start = start;
                return true;
            }

            if (end <= start) return false;
            var newStartValue = TimeRange.StartValue + Convert.ToInt32(start.Subtract(Start).TotalMinutes);
            var newEndValue = TimeRange.EndValue + Convert.ToInt32(end.Subtract(End).TotalMinutes);

            var newTimeValueRange = new TimeValueRange(newStartValue, newEndValue);
            if (newTimeValueRange.Invalid)
                return false;

            TimeRange = newTimeValueRange;
            
            Start = start;
            return true;
        }

        public virtual int CompareTo(object obj)
        {
            var other = obj as ICanConvertToValueTerm;
            if (other == null) return -1;
            return CompareTo(other);
        }

        public virtual int CompareTo(ICanConvertToValueTerm other)
        {
            var cLevel = other.Level.CompareTo(Level);
            if (cLevel == 0)
            {
                cLevel = other.Start.CompareTo(Start);
            }
            return -cLevel;
        }
    }
}
