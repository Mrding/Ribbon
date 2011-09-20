using System;
using System.Collections;
//using Luna.Globalization;
using Luna.Core.Extensions;

namespace Luna.Common.Extensions
{
    public static class TermExt
    {

        public static DateRange MeasureCoverage(this ITerm source, DateTime start, DateTime end)
        {
            var coverage = new { Start = source.Start <= start ? source.Start : start, End = source.End >= end ? source.End : end };
            return new DateRange(coverage.Start, coverage.End);
        }

        /// <summary>
        /// AnyOverlap also include Enclosed 
        /// a  ----
        /// b    ----
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AnyOverlap(this ITerm a, ITerm b)
        {
            var overlap = a.End > b.Start && a.Start < b.End;
            return overlap;
        }


        public static bool AnyOverlap(this ITerm b, DateTime a_start, DateTime a_end)
        {
            var overlap = a_end > b.Start && a_start < b.End;
            return overlap;
        }

        
        /// <summary>
        /// a    ----   |  ----
        /// b ---       |      ----
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsIsolate(this ITerm a, ITerm b)
        {
            return a.Start >= b.End || b.Start >= a.End;
        }

        public static bool AnyOverlap(this ITerm exist, ITerm other, TimeSpan buffer)
        {
            return exist.End > other.Start.Add(-buffer) && exist.Start < other.End.Add(buffer);
        }


        ///// <summary>
        ///// Enclosed (A distinction between before and after)
        ///// </summary>
        ///// <param name="a">t</param>
        ///// <param name="b">other</param>
        ///// <returns></returns>
        //public static bool Enclosed(this ITerm a, ITerm b)
        //{
        //    if (a == b) return true;
        //    if (b == null) return false;

        //    return b.Start >= a.Start && b.End <= a.End;
        //}


        /// <summary>
        /// a ----
        /// b ----
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool TimeEquals(this ITerm a, ITerm b)
        {
            return a.Start == b.Start && a.End == b.End;
        }


        /// <summary>
        /// start       start
        ///       -------
        /// </summary>
        /// <param name="start"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsOutOfRange(this DateTime start, DateRange range)
        {
            return start < range.Start || start >= range.End;
        }
        
        /// <summary>
        ///  -----------------------
        ///     --------------
        /// </summary>
        /// <param name="term"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsOutOfRange(this ITerm term, DateTime start, DateTime end)
        {
            return term.Start >= end || term.End <= start;
        }

        /// <summary>
        ///     -----
        /// ------------
        /// </summary>
        /// <param name="term"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsInTheRange(this ITerm term, DateTime start, DateTime end)
        {
            return term.Start >= start && term.End <= end;
        }

        

        public static bool IsInTheRange(this ITerm term, DateRange range)
        {
            return IsInTheRange(term, range.Start, range.End);
        }

        /// <summary>
        /// a  ----    |  ----
        /// b    ----  |  ----
        /// </summary>
        /// <param name="term"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsCoverd(this ITerm term, DateTime start, DateTime end)
        {
            return term.Start < end && term.End > start || term.Start == start && term.End == end;
        }

        public static bool IsCoverd(this ITerm term, ITerm other)
        {
            return term.Start < other.End && term.End > other.Start || term.Start == other.Start && term.End == other.End;
        }

        public static bool IsCoverd(this ITerm term, DateRange range)
        {
            return term.Start < range.End && term.End > range.Start || term.Start == range.Start && term.End == range.End;
        }

        public static bool StartIsCoverd(this ITerm term, DateTime start, DateTime end)
        {
            return term.Start < end && term.Start >= start;
        }

        public static TimeSpan GetLength(this ITerm term)
        {
            return term.SaftyGetProperty<TimeSpan, DateRange>(r => r.Duration, () => term.End.Subtract(term.Start));
        }

        public static DateRange Get(this ITerm a, ITerm b)
        {
            if (a == null && b == null)
            {
                //throw new ArgumentNullException(LanguageReader.GetValue("StaleObjectStateException"));
                throw new ArgumentNullException("StaleObjectStateException");
            }
            if (a == null)
            {
                return new DateRange(b.Start, b.End);
            }
            if (b == null)
            {
                return new DateRange(a.Start, a.End);
            }
            var start = a.Start < b.Start ? a.Start : b.Start;
            var end = a.End > b.End ? a.End : b.End;
            return new DateRange(start, end);
        }
    }
}