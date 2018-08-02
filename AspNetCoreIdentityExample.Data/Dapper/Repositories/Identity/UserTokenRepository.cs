﻿using System.Collections.Generic;
using System.Data;
using AspNetCoreIdentityExample.Domain.Entities.Identity;
using AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity;

namespace AspNetCoreIdentityExample.Data.Dapper.Repositories
{
    internal class UserTokenRepository : RepositoryBase, IRepository<UserToken, UserTokenKey>
    {
        public UserTokenRepository(IDbTransaction transaction)
            : base(transaction)
        { }

        public void Add(UserToken entity)
        {
            Execute(
                sql: @"
                    INSERT INTO AspNetUserTokens(UserId, LoginProvider, [Name], Value)
                    VALUES(@UserId, @LoginProvider, @Name, @Value)",
                param: entity
            );
        }

        public IEnumerable<UserToken> All()
        {
            return Query<UserToken>(
                sql: "SELECT * FROM AspNetUserTokens"
            );
        }

        public UserToken Find(UserTokenKey key)
        {
            return QuerySingleOrDefault<UserToken>(
                sql: @"
                    SELECT * FROM AspNetUserTokens
                    WHERE UserId = @UserId AND LoginProvider = @LoginProvider
                        AND [Name] = @Name",
                param: key
            );
        }

        public void Remove(UserTokenKey key)
        {
            Execute(
                sql: @"
                    DELETE FROM AspNetUserTokens
                    WHERE UserId = @UserId AND LoginProvider = @LoginProvider
                        AND [Name] = @Name",
                param: key
            );
        }

        public void Update(UserToken entity)
        {
            Execute(
                sql: @"
                    UPDATE AspNetUserTokens SET Value = @Value
                    WHERE UserId = @UserId
                        AND LoginProvider = @LoginProvider
                        AND [Name] = @Name",
                param: entity
            );
        }
    }
}
