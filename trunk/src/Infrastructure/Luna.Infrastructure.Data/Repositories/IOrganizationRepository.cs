using System.Collections.Generic;
using Luna.Common;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface IOrganizationRepository : IRepository<Organization>
    {     
        IList<Organization> GetHierarchicalTree();

        Organization GetRootOrganization();

        void MakePersistentWithSync(Organization entity);

        IEnumerable<Organization> GetByCampaign(Entity campaign);

        void DeleteOrgAndManager(Organization organization);

        IList<Organization> GetLaborRuleOrganiztion(LaborRule laborRule);
    }
}
