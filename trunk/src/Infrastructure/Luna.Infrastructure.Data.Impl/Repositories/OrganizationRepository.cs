using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        private Dictionary<string, LaborRule> _laborRules;
        public override void LoadRelatedEntities()
        {
            _laborRules = DetachedCriteria.For<LaborRule>()
                                          .GetExecutableCriteria(Session).List<LaborRule>()
                                          .ToDictionary(o => o.GetUniqueKey());
        }

        public void MakePersistentWithSync(Organization entity)
        {
            entity.LaborRule = _laborRules[entity.LaborRule.GetUniqueKey()];
            MakePersistent(entity);
        }

        public IEnumerable<Organization> GetByCampaign(Entity campaign)
        {
            return Session.Query<Campaign>().First(o => o == campaign).Organizations.ToList().OfType<Organization>();
        }
        public override bool HaveAnyRelationships(Organization entity)
        {
            const string campaignOrgCount = "select count(*) from CampaignOrg where OrganizationId=:orgId";
            const string csOrgCount = "select count(*) from CSOrg where OrganizationId=:orgId";
            var sql = string.Format("select ({0})+({1})", campaignOrgCount, csOrgCount);
            var results = Session.CreateSQLQuery(sql).SetGuid("acdqId", entity.Id).UniqueResult<int>();
            return results > 0;
        }

        public void DeleteOrgAndManager(Organization organization)
        {
            Session.Delete(string.Format("from OrganizationManager om where om.Organization='{0}'", organization.Id));
            Session.Delete(organization);
        }

        public Organization GetRootOrganization()
        {
            return Session.CreateCriteria<Organization>()
                .Add(Restrictions.IsNull("Parent"))
                .SetFetchMode("Children", FetchMode.Eager)
                .SetResultTransformer(new RootEntityResultTransformer())
                .UniqueResult<Organization>();
        }

        public IList<Organization> GetHierarchicalTree()
        {
            return Session.CreateCriteria<Organization>()
                .Add(Restrictions.IsNull("Parent"))
                .SetFetchMode("Children", FetchMode.Eager)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .List<Organization>();
        }

        public IList<Organization> GetLaborRuleOrganiztion(LaborRule laborRule)
        {
           return this.Where(o => o.LaborRule == laborRule).ToList();
        }
    }
}
