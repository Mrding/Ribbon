using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain
{
    public class SubEventInsertRule : ICanConvertToValueTerm, IComparable, IComparable<ICanConvertToValueTerm>, IEquatable<SubEventInsertRule>, IEqualityComparer<SubEventInsertRule>
    {
        private TermStyle _subEvent;
        private VisibleSubEventTerm _visibleSubEventTerm;

        private TimeValueRange _timeRange;
        public virtual TimeValueRange TimeRange
        {
            get { return _timeRange; }
            set
            {
                _timeRange = value;
                if (SubEvent != null && SubEvent.TimeRange.Length != 0)
                    MaxOccurScale = _timeRange.Length - SubEvent.TimeRange.Length;
            }
        }

        public int Level { get { return 1; } }

        private int? _maxOccurScale;
        public virtual int? MaxOccurScale
        {
            get
            {
                if (_maxOccurScale == null && SubEvent != null)
                    _maxOccurScale = TimeRange.Length - SubEvent.TimeRange.Length;
                return _maxOccurScale;
            }
            set
            {
                _maxOccurScale = value < 0 ? 0 : value;
                if (OccurScale > _maxOccurScale && _maxOccurScale != null)
                    OccurScale = _maxOccurScale.Value;
            }
        }

        private int _occurScale = 0;
        public virtual int OccurScale
        {
            get { return _occurScale; }
            set
            {
                if (_subEvent != null && value > TimeRange.Length)
                    return;

                _occurScale = value;
            }
        }

        public virtual TermStyle SubEvent { get { return _subEvent; } set { _subEvent = value; } }

        public virtual int SubEventLength { get; set; }

        private DateTime _start;
        public virtual DateTime Start
        {
            get { return _start; }
            private set
            {
                _start = value;
                if (_visibleSubEventTerm != null)
                {
                    _visibleSubEventTerm.Start = _start;
                }
                End = _start.AddMinutes(TimeRange.Length);
            }
        }

        private DateTime _end;
        public virtual DateTime End
        {
            get { return _end; }
            private set { _end = value; }
        }

        public ICanConvertToValueTerm CreateVisibleSubEvent()
        {
            _visibleSubEventTerm = new VisibleSubEventTerm(_subEvent, new TimeValueRange(0, SubEventLength) , Start);
            return _visibleSubEventTerm;
        }

        public ICanConvertToValueTerm VisibleSubEvent { get { return _visibleSubEventTerm; } }

        private int? _possibleMovementTimes;

        public int GetPossibleMovementTimes()
        {
            if (!_possibleMovementTimes.HasValue)
                _possibleMovementTimes = OccurScale == 0 ? 0 : (TimeRange.Length - SubEvent.TimeRange.Length) / OccurScale;

            return _possibleMovementTimes.Value;
        }

        public int GetAmountOfAvailableOccurPoints()
        {
            return OccurScale > TimeRange.Length || OccurScale == 0 ? 1 : ((TimeRange.EndValue - SubEventLength - TimeRange.StartValue) / OccurScale) + 1;
        }

        public override int GetHashCode()
        {
            return TimeRange.StartValue;
        }

        public bool Equals(SubEventInsertRule x, SubEventInsertRule y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public int GetHashCode(SubEventInsertRule obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(SubEventInsertRule other)
        {
            if (ReferenceEquals(this, other)) return true;
            return SubEvent.Equals(other.SubEvent) && (
                 TimeRange.StartValue == other.TimeRange.StartValue ||
                 TimeRange.EndValue == other.TimeRange.EndValue);
        }

        public override bool Equals(object obj)
        {
            var other = obj as SubEventInsertRule;
            if (other == null) return false;
            return this.Equals(other);
        }

        public virtual int CompareTo(object obj)
        {
            var other = obj as ICanConvertToValueTerm;
            if (other == null) return -1;
            return CompareTo(other);
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

        public string Background
        {
            get { return "WhiteSmoke"; }
            set
            { //do nothing 
            }
        }

        public bool SetNewTime(DateTime start, DateTime end)
        {
            if (end.Equals(default(DateTime)))
            {
                Start = start;
                return true;
            }

            if (end <= start) return false;

            var oldStart = Start;
            var oldTimeRange = TimeRange;
            var oldOccurScale = OccurScale;

            var newStartValue = TimeRange.StartValue + Convert.ToInt32(start.Subtract(Start).TotalMinutes);
            var newEndValue = TimeRange.EndValue + Convert.ToInt32(end.Subtract(End).TotalMinutes);

            TimeRange = new TimeValueRange(newStartValue, newEndValue);
            Start = start;

            if (!_visibleSubEventTerm.IsInTheRange(start, end))
            {
                TimeRange = oldTimeRange;
                Start = oldStart;
                OccurScale = oldOccurScale;
                return false;
            }

            return true;
        }
    }
}