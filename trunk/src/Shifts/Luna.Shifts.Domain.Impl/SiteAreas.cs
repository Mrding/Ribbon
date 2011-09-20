using System.Collections.Generic;
using Luna.Common;
using Luna.Shifts.Domain.Model;

namespace Luna.Shifts.Domain.Impl
{


    //public class SiteAreas : ISeatingSite<IAreaOrganizations>
    //{
    //    public SiteAreas(IEnumerable<IAreaOrganizations> areas)
    //    {
    //        Areas = areas;
    //    }

    //    public Entity Campaign { get; set; }

    //    public Entity Site { get; set; }

    //    public IEnumerable<IAreaOrganizations> Areas { get; set; }
    //}

    public class CampaignSite : ISeatingSite<Entity>
    {
        public Entity Campaign { get; set; }

        public Entity Site { get; set; }

        public IList<Entity> Areas { get; set; }

        public override string ToString()
        {
            return Site.Name.ToString();
        }
        

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;

            var other = obj as CampaignSite;
            if (other == null) return false;

            return other.Campaign.Equals(Campaign) && other.Site.Equals(Site);

        }

       
    }


}