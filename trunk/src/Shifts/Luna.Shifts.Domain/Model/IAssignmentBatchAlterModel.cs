using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    public interface IAssignmentBatchAlterModel : IBatchAlterModel
    {
        IEnumerable<AssignmentType> Types { get; set; }

        AssignmentType QueryType { get; set; }

        DateTime NewStartTime { get; set; }

        DateTime NewEndTime { get; set; }

        IEnumerable<ICustomFilter> Filters { get; }

        //IEnumerable<ICustomAction> OptionalActions { get; }


        IEnumerable<Term> FindTargetTerms(IAgent agent, Func<Term, bool> pred);


    }


}