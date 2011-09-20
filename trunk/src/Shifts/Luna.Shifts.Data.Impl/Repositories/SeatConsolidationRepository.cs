using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate.Criterion;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class SeatConsolidationRepository : Repository<SeatConsolidationRule>, ISeatConsolidationRepository
    {

        public IList<SeatConsolidationRule> GetByStie(Entity site)
        {
            var rules = Session.CreateCriteria<SeatConsolidationRule>().Add(Restrictions.Eq("Site", site))
                              .AddOrder(new Order("Index", true))
                              .List<SeatConsolidationRule>();
            return rules;
        }

        public IList<SeatConsolidationRule> GetByStieAndCampaign(Entity site, Entity campaign)
        {
            var rules = Session.CreateCriteria<SeatConsolidationRule>()
                .CreateAlias("TargetSeat", "seat")
                .CreateAlias("seat.Area", "area")
                .Add(Restrictions.Eq("Site", site))
                              .Add(Restrictions.Eq("area.Campaign", campaign))
                              .AddOrder(new Order("Index", true))
                              .List<SeatConsolidationRule>();
            return rules;

            //var so = Session.CreateQuery("from  SeatConsolidationRule r left join r.TargetSeat s left join s.Area a where a.Campaign.Id =:camp and r.Site.Id=:site order by r.Index")
            //    .SetGuid("site", site.Id)
            //    .SetGuid("camp", campaign.Id)
            //    .List<SeatConsolidationRule>();

            //return so;
        }
    }
}
