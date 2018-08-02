using System;
using System.Data;
using System.Data.SqlClient;
using AspNetCoreIdentityExample.Data.Dapper.Repositories;
using AspNetCoreIdentityExample.Domain.Entities.Identity;
using AspNetCoreIdentityExample.Domain.Interfaces;
using AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity;

namespace AspNetCoreIdentityExample.Data.UoW
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        #region Fields
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IRoleRepository _roleRepository;
        private IRoleClaimRepository _roleClaimRepository;
        private IUserRepository _userRepository;
        private IUserClaimRepository _userClaimRepository;
        private IUserLoginRepository _userLoginRepository;
        private IRepository<UserToken, UserTokenKey> _userTokenRepository;
        private IUserRoleRepository _userRoleRepository;
        #endregion

        public DapperUnitOfWork(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        #region IUnitOfWork Members
        public IRoleRepository RoleRepository
        {
            get
            {
                return _roleRepository
                    ?? (_roleRepository = new RoleRepository(_transaction));
            }
        }

        public IRoleClaimRepository RoleClaimRepository
        {
            get
            {
                return _roleClaimRepository
                    ?? (_roleClaimRepository = new RoleClaimRepository(_transaction));
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository
                    ?? (_userRepository = new UserRepository(_transaction));
            }
        }

        public IUserClaimRepository UserClaimRepository
        {
            get
            {
                return _userClaimRepository
                    ?? (_userClaimRepository = new UserClaimRepository(_transaction));
            }
        }
        
        public IUserLoginRepository UserLoginRepository
        {
            get
            {
                return _userLoginRepository
                    ?? (_userLoginRepository = new UserLoginRepository(_transaction));
            }
        }

        public IRepository<UserToken, UserTokenKey> UserTokenRepository
        {
            get
            {
                return _userTokenRepository
                    ?? (_userTokenRepository = new UserTokenRepository(_transaction));
            }
        }

        public IUserRoleRepository UserRoleRepository
        {
            get
            {
                return _userRoleRepository
                    ?? (_userRoleRepository = new UserRoleRepository(_transaction));
            }
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                resetRepositories();
                _transaction = _connection.BeginTransaction();
            }
        }

        #endregion

        #region Private Methods
        private void resetRepositories()
        {
            _roleRepository = null;
            _roleClaimRepository = null;
            _userRepository = null;
            _userClaimRepository = null;
            _userLoginRepository = null;
            _userTokenRepository = null;
            _userRoleRepository = null;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EFUnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
