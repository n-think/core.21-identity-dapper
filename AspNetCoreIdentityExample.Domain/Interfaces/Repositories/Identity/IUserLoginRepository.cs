using System.Collections.Generic;
using AspNetCoreIdentityExample.Domain.Entities.Identity;

namespace AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity
{
    public interface IUserLoginRepository : IRepository<UserLogin, UserLoginKey>
    {
        IEnumerable<UserLogin> FindByUserId(string userId);
    }
}
