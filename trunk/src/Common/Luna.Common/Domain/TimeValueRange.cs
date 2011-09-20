using System;
using System.Collections.Generic;

namespace Luna.Common
{
    public static class TiemeValueExt
    {
        public static int ToTimeValue(this string timeString)
        {
            var time = timeString.Split(':');
            if (time == null || time.Length < 2)
                return 0;
            return Convert.ToInt32(time[0]) * 60 + Convert.ToInt32(time[1]);
        }

        public static int GetDaysCount(this DateTime startDate, DateTime endDate)
        {
            return (int)endDate.Date.Subtract(startDate.Date).TotalDays + 1;
        }
    }

    public struct TimeValueRange : IEquatable<TimeValueRange>, IEqualityComparer<TimeValueRange>
    {
        public TimeValueRange(int startValue, int endValue)
            : this()     //默认继承this()是因为先调用默认构造函数来赋所有字段为默认值，然后才可以赋值。
        {
            if (endValue < startValue)
                endValue = endValue + 1440;

            CheckValue(startValue);
            _startValue = startValue;
            CheckValue(endValue);
            _endValue = endValue;

            if (_endValue < _startValue)
                Invalid = true;
        }

        private int _startValue;
        public int StartValue
        {
            get { return _startValue; }
            set
            {
                CheckValue(value);
                _startValue = value;
            }
        }

        private int _endValue;
        public int EndValue
        {
            get { return _endValue; }
            set
            {
                CheckValue(value);
                _endValue = value;
            }
        }

        public bool Invalid { get; set; }

        public int Length
        {
            get { return this.EndValue - this.StartValue; }
        }

        private void CheckValue(int newValue)
        {
            if (newValue > 2880 || newValue < -720)
                Invalid = true;
        }


        public override bool Equals(object obj)
        {
            if (obj is TimeValueRange)
            {
                var other = (TimeValueRange)obj;
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (_startValue ^ _endValue).GetHashCode();
        }

        public bool Equals(TimeValueRange other)
        {
            return _startValue == other.StartValue && _endValue == other.EndValue;
        }

        public bool Equals(TimeValueRange x, TimeValueRange y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(TimeValueRange obj)
        {
            return obj.GetHashCode();
        }
    }
}