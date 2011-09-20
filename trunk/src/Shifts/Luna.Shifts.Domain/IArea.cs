using Luna.Common;
using System.Collections.Generic;

namespace Luna.Shifts.Domain
{
    public interface IArea
    {
        string Name { get; }

        Dimension Dimension { get; }

        Entity Site { get; }

        Entity Campaign { get; }

        ICollection<ISeat> Seats { get; }
    }
}
