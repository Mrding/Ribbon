using System;
using Luna.Common;
using Luna.Globalization;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain
{
    public class TermException : Exception
    {
        public TermException(Term causeTarget, string reason)
        {
            CauseTarget = causeTarget;
            Reason = reason;
        }

        public Term CauseTarget { get; private set; }

        public string Reason { get; private set; }
    }

    public enum LaborRuleCategory
    {
        Gap,
        MCWD,
        MCDO
    }

    public abstract class LaborRuleException : Exception, ITerm
    {
        protected string _discription;

        protected LaborRuleException(string agentId, DateTime start, DateTime end, LaborRuleCategory category, string message)
            : base(message)
        {
            AgentId = agentId;
            Start = start;
            End = end;
            Category = category;
        }

        public LaborRuleCategory Category { get; private set; }

        public string AgentId { get; private set; }

        public DateTime Start { get; private set; }

        public DateTime End { get; private set; }

        public string TimeRangeDisplayText { get; set; }

        public virtual bool HasChanged(bool compareWithDateOnly, out DateTime oldStart, out DateTime oldEnd)
        {
            throw new NotImplementedException();
        }
    }

    public class ShiftGapException : LaborRuleException
    {
        private readonly TimeSpan _timespan;
        public ShiftGapException(string agentId, DateTime start, DateTime end, TimeSpan timespan, LaborRuleCategory category)
            : base(agentId, start, end, category, string.Format(LanguageReader.GetValue("Shifts_ShiftGapException"), timespan.TotalHours))
        {
            _timespan = timespan;
            TimeRangeDisplayText = string.Format("{0:MM/dd HH:mm} - {1:MM/dd HH:mm}", start, end);
        }
    }

    public class OvertimeWorkingException : LaborRuleException
    {
        private readonly TimeSpan _timeSpan;
        public OvertimeWorkingException(string agentId, DateTime start, DateTime end, TimeSpan timeSpan, LaborRuleCategory category)
            : base(agentId, start, end, category, string.Format(LanguageReader.GetValue("Shifts_OvertimeWorkingException"), timeSpan.Days, timeSpan.Hours, timeSpan.Minutes))
        {
            _timeSpan = timeSpan;
            TimeRangeDisplayText = string.Format("{0:yyyy MM/dd HH:mm} - {1:yyyy MM/dd HH:mm}", start, end);
        }
    }

    public class DayOffException : LaborRuleException
    {
        private readonly double _dayOffDays;
        public DayOffException(string agentId, DateTime start, DateTime end, double dayOffDays, LaborRuleCategory category)
            : base(agentId, start, end, category, string.Format(LanguageReader.GetValue("Shifts_DayOffException"), dayOffDays))
        {
            _dayOffDays = dayOffDays;
            TimeRangeDisplayText = string.Format("{0:yyyy MM/dd} - {1:yyyy MM/dd}", start, end);
        }

    }

    public class FWException : LaborRuleException
    {
        public FWException(string agentId, DateTime start, DateTime end, int fw, DayOffRule rule)
            : base(agentId, start, end, LaborRuleCategory.Gap, string.Format(LanguageReader.GetValue("Shifts_FWException"), fw,
            rule.MinFWTimes, rule.MaxFWTimes))
        {
            TimeRangeDisplayText = string.Format("{0:yyyy MM/dd} - {1:yyyy MM/dd}", start, end);
        }
    }

    public class PWException : LaborRuleException
    {
        public PWException(string agentId, DateTime start, DateTime end, int pw, DayOffRule rule)
            : base(agentId, start, end, LaborRuleCategory.Gap, string.Format(LanguageReader.GetValue("Shifts_PWException"), pw,
           rule.MinFWTimes, rule.MaxFWTimes))
        {
            TimeRangeDisplayText = string.Format("{0:yyyy MM/dd} - {1:yyyy MM/dd}", start, end);
        }
    }
}