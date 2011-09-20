using System.Collections.Generic;
using Luna.Data;
using Luna.Infrastructure.Domain;

namespace Luna.Infrastructure.Data.Repositories
{
    public interface IOrganizationManagerRepository : IRepository<OrganizationManager>
    {
        IList<OrganizationManager> GetOrganizationManager(Organization organization);
    }
}
