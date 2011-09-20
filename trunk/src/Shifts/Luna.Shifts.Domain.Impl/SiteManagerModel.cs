using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Common.Extensions;
using Luna.Core.Extensions;
using Luna.Infrastructure.Data.Repositories;
using Luna.Infrastructure.Domain;
using Luna.Shifts.Data.Repositories;
using Luna.Shifts.Domain.Model;
using uNhAddIns.Adapters;
using System;

namespace Luna.Shifts.Domain.Impl
{
    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Implicit)]
    public class SiteManagerModel : ISiteManagerModel
    {
        private readonly IAreaRepository _areaRepository;
        private readonly ISeatRepository _seatRepository;
        
        private readonly ISeatBoxRepository _seatBoxRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ICampaignRepository _campaignRepository;

        public SiteManagerModel(IAreaRepository areaRepository, ISeatRepository seatRepository, ISeatBoxRepository seatBoxRepository,
          ISiteRepository siteRepository, ICampaignRepository campaignRepository)
        {
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            _seatBoxRepository = seatBoxRepository;
            _siteRepository = siteRepository;
            _campaignRepository = campaignRepository;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public ICampaignSites[] GetAll()
        {
            //todo »á¶ÁÈ¡N+1 Campaign
            var q = from a in _areaRepository.GetAll()
                    group a by a.Campaign into g
                    select new CampaignSites
                    {
                        Campaign = g.Key,
                        Sites = (from a2 in g
                                 group a2 by a2.Site into g2
                                 select new CampaignSite
                                {
                                    Campaign = g.Key,
                                    Site = g2.Key,
                                    Areas = g2.Where(o => o.Campaign == g.Key).Select(o =>
                                                                                          {
                                                                                              return o as Entity;
                                                                                          }).ToList()
                                }).ToArray()
                    };
            return q.ToArray();
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public IEnumerable<IArea> GetAll(Entity campaign)
        {
            return _areaRepository.GetAreaByCampaign(campaign).OfType<IArea>();
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void ReloadCampaignSite(ISeatingSite<Entity> item)
        {
            item.Site = _siteRepository.Get(item.Site.Id);
            item.Areas = _areaRepository.GetBySite(item.Site).Cast<Entity>().ToList();
        }



        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public IEnumerable<IArea> GetAreaWithSeat(Entity site)
        {
            return _areaRepository.GetWithSeats(site).OfType<IArea>();
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public Area ReloadWithSeat(Area area)
        {
            return _areaRepository.GetWithSeats(area.Id);
        }

        /// <summary>
        /// Save a new seat 
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="seats">existed seat of same area</param>
        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void SaveAsNew(Seat seat)
        {
            if (!seat.IsNew())
                throw new Exception("Crearw new seat fail & entity is not a new object");

            seat.InUse = true;
            seat.IsActivated = true;
            seat.IsOpen = true;
            var maxPriority = _seatRepository.GetMaxPriority(seat.Area);
            seat.Index = maxPriority == 0 ? 0 : maxPriority + 1;

            _seatRepository.MakePersistent(seat);
            _seatBoxRepository.MakePersistent(new SeatBox(seat));
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void PersistentSeat(Seat seat)
        {
            if (seat.IsNew())
                throw new Exception("save seat fail & entity cannpt be a new object");

            _seatRepository.MakePersistent(seat);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void PersistentSite(Entity site)
        {
            if (site.IsNew())
                throw new Exception("save seat fail & entity cannpt be a new object");

            _siteRepository.MakePersistent(site as Site);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void RebuildSeatsPriority(Area area)
        {
            _areaRepository.Get(area.Id).Seats.RebuildPriority<ISeat, Seat>(true,o =>
            {
                _seatRepository.MakePersistent(o);
            });
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Abort)]
        public void Abort()
        {
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void SubmitChanges()
        {
        }


        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public bool RemoveSeat(Seat seat, Action<Seat> removeRelation)
        {
            if (_seatRepository.HaveAnyRelationships(seat))
                return false;

            removeRelation(seat);

            _seatRepository.MakeTransient(seat);
            return true;
        }

        [PersistenceConversation(ConversationEndMode = EndMode.End)]
        public void Save(Area area)
        {
            if (area.Site.IsNew())
            {
                _siteRepository.MakePersistent(area.Site as Site);
            }
            _areaRepository.MakePersistent(area);
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public Luna.Core.Tuple<IEnumerable<Site>, IEnumerable<Campaign>> GetAllSitesAndCampains()
        {
            return new Luna.Core.Tuple<IEnumerable<Site>, IEnumerable<Campaign>>(_siteRepository.DelayGetAll(), _campaignRepository.DelayGetAll());
        }

        [PersistenceConversation(ConversationEndMode = EndMode.CommitAndContinue)]
        public void ReCalculate(IEnumerable<ISeat> seats, int oldColumn, int newColumn)
        {
            if(seats.Count()==0) return;
            var orderseat = newColumn > oldColumn ? seats.OrderByDescending(o => o.LocationIndex) : seats;
            _seatRepository.Clear();
            foreach (Seat seat in orderseat)
            {
                seat.LocationIndex += (newColumn - oldColumn) * (seat.LocationIndex / oldColumn);
                _seatRepository.MakePersistent(seat);
            }
            //orderseat.ForEach(o => Console.Write(o.Number + "+ " + o.LocationIndex + ","));
        }
    }
}
