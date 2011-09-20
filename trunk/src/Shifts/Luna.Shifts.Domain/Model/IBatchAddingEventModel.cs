using System;
using System.Collections.Generic;

namespace Luna.Shifts.Domain.Model
{
    public interface IBatchAddingEventModel : IBatchAlterModel
    {
        IEnumerable<TermStyle> EventTypes { get; set; }

        TermStyle SelectedEventType { get; set; }

        IEnumerable<AssignmentType> AssignmentTypes { get; set; }

        Action<Term, TimeBox> GetDefaultAction(TimeSpan length);
    }
}