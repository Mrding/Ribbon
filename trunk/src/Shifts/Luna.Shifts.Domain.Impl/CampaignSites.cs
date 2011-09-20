using System.Collections.Generic;
using Luna.Common;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Domain.Impl
{
    public class CampaignSites : ICampaignSites
    {
        public Entity Campaign { get; set; }

        public IEnumerable<ICampaignSite> Sites { get; set; }

        public override string ToString()
        {
            return Campaign.Name.ToString();
        }

       
    }
}