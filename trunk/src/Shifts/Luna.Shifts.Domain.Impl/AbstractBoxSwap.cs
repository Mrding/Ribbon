using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.Globalization;

namespace Luna.Shifts.Domain.Impl
{
    public abstract class AbstractBoxSwap
    {
        protected internal void ReSet()
        {
            //ScheduleDate = new DateRange();
            //SwapingDate = new DateRange();
            TimeBox = null;
            Agent = null;
            Message = new StringBuilder(50);
            Term = null;
            TimeOff = null;
            SubEvents = new List<Term>();
            Lockeds = new List<Term>();
            DayOffs = new List<Term>();
            LevelOnes = new List<Term>();
            LevelTwos = new List<Term>();
            LevelThrees = new List<Term>();

            CurrentTerms=new List<Term>();
            TempTerms=new List<Term>();
            AbsentEvents=new List<Term>();

            IsMcdoNotMatched = false;
            IsMcwdNotMatched = false;
            IsMinIdleGapNotMatched = false;
            MonthHourTotals = TimeSpan.Zero;
            //日工時上限
            DayTotals = TimeSpan.Zero;
            //月總工時上限
            MonthTotals = TimeSpan.Zero;
            //月接班時數上限
            MonthSwapHour = TimeSpan.Zero;

            FailedTerm = new List<Term>();

            HasExchanged = false;
            HasLockedAssignment = false;
            HasLockedSubEvent = false;
            HasTimeOff = false;
            HasAbsentEvent = false;
            HasOvertimeAssignment = false;
            HasUnlaboredSubEvent = false;
            HasShrink = false;
            HasIdleGap = false;
            IsNeedSeat = false;
        }
        public DateRange ScheduleDate { get; set; }
        public DateRange SwapingDate { get; set; }

        public TimeBox TimeBox { get; set; }
        public IAgent Agent { get; set; }
        /// <summary>
        /// 提供错误信息
        /// </summary>
        public StringBuilder Message { get; set; }
        /// <summary>
        /// 当前班表
        /// </summary>
        public Term Term { get; set; }
        public Term TimeOff { get; set; }
        /// <summary>
        /// 事件
        /// </summary>
        public List<Term> SubEvents { get; set; }
        /// <summary>
        /// 时段代换班：锁定的班表
        /// </summary>
        public List<Term> Lockeds { get; set; }
        public List<Term> DayOffs { get; set; }
        public List<Term> LevelOnes { get; set; }
        public List<Term> LevelTwos { get; set; }
        public List<Term> LevelThrees { get; set; }

        public List<Term> CurrentTerms { get; set; }
        public List<Term> TempTerms { get; set; }
        public List<Term> AbsentEvents { get; set; }

        /// <summary>
        /// 最大连续休假日数
        /// </summary>
        public bool IsMcdoNotMatched { get; set; }
        /// <summary>
        /// 最大连续上班天数
        /// </summary>
        public bool IsMcwdNotMatched { get; set; }
        /// <summary>
        /// 最小班距
        /// </summary>
        public bool IsMinIdleGapNotMatched { get; set; }
        public TimeSpan MonthHourTotals { get; set; }
        /// <summary>
        /// 日工時上限
        /// </summary>
        public TimeSpan DayTotals { get; set; }
        /// <summary>
        /// 月總工時上限
        /// </summary>
        public TimeSpan MonthTotals { get; set; }
        /// <summary>
        /// 月接班時數上限
        /// </summary>
        public TimeSpan MonthSwapHour { get; set; }

        public IList<Term> FailedTerm { get; set; }

        //-------------------------------------------------------
        //用于验证
        /// <summary>
        /// 是否重复
        /// </summary>
        public bool HasExchanged { get; set; }
        /// <summary>
        /// 班是否有锁
        /// </summary>
        public bool HasLockedAssignment { get; set; }
        /// <summary>
        /// 班上的事件是否有锁
        /// </summary>
        public bool HasLockedSubEvent { get; set; }
        /// <summary>
        /// 是否有TimeOff
        /// </summary>
        public bool HasTimeOff { get; set; }
        /// <summary>
        /// 是否有AbsentEvent
        /// </summary>
        public bool HasAbsentEvent { get; set; }
        /// <summary>
        /// 是否有OvertimeAssignment
        /// </summary>
        public bool HasOvertimeAssignment { get; set; }
        /// <summary>
        /// 是否有UnlaboredSubEvent
        /// </summary>
        public bool HasUnlaboredSubEvent { get; set; }
        /// <summary>
        /// 是否有Shrink
        /// </summary>
        public bool HasShrink { get; set; }
        /// <summary>
        /// 是否有空档
        /// </summary>
        public bool HasIdleGap { get; set; }
        /// <summary>
        /// 是否有空档
        /// </summary>
        public bool IsNeedSeat { get; set; }
        //-----------------------------------------------------------
        protected void InitializeTerm(long? termid)
        {
            Term = TimeBox.TermSet.SingleOrDefault(o => termid.HasValue ? o.Id == termid.Value : false);
        }

        protected internal Dictionary<string, bool> SetValidate()
        {
            return new Dictionary<string, bool>
                       {
                           {"Exchanged", HasExchanged},
                           {"Assignment", HasLockedAssignment},
                           {"SubEvent", HasLockedSubEvent},
                           {"TimeOff", HasTimeOff},
                           {"AbsentEvent", HasAbsentEvent},
                           {"OvertimeAssignment", HasOvertimeAssignment},
                           {"UnlaboredSubEvent", HasUnlaboredSubEvent},
                           {"Shrink", HasShrink},
                           {"IdleGap",HasIdleGap},
                           {"IsNeedSeat",IsNeedSeat}
                       };
        }

        public bool NotSubEventAndGap(DateRange dateRange)
        {
            List<Term> inside, outside;
            Term.SliceTerm(CurrentTerms, dateRange.Start, dateRange.End, out outside, out inside);
            //Gap
            if (inside.Count == 0)
            {
                return false;
            }
            //true是代表俯视有不是Un或者Gap的，有其它班类型
            var gaporun = inside.ConvertToCell(dateRange, 5, t => t.IsNot<UnlaboredSubEvent>()).Contains(true);
            return gaporun;
        }



        public void DealWithDateRange(BoxSwapModel opp, DateRange dateRange)
        {
            List<Term> inside, outside;
            Term.SliceTerm(CurrentTerms, dateRange.Start, dateRange.End, out outside, out inside);
            var day = TempTerms.HasDayTerms();

            //不符合的不创建了。
            var gaporun = inside.ConvertToCell(dateRange, 5, t => t.IsNot<UnlaboredSubEvent>()).Contains(true);
            if (gaporun)
            {
                DealWithTerms(inside, day);
                inside.ForEach(opp.TimeBoxCreateTerm);
            }
            //自己创建之外的班
            DealWithTerms(outside, day);
            outside.ForEach(TimeBoxCreateTerm);
        }

        public void DealWithTerms(List<Term> terms,bool isDay)
        {
            if (!isDay)
                return;
            foreach (var term in terms)
            {
                if (term is Assignment)
                {
                    var a = TempTerms.First(o => o.Start <= term.Start && o.End >= term.End);
                    if (term.InDate(a))
                        (term as Assignment).BelongToPrv = true;
                }
            }
        }

        public void DealWithDateRange(BoxSwapModel opp, Pair<DateRange> dateRange)
        {
            List<Term> frontTerms, insideTerms, backTerms;
            Term.SliceTerm(CurrentTerms, dateRange.Applier.Start, dateRange.Applier.End, out insideTerms, out frontTerms,
                           out backTerms);
            var day = TempTerms.HasDayTerms();
            DealWithTerms(insideTerms, day);
            insideTerms.ForEach(opp.TimeBoxCreateTerm);
            //获取切割时间
            var cutDateRange = DateRange.Cut(dateRange.Applier, dateRange.Replier);
            var replierRegion = cutDateRange.Where(t => t.Item2 == RegionType.Replier).Select(o => o.Item1);
            List<Term> outside = null;
            List<Term> outside2 = null;
            if (replierRegion.Count() == 1)
            {
                var range = replierRegion.FirstOrDefault();
                List<Term> inside;
                //右边
                if (range.Start >= dateRange.Applier.End)
                {
                    Term.SliceTerm(backTerms, range.Start, range.End, out outside, out inside);
                    DealWithTerms(frontTerms, day);
                    frontTerms.ForEach(TimeBoxCreateTerm);
                }
                //左边
                if (range.End <= dateRange.Applier.Start)
                {
                    Term.SliceTerm(frontTerms, range.Start, range.End, out outside, out inside);
                    DealWithTerms(backTerms, day);
                    backTerms.ForEach(TimeBoxCreateTerm);
                }
            }
            if (replierRegion.Count() > 1)
            {
                foreach (var range in replierRegion)
                {
                    List<Term> inside;
                    //右边
                    if (range.Start >= dateRange.Applier.End)
                    {
                        Term.SliceTerm(backTerms, range.Start, range.End, out outside, out inside);
                    }
                    //左边
                    if (range.End <= dateRange.Applier.Start)
                    {
                        Term.SliceTerm(frontTerms, range.Start, range.End, out outside2, out inside);
                    }
                }
            }
            if (outside2 != null)
            {
                DealWithTerms(outside2, day);
                outside2.ForEach(TimeBoxCreateTerm);
            }
            if (outside != null)
            {
                DealWithTerms(outside, day);
                outside.ForEach(TimeBoxCreateTerm);
            }
            else
            {
                DealWithTerms(frontTerms, day);
                DealWithTerms(backTerms, day);
                frontTerms.ForEach(TimeBoxCreateTerm);
                backTerms.ForEach(TimeBoxCreateTerm);
            }
        }

        protected internal void SetLaborRule(IList<Exception> exceptions)
        {
            foreach (var exception in exceptions)
            {
                //最小班距
                if (exception is ShiftGapException)
                {
                    var assignmentGaps = exception as LaborRuleException;
                    if ((assignmentGaps.Start <= SwapingDate.Start && assignmentGaps.End >= SwapingDate.Start) ||
                        (assignmentGaps.Start <= SwapingDate.End && assignmentGaps.End >= SwapingDate.End))
                    {
                        IsMinIdleGapNotMatched = true;
                    }
                }
                //连续上班天数
                if (exception is OvertimeWorkingException)
                {
                    var continueWorkDay = exception as LaborRuleException;
                    if (continueWorkDay.Start.Date <= SwapingDate.Start.Date && continueWorkDay.End.Date >= SwapingDate.Start.Date)
                    {
                        IsMcwdNotMatched = true;
                    }
                }
                //连续休假日数
                if (exception is DayOffException)
                {
                    var continueDayOff = exception as LaborRuleException;
                    if (continueDayOff.Start.Date <= SwapingDate.Start.Date && continueDayOff.End.Date >= SwapingDate.Start.Date)
                    {
                        IsMcdoNotMatched = true;
                    }
                }
            }
        }

        protected TimeSpan LaborRuleHours(IEnumerable<AssignmentBase> terms)
        {
            TimeSpan overtimeTotals = TimeSpan.Zero, laborHourTotals = TimeSpan.Zero, shrinkageTotals = TimeSpan.Zero;
            foreach (var term in terms)
            {
                overtimeTotals += term.OvertimeTotals;
                laborHourTotals += term.WorkingTotals;
                shrinkageTotals += term.ShrinkageTotals;
            }
            return laborHourTotals + overtimeTotals - shrinkageTotals;
        }

        protected internal void SetTimeOff(Term timeOff, Term term)
        {
            if (TimeOff != null)
            {
                TimeBox.SetTime(timeOff, term.Start.Date, term.Start.AddDays(1).Date, (terms, success) =>
                {
                    if (!success)
                    {
                        Message.AppendLine(LanguageReader.GetValue("Shifts_BoxPairSwap_SetTimeOff"));
                        return;
                    }
                }, true);
            }
        }

        protected internal void CreateTerm(Term term)
        {
            //基本代换班,多日代换班
            TimeBoxCreateTerm(CreateNewTerm(term, term.Start, term.End));
        }

        protected internal void CreateTermWithSeat(Term term, string seat)
        {
            var newterm = CreateNewTerm(term, term.Start, term.End);
            newterm.Seat = seat;
            TimeBoxCreateTerm(newterm);
        }

        private static Term CreateNewTerm(Term term, DateTime start, DateTime end)
        {
            var newTerm = Term.CopyAsNew(start, term.GetType(), term, (int)(end - start).TotalMinutes);
            newTerm.Seat = term.Seat;
            newTerm.Tag = term.Tag;
            return newTerm;
        }

        protected internal void TimeBoxCreateTerm(Term term)
        {
            TimeBox.Create(term, (terms, success) =>
            {
                if (!success)
                {
                    FailedTerm.Add(term);
                    return;
                }
            }, true);
        }

        protected internal void DeleteTerm(Term term)
        {
            if (term != null)
            {
                TimeBox.Delete(term, false);
            }
        }

        public override string ToString()
        {
            return "最大连续休假日数：" + IsMcdoNotMatched + "，最大连续上班天数：" + IsMcwdNotMatched + "，最小班距：" +IsMinIdleGapNotMatched +
                   "。日工時上限:" + DayTotals + ",月總工時上限:" + MonthTotals + ",月接班時數上限:" + MonthSwapHour;
        }
    }
}
