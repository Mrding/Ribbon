using System;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    public interface ISeat : IIndexable , ICloze , ISeatingSeat
    {
        Guid Id { get; }

        Entity Area { get; }

        string Number { get; set; }

        string ExtNo { get; set; }

        bool InUse { get; set; }

        int Rank { get; set; }

        bool IsActivated { get; }

        //SequentialEntity<Entity> UsingOrganozation { get; set; }
    }
}
