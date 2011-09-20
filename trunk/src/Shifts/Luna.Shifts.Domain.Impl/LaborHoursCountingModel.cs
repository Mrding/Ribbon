using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Domain.Impl
{
  
    public class LaborHoursCountingModel : ILaborHoursCountingModel
    {
        
        public IList<Exception> SummarizeLaborRule(IAgent agent)
        {
            return SummarizeLaborRule(agent, agent.Schedule.TermSet);
        }

        public IList<Exception> SummarizeLaborRule(IAgent agent, IEnumerable<Term> terms)
        {
            var exceptions = new List<Exception>();
            SumLaborRuleHours(agent, terms);

            return exceptions; // express

            var assignments = terms.OfType<AssignmentBase>();

            GetAssignmentGaps(assignments, agent.LaborRule.MinIdleGap, a =>
            {
                if (a.Duration >= TimeSpan.FromDays(1) && a.Start < agent.Schedule.Boundary.End && a.End > agent.Schedule.Boundary.Start)
                {
                    exceptions.Add(new ShiftGapException(agent.Profile.AgentId, a.Start, a.End, a.Duration, LaborRuleCategory.Gap));
                }
            });

            GetContinueWorkDay(assignments, (a, b) =>
            {
                var timeSpan = b.Subtract(a);
                //計算出這些空檔之間的間隔小時數 
                var hours = timeSpan.TotalHours;
                //若這些間隔有任何>=24*n則告警之
                if (hours >= agent.LaborRule.MCWD * 24 &&
                    a < agent.Schedule.Boundary.End && b > agent.Schedule.Boundary.Start)
                    exceptions.Add(new OvertimeWorkingException(agent.Profile.AgentId, a, b, timeSpan, LaborRuleCategory.MCWD));
            });

            GetContinueDayOff(agent.Schedule.Boundary, assignments, (a, b) =>
            {
                var dayOffDays = a.GetDaysCount(b);
                if (dayOffDays > agent.LaborRule.MCDO &&
                    a < agent.Schedule.Boundary.End && b > agent.Schedule.Boundary.Start)
                    exceptions.Add(new DayOffException(agent.Profile.AgentId, a, b, dayOffDays, LaborRuleCategory.MCDO));
            });

            agent.LaborRule.Exceptions = exceptions;

            return exceptions;
        }

        private static void GetContinueWorkDay(IEnumerable<AssignmentBase> assignments, Action<DateTime, DateTime> workDelegate)
        {
            AssignmentBase previousAssignment = null;
            AssignmentBase headAssignment = null;

            foreach (var item in assignments)
            {
                //在排班期內找出所有>=24小時的空檔. (排班期起訖要外擴x日) 
                if (previousAssignment != null && previousAssignment.End.AddDays(1) <= item.Start
                          && previousAssignment.Start.Date != item.Start.Date)
                {
                    workDelegate(headAssignment.Start, previousAssignment.End);
                    headAssignment = item;
                }
                if (headAssignment == null)
                    headAssignment = item;

                previousAssignment = item;

            }
            //判断边界
            if (previousAssignment != null && headAssignment != previousAssignment)
                workDelegate(headAssignment.Start, previousAssignment.End);

        }

        private static void GetContinueDayOff(DateRange boundary, IEnumerable<AssignmentBase> assignments, Action<DateTime, DateTime> gapDelegate)
        {
            var firstAssignment = assignments.FirstOrDefault();
            var endAssignment = assignments.LastOrDefault();
            //for Assignment < Schedule.Start || Assignment >Schedule.End
            var start = firstAssignment == null || firstAssignment.Start >= boundary.Start ? boundary.Start : firstAssignment.Start;
            var end = endAssignment == null || endAssignment.Start <= boundary.End ? boundary.End : endAssignment.Start;

            if (firstAssignment == null)
            {
                gapDelegate(start, end.AddDays(-1));
                return;
            }

            AssignmentBase lastAssignment = null;

            foreach (var item in assignments)
            {
                if (item.AsAWork)
                {
                    if (lastAssignment != null && lastAssignment.Start.Date != item.Start.Date.AddDays(-1)
                        && lastAssignment.Start.Date != item.Start.Date)
                    {
                        gapDelegate(lastAssignment.Start.Date.AddDays(1), item.Start.Date.AddDays(-1));
                    }

                    if (lastAssignment == null)
                    {
                        if (item.Start.Date == start.Date)
                        {
                            lastAssignment = item;
                            continue;
                        }

                        gapDelegate(start.Date, item.Start.Date.AddDays(-1));
                    }

                    lastAssignment = item;
                }
            }


            if (lastAssignment != null && lastAssignment.Start.Date < end.Date.AddDays(-1))
            {
                gapDelegate(lastAssignment.Start.Date.AddDays(1), end.Date.AddDays(-1));
            }
        }

        private static void GetAssignmentGaps(IEnumerable<AssignmentBase> assignments, TimeSpan minIdleGap, Action<DateRange> gapDelegate)
        {
            var dateRanges = new List<DateRange>();
            AssignmentBase previousAssignment = null;
            AssignmentBase headAssignment = null;
            foreach (var item in assignments.OrderBy(o => o.Start))
            {
                //在排班期內找出所有<最小班距的空檔---IllegalGap
                if (previousAssignment != null && item.From.Subtract(previousAssignment.Finish) < minIdleGap)
                {
                    dateRanges.Add(new DateRange(previousAssignment.From, item.Finish));
                    headAssignment = item;
                }
                if (headAssignment == null)
                    headAssignment = item;
                previousAssignment = item;
            }
            if (dateRanges.Count != 0)
            {
                var start = dateRanges[0].Start;
                var end = dateRanges[0].End;
                for (var i = 0; i < dateRanges.Count; i++)
                {
                    if (i == dateRanges.Count - 1)
                    {
                        end = dateRanges[i].End;
                        gapDelegate(new DateRange(start, end));
                    }
                    else
                    {
                        if (end > dateRanges[i + 1].Start)
                        {
                            end = dateRanges[i + 1].End;
                        }
                        else
                        {
                            gapDelegate(new DateRange(start, end));
                            start = dateRanges[i + 1].Start;
                            end = dateRanges[i + 1].End;
                        }
                    }
                }
            }
        }

        public void SumLaborRuleHours(IAgent agent, IEnumerable<Term> terms)
        {
            TimeSpan overtimeTotals = TimeSpan.Zero, laborHourTotals = TimeSpan.Zero, shrinkageTotals = TimeSpan.Zero;

            var validRange = new DateRange(agent.LaborRule.Start, agent.LaborRule.End);

            //只针对 IAssignment, 和 IIndependenceTerm(DayOff,Timeoff) 进行统计
            var assignments = terms.Where(o =>
            {
                return o.SaftyGetProperty<bool, IAssignment>(
                        assignment =>
                        assignment.SaftyGetHrDate().IsInTheRange(validRange), () => o.Is<IIndependenceTerm>());
            }).ToArray();

            foreach (var term in assignments)
            {
                var assignment = term as AssignmentBase;
                if (assignment != null)
                {
                    overtimeTotals += assignment.OvertimeTotals;
                    laborHourTotals += assignment.WorkingTotals;
                    shrinkageTotals += assignment.ShrinkageTotals;
                }
                if (term is TimeOff)
                    laborHourTotals += agent.LaborRule.StdDailyLaborHour;
            }

            ReportingOffWorkDays(agent.LaborRule.Start, agent.LaborRule.End, assignments, (fw, pw) =>
           {
               agent.LaborRule.FullWeekendTotals = fw;
               agent.LaborRule.PartialWeekendTotals = pw;
           });


            agent.LaborRule.LaborHourTotals = laborHourTotals;
            agent.LaborRule.OvertimeTotals = overtimeTotals;
            agent.LaborRule.ShrinkedTotals = shrinkageTotals;
        }

        private static void ReportingOffWorkDays(DateTime scheduleStart, DateTime scheduleEnd, IEnumerable<Term> assignments, Action<int, int> gapDelegate)
        {
            int pw = 0, fw = 0;
            int continueCount = 0;
            var start = scheduleStart.AddDays(-1);
            var end = scheduleEnd.AddDays(-1);

            while (start <= end)
            {
                if (end.IsHoliday(Country.Local) && !HasAssignment(assignments, end.Date))
                {
                    continueCount++;
                }
                else
                {
                    fw += continueCount / 2;
                    pw += continueCount % 2;
                    continueCount = 0;
                }
                end = end.AddDays(-1);
            }

            gapDelegate(fw, pw);
        }

        private static bool HasAssignment(IEnumerable<Term> assignments, DateTime date)
        {
            //xreturn assignments.Any(a => a.StartIsCoverd(date, date.AddDays(1)));
            //var range = new TimeRange(date, date.AddDays(1));
            return assignments.Any(a => a.SaftyGetProperty<bool,IAssignment>(o=> o.SaftyGetHrDate() == date));
        }
    }
}
