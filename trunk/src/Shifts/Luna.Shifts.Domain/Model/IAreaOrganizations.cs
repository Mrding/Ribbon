using System.Collections.Generic;
using Luna.Common;

namespace Luna.Shifts.Domain.Model
{
    //public interface IAreaOrganizations : IIndexable, ISelectable
    //{
    //    ISeatingArea Area { get; }

    //    Entity Site { get; }

    //    /// <summary>
    //    /// SequentialEntity 
    //    /// </summary>
    //    IEnumerable Seats { get; set; }

    //    IEnumerable<OrganizationSeatingRule> Organizations { get; }

    //    void EndEdit();

    //}

    public interface ICampaignSite
    {
        Entity Campaign { get; set; }

        Entity Site { get; set; }

        //IEnumerable<Area> GetArea();
    }

    [IgnoreRegister]
    public interface ISeatingSite<T> : ICampaignSite
    {
     
        //IEnumerable<IAreaOrganizations> Areas { get; set; }

        IList<T> Areas { get; set; }
    }

    public interface ICampaignSites
    {
        Entity Campaign { get; set; }

        IEnumerable<ICampaignSite> Sites { get; set; }
    }
}
