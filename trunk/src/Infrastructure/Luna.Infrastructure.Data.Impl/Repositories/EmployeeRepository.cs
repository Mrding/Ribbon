using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private Dictionary<string, Skill> _skills;
        private readonly Dictionary<string, Employee> Employees = new Dictionary<string, Employee>();
        private Dictionary<string, Organization> _organizations;

        public override IList<TConverted> GetAll<TConverted>(IResultTransformer resultTransformer)
        {
            return Session.CreateCriteria<Employee>()
                          .AddOrder(new Order("Name", true))
                          .SetResultTransformer(resultTransformer)
                          .List<TConverted>();
        }

        public IList<Employee> GetAllWithFullDetail()
        {
            return Session.CreateQuery("select distinct e from Employee e left join fetch e.SkillMap left join fetch e.CustomLaborRule left join fetch e.Organization").List<Employee>();
        }

        public override void LoadRelatedEntities()
        {
            _skills = DetachedCriteria.For<Skill>()
                .GetExecutableCriteria(Session).List().OfType<Skill>()
                .ToDictionary(o => o.GetUniqueKey());

            _organizations = DetachedCriteria.For<Organization>()
                .GetExecutableCriteria(Session).List().OfType<Organization>()
                .ToDictionary(o => o.GetUniqueKey());
        }

        public void MakePersistentWithSync(Action<Employee, Dictionary<string, Skill>> updateWithSkills, Employee entity)
        {
            entity.Organization = _organizations[entity.Organization.GetUniqueKey()];
            MakePersistent(entity);
            updateWithSkills(entity, _skills);
        }

        public IList<Employee> GetEmployeeByOrganizations(Entity[] organizations, DateTime date)
        {
            if (organizations.Length == 0) return new List<Employee>();
            return this.Where(o => organizations.Contains(o.Organization) && o.LeavingDate >= date).OrderByDescending(o => o.Organization.Name).ToList();
        }

        public IList<Employee> GetEmployeeByLaborRule(LaborRule laborRule)
        {
            return this.Where(o => o.CustomLaborRule == laborRule).ToList();
        }

        public object GetExpandObject(object arg)
        {
            return GetEmployeeByAcdid(arg.ToString());
        }

        private Employee GetEmployeeByAcdid(string acdid)
        {
            if (Employees.ContainsKey(acdid))
            {
                return Employees[acdid];
            }

            var employee = NHibernateManager.Factory.OpenSession()
                .CreateSQLQuery("select * from employee where employeeid in (select employeeid from employeeskill where agentacdid =:acdid)")
                .AddEntity(typeof(Employee))
                .SetString("acdid", acdid)
                .UniqueResult<Employee>();

            Employees[acdid] = employee;
            return employee;
        }

        public IList<string> GetAgentAcdid(Guid employeeId)
        {
            //var acdids = new List<string>(4);
            //var conn = NHibernateManager.GetConnection();
            //using (var cmd = conn.CreateCommand())
            //{
            //    cmd.CommandText = string.Format(@"select distinct t.AgentAcdid from EmployeeSkill t where t.EmployeeId = '{0}'",
            //                                    employeeId);
            //    using (var reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            acdids.Add(reader.GetInt32(0).ToString());
            //        }
            //    }
            //}
            //return acdids.ToArray();
            return Session.CreateSQLQuery(
                string.Format(@"select distinct CONVERT(VARCHAR(16),t.AgentAcdid) as acdid from EmployeeSkill t where t.EmployeeId = '{0}'",
                              employeeId)).List<string>();

        }
    }
}
