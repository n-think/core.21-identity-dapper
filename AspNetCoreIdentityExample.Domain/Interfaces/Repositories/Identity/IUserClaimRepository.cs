using System.Collections.Generic;
using AspNetCoreIdentityExample.Domain.Entities.Identity;

namespace AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity
{
    public interface IUserClaimRepository : IRepository<UserClaim, int>
    {
        IEnumerable<UserClaim> GetByUserId(string userId);
        IEnumerable<User> GetUsersForClaim(string claimType, string claimValue);
    }
}
