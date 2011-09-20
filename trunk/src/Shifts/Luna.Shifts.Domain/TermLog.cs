using System;

namespace Luna.Shifts.Domain
{
    public class TermLog
    {
        public Int64? SourceId { get; set; }

        public Guid? AlterEmployeeId { get; set; }

        public Guid? EmployeeId { get; set; }

        public string Action { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string OldTime { get; set; }

        public string NewTime { get; set; }

        public string Remark { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
