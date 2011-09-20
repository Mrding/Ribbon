using System;

namespace Luna.Shifts.Domain
{
    public class DeletedTerm
    {
        public DeletedTerm(){}
        public DeletedTerm(AssignmentBase term, Guid empId)
        {
            Id = term.Id;
            Start = term.Start;
            End = term.End;
            Locked = term.Locked;
            Level = term.Level;
            Name = term.Text;
            TimeBoxId = empId;
        }
        public DeletedTerm(Term term, Guid empId)
        {
            Id = term.Id;
            Start = term.Start;
            End = term.End;
            Locked = term.Locked;
            Level = term.Level;
            Name = term.Text;
            TimeBoxId = empId;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Locked { get; set; }
        public int Level { get; set; }
        public Guid TimeBoxId { get; set; }
    }
}
