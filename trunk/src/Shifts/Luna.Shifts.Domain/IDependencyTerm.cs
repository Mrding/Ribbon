using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    public interface IDependencyTerm
    {
    }

    public interface  IImmutableTerm
    {
        
    }

    public interface IIndependenceTerm : IAdherenceTerm
    {
        bool? BelongToPrv { get; }

        DateTime HrDate { get; set; }
    }

    public interface IAdherenceTerm : ITerm
    {
        bool IgnoreAdherence { get; }
    }
}
