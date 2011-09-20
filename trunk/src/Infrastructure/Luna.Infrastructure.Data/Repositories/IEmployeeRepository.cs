using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Common.Interfaces;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>,IExpandMethod
    {
        IList<Employee> GetAllWithFullDetail();

        void MakePersistentWithSync(Action<Employee, Dictionary<string,Skill>> updateWithSkills, Employee entity);

        IList<Employee> GetEmployeeByOrganizations(Entity[] organizations, DateTime date);

        IList<Employee> GetEmployeeByLaborRule(LaborRule laborRule);

        IList<string> GetAgentAcdid(Guid employeeId);
    }
}
