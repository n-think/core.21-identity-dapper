﻿using AspNetCoreIdentityExample.Domain;
using AspNetCoreIdentityExample.Domain.Entities.Identity;
using AspNetCoreIdentityExample.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreIdentityExample.Domain.Interfaces;

namespace AspNetCoreIdentityExample.Web.Identity
{
    public class CustomUserStore :
        IUserStore<IdentityUser>,
        IUserPasswordStore<IdentityUser>,
        IUserEmailStore<IdentityUser>,
        IUserLoginStore<IdentityUser>,
        IUserRoleStore<IdentityUser>,
        IUserSecurityStampStore<IdentityUser>,
        IUserClaimStore<IdentityUser>,
        IUserAuthenticationTokenStore<IdentityUser>,
        IUserTwoFactorStore<IdentityUser>,
        IUserPhoneNumberStore<IdentityUser>,
        IUserLockoutStore<IdentityUser>,
        IQueryableUserStore<IdentityUser>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomUserStore(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IQueryableUserStore<IdentityUser> Members
        public IQueryable<IdentityUser> Users
        {
            get
            {
                return _unitOfWork.UserRepository.All()
                    .Select(x => getApplicationUser(x))
                    .AsQueryable();
            }
        }
        #endregion

        #region IUserStore<IdentityUser> Members
        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                var userEntity = getUserEntity(user);

                _unitOfWork.UserRepository.Add(userEntity);
                _unitOfWork.Commit();

                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message }));
            }
        }

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                _unitOfWork.UserRepository.Remove(user.Id);
                _unitOfWork.Commit();

                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message }));
            }
        }

        public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            if (!Guid.TryParse(userId, out Guid id))
                throw new ArgumentOutOfRangeException(nameof(userId), $"{nameof(userId)} is not a valid GUID");

            var userEntity = _unitOfWork.UserRepository.Find(id.ToString());

            return Task.FromResult(getApplicationUser(userEntity));
        }

        public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            var userEntity = _unitOfWork.UserRepository.FindByNormalizedUserName(normalizedUserName);

            return Task.FromResult(getApplicationUser(userEntity));
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;

            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                var userEntity = getUserEntity(user);

                _unitOfWork.UserRepository.Update(userEntity);
                _unitOfWork.Commit();

                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = ex.Message, Description = ex.Message }));
            }
        }

        public void Dispose()
        {
            // Lifetimes of dependencies are managed by the IoC container, so disposal here is unnecessary.
        }
        #endregion

        #region IUserPasswordStore<IdentityUser> Members
        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }
        #endregion

        #region IUserEmailStore<IdentityUser> Members
        public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = email;

            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

            var userEntity = _unitOfWork.UserRepository.FindByNormalizedEmail(normalizedEmail);

            return Task.FromResult(getApplicationUser(userEntity));
        }

        public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }
        #endregion

        #region IUserLoginStore<IdentityUser> Members
        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            if (string.IsNullOrWhiteSpace(login.LoginProvider))
                throw new ArgumentNullException(nameof(login.LoginProvider));

            if (string.IsNullOrWhiteSpace(login.ProviderKey))
                throw new ArgumentNullException(nameof(login.ProviderKey));

            var loginEntity = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey,
                UserId = user.Id
            };

            _unitOfWork.UserLoginRepository.Add(loginEntity);
            _unitOfWork.Commit();

            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(IdentityUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            _unitOfWork.UserLoginRepository.Remove(new UserLoginKey { LoginProvider = loginProvider, ProviderKey = providerKey });
            _unitOfWork.Commit();
            
            return Task.CompletedTask;
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<UserLoginInfo> result = _unitOfWork.UserLoginRepository.FindByUserId(user.Id)
                .Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
                .ToList();

            return Task.FromResult(result);
        }

        public Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            var loginEntity = _unitOfWork.UserLoginRepository.Find(new UserLoginKey { LoginProvider = loginProvider, ProviderKey = providerKey });
            if (loginEntity == null)
                return Task.FromResult(default(IdentityUser));

            var userEntity = _unitOfWork.UserRepository.Find(loginEntity.UserId);

            return Task.FromResult(getApplicationUser(userEntity));
        }
        #endregion

        #region IUserRoleStore<IdentityUser> Members
        public Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            _unitOfWork.UserRoleRepository.Add(user.Id, roleName);
            _unitOfWork.Commit();

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            _unitOfWork.UserRoleRepository.Remove(user.Id, roleName);

            _unitOfWork.Commit();

            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<string> result = _unitOfWork.UserRoleRepository.GetRoleNamesByUserId(user.Id)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var result = _unitOfWork.UserRoleRepository.GetRoleNamesByUserId(user.Id).Any(x => x == roleName);

            return Task.FromResult(result);
        }

        public Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            IList<IdentityUser> result = _unitOfWork.UserRoleRepository.GetUsersByRoleName(roleName)
                .Select(x => getApplicationUser(x))
                .ToList();

            return Task.FromResult(result);
        }
        #endregion

        #region IUserSecurityStampStore<IdentityUser> Members
        public Task SetSecurityStampAsync(IdentityUser user, string stamp, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }
        #endregion

        #region IUserClaimStore<IdentityUser> Members
        public Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<Claim> result = _unitOfWork.UserClaimRepository.GetByUserId(user.Id)
                .Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

            return Task.FromResult(result);
        }

        public Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            var claimEntities = claims.Select(x => getUserClaimEntity(x, user.Id));
            if(claimEntities.Count() > 0)
            {
                claimEntities.ToList().ForEach(claimEntity =>
                {
                    _unitOfWork.UserClaimRepository.Add(claimEntity);
                });

                _unitOfWork.Commit();
            }

            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            var claimEntity = _unitOfWork.UserClaimRepository.GetByUserId(user.Id)
                .SingleOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

            if(claimEntity != null)
            {
                claimEntity.ClaimType = newClaim.Type;
                claimEntity.ClaimValue = newClaim.Value;

                _unitOfWork.UserClaimRepository.Update(claimEntity);
                _unitOfWork.Commit();
            }

            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            var userClaimEntities = _unitOfWork.UserClaimRepository.GetByUserId(user.Id);
            if (claims.Count() > 0)
            {
                claims.ToList().ForEach(claim =>
                {
                    var userClaimEntity = userClaimEntities.SingleOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
                    _unitOfWork.UserClaimRepository.Remove(userClaimEntity.Id);
                });

                _unitOfWork.Commit();
            }

            return Task.CompletedTask;
        }

        public Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            IList<IdentityUser> result = _unitOfWork.UserClaimRepository.GetUsersForClaim(claim.Type, claim.Value).Select(x => getApplicationUser(x)).ToList();

            return Task.FromResult(result);
        }
        #endregion

        #region IUserAuthenticationTokenStore<IdentityUser> Members
        public Task SetTokenAsync(IdentityUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var userTokenEntity = new UserToken
            {
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                UserId = user.Id
            };

            _unitOfWork.UserTokenRepository.Add(userTokenEntity);
            _unitOfWork.Commit();

            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var userTokenEntity = _unitOfWork.UserTokenRepository.Find(new UserTokenKey { UserId = user.Id, LoginProvider = loginProvider, Name = name });
            if(userTokenEntity != null)
            {
                _unitOfWork.UserTokenRepository.Remove(userTokenEntity);
                _unitOfWork.Commit();
            }

            return Task.CompletedTask;
        }

        public Task<string> GetTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var userTokenEntity = _unitOfWork.UserTokenRepository.Find(new UserTokenKey { UserId = user.Id, LoginProvider = loginProvider, Name = name });

            return Task.FromResult(userTokenEntity?.Name);
        }
        #endregion

        #region IUserTwoFactorStore<IdentityUser> Members
        public Task SetTwoFactorEnabledAsync(IdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion

        #region IUserPhoneNumberStore<IdentityUser> Members
        public Task SetPhoneNumberAsync(IdentityUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }
        #endregion

        #region IUserLockoutStore<IdentityUser> Members
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnd = lockoutEnd;

            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(++user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(IdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }
        #endregion

        #region Private Methods
        private User getUserEntity(IdentityUser ApplicationUser)
        {
            if (ApplicationUser == null)
                return null;

            var result = new User();
            populateUserEntity(result, ApplicationUser);

            return result;
        }

        private void populateUserEntity(User entity, IdentityUser ApplicationUser)
        {
            entity.AccessFailedCount = ApplicationUser.AccessFailedCount;
            entity.ConcurrencyStamp = ApplicationUser.ConcurrencyStamp;
            entity.Email = ApplicationUser.Email;
            entity.EmailConfirmed = ApplicationUser.EmailConfirmed;
            entity.Id = ApplicationUser.Id;
            entity.LockoutEnabled = ApplicationUser.LockoutEnabled;
            entity.LockoutEnd = ApplicationUser.LockoutEnd;
            entity.NormalizedEmail = ApplicationUser.NormalizedEmail;
            entity.NormalizedUserName = ApplicationUser.NormalizedUserName;
            entity.PasswordHash = ApplicationUser.PasswordHash;
            entity.PhoneNumber = ApplicationUser.PhoneNumber;
            entity.PhoneNumberConfirmed = ApplicationUser.PhoneNumberConfirmed;
            entity.SecurityStamp = ApplicationUser.SecurityStamp;
            entity.TwoFactorEnabled = ApplicationUser.TwoFactorEnabled;
            entity.UserName = ApplicationUser.UserName;
        }

        private IdentityUser getApplicationUser(User entity)
        {
            if (entity == null)
                return null;

            var result = new IdentityUser();
            populateApplicationUser(result, entity);

            return result;
        }

        private void populateApplicationUser(IdentityUser ApplicationUser, User entity)
        {
            ApplicationUser.AccessFailedCount = entity.AccessFailedCount;
            ApplicationUser.ConcurrencyStamp = entity.ConcurrencyStamp;
            ApplicationUser.Email = entity.Email;
            ApplicationUser.EmailConfirmed = entity.EmailConfirmed;
            ApplicationUser.Id = entity.Id;
            ApplicationUser.LockoutEnabled = entity.LockoutEnabled;
            ApplicationUser.LockoutEnd = entity.LockoutEnd;
            ApplicationUser.NormalizedEmail = entity.NormalizedEmail;
            ApplicationUser.NormalizedUserName = entity.NormalizedUserName;
            ApplicationUser.PasswordHash = entity.PasswordHash;
            ApplicationUser.PhoneNumber = entity.PhoneNumber;
            ApplicationUser.PhoneNumberConfirmed = entity.PhoneNumberConfirmed;
            ApplicationUser.SecurityStamp = entity.SecurityStamp;
            ApplicationUser.TwoFactorEnabled = entity.TwoFactorEnabled;
            ApplicationUser.UserName = entity.UserName;
        }

        private UserClaim getUserClaimEntity(Claim value, string userId)
        {
            return value == null
                ? default(UserClaim)
                : new UserClaim
                {
                    ClaimType = value.Type,
                    ClaimValue = value.Value,
                    UserId = userId
                };
        }
        #endregion
    }
}
