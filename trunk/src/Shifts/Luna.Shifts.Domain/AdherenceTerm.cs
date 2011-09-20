using System;
using System.Diagnostics;
using Luna.Common;
using Luna.Core.Extensions;

namespace Luna.Shifts.Domain
{
    [DebuggerDisplay("{Start} - {End}  :{Text}")]
    public class AdherenceTerm : IVisibleTerm
    {
        public AdherenceTerm(DateTime start, DateTime end, DateRange monitoringRange)
        {
            //var startValue = monitoringRange.Start == start ? start : start;
            //Start = startValue;
            Start = start;

            //var endValue = monitoringRange.End == end ? end : end;
            End = end <= start ? start : end;
        }
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public virtual bool HasChanged(bool compareWithDateOnly, out DateTime oldStart, out DateTime oldEnd)
        {
            throw new NotImplementedException();
        }

        public string Text { get; set; }

        public string Remark { get; set; }
    }
}
