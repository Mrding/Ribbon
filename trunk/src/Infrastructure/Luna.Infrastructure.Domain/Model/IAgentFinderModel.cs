using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Common;

namespace Luna.Infrastructure.Domain.Model
{
    public interface IAgentFinderModel
    {
        IEnumerable<ICustomFilter> GetFilters();

        void CreateTimeBoxFilter(IEnumerable agents, DateTime targetDate);

        void TearDown();
    }
}
