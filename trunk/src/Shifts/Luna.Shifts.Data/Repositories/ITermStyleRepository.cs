using System.Collections;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Domain;
using System;
using Luna.Common.Domain;
using NHibernate.Transform;

namespace Luna.Shifts.Data.Repositories
{
    public interface ITermStyleRepository : IRepository<TermStyle>
    {
        Int64 Count(TermStyle example);

        IEnumerable<TermStyle> GetByCategory(TermStyleCategory category);

        IEnumerable<AssignmentType> GetAssignmentTypes();

        IList<TConverted> GetAssignmentTypes<TConverted>(IResultTransformer resultTransformer);

        IEnumerable<AssignmentType> GetAssignmentTypesWithInsertRules();

        IDictionary<string, HeaderContainer<AssignmentType, DailyCounter<AssignmentType>, DateTime>> GetAssignmentTypeDailyCounter(DateTime from, DateTime end, ICollection<ServiceQueue> svcQueues);

        IEnumerable<TermStyle> GetEventTypes();

        IEnumerable<TermStyle> GetInUseAbsentTypes();

        IEnumerable<TermStyle> GetAllUnlaboredSubEvents();

        IEnumerable<TermStyle> GetAllRegularSubEvents();

        IEnumerable<BasicAssignmentType> GetWholeAssignmentTypes();

        IEnumerable<TermStyle> GetWholeEventTypes();

        IEnumerable<TermStyle> GetAbsentTypes();
    }
}
