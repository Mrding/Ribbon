using System;
using System.Diagnostics;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    [DebuggerDisplay("{Start} - {End}  {OnService}")]
    public class AgentStatusTerm : ITerm
    {
        public AgentStatusTerm()
        {

        }

        public AgentStatusTerm(DateTime start, DateTime end, bool onService)
        {
            Start = start;
            End = end;
            OnService = onService;
        }
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public virtual bool HasChanged(bool compareWithDateOnly, out DateTime oldStart, out DateTime oldEnd)
        {
            throw new NotImplementedException();
        }

        public bool OnService { get; set; }
    }
}
