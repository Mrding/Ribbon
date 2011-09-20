using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    public interface IEventBatchAlterModel : IBatchAlterModel
    {
        IEnumerable<ICustomFilter> OptionalFilters { get; }

        DateTime End { get; set; }

        IEnumerable<TermStyle> Types { get; }

        IEnumerable<AssignmentType> ParentTypes { get; }

        AssignmentType QueryParentType { get; set; }

        TermStyle QueryType { get; set; }

        DateTime NewStartTime { get; set; }

        DateTime NewEndTime { get; set; }

        IEnumerable<ICustomFilter> Filters { get; }

        //IEnumerable<ICustomAction> OptionalActions { get; }

        TimeSpan StartAlterOffSet { get; set; }

        TimeSpan EndAlterOffSet { get; set; }

        TimeSpan MoveOffSet { get; set; }

        IEnumerable<Term> FindTargetTerms(IAgent agent, Func<Term, bool> pred);

    }
}
