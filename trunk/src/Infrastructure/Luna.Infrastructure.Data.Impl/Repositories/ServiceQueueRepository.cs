using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class ServiceQueueRepository : Repository<ServiceQueue>, IServiceQueueRepository
    {
        private Dictionary<string, Skill> _skills;
        private Dictionary<string, Campaign> _campaigns;

        public override IList<ServiceQueue> GetAll()
        {
            return this.Fetch(o => o.MappedSkill).ToList();
        }

        public override void LoadRelatedEntities()
        {
            _skills = DetachedCriteria.For<Skill>()
                .GetExecutableCriteria(Session).List().OfType<Skill>()
                .ToDictionary(o => o.GetUniqueKey());

            _campaigns = DetachedCriteria.For<Campaign>()
                .GetExecutableCriteria(Session).List().OfType<Campaign>()
                .ToDictionary(o => o.GetUniqueKey());
        }

        public IEnumerable<ServiceQueue> GetServiceQueueByCampaign(ICampaign campaign)
        {
            return this.Where(o => o.Campaign == campaign).Fetch(o => o.MappedSkill).ToList();
        }

        public void MakePersistentWithSync(ServiceQueue entity)
        {
            var campaignKey = entity.Campaign.GetUniqueKey();
            if (!_campaigns.ContainsKey(campaignKey))
                return;

            entity.Campaign = _campaigns[campaignKey];


            var skillKey = entity.MappedSkill.GetUniqueKey();
            if (!_skills.ContainsKey(skillKey))
                return;

            entity.MappedSkill = _skills[skillKey];
            
            MakePersistent(entity);
        }

        public override bool HaveAnyRelationships(ServiceQueue entity)
        {
            using (var session = Factory.OpenStatelessSession())
            {
                const string forecastCount = "select count(*) from Forecast where ServiceQueueId=:sqId ";
                const string serviceQueueTrafficCount = "select count(*) from ServiceQueueTraffic where ServiceQueueId=:sqId";
                const string cssqCount = "select count(*) from CSSQ where ServiceQueueId=:sqId";
                var sql = string.Format("select ({0})+({1})+({2})", forecastCount, serviceQueueTrafficCount, cssqCount);
                var results = session.CreateSQLQuery(sql).SetGuid("sqId", entity.Id).UniqueResult<int>();
                return results > 0;
            }
        }
    }
}
