using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Data;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain;
using NHibernate;
using NHibernate.Criterion;

namespace Luna.Shifts.Data.Impl.Repositories
{
    public class AreaRepository : Repository<Area>, IAreaRepository
    {
        private Dictionary<string, Site> _sites;
        private Dictionary<string, Entity> _campaigns;

        public override IList<Area> GetAll()
        {
            return Session.CreateQuery("select distinct a from Area a inner join fetch a.Site").List<Area>();
        }

        public override void LoadRelatedEntities()
        {
            _sites = DetachedCriteria.For<Site>()
                                          .GetExecutableCriteria(Session).List<Site>()
                                          .ToDictionary(o => o.GetUniqueKey());
            _campaigns = Session.CreateQuery("from Campaign").List<Entity>().ToDictionary(o => o.GetUniqueKey());
        }

        public void MakePersistentWithSync(Area entity)
        {
            entity.Site = _sites[entity.Site.GetUniqueKey()];
            entity.Campaign = _campaigns[entity.Campaign.GetUniqueKey()];
            MakePersistent(entity);
        }

        public IList<Area> GetAreaByCampaign(Entity campaign)
        {
            return Session.CreateQuery(@"from Area a
                                         where a.Campaign =:campaign
                                         order by a.Index")
                    .SetGuid("campaign", campaign.Id)
                    .List<Area>();
        }

        //public IEnumerable<Area> GetAllWithDetail()
        //{
        //    return Session.CreateCriteria<Area>()

        //                  .AddOrder(new Order("Index", true))
        //                  .List<Area>();
        //}



        public Area GetWithSeats(Guid areaId)
        {
            return
                Session.CreateQuery(
                    "select distinct a from Area a left join fetch a.Seats where a.Id =:areaId")
                    .SetGuid("areaId", areaId)
                    .UniqueResult<Area>();

          
        }


        public IEnumerable<Area> GetBySite(Entity site)
        {
            return Session.CreateQuery("from Area a where a.Site =:Site")
                          .SetGuid("Site", site.Id)
                          .List<Area>();
        }
        public IEnumerable<Area> GetWithSeats(Entity site)
        {
            var ares = Session.CreateCriteria<Area>().Add(Restrictions.Eq("Site", site))
                
                              .SetFetchMode("Seats", FetchMode.Join)
                              .AddOrder(new Order("Index", true))
                              .List<Area>();
            return ares;
            //return Session.CreateQuery("select distinct a from Area a left join fetch a.Seats where a.Site.Id =:SiteId order by a.Index")
            //              .SetGuid("SiteId", site.Id)
            //              .List<Area>();
        }

        public IEnumerable<Area> GetWithSeatPriorityDetails(Entity site, Entity campaign)
        {

           //return  Session.CreateCriteria<Area>().Add(Restrictions.Eq("Site", site))
           //     .Add(Restrictions.Eq("Campaign", campaign))
           //     .AddOrder(new Order("Index", true))
           //     .SetFetchMode("Seats", FetchMode.Join)
           //     .List<Area>();

            return Session.CreateQuery(@"from Area a
                                         where a.Site =:Site and a.Campaign =:Campaign
                                         order by a.Index")

                      .SetGuid("Site", site.Id)
                      .SetGuid("Campaign", campaign.Id)
                      .List<Area>();
        }

        public IEnumerable<OrganizationSeatingArea> GetOrganizationSeatingArea(Guid areaId)
        {
            var a= Session.CreateQuery(@"from OrganizationSeatingArea o
                                         where o.Area.Id =:areaId
                                         order by o.Index")
                     .SetGuid("areaId", areaId)
                     .List<OrganizationSeatingArea>();

            return a;
        }

        public OrganizationSeatingArea MakePersistent(OrganizationSeatingArea obj)
        {
            Session.SaveOrUpdate(obj);
            return obj;
        }

        public IEnumerable<PriorityEmployee> GetPriorityEmployees(Entity[] areas)
        {
            var results = Session.CreateQuery(
                        "from PriorityEmployee p where p.Seat.Area in (:areas) order by p.Object.Id, p.Index")
                        .SetParameterList("areas", areas)
                        .List<PriorityEmployee>();
            return results;
        }
    }
}