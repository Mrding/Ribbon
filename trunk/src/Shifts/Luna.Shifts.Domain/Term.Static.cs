using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain
{
    public abstract partial class Term
    {
        public static void OccupiedTermHaveSeat(Term begin, Term current, ref Term target)
        {
            var theChild = begin.Level > current.Level ? begin : current;
            var theParent = begin.Level < current.Level ? begin : current;

            var childAndParentNotEqual = theChild != theParent && theChild.Level > theParent.Level;
            var bothAreLinealRelative = childAndParentNotEqual && theChild.ParentTerm == theParent; //确定是否为父子直属关系


            var occupiedTermHaveSeat = bothAreLinealRelative
                                       && theChild.IsNeedSeat && !theChild.SeatIsEmpty();
            if (occupiedTermHaveSeat)
            {
                target = theParent;
            }
        }

        public static T NewExample<T>() where T : ITerm
        {
            var ci = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
             CallingConventions.HasThis, new Type[] { }, null);

            var obj = (T)ci.Invoke(new object[] { });
            return obj;
        }

        public static T New<T>(DateTime uncheckedStart, TermStyle termStyle) where T : Term
        {
            var start = uncheckedStart.TurnToMultiplesOf5();

            DateTime startTime;
            if (termStyle is AssignmentType)
                startTime = start.Date.AddMinutes(termStyle.TimeRange.StartValue);
            else
                startTime = start;
            var length = TimeSpan.FromMinutes(termStyle.TimeRange.Length);

            var ci = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
             CallingConventions.HasThis, new[] { typeof(DateTime), typeof(TimeSpan) }, null);

            var obj = (T)ci.Invoke(new object[] { startTime, length });

            obj.MAssign(new { termStyle.Background, Text = termStyle.Name, termStyle.OnService, termStyle.AsARest, TermStyleId = termStyle.Id });
            obj.IsNeedSeat = termStyle.Occupied;
            
            return obj;
        }

        public static Term New(DateTime start, TermStyle termStyle)
        {
            return New(start, termStyle, termStyle.TimeRange.Length);
        }

        public static IList<Term> NewAssignmentWithEvents<T>(DateTime startDate, TermStyle termStyle
            , IEnumerable<SubEventInsertRule> rules)
            where T : Term
        {

            var newTerm = New<T>(startDate, termStyle);
            var terms = new List<Term>(rules.Count()+1) { newTerm };
            foreach (var rule in rules)
            {
                DateTime startTime;
                // Whole SubEvent should be inside the range of StartValue~EndValue
                var amountOfAvailableOccurPoints = rule.GetAmountOfAvailableOccurPoints();
                if (amountOfAvailableOccurPoints == 1)
                { // SubEvent can only occur at the begin of range
                    startTime = newTerm.Start.AddMinutes(rule.TimeRange.StartValue);
                }
                else
                {
                    var randomOffset = TermExt.ArrangeSubEventRandom.Next(0, amountOfAvailableOccurPoints);
                    startTime = newTerm.Start.AddMinutes(rule.TimeRange.StartValue + (randomOffset * rule.OccurScale));
                }

                var newSubEvent = New(startTime, rule.SubEvent, rule.SubEventLength);

                if (newSubEvent.IsInTheRange(newTerm.Start, newTerm.End))
                {
                  terms.Add(newSubEvent);
                }
            }
            return terms;
        }

        public static Term NewAssignment(DateTime unCheckedstart, DateTime uncheckedEnd, TermStyle termStyle)
        {
            var start = unCheckedstart.TurnToMultiplesOf5();
            var end = uncheckedEnd.TurnToMultiplesOf5();

            if (termStyle.Type.FullName != null)
                if (termStyle as BasicAssignmentType == null && termStyle.Type.FullName.Contains("Event"))
                    throw new ArgumentNullException("termStyle");

            if (end < start)
                end = end.AddDays(1);

            var length = end.Subtract(start);

            var ci = termStyle.Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
             CallingConventions.HasThis, new[] { typeof(DateTime), typeof(TimeSpan) }, null);

            var obj = ci.Invoke(new object[] { start, length });

            var type = termStyle.As<BasicAssignmentType>();
            obj.MAssign(new
            {
                type.Background,
                Text = type.Name,
                type.OnService,
                IsNeedSeat = type.Occupied,
                type.AsAWork,
                type.AsARest,
                type.IgnoreAdherence,
                type.GapGuaranteed,
                TermStyleId = type.Id
            });

            return obj as Term;
        }


        /// <summary>
        /// New Term 
        /// </summary>
        /// <param name="startDate">Can't be 00:00</param>
        /// <param name="termStyle"></param>
        /// <param name="lenghtOfMinutes"></param>
        /// <returns></returns>
        public static Term New(DateTime startDate, TermStyle termStyle, int lenghtOfMinutes)
        {
            if (termStyle == null)
                throw new ArgumentNullException("termStyle");

            var startTime = startDate;
            if (termStyle is AssignmentType)
            {
                //用途不明却,需注意旧版使用情境

                var startValue = startDate.Hour * 60 + startDate.Minute;

                if (startValue == termStyle.TimeRange.StartValue)
                    startTime = startDate.Date.AddMinutes(termStyle.TimeRange.StartValue);
            }

            var length = TimeSpan.FromMinutes(lenghtOfMinutes);

            var ci = termStyle.Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
             CallingConventions.HasThis, new[] { typeof(DateTime), typeof(TimeSpan) }, null);

            var obj = ci.Invoke(new object[] { startTime, length });

            obj.MAssign(new { termStyle.Background, Text = termStyle.Name, termStyle.OnService, termStyle.AsARest, TermStyleId = termStyle.Id });
            
            obj.SaftyInvoke<Term>(t=> t.IsNeedSeat = termStyle.Occupied);
            obj.SaftyInvoke<IAssignment>(t => { t.HrDate = startDate.Date; });
            
            if (termStyle is AssignmentType)
            {
                var type = termStyle.As<AssignmentType>();
                obj.MAssign(new { type.AsAWork, type.IgnoreAdherence, type.GapGuaranteed });
            }


            return obj as Term;
        }

        /// <summary>
        /// For rescue TermStyle type incorrect perpouse
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="classType"></param>
        /// <param name="termStyle"></param>
        /// <param name="lenghtOfMinutes"></param>
        /// <returns></returns>
        public static Term New(DateTime startDate, Type classType, TermStyle termStyle, int lenghtOfMinutes)
        {
            if (termStyle == null || classType == null)
                throw new ArgumentNullException("termStyle");

            var startTime = startDate;
            if (termStyle is AssignmentType)
            {
                var startValue = startDate.Hour * 60 + startDate.Minute;

                if (startValue == termStyle.TimeRange.StartValue)
                    startTime = startDate.Date.AddMinutes(termStyle.TimeRange.StartValue);
            }

            var length = TimeSpan.FromMinutes(lenghtOfMinutes);

            var ci = classType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
             CallingConventions.HasThis, new[] { typeof(DateTime), typeof(TimeSpan) }, null);

            var obj = ci.Invoke(new object[] { startTime, length });

            obj.MAssign(new { termStyle.Background, Text = termStyle.Name, termStyle.OnService, termStyle.AsARest, TermStyleId = termStyle.Id });
            obj.SaftyInvoke<Term>(t => t.IsNeedSeat = termStyle.Occupied);

            if (termStyle is AssignmentType)
            {
                var type = termStyle.As<AssignmentType>();
                obj.MAssign(new { type.AsAWork, type.IgnoreAdherence, type.GapGuaranteed });
            }

            return obj as Term;
        }

        public static Term CopyAsNew(DateTime uncheckedStartTime, Type classType, Term styleSource, int lenghtOfMinutes)
        {
            var startTime = uncheckedStartTime.TurnToMultiplesOf5();

            if (styleSource == null || classType == null)
                throw new ArgumentNullException("termStyle");

            var length = TimeSpan.FromMinutes(lenghtOfMinutes);

            var ci = classType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
             CallingConventions.HasThis, new[] { typeof(DateTime), typeof(TimeSpan) }, null);

            var obj = ci.Invoke(new object[] { startTime, length });

            obj.MAssign(new { styleSource.Background, styleSource.Text, styleSource.OnService, styleSource.IsNeedSeat, styleSource.AsARest, styleSource.TermStyleId });

            if (styleSource is IAssignment)
            {
                var type = styleSource.As<IAssignment>();
                obj.MAssign(new { type.AsAWork, type.IgnoreAdherence, type.GapGuaranteed, type.BelongToPrv});
            }


            return obj as Term;
        }

        public static bool SliceTerm(IEnumerable<Term> orginalTerms, DateTime start, DateTime end, out List<Term> outsideTerms, out List<Term> insideTerms)
        {
            List<Term> tempTerms;
            List<Term> tempTerms2;
            bool result = true;
            if (!SliceTerm(orginalTerms, start, out outsideTerms, out tempTerms))
                result = false;
            if (!SliceTerm(tempTerms, end, out insideTerms, out tempTerms2))
                result = false;
            outsideTerms.AddRange(tempTerms2);
            if (!result)
            {
                outsideTerms.Clear();
                insideTerms.Clear();
            }
            return result;
        }

        public static bool SliceTerm(IEnumerable<Term> orginalTerms, DateTime start, DateTime end, out List<Term> insideTerms, out List<Term> frontTerms, out List<Term> backTerms)
        {
            List<Term> tempTerms;
            var result = true;
            if (!SliceTerm(orginalTerms, start, out frontTerms, out tempTerms))
                result = false;
            if (!SliceTerm(tempTerms, end, out insideTerms, out backTerms))
                result = false;
            if (!result)
            {
                frontTerms.Clear();
                backTerms.Clear();
                insideTerms.Clear();
            }
            return result;
        }

        public static void BalanceLabourHour(Term source, IEnumerable<Term> terms)
        {
            var term = source as IAssignment;
            var coveredTerms = terms.CollideTerms(source.Start, source.End).Where(t => !(t is AbsentEvent) && t.Level > 0).OrderBy(o => o.Level);

            term.OvertimeTotals = term.Payment == WorkHourType.ExtraPaid ? term.End.Subtract(term.Start) : new TimeSpan();
            term.ShrinkageTotals = new TimeSpan();
            term.WorkingTotals = term.Payment == WorkHourType.Paid ? term.End.Subtract(term.Start) : new TimeSpan();

            foreach (var item in coveredTerms)
            {
                var timeSpan = item.End.Subtract(item.Start);
                if (item.Level == 1)
                {
                    if (term.Payment == WorkHourType.Paid)
                    {
                        if (item.Payment == WorkHourType.ExtraPaid)
                        {
                            term.OvertimeTotals += timeSpan;
                            term.WorkingTotals -= timeSpan;
                        }
                        else if (item.Payment == WorkHourType.Unpaid)
                            term.WorkingTotals -= timeSpan;
                        else if (item.Payment == WorkHourType.Shrink)
                            term.ShrinkageTotals += timeSpan;
                    }
                    else if (term.Payment == WorkHourType.ExtraPaid)
                    {
                        if (item.Payment == WorkHourType.Unpaid)
                            term.OvertimeTotals -= timeSpan;
                    }
                }
                else if (item.IsNot<IImmutableTerm>())//level2
                {
                    if (term.Payment == WorkHourType.Paid)
                    {
                        if (item.Payment == WorkHourType.ExtraPaid)
                        {
                            term.OvertimeTotals += timeSpan;
                            //term.WorkingTotals -= timeSpan;
                        }
                        else
                            term.WorkingTotals += timeSpan;
                    }
                    else //assignmentTerm.Payment == WorkHourTypes.ExtraPaid
                    {
                        term.OvertimeTotals += timeSpan;
                    }
                }
            }
        }

        protected internal static Term New(Term term, DateTime start, DateTime end, IEnumerable<Term> terms)
        {
            var newTerm = CopyAsNew(start, term.GetType(), term, (int)(end - start).TotalMinutes);
            newTerm.Seat = term.Seat;
            if (newTerm is IAssignment)
            {
                BalanceLabourHour(newTerm, terms);
                //从原始班表中获取最新的带Seat的事件，主要避免一个班表上面是两个座位表，当换后段的时候需要考虑
                var seatReferenceTerm = terms.CollideTerms(start,end).Where(o => !o.SeatIsEmpty() && o.Level > 0)
                    .Select(o => new {Source = o, Distance = o.End.Subtract(start)})
                    .Where(o => o.Distance.TotalMinutes < 0)
                    .OrderByDescending(o => o.Distance)
                    .FirstOrDefault();
                if (seatReferenceTerm != null)
                {
                    newTerm.Seat = seatReferenceTerm.Source.Seat;
                }
            }
            return newTerm;
        }

        public static bool SliceTerm(IEnumerable<Term> orginalTerms, DateTime cutTime, out List<Term> frontTerms, out List<Term> backTerms)
        {
            frontTerms = new List<Term>();
            backTerms = new List<Term>();
            foreach(var te in orginalTerms)
            {
                if (te.Start >= cutTime)
                {
                    backTerms.Add(New(te,te.Start,te.End,orginalTerms));
                    continue;
                }
                if (cutTime >= te.End)
                {
                    frontTerms.Add(New(te, te.Start, te.End, orginalTerms));
                    continue;
                }
                if(te is AbsentEvent)
                {
                    frontTerms.Clear();
                    backTerms.Clear();
                    return false;
                }
                frontTerms.Add(New(te, te.Start, cutTime, orginalTerms));
                backTerms.Add(New(te, cutTime, te.End, orginalTerms));
            }
            return true;
        }
    }
}
