using System.Collections.Generic;
using AspNetCoreIdentityExample.Domain.Entities.Identity;

namespace AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity
{
    public interface IRoleClaimRepository : IRepository<RoleClaim, int>
    {
        IEnumerable<RoleClaim> FindByRoleId(string roleId);
    }
}
