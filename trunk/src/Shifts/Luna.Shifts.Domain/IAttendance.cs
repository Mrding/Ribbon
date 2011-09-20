using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public interface IAttendance
    {
        bool Joined { get; }
        DateTime Start { get; }
        DateTime End { get; }
        TimeSpan MaxOvertimeThreshold { get; set; }
        TimeSpan MaxShrinkedThreshold { get; set; }
        TimeSpan OvertimeTotals { get; set; }
        TimeSpan ShrinkedTotals { get; set; }

        //int MaxSwapTimes { get; set; }


        int MCDO { get; set; }
        int MCWD { get; set; }
        TimeSpan MinIdleGap { get; set; }
        TimeSpan StdDailyLaborHour { get; set; }
        TimeSpan MaxLaborHour { get; set; }
        TimeSpan MinLaborHour { get; set; }
        TimeSpan LaborHourTotals { get; set; }
        int WorkingDayCount { get; }
        bool EnrolmentDateNotQualify { get; }
        ICollection<Exception> Exceptions { get; set; }

        bool HasError { get; set; }
    }
}