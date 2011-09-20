using System;
using System.Collections.Generic;
using Luna.Core;

namespace Luna.Common
{

    public class TimeRange : ITerm
    {
        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public bool Invalid
        {
            get { return Start >= End; }
        }
    }

    public class DateRangeRef
    {
        private TimeSpan _timeSpan;

        private DateTime _start, _end;

        public DateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                if (_end != default(DateTime))
                    _timeSpan = _end.Subtract(_start);
            }
        }


        public DateTime End
        {
            get { return _end; }
            set
            {
                _end = value;
                if (_start != default(DateTime))
                    _timeSpan = _end.Subtract(_start);
            }
        }

        public DateRangeRef()
        {
        }

        public DateRangeRef(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
            if (_start > _end)
                throw new Exception("The start time can't less then end");
            _timeSpan = _end.Subtract(_start);
        }

        public DateRangeRef(DateTime start, TimeSpan duration)
        {
            _start = start;
            _end = start.Add(duration);
            if (_start > _end)
                throw new Exception("The start time can't less then end");
            _timeSpan = _end.Subtract(_start);
        }

        public DateRange ExtendDays(double value)
        {
            return new DateRange(Start, End.AddDays(value).Subtract(Start));
        }

        public bool IsOutOfRange(DateTime value)
        {
            return (value < Start || value >= End);
        }

        public TimeSpan Duration
        {
            get
            {
                if (_timeSpan == TimeSpan.Zero)
                    _timeSpan = End.Subtract(Start);
                return _timeSpan;
            }
        }

        public static bool operator ==(DateRangeRef r1, DateRangeRef r2)
        {
            return r1.End > r2.Start && r1.Start < r2.End || r2.Start == r1.Start && r2.End == r1.End;
        }

        public static bool operator !=(DateRangeRef r1, DateRangeRef r2)
        {
            return r1.Start >= r2.End || r2.Start >= r1.End;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Start.GetHashCode() + Duration.TotalMinutes.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("{0:yyyy/MM/dd hh:mm} - {1:yyyy/MM/dd hh:mm}", Start, End);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is DateRangeRef)
                return this == (DateRangeRef)obj;
            return false;
        }
    }


    public struct DateRange : IEquatable<DateRange>, IEqualityComparer<DateRange>, ITerm, IDateRange
    {
        private TimeSpan _timeSpan;

        private DateTime _start, _end;

        public DateTime Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public DateTime End
        {
            get { return _end; }
            set { _end = value; }
        }

        public DateRange(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
            //if (_start > _end)
            //    throw new Exception("The start time can't less then end");
            _timeSpan = _end.Subtract(_start);
        }

        public DateRange(DateTime start, TimeSpan duration)
        {
            _start = start;
            _end = start.Add(duration);
            //if (_start > _end)
            //    throw new Exception("The start time can't less then end");
            _timeSpan = _end.Subtract(_start);
        }

        public bool InValid
        {
            get { return _start > _end || _start == default(DateTime) || _end == default(DateTime); }
        }

        public DateRange ExtendEndDay(double value)
        {
            return new DateRange(Start, End.AddDays(value).Subtract(Start));
        }

        /// <summary>
        /// 前后扩展N天
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DateRange ExtendDay(double value)
        {
            return new DateRange(Start.AddDays(-value), End.AddDays(value));
        }
        /// <summary>
        /// 取得两个时间最多部分
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public DateRange GetSwapingDate(DateRange a, DateRange b)
        {
            var start = a.Start < b.Start ? a.Start : b.Start;
            var end = a.End > b.End ? a.End : b.End;
            return new DateRange(start, end);
        }

        /// <summary>
        /// 切割时间块
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static IList<Core.Tuple<DateRange, RegionType>> Cut(DateRange a, DateRange b)
        {
            IList<Core.Tuple<DateRange, RegionType>> result = new List<Core.Tuple<DateRange, RegionType>>();

            if (a.Start < b.Start)
            {
                if (a.End <= b.Start)
                {
                    //|__A__|
                    //       |__B__|

                    var t1 = new Core.Tuple<DateRange, RegionType>(a, RegionType.Applier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(b, RegionType.Replier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2 };
                }
                if (a.End < b.End)
                {
                    //       |_A_|
                    //           |_B_|

                    DateRange r1 = new DateRange(a.Start, b.Start);
                    DateRange r2 = new DateRange(b.Start, a.End);
                    DateRange r3 = new DateRange(a.End, b.End);
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Applier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Overlap);
                    var t3 = new Core.Tuple<DateRange, RegionType>(r3, RegionType.Replier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2, t3 };
                }
                if (a.End == b.End)
                {
                    //      |__A__|
                    //          |_B_|

                    DateRange r1 = new DateRange(a.Start, b.Start);
                    DateRange r2 = b;
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Applier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Overlap);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2 };
                }
                else 
                {
                    //      |__A__|
                    //        |_B_|

                    DateRange r1 = new DateRange(a.Start, b.Start);
                    DateRange r2 = b;
                    DateRange r3 = new DateRange(b.End, a.End);
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Applier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Overlap);
                    var t3 = new Core.Tuple<DateRange, RegionType>(r3, RegionType.Applier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2, t3 };
                }
            }

            if (a.Start == b.Start)
            {
                if (a.End < b.End)
                {
                    //      |_A_|
                    //      |__B__|

                    DateRange r1 = a;
                    DateRange r2 = new DateRange(a.End, b.End);
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Overlap);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Replier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2 };
                }
                if (a.End == b.End)
                {
                    //      |__A__|
                    //      |__B__|

                    var t1 = new Core.Tuple<DateRange, RegionType>(a, RegionType.Overlap);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1 };
                }
                else
                {
                    //      |__A__|
                    //      |_B_|

                    DateRange r1 = b;
                    DateRange r2 = new DateRange(b.End, a.End);
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Overlap);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Applier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2 };
                }
            }

            if (a.Start > b.Start)
            {
                if (a.End < b.End)
                {
                    //     |_A_|
                    //    |__B__|

                    DateRange r1 = new DateRange(b.Start, a.Start);
                    DateRange r2 = a;
                    DateRange r3 = new DateRange(a.End, b.End);
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Replier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Overlap);
                    var t3 = new Core.Tuple<DateRange, RegionType>(r3, RegionType.Replier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2, t3 };
                }
                if (a.End == b.End)
                {
                    //      |_A_|
                    //    |__B_|

                    DateRange r1 = new DateRange(b.Start, a.Start);
                    DateRange r2 = a;
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Replier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Overlap);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2 };
                }
                if (a.Start >= b.End)
                {
                    //          |__A__|
                    //|__B__|

                    var t1 = new Core.Tuple<DateRange, RegionType>(a, RegionType.Applier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(b, RegionType.Replier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2 };
                }
                else
                {
                    //      |__A__|
                    //    |__B__|

                    DateRange r1 = new DateRange(b.Start, a.Start);
                    DateRange r2 = new DateRange(a.Start, b.End);
                    DateRange r3 = new DateRange(b.End, a.End);
                    var t1 = new Core.Tuple<DateRange, RegionType>(r1, RegionType.Replier);
                    var t2 = new Core.Tuple<DateRange, RegionType>(r2, RegionType.Overlap);
                    var t3 = new Core.Tuple<DateRange, RegionType>(r3, RegionType.Applier);
                    return new List<Core.Tuple<DateRange, RegionType>> { t1, t2, t3 };
                }
            }

            return result;
        }

        public bool IsOutOfRange(DateTime value)
        {
            return (value < Start || value >= End);
        }

        public bool IsInRange(DateRange dateRange)
        {
            return (Start < dateRange.End && End > dateRange.Start) || (Start == dateRange.Start && End == dateRange.End);
        }

        public bool IsOutofRange(DateRange dateRange)
        {
            return Start > dateRange.End || End < dateRange.Start;
        }

        public TimeSpan Duration
        {
            get
            {
                if (_timeSpan == TimeSpan.Zero)
                    _timeSpan = End.Subtract(Start);
                return _timeSpan;
            }
        }

        public static bool operator ==(DateRange a, DateRange b)
        {
            return (a.Start == b.Start) && (a.End == b.End);
        }

        public static bool operator !=(DateRange a, DateRange b)
        {
            return (a.Start != b.Start) || (a.End != b.End);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Start.GetHashCode() + Duration.TotalMinutes.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("{0:g} - {1:g}", Start, End);
        }

        public string ToDayString()
        {
            return string.Format("{0:yyyy.MM.dd} - {1:yyyy.MM.dd}", Start, End);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is DateRange)
                return this.Hit((DateRange)obj);
            return false;
        }

        public bool Equals(DateRange other)
        {
            return this.Hit(other);
        }

        public bool Equals(DateRange x, DateRange y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(DateRange obj)
        {
            return obj.GetHashCode();
        }
    }

    public enum RegionType
    {
        /// <summary>
        /// A
        /// </summary>
        Applier,
        /// <summary>
        /// B
        /// </summary>
        Replier,
        /// <summary>
        /// AB
        /// </summary>
        Overlap
    }

    public static class DateRangeExtentsion
    {
        public static bool Hit(this DateRange r1, DateRange r2)
        {
            return r1.End > r2.Start && r1.Start < r2.End || r2.Start == r1.Start && r2.End == r1.End;
        }
    }
}