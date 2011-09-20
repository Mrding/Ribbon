using System.Collections.Generic;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Common.Extensions;
using Luna.Infrastructure.Domain;
using System;

namespace Luna.Shifts.Domain
{
    public static class LaborRuleExt
    {
        public static int InitialDayOffCount(this Attendance attendance)
        {
            var dayOffRule = attendance.DayOffRule;

            if (!dayOffRule.SystemAccumulate)
                return dayOffRule.SpecifiedNumberOfDays;

            var dayOffCounts = 0;
            var i = 0;
            DateTime day;
            while ((day = attendance.Start.AddDays(i)) < attendance.End)
            {
                var dayOffWeek = day.DayOfWeek;

                if (dayOffRule.Add1DayOffEachSaturdayInCS && dayOffWeek == DayOfWeek.Saturday)
                {
                    dayOffCounts++;
                }
                else if (dayOffRule.Add1DayOffEachSundayInCS && dayOffWeek == DayOfWeek.Sunday)
                {
                    dayOffCounts++;
                }
                else if (dayOffRule.Add1DayOffEachHolidayInCS && day.Date.IsHoliday(Country.Local))
                {
                    dayOffCounts++;
                }

                i++;
            }

            return dayOffCounts;
        }

        public static Attendance CopyRule(this Attendance attendance, ISimpleEmployee employee)
        {
            if (employee == null)
                return attendance;

            var rule = employee.LaborRule;
            if (rule != null)
            {
                attendance.MaxOvertimeThreshold = rule.MaxOvertime;
                attendance.MaxShrinkedThreshold = rule.MaxShrinked;
                attendance.MCDO = rule.MCDO;
                attendance.MCWD = rule.MCWD;
                attendance.MinIdleGap = rule.MinIdleGap;
                attendance.StdDailyLaborHour = rule.StdDailyLaborHour;
                attendance.MaxLaborHour = rule.MaxLaborHour;
                attendance.MinLaborHour = rule.MinLaborHour;
                attendance.MaxSwapTimes = rule.MaxSwapTimes;


                //addtional properties for schduling
                attendance.DayOffRule = new DayOffRule()
                {
                    SystemAccumulate = rule.DayOffRule.SystemAccumulate,
                    Add1DayOffEachSaturdayInCS = rule.DayOffRule.Add1DayOffEachSaturdayInCS,
                    Add1DayOffEachHolidayInCS = rule.DayOffRule.Add1DayOffEachHolidayInCS,
                    Add1DayOffEachSundayInCS = rule.DayOffRule.Add1DayOffEachSundayInCS,
                    HolidayShiftRule = rule.DayOffRule.HolidayShiftRule,
                    SpecifiedNumberOfDays = rule.DayOffRule.SpecifiedNumberOfDays,
                    MaxFWTimes = rule.DayOffRule.MaxFWTimes,
                    MaxPWTimes = rule.DayOffRule.MaxPWTimes,
                    MinFWTimes = rule.DayOffRule.MinFWTimes,
                    MinPWTimes = rule.DayOffRule.MinPWTimes
                };

                attendance.SchedulingPayload.DayOffMask = new MaskOfDay()
                {
                    Monthdays = rule.DayOffMask.Monthdays,
                    Weekdays = rule.DayOffMask.Weekdays
                };
                /*attendance.SchedulingPayload.GroupingArrangeShift = new GroupingArrangeShift()
                {
                    IsGrouping = rule.GroupingArrangeShift.IsGrouping,
                    IsMappingEvent = rule.GroupingArrangeShift.IsMappingEvent
                };*/

                attendance.AmountDayOff = attendance.InitialDayOffCount();
            }
            return attendance;
        }
    }
}