using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public interface ISimpleEmployee
    {
        Guid Id { get; }

        string Name { get; }

        string Name2 { get; }

        string AgentId { get; }

        string[] AgentAcdids { get; }

        Entity Organization { get; }
        int Rank { get; }
        IDictionary<Skill, double> Skills { get; }

        LaborRule LaborRule { get; set; }

        DateTime EnrollmentDate { get; }
    }
}