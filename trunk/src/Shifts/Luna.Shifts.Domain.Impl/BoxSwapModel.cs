using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Domain.Impl
{
    public class BoxSwapModel : AbstractBoxSwap
    {
        private readonly ITimeBoxRepository _timeBoxRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILaborHoursCountingModel _laborHoursCountingModel;
        public BoxSwapModel(ITimeBoxRepository timeBoxRepository, IAttendanceRepository attendanceRepository,ILaborHoursCountingModel laborHoursCountingModel)
        {
            FailedTerm=new List<Term>();
            _timeBoxRepository = timeBoxRepository;
            _attendanceRepository = attendanceRepository;
            _laborHoursCountingModel = laborHoursCountingModel;
        }
        /// <summary>
        /// 基本代换班
        /// </summary>
        public void InitializeSwapingForTerm(long? assignment)
        {
            if (!assignment.HasValue) return;
            InitializeTerm(assignment);
            CurrentTerms = TimeBox.CollideTerms(Term).ToList(); 
            SubEvents = CurrentTerms.Where(o => o.Id != Term.Id).OrderBy(o => o.Level).ToList();

            HasLockedAssignment = Term.Locked;
            HasLockedSubEvent = CurrentTerms.Any(o => o.Locked);
            HasAbsentEvent = CurrentTerms.SpecificTerm<AbsentEvent>().Any();
        }
        /// <summary>
        /// 事件代换班
        /// </summary>
        public void InitializeSwapingForSubEvent(Pair<DateRange> dateRange, long subevent)
        {
            var tempassignment = TimeBox.CollideTerms(dateRange.Applier).SpecificTerm<AssignmentBase>().OrderBy(o => o.Start).ToList();
            if (tempassignment.Count == 0) return;
            InitializeTerm(subevent);
            CurrentTerms = TimeBox.CenterTerms(new DateRange(tempassignment[0].Start, tempassignment[tempassignment.Count - 1].End)).ToList();
            AbsentEvents = CurrentTerms.SpecificTerm<AbsentEvent>().ToList();
            HasAbsentEvent = AbsentEvents.CollideTerms(dateRange.Applier).Any() ||
                             AbsentEvents.CollideTerms(dateRange.Replier).Any();
            //验证是否重叠
            HasExchanged = CurrentTerms.Any(o => o.Level > 0 && o.Id != subevent && o.IsCoverd(dateRange.Replier));
        }
        /// <summary>
        /// 多日代换班
        /// </summary>
        public void InitializeSwapingForMultiDay()
        {
            var tempassignment = TimeBox.CollideTerms(SwapingDate).SpecificTerm<AssignmentBase>().OrderBy(o => o.Start).ToList();
            var count = tempassignment.Count;
            if (count == 0) return;
            DateRange range;
            if (count == 1)
            {
                range = new DateRange(tempassignment[0].Start, tempassignment[0].End);
            }
            else
            {
                var frist = tempassignment.First();
                var last = tempassignment.Last();
                range = new DateRange
                {
                    Start = frist.Start < SwapingDate.Start ? tempassignment[1].Start : frist.Start,
                    End = last.Start < SwapingDate.End ? last.End : tempassignment[count - 1].End
                };
            }
            var collide = TimeBox.CenterTerms(range).ToList();
            //初始化班
            CurrentTerms = collide.SpecificTerm<AssignmentBase>().ToList();
            //初始化第一层事件
            LevelOnes = collide.Where(o => o.Level == 1).ToList();
            //初始化第二层事件
            LevelTwos = collide.Where(o => o.Level == 2).ToList();
            //初始化第三层事件
            LevelThrees = collide.Where(o => o.Level == 3).ToList();

            //是否存在TimeOff
            HasTimeOff = collide.Any(o => o is TimeOff);
            //是否存在AbsentEvent
            HasAbsentEvent = collide.Any(o => o is AbsentEvent);
            //是否存在锁
            Lockeds = collide.Where(o => o.Locked).ToList();
            //DayOff
            DayOffs = collide.Where(o => o is DayOff).ToList();
        }

        /// <summary>
        /// 时段代换班(scheduleDate用于取TimeBox班表，如果计算工时则需要获取整个排班期班表，如果换班则需要取两人交换范围的班表，dateRange是交换时间段)
        /// </summary>
        public void InitializeSwapingForDateRange(DateRange dateRange)
        {
            var tempassignment = TimeBox.CollideTerms(dateRange).SpecificTerm<AssignmentBase>().OrderBy(o => o.Start).ToList();
            if (tempassignment.Count == 0) return;
            CurrentTerms = TimeBox.CenterTerms(new DateRange(tempassignment[0].Start,tempassignment[tempassignment.Count-1].End)).ToList();
            //初始化班
            TempTerms = CurrentTerms.SpecificTerm<AssignmentBase>().ToList();
            HasAbsentEvent = CurrentTerms.CollideTerms(dateRange).SpecificTerm<AbsentEvent>().Any();
            AbsentEvents = CurrentTerms.SpecificTerm<AbsentEvent>().ToList();
            HasLockedSubEvent = CurrentTerms.Any(o => o.Level > 0 && o.Locked);
            HasTimeOff = CurrentTerms.CollideTerms(dateRange).SpecificTerm<TimeOff>().Any();
            HasShrink = CurrentTerms.CollideTerms(dateRange).SpecificTerm<Shrink>().Any();
            HasLockedAssignment = TempTerms.Any(o => o.Locked);
        }

        /// <summary>
        /// 新验证
        /// </summary>
        public Dictionary<string, bool> NewRangeValidate(Guid agent, DateRange dateRange)
        {
            InitializeTimeBox(agent, dateRange);
            InitializeSwapingForDateRange(dateRange);
            if (CurrentTerms.Count==0)
            {
                HasIdleGap = true;
                return SetValidate();
            }
            var terms = TempTerms.OrderBy(o => o.Start).ToList();
            var count = terms.Count;
            if (terms[0].Start > dateRange.Start || terms[count - 1].End < dateRange.End)
            {
                HasIdleGap = true;
            }
            for (var i = 0; i < count-1 ; i++)
            {
                if (terms[i].End!=terms[i+1].Start)
                {
                    HasIdleGap = true;
                    break;
                }
            }
            return SetValidate();
        }

        protected internal void InitializeAttendance(Guid agent)
        {
            var attendance = _attendanceRepository.Where(o => o.Agent.Id == agent && o.Start <= SwapingDate.Start).OrderByDescending(o => o.Start).FirstOrDefault();
            Agent = _timeBoxRepository.GetAgent(attendance, typeof(Agent));
            ScheduleDate = new DateRange(Agent.LaborRule.Start, Agent.LaborRule.End);
        }

        private IEnumerable<Term> GetTerm()
        {
            //工时：1/1-1/31工时 取1/1-2/1 term
            //检查1/1日term是否有BelongToPrv=true，有则将其工时排除在1/31内
            //检查2/1日term是否有BelongToPrv=false，有则将其工时排除在1/31内
            var terms = TimeBox.CollideTerms(new DateRange(ScheduleDate.Start,ScheduleDate.End.AddDays(1))).OrderBy(o => o.Start).ToList();
            terms.ReMove(o => o.End.Date == ScheduleDate.Start.Date, true);
            terms.ReMove(o => o.Start.Date == ScheduleDate.End.Date, false);
            return terms;
        }

        /// <summary>
        /// 计算原始工时
        /// </summary>
        protected internal void GetQLaborRule()
        {
            MonthHourTotals = LaborRuleHours(GetTerm().OfType<AssignmentBase>());
        }

        /// <summary>
        /// 计算工时
        /// </summary>
        protected internal void GetLaborRule()
        {
            var terms = GetTerm();

            var exceptions = _laborHoursCountingModel.SummarizeLaborRule(Agent, terms);
            SetLaborRule(exceptions);
            MonthTotals = LaborRuleHours(terms.OfType<AssignmentBase>());

            var start = SwapingDate.Start.Date;
            var end = SwapingDate.End.Date;
            var dayrange = (start != end && SwapingDate.Start != start && SwapingDate.End != end) ? new DateRange(start, start.AddDays(2)) : new DateRange(start, start.AddDays(1));
            
            var currentdayterms = terms.CollideTerms(dayrange);
            List<Term> inside, outside;
            Term.SliceTerm(currentdayterms, dayrange.Start, dayrange.End, out outside, out inside);

            DayTotals = LaborRuleHours(inside.OfType<AssignmentBase>());
            
            MonthSwapHour = MonthTotals - MonthHourTotals;
        }

        public void InitializeTimeBox(Guid agent, DateRange dateRange)
        {
            TimeBox = _timeBoxRepository.GetTermsWithSiblings(agent, dateRange.Start, dateRange.End);
            TimeBox.Boundary = ScheduleDate;
        }
        public void InitializeTimeBox(Guid agent)
        {
            InitializeTimeBox(agent, ScheduleDate);
        }

        public void Submit()
        {
            _timeBoxRepository.MakePersistent(TimeBox);
        }

        public void SaveLog(Term term)
        {
            if (term != null)
            {
                _timeBoxRepository.SaveLog(term, TimeBox);
            }
        }
    }
}
