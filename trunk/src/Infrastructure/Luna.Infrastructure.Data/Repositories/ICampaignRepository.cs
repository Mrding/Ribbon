using System;
using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface ICampaignRepository : IRepository<Campaign>
    {
        void MakePersistentWithSync(Action<Campaign, Dictionary<string, Organization>> updateWithOrgs, Campaign entity);
        IList<Campaign> GetAllCampaignWithOrganization();
    }
}
