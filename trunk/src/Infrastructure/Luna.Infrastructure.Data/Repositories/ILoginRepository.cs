using System;
using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface ILoginRepository : IRepository<Employee>
    {
        Employee GetMachedEmployee(string agentId);

        IList<string> GetFunctionKeys(string role);

        IList<string> GetFunctionKeys(Guid roleId);

        IList<string> GetRoles(Guid employeeId);
    }
}
