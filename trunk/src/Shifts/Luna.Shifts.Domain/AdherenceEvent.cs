using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    [DebuggerDisplay("{Start} - {End}  :{Text}")]
    public class AdherenceEvent : IVisibleTerm, IWritableTerm
    {
        public Guid EmployeeId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Text { get; set; }

        public string Remark { get; set; }

        public string Reason { get; set; }

        public string Assigner { get; set; }

        public DateTime TimeStamp { get; set; }

    }
}
