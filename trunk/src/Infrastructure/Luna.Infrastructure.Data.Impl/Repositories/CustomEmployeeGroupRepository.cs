using System;
using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using NHibernate.Criterion;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class CustomEmployeeGroupRepository : Repository<CustomEmployeeGroup>, ICustomEmployeeGroupRepository
    {
        public IList<CustomEmployeeGroup> LoadMyCustomGroups(Employee owner)
        {
            return Session.CreateCriteria<CustomEmployeeGroup>()
                .Add(Restrictions.Eq("Owner", owner))
                .List<CustomEmployeeGroup>();
        }

        public bool GetCustomGroups(string groupName, Employee owner)
        {
            var results = Session.CreateCriteria<CustomEmployeeGroup>()
                 .Add(Restrictions.Eq("Owner", owner))
                .Add(Restrictions.Eq("GroupName", groupName))
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();
            return results != 0;
        }

        public IList<Employee> GetEmployees(Guid[] employeeIds)
        {
            return Session.CreateCriteria<Employee>()
                .Add(Restrictions.In("Id", employeeIds))
                .List<Employee>();
        }

        public override void MakeTransient(CustomEmployeeGroup entity)
        {
            var d1 = string.Format("delete from CustomGroupEmployees where CustomEmployeeGroupId = '{0}'", entity.Id);
            var d2 = string.Format("delete from CustomEmployeeGroup where CustomEmployeeGroupId = '{0}'", entity.Id);
            var sql = string.Format("{0};{1}", d1, d2);
            Session.CreateSQLQuery(sql).ExecuteUpdate();
        }
    }
}
