using System;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    public interface ISchedule : ITerm
    {
        Guid Id { get; }

        string Name { get; set; }

        Progress Progress { get; set; }

        Entity Campaign { get; set; }

        IDictionary<ServiceQueue, int> ServiceQueues { get; set; }

        ICollection<Entity> Organizations { get; set; }

        int SeatCapacity { get; set; }
    }
}
