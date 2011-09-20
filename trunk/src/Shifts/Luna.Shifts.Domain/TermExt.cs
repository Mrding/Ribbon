using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Common.Extensions;

namespace Luna.Shifts.Domain
{
    public static class TermExt
    {
        public static readonly Random ArrangeSubEventRandom = new Random();

        public static bool TryReplaceSeat(this Term t)
        {
            var typeIsvalid = t.IsNot<AbsentEvent>() && t.IsNot<AssignmentBase>();
            if (typeIsvalid && t.IsNeedSeat && t.Bottom.IsNeedSeat)
            {
                t.Seat = null;
                return true;
            }
            return false;
        }

        public static void ForceAssignSeat(this Term t, IEnumerable<Term> foundTerms, bool isNewCreated)
        {
            if (t.Is<IOffWork>()) return;

            if (t is AssignmentBase || t.IsNeedSeat)
            {
                t.TryReplaceSeat();
                return;
            }

            if (t.IsNot<AbsentEvent>() && isNewCreated)
            {
                if (t.SeatIsEmpty())
                    t.Seat = t.DiveInto(o => o.OcuppiedAndHasSeat()).Seat;
            }
            else if (t is AbsentEvent)
            {
                var seatReferenceTerm = foundTerms.Where(o => !ReferenceEquals(o, t) && !o.SeatIsEmpty()).Select(o => new { Source = o, Distance = t.Start.Subtract(o.Start) })
                                                             .Where(o => o.Distance.TotalMinutes >= 0)
                                                             .OrderBy(o => o.Distance)
                                                             .FirstOrDefault();
                if (seatReferenceTerm == null) return;

                t.Seat = seatReferenceTerm.Source.CanBeSeatSource() ?
                    seatReferenceTerm.Source.Seat : seatReferenceTerm.Source.DiveInto(o => o.CanBeSeatSource()).Seat;
            }
        }

        public static bool CanBeSeatSource(this Term t)
        {
            return t.Level <= 1 || t is AbsentEvent;
        }

        public static bool? StillInParentRange(this Term t)
        {
            if (t.Bottom != null)
                return t.IsInTheRange(t.Bottom.Start, t.End);

            return null;
        }

        public static bool LowestParentEq(this Term t, Term other)
        {
            return Term.FindBottomMost(t) == other;
        }


        public static bool EndEq(this ITerm a, IAssignment b)
        {
            return a.End == b.Finish;
        }

        public static bool StartEq(this ITerm a, IAssignment b)
        {
            return a.Start == b.From;
        }

        public static bool HrDateEq(this ITerm a, ITerm b)
        {
            return a.GetLowestTerm().HrDate == b.GetLowestTerm().HrDate;
        }

        public static IIndependenceTerm GetLowestTerm(this ITerm term)
        {
            if (term is IIndependenceTerm) return term as IIndependenceTerm;

            var temp = (Term)term;
            while (temp.Bottom != null)
            {
                temp = temp.Bottom;
            }

            if (term.Is<IOffWork>())
            {

            }

            return temp as IIndependenceTerm;
        }



        public static bool SeatIsEmpty(this ISeatingTerm term)
        {
            return string.IsNullOrEmpty(term.Seat);
        }


        public static bool OcuppiedAndHasSeat(this Term term)
        {
            return term.OcuppiedAndHasSeat(t =>
                                                 {
                                                     if (t is UnlaboredSubEvent) return false;
                                                     return t.IsNeedSeat;
                                                 });
        }

        public static bool OcuppiedAndHasSeat(this Term term, Func<Term, bool> isNeedSeat)
        {
            return isNeedSeat(term) && !term.SeatIsEmpty();
        }

        public static void ReleaseSeat(this Term term)
        {
            term.Seat = null;
            //term.SaftyInvoke<AssignmentBase>(x => x.OccupyStatus = "W");
        }

        public static Term DiveInto(this Term term, Func<Term, bool> func)
        {
            if (term is IAssignment) return term;

            var temp = term;
            while (temp.Bottom != null)
            {
                temp = temp.Bottom;
                if (func(temp))
                    break;
            }
            return temp;
        }



        public static bool LookDown(this Term term, Func<Term, bool> func, bool includeSelf)
        {
            var result = false;
            var temp = includeSelf ? term : term.Bottom;

            while (temp != null)
            {
                if (func(temp))
                {
                    result = true;
                    break;
                }
                temp = temp.Bottom;
            }

            return result;
        }


        public static bool IgnoreAdherence(this IIndependenceTerm term)
        {
            return term.IgnoreAdherence;
        }

        public static bool BottomIsLocked(this Term term)
        {
            return term.Bottom != null && term.Bottom.Locked;
        }


        public static object GetBottomStyle(this ITerm term)
        {
            if (term is Term)
            {
                var bottom = (term as Term).Bottom;
                if (bottom != null)
                    return bottom.Text;
            }
            return null;
        }

        public static void LoopFromLast<T>(this T[] array, Func<int, bool> condition, Action<int> action)
        {
            var length = array.Length;
            for (var i = length - 1; i > 0; i--)
            {
                if (condition(i))
                {
                    action(i);
                    break;
                }
            }
        }

        public static void Loop<T>(this T[] array, Func<int, bool> condition, Action<int> action)
        {
            var length = array.Length;
            for (var i = 0; i < length; i++)
            {
                if (condition(i))
                {
                    if (condition(0))
                        action(i);
                    else
                        action(i - 1);
                    break;
                }
            }
        }


        //public static bool IsOutOfBoundary(this Term term, DateTime checkPoint, DateRange boundary)
        //{
        //    var lowestTerm = term.GetLowestTerm();

        //    var isDayOffOrTimeOff = term is DayOff || term is TimeOff;

        //    var compareTime = lowestTerm == term || isDayOffOrTimeOff ? checkPoint : lowestTerm.Start;

        //    if (compareTime < boundary.Start)
        //        return true;

        //    if (compareTime >= boundary.End && !lowestTerm.BelongToPrv)
        //        return true;

        //    return false;
        //}


        public static bool IsOutOfBoundary(this IIndependenceTerm lowestTerm, DateTime compareTime, TimeBox timeBox)
        {
            var originalStartTime = lowestTerm.Start;

            var insideTimeBox = originalStartTime.IsInTheRange(timeBox.Boundary.Start, timeBox.Boundary.End);

            if (insideTimeBox == false && lowestTerm.BelongToPrv == false)
                return true;

            if (insideTimeBox && lowestTerm.BelongToPrv == true)
                return true;

            var distanceBetweenCompareTimeAndBoundaryEnd = compareTime.Subtract(timeBox.Boundary.End);

            if (insideTimeBox == false && compareTime.IsInTheRange(timeBox.Boundary.Start, timeBox.Boundary.End) && lowestTerm.BelongToPrv == true)
                return false;

            if (distanceBetweenCompareTimeAndBoundaryEnd > TimeSpan.FromHours(12))
                return true;

            if (compareTime < timeBox.Boundary.Start)
                return true;

            //if (compareTime >= timeBox.Boundary.End && !lowestTerm.BelongToPrv)
            //    return true;

            return false;
        }

        public static bool IsOutOfBoundary(this Term term, Action<IIndependenceTerm> lowestTermDetection, TimeBox timeBox)
        {
            var lowestTerm = term.GetLowestTerm();

            lowestTermDetection(lowestTerm);

            //var compareTime = (lowestTerm as Term ?? term).Start;

            //if (compareTime < timeBox.Boundary.Start)
            //    return true;

            //if (compareTime >= timeBox.Boundary.End && !lowestTerm.BelongToPrv)
            //    return true;

            return lowestTerm.IsOutOfBoundary(lowestTerm.Start, timeBox);
        }

        public static void Reset(this Term source, TimeBox timeBox, Func<Term, bool> isNeedReset)
        {
            //TODO:Fix it for better performance
            //var results = timeBox.GetCoveredTermsWithAbsent(source);

            foreach (var term in timeBox.TermSet)
            {
                if (isNeedReset(term))
                    term.Reset();
            }
            source.Reset();
        }



        public static bool VerifyAnyOverlap(this Term source, TimeBox timeBox)
        {
            var lowestTerm = source.GetLowestTerm() as Term;
            var convertTerms = timeBox.GetCoveredTermsWithAbsent(lowestTerm).ToArray();

            if (lowestTerm == source)
            {
                return timeBox.TermSet.Where(o => o != source && o.Level == source.Level).Any(o =>
                    {
                        var overlap = source.AnyOverlap(o);
                        return overlap;
                    });
            }

            var closestBottomTerm = timeBox.GetClosestBottom(source);
            if (closestBottomTerm != source.Bottom)
                return true;

            return convertTerms.Where(o => o != source || o.Id != source.Id).OrderBy(o => o.Start).Any(o => o.Id != source.Id && o.Level > source.Level ? source.OverlapNotEnclosed(o) : o.OverlapNotEnclosed(source));
        }

        public static string GetAction(this Term entity)
        {
            var start = entity.GetSnapshotValue<DateTime>("Start");
            var end = entity.GetSnapshotValue<DateTime>("End");
            var locked = entity.GetSnapshotValue<bool>("Locked");

            if (start == entity.Start && end == entity.End && locked == entity.Locked) // no changed
                return string.Empty;

            var length = (entity.End - entity.Start).TotalMinutes;
            var oldtime = (end - start).TotalMinutes;

            if (length > oldtime)
                return "E";
            if (length < oldtime)
                return "S";
            if (length == oldtime && entity.Start != start)
                return "M";
            if (entity.Locked)
                return "LK";

            return "UK";
        }

        public static Term GetClosestBottom(this Term target, IEnumerable<Term> terms)
        {
            return terms.OrderByDescending(o => o.Level).FirstOrDefault(o => target.IsInTheRange(o.Start, o.End));
        }

        public static bool NotAssignment(this Term term)
        {
            return !(term is AssignmentBase);
        }

        public static void RectifyAttribution(this Term term, DateRange boundary, DateTime newStart)
        {
            term.SaftyInvoke<AssignmentBase>(o =>
                                                 {
                                                     var isCreation = term.Start == newStart;
                                                     if(isCreation && o.BelongToPrv.HasValue)
                                                         return;

                                                     var newStartBehindBoundary = newStart >= boundary.End;
                                                     if ((!boundary.IsOutOfRange(term.Start) || isCreation) && newStartBehindBoundary)
                                                         o.BelongToPrv = true;

                                                     if (o.BelongToPrv == true && !boundary.IsOutOfRange(newStart))
                                                         o.BelongToPrv = false;
                                                 });
        }

        public static bool OccupationComparison(this Term begin, Term current, Func<Term, bool> isNeedSeat)
        {
            var currentOccupied = current != null ? isNeedSeat(current) : false;
            var beginOccupied = begin != null ? isNeedSeat(begin) : false;

            var bothEqual = ReferenceEquals(begin, current);

            if (bothEqual)
                return false;

            //if (begin is AbsentEvent || current is AbsentEvent)
            //    return false;

            var bothOccupied = currentOccupied & beginOccupied;
            var bothNotNull = current != null & begin != null;
            var bothHaveSeat = false;
            var bothNotLinealRelative = false; //判断跳层切割
            var occupiedTermHaveSeat = false; //事件占席已配席位
            var bothAreSameUpperLevel = false;
            var isFamily = false;

            if (bothNotNull)
            {
                bothHaveSeat = !begin.SeatIsEmpty() && !current.SeatIsEmpty();

                //前后位置区分
                var firstPart = begin.Start < current.Start ? begin : current;
                var endPart = current.End > begin.End ? current : begin;

                //上下阶层区分
                var theChild = begin.Level > current.Level ? begin : current;
                var theParent = begin.Level < current.Level ? begin : current;

                if ((begin.Level == current.Level) && begin.Level > 0)
                {
                    if (begin.Level > 1 && bothOccupied)//independence term
                        bothAreSameUpperLevel = true;
                    else if (!bothOccupied)
                    {
                        bothAreSameUpperLevel = true;
                    }
                }


                var childAndParentNotEqual = theChild != theParent && theChild.Level > theParent.Level;

                if (childAndParentNotEqual)
                {
                    bothNotLinealRelative = theChild.Bottom != theParent; //确定是否为父子直属关系

                    //必须是直系父子关系, 判断才有意义
                    occupiedTermHaveSeat = !bothNotLinealRelative && theChild.OcuppiedAndHasSeat(isNeedSeat);
                }
                else if (begin.Level == current.Level) // 无任何父子关系, 只是左右邻居
                {
                    //同层边缘相同
                    occupiedTermHaveSeat = firstPart.End == endPart.Start && (firstPart.OcuppiedAndHasSeat(isNeedSeat) || (firstPart.Level == 0 && endPart.Level == 0));
                }

                isFamily = begin.GetLowestTerm().If(o => o.Equals(current.GetLowestTerm())) == true;
            }
            else
                return true;

            return beginOccupied != currentOccupied || (bothNotNull && (!isFamily || bothHaveSeat || bothNotLinealRelative || occupiedTermHaveSeat || bothAreSameUpperLevel));
        }



        public static void X(Term prv, Term term, ref Term result)
        {
            var source = term;
            var prvIsNull = prv == null;

            if (!prvIsNull && prv.GetLowestTerm().If(o => o.Equals(term.GetLowestTerm())) != true)
                prvIsNull = true;

            if (!prvIsNull && term.Level <= 1)
            {
                if (prv.Bottom != null)
                {
                    if (prv.IsNeedSeat == false && prv.Level > 0 || (prv.Level - term.Level == 1))
                    {
                        if (!prv.CanBeSeatSource() && prv.Bottom.If(b => !b.SeatIsEmpty()) == true) // 第三层不能做为下一段位置的依据
                            source = prv.Bottom; // 改变参考的来源
                        else
                            source = prv;
                    }
                    else
                    {
                        prv.DiveInto(o =>
                        {
                            if (!o.IsNeedSeat) //不占席事件所产生的座位表
                            {
                                //if (!o.SeatIsEmpty())
                                source = o;
                                return true;
                            }


                            //var prvIsCutPoint = prv.IsNeedSeat == false && prv.Level > 0;

                            //if (prvIsCutPoint)
                            //{
                            //    source = prv;

                            //}

                            if (!o.SeatIsEmpty())
                            {
                                source = o;
                                return true;
                            }
                            return false;
                        });
                    }
                }
                else
                {
                    if (prv.Level == 0 && term.Level == 0 && prv.AnyOverlap(term) && !prv.IsInTheRange(term.Start, term.End))
                    {
                    }
                    else if (term.Level == 1 && prv.Level == 0 && !term.IsInTheRange(prv.Start, prv.End) && term.Bottom != null && term.Bottom.IsNeedSeat)
                    {
                        source = term.Bottom;
                    }
                    else
                        source = prv;
                }
            }
            else if (prvIsNull && term.Level == 1)
            {
                if (!term.Bottom.SeatIsEmpty())
                    source = term.Bottom;
            }

            //用于判断当assignment为不占席
            source.GetLowestTerm().SaftyInvoke<IAssignment>(a =>
            {
                if (!a.IsNeedSeat && term.OcuppiedAndHasSeat())
                    source = term;
            });

            result = source;
        }
    }
}