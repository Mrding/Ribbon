using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common.Interfaces;
using Luna.Infrastructure.Domain;
using Luna.Common;
using Luna.Common.Domain;

namespace Luna.Shifts.Domain
{
    public interface IWorkingAgent : IAgent
    {
        ISeat CurrentSeat { get; set; }

        IList<Occupation> Occupations { get; set; }
    }


    public interface IAgent : IEnumerable
    {
        TimeBox Schedule { get; }
        Attendance LaborRule { get; }
        ISimpleEmployee Profile { get; }

        //Func<Term, bool> Filter { get; set; }

        void BuildOnlines();

        bool? OperationFail { get; set; }

        IEnumerable<Term> SelectTargetTerms(Func<Term, bool> predicate);

        bool[] Onlines { get; }

        IAgent TransferPropertiesFrom(IAgent original);
    }
}