using System;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    public class BackupTerm : IVisibleTerm, IStyledTerm
    {
        protected BackupTerm() { }

        public BackupTerm(Int64 id, Guid employeeId,DateTime start, DateTime end, string text, string background,int level )
        {
            Id = id;
            EmployeeId = employeeId;
            Start = start;
            End = end;
            Text = text;
            Background = background;
            Level = level;
        }

        public Int64 Id { get; set; }

        public Guid EmployeeId { get; private set; }

        public Int64? ParentTermId { get; set; }

        public DateTime Start { get; private set; }

        public DateTime End { get; private set; }

        public int Level { get; private set; }

        public TimeSpan WorkingTotals { get; set; }

        public DateTime HrDate { get; set; }

        public string Text
        {
            get; internal set;
        }

        public virtual string Remark { get; set; }

        public string Background { get; set; }
    }
}
