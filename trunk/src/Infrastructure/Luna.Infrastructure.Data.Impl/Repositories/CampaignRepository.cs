using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class CampaignRepository : Repository<Campaign>, ICampaignRepository
    {
        private Dictionary<string, Organization> _organizations;
        public override void LoadRelatedEntities()
        {
            _organizations = DetachedCriteria.For<Organization>()
                .GetExecutableCriteria(Session).List().OfType<Organization>()
                .ToDictionary(o => o.GetUniqueKey());
        }

        public void MakePersistentWithSync(Action<Campaign, Dictionary<string, Organization>> updateWithOrgs, Campaign entity)
        {
            MakePersistent(entity);
            updateWithOrgs(entity, _organizations);
        }

        //public override IList<Campaign> GetAll()
        //{
        //    var campaigns = Session.CreateQuery("from Campaign c left outer join fetch c.Organizations o left outer join fetch o.Children")
        //        //return Session.CreateCriteria<Campaign>()
        //        //    .CreateAlias("Organizations","o")
        //        //    .SetFetchMode("o", FetchMode.Eager)
        //        //    .SetFetchMode("o.Children", FetchMode.Eager)
        //        .SetResultTransformer(new DistinctRootEntityResultTransformer())
        //        .List<Campaign>();

        //    //foreach (var campaign in campaigns)
        //    //{
        //    //    foreach (Organization org in campaign.Organizations)
        //    //    {
        //    //        var rootOrg = org.FindRootOrganization();
        //    //        rootOrg.VisitOrganizationTree();
        //    //    }
        //    //}

        //    return campaigns;
        //}

        public  IList<Campaign> GetAllCampaignWithOrganization()
        {
            var campaigns = Session.CreateQuery("from Campaign c left outer join fetch c.Organizations o left outer join fetch o.Children")
                //return Session.CreateCriteria<Campaign>()
                //    .CreateAlias("Organizations","o")
                //    .SetFetchMode("o", FetchMode.Eager)
                //    .SetFetchMode("o.Children", FetchMode.Eager)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .List<Campaign>();

            //foreach (var campaign in campaigns)
            //{
            //    foreach (Organization org in campaign.Organizations)
            //    {
            //        var rootOrg = org.FindRootOrganization();
            //        rootOrg.VisitOrganizationTree();
            //    }
            //}

            return campaigns;
        }

        public override bool HaveAnyRelationships(Campaign entity)
        {
            using (var session = Factory.OpenStatelessSession())
            {
                const string serviceQueue = "select count(*) from ServiceQueue s where s.Campaign =:campaign ";
                const string area = "select count(*) from Area s where s.Campaign =:campaign";
                const string schedule = "select count(*) from CampaignSchedule s where s.Campaign =:campaign ";

                var sql = string.Format("select ({0})+({1})+({2})", serviceQueue, area, schedule);

                var results = session.CreateSQLQuery(sql).SetGuid("campaign", entity.Id).UniqueResult<int>();

                return results > 0;
            }
        }
    }
}
