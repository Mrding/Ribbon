using System.Collections.Generic;
using System.Linq;
using Luna.Data;
using Luna.Infrastructure.Domain;
using Luna.Infrastructure.Data.Repositories;

namespace Luna.Infrastructure.Data.Impl.Repositories
{
    public class OrganizationManagerRepository : Repository<OrganizationManager>, IOrganizationManagerRepository
    {
        public IList<OrganizationManager> GetOrganizationManager(Organization organization)
        {
            return this.Where(o => o.Organization == organization).ToList();
        }
    }
}
