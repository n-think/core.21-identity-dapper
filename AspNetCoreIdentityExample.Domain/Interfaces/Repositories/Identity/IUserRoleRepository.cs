using System.Collections.Generic;
using AspNetCoreIdentityExample.Domain.Entities.Identity;

namespace AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity
{
    public interface IUserRoleRepository
    {
        void Add(string UserId, string roleName);
        void Remove(string userId, string roleName);
        IEnumerable<string> GetRoleNamesByUserId(string userId);
        IEnumerable<User> GetUsersByRoleName(string roleName);
    }
}
