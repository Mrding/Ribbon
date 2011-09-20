using System;
using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface ICustomEmployeeGroupRepository : IRepository<CustomEmployeeGroup>
    {
        IList<CustomEmployeeGroup> LoadMyCustomGroups(Employee owner);
        bool GetCustomGroups(string groupName, Employee owner);
        IList<Employee> GetEmployees(Guid[] employeeIds);
    }
}
