using AspNetCoreIdentityExample.Domain.Entities.Identity;

namespace AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity
{
    public interface IUserRepository : IRepository<User, string>
    {
        User FindByNormalizedUserName(string normalizedUserName);

        User FindByNormalizedEmail(string normalizedEmail);
    }
}
