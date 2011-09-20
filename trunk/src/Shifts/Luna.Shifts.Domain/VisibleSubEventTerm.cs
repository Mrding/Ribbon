using System;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    public sealed class VisibleSubEventTerm : ICanConvertToValueTerm, IComparable, IComparable<ICanConvertToValueTerm>, IVisibleTerm
    {
        private readonly TermStyle _source;
        public VisibleSubEventTerm(TermStyle source, TimeValueRange timeValueRange, DateTime start)
        {
            _source = source;
            TimeRange = timeValueRange;
            Background = _source.Background;
            Start = start;
        }

        public VisibleSubEventTerm(TermStyle source, DateTime start):this(source, source.TimeRange, start)
        {}

        public TermStyle Source { get { return _source; } }

        public TimeValueRange TimeRange { get; set; }


        private DateTime _start;
        public DateTime Start
        {
            get { return _start; }
            internal set
            {
                _start = value;
                End = Start.AddMinutes(TimeRange.Length);
            }
        }

        public DateTime End { get; private set; }

        public int Level { get { return 2; } }

        public string Text { get { return _source.Text; } }

        public string Remark { get; set; }

        public string Background { get; set; }

        public int CompareTo(object obj)
        {
            var other = obj as ICanConvertToValueTerm;

            if (other == null) return -1;
            var cLevel = other.Level.CompareTo(Level);
            if (cLevel == 0)
            {
                cLevel = other.Start.CompareTo(Start);
            }

            return -cLevel;
        }

        public bool SetNewTime(DateTime start, DateTime end)
        {
            return false;
        }

        public int CompareTo(ICanConvertToValueTerm other)
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