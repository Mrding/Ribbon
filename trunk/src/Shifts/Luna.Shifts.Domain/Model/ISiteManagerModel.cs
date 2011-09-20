using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Infrastructure.Domain;

namespace Luna.Shifts.Domain.Model
{
    public interface ISiteManagerModel
    {
        ICampaignSites[] GetAll();

        IEnumerable<IArea> GetAreaWithSeat(Entity site);

        IEnumerable<IArea> GetAll(Entity campaign);


        void ReloadCampaignSite(ISeatingSite<Entity> item);

        void PersistentSeat(Seat seat);

        void PersistentSite(Entity site);

        void RebuildSeatsPriority(Area area);
      
        void SaveAsNew(Seat seat);

        bool RemoveSeat(Seat seat, Action<Seat> removeRelation);

        void Save(Area area);

        Core.Tuple<IEnumerable<Site>, IEnumerable<Campaign>> GetAllSitesAndCampains();

        //ISeatingSite<IAreaOrganizations> GetSiteForPrioritySetup(Entity site, Entity campaign);

        //void ReloadSeatingSiteSeats(ISeatingSite<IAreaOrganizations> site);

        Area ReloadWithSeat(Area area);

        //void SubmitChanges();

        void ReCalculate(IEnumerable<ISeat> seats, int oldColumn, int newColumn);

        void Abort();


        void SubmitChanges();
    }
}
