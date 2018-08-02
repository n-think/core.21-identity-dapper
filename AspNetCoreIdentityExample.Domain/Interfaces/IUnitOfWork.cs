using System;
using AspNetCoreIdentityExample.Domain.Entities.Identity;
using AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity;

namespace AspNetCoreIdentityExample.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRoleRepository RoleRepository { get; }
        IRoleClaimRepository RoleClaimRepository { get; }
        IUserRepository UserRepository { get; }
        IUserClaimRepository UserClaimRepository { get; }
        IUserLoginRepository UserLoginRepository { get; }
        IRepository<UserToken, UserTokenKey> UserTokenRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }

        void Commit();
    }
}
