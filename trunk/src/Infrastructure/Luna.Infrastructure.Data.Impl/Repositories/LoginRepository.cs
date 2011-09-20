using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class LoginRepository : Repository<Employee>, ILoginRepository
    {
        public Employee GetMachedEmployee(string agentId)
        {
            return this.FirstOrDefault(o => o.AgentId == agentId);
        }

        public IList<string> GetFunctionKeys(string role)
        {
            return Session.Query<AuthRoleFunction>().Where(o => o.AuthRole.RoleName == role).Select(o => o.Name).ToList();
        }

        public IList<string> GetFunctionKeys(Guid roleId)
        {
            return Session.CreateQuery("select f.Name from AuthRoleFunction f left join f.AuthRole r where r.Id = :id")
                    .SetGuid("id", roleId)
                    .SetReadOnly(true)
                    .SetResultTransformer(new DistinctRootEntityResultTransformer())
                    .List<string>();
        }

        public IList<string> GetRoles(Guid employeeId)
        {
            //var results = Session.QueryOver<AuthRole>().Where(o => o.Employees.Select(e=>e.Id).Contains(employeeId)).Select(o => o.RoleName).List();
            return Session.CreateQuery("select r.RoleName from AuthRole r left join r.Employees e where e.Id=:employeeId")
                .SetGuid("employeeId", employeeId)
                .SetReadOnly(true)
                .List<string>();
        }
    }
}
