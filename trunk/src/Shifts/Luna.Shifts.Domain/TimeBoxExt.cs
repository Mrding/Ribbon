using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using System.Text;
using System.Security.Cryptography;
using Luna.Core.Extensions;
using Luna.Common.Extensions;
using System.Diagnostics;


namespace Luna.Shifts.Domain
{
    public static class TimeBoxExt
    {
        public static IOrderedEnumerable<Term> OrderByStartAndLevel(this IEnumerable<Term> terms)
        {
            return terms.OrderBy(t => t.Start).ThenByDescending(t => t.Level);
        }

        /// <summary>
        /// OrderByStartAndLevel with direction
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="direction">"L" or "R"</param>
        /// <returns></returns>
        public static IOrderedEnumerable<Term> OrderByStartAndLevel(this IEnumerable<Term> terms, string direction)
        {
            if (direction == "L")
                return terms.OrderBy(t => t.Start).ThenBy(t => t.Level);


            return terms.OrderByDescending(t => t.End).ThenBy(t => t.Level);
        }

        public static IOrderedEnumerable<Term> OrderByStartAndLevel(this IEnumerable<Term> terms, TimeSpan moveOffset)
        {
            if (moveOffset.Ticks > 0) // move right
                return terms.OrderByDescending(t => t.End).ThenBy(t => t.Level);
            //move left
            return terms.OrderBy(t => t.Start).ThenBy(t => t.Level);
        }

        public static IEnumerable<Term> WhereEditableTerms(this IEnumerable<Term> terms, Term source)
        {
            return terms.Where(t => !ReferenceEquals(source, t) && source.IsEditableChildTerm(t));
        }

        public static IEnumerable<Term> WhereDeletableTerms(this IEnumerable<Term> terms, Term source)
        {
            return terms.Where(t => !ReferenceEquals(source, t) && source.IsDeletableChildTerm(t));
        }

        public static IEnumerable<Term> WhereFamilyTerms(this IEnumerable<Term> terms, Term source)
        {
            return terms.Where(o => o.IsNot<IImmutableTerm>() && o.Level > 0 && (o.ParentTerm.Equals(source) || o.GetLowestTerm().Equals(source)));
        }

        public static Term FindOne(this TimeBox timeBox, Type type, DateTime start)
        {
            return timeBox.TermSet.Retrieve<Term>(start, start).SingleOrDefault(t => t.Start == start && t.GetType() == type);
        }

        public static IEnumerable<Term> RetriveTerms(this TimeBox timeBox, DateTime start, DateTime end)
        {
            var terms = timeBox.GetAllTerm<Term>(start, end).ToList();
            terms.RemoveAll(o => o.IsInTheRange(start, end));
            return terms;
        }

        public static T FindCrossTerm<T, TExcpet>(this IEnumerable<Term> terms, DateTime time)
            where T : Term
            where TExcpet : Term
        {
            return terms.OfType<T>().SingleOrDefault(t => !(t is TExcpet) && time > t.Start && time < t.End);

        }

        /// <summary>
        /// Get Assignment DNA
        /// </summary>
        /// <param name="timeBox"></param>
        /// <param name="source">Should be assignment type</param>
        /// <returns></returns>
        public static string GetShiftprint(this TimeBox timeBox, Term source)
        {
            var summary = new StringBuilder(string.Format("{0:yyyy/MM/dd HH:mm}%{1:yyyy/MM/dd HH:mm}#{2}", source.Start, source.End, source.Id));
            foreach (var item in timeBox.GetCoveredTermsWithAbsent(source))
            {
                summary.Append(string.Format("{0:yyyy/MM/dd HH:mm}%{1:yyyy/MM/dd HH:mm}#{2}", item.Start, item.End, item.Id));
            }

            var m = new MD5CryptoServiceProvider();
            byte[] s = m.ComputeHash(Encoding.UTF8.GetBytes(summary.ToString()));
            return BitConverter.ToString(s);
        }

        //代换班通用扩展方法
        public static IEnumerable<Term> ReMove(this List<Term> terms, Func<Term, bool> predicate, bool hasBelongToPrv)
        {
            var assignments = terms.SpecificTerm<AssignmentBase>().Where(predicate).OfType<AssignmentBase>();
            foreach (var assignment in assignments)
            {
                if (assignment.BelongToPrv.HasValue &&assignment.BelongToPrv.Value==hasBelongToPrv)
                    terms.RemoveAll(o => o.Start >= assignment.Start && o.End <= assignment.End);
            }
            return terms;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool InDate(this Term source,Term term)
        {
            return source.Start.Date != term.Start.Date;
        }
        /// <summary>
        /// 是否存在跨天的班
        /// </summary>
        public static bool HasDayTerms(this IEnumerable<Term> terms)
        {
            return terms.Any(o => o.Start.Date.AddDays(1) == o.End.Date);
        }
        /// <summary>
        /// 取范围之内的
        /// </summary>
        public static IEnumerable<Term> CenterTerms(this IEnumerable<Term> terms, DateRange range)
        {
            return terms.Where(o => o.Start >= range.Start && o.End <= range.End);
        }
        /// <summary>
        /// 取范围之内的
        /// </summary>
        public static IEnumerable<Term> CenterTerms(this TimeBox timeBox, DateRange range)
        {
            return timeBox.TermSet.CenterTerms(range);
        }
        /// <summary>
        /// 取范围之内的
        /// </summary>
        public static IEnumerable<Term> CenterTerms(this TimeBox timeBox, Term source)
        {
            return timeBox.CenterTerms(new DateRange(source.Start, source.End));
        }
        /// <summary>
        /// 取范围碰到的
        /// </summary>
        public static IEnumerable<Term> CollideTerms(this IEnumerable<Term> terms, DateRange range)
        {
            return terms.Where(o => o.IsCoverd(range.Start, range.End));
        }
        /// <summary>
        /// 取范围碰到的
        /// </summary>
        public static IEnumerable<Term> CollideTerms(this IEnumerable<Term> terms, Term source)
        {
            return terms.CollideTerms(new DateRange(source.Start, source.End));
        }
        /// <summary>
        /// 取范围碰到的
        /// </summary>
        public static IEnumerable<Term> CollideTerms(this IEnumerable<Term> terms, DateTime start,DateTime end)
        {
            return terms.CollideTerms(new DateRange(start, end));
        }
        /// <summary>
        /// 取范围碰到的
        /// </summary>
        public static IEnumerable<Term> CollideTerms(this TimeBox timeBox, Term range)
        {
            return timeBox.TermSet.CollideTerms(range);
        }
        /// <summary>
        /// 取范围碰到的
        /// </summary>
        public static IEnumerable<Term> CollideTerms(this TimeBox timeBox, DateRange range)
        {
            return timeBox.TermSet.CollideTerms(range);
        }
        /// <summary>
        /// 取范围之外的
        /// </summary>
        public static IEnumerable<Term> OutOfTerms(this IEnumerable<Term> terms, DateRange range)
        {
            return terms.Where(o => o.IsOutOfRange(range.Start, range.End));
        }
        /// <summary>
        /// 特定类型的
        /// </summary>
        public static IEnumerable<Term> SpecificTerm<T>(this IEnumerable<Term> terms) where T : Term
        {
            return terms.Where(o => o is T);
        }
        /// <summary>
        /// 特定类型的
        /// </summary>
        public static IEnumerable<Term> SpecificTerm<T>(this TimeBox timeBox) where T : Term
        {
            return timeBox.TermSet.SpecificTerm<T>();
        }
        /// <summary>
        /// 切割指定班表，返回占席区域
        /// </summary>
        public static IList<T> SliceOccupied<T>(this IEnumerable<Term> terms, Func<DateRange, Term, T> construct, Func<Term, bool> isNeedSeat)
        {
            var orderTerms = terms.OrderBy(o => o.Level).ThenBy(o => o.Start).ToList();
            var termsCutter = new TermsCutter<Term, T>
                (orderTerms, o => true, (s, e, t) =>
                {
                    return construct(new DateRange(s, e), t);
                });

            return termsCutter.ToList((begin, current) =>
                                          {
                                              //发生于首个term刚好为subevent
                                              if (begin == null && current != null && current.Level > 0)
                                                  return true;
                                              return begin.OccupationComparison(current, isNeedSeat);
                                          });
        }

        public static IList<T> GetSeatArrangement<T>(this IEnumerable<Term> terms, Func<DateRange, Term, bool, T> construct)
        {
            var orderTerms = terms.OrderBy(o => o.Level).ThenBy(o => o.Start).ToList();
            var termsCutter = new TermsCutter<Term, T>
                (orderTerms, o =>
                                 {
                                     return true;
                                     // //if (o.Is<AbsentEvent>()) return false;
                                     // var loweatTerm = o.GetLowestTerm();
                                     // if (loweatTerm == null) return false;
                                     //return  !loweatTerm.IsNeedSeat;
                                 },
                                 (s, e, t) =>
                                 {
                                     return construct(new DateRange(s, e), t, t == null ? false : t.IsNeedSeat);
                                 });

            return termsCutter.ToList((begin, current) =>
            {
                return begin.OccupationComparison(current, t => t.IsNeedSeat);
            });
        }


        public static IEnumerable<RtaaSlicedTerm> SliceIntoPieces(this IEnumerable<Term> terms)
        {
            var orderTerms = terms.OrderBy(o => o.Level).ThenBy(o => o.Start).ToList();
            var termsCutter = new TermsCutter<Term, RtaaSlicedTerm>
                (orderTerms, o => true, (s, e, t) =>
                                 {
                                     //if (t == null)
                                     //    return default(SlicedTerm);
                                     return new RtaaSlicedTerm
                                                {
                                                    Start = s,
                                                    End = e,
                                                    OnService = t == null ? false : t.OnService,
                                                    Text = t == null ? string.Empty : t.Text,
                                                    Level = t == null ? -1 : t.Level
                                                };
                                 });

            return termsCutter.ToList((begin, current) =>
                                          {
                                              bool currentOnService = current != null ? current.OnService : false;
                                              bool beginOnService = begin != null ? begin.OnService : false;
                                              if (beginOnService != currentOnService)
                                                  return true;

                                              //如果onService 的不同种类要分开加下面的
                                              //if (next != current)
                                              //    return true;

                                              return false;
                                          });
        }

        public static bool[] ConvertToCell(this IEnumerable<Term> terms, ITerm boundary, int cellUnit, Func<Term, bool> cellResult, Action<Term> additionalLooping)
        {
//#if DEBUG
//            var sw = Stopwatch.StartNew();
//#endif

            var capacity = Convert.ToInt32(boundary.GetLength().TotalMinutes) / cellUnit;

            var cells = new bool[capacity];

            var boundaryStart = boundary.Start;
            var boundaryCapacity = cells.Length;
            foreach (var item in terms)
            {
                if (item.Start >= boundary.End) continue;
                var content = cellResult(item);
                var endIndex = item.End.GetIndex(boundaryStart, cellUnit, boundaryCapacity, 0);
                for (var i = item.Start.GetIndex(boundaryStart, cellUnit, boundaryCapacity, 0); i < endIndex; i++)
                {
                    cells[i] = content;
                }
                if (additionalLooping != null)
                    additionalLooping(item);
            }
//#if DEBUG
//            sw.Stop();
//            Console.WriteLine("{0}", sw.Elapsed.Seconds);
//#endif
            return cells;
        }

        public static bool[] ConvertToCell(this IEnumerable<Term> terms, ITerm boundary, int cellUnit, Func<Term, bool> func)
        {
            return terms.ConvertToCell(boundary, cellUnit, func, null);
        }
    }
}
