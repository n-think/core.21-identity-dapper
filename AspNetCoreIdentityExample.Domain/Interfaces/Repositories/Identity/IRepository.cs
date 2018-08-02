using System.Collections.Generic;

namespace AspNetCoreIdentityExample.Domain.Interfaces.Repositories.Identity
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        IEnumerable<TEntity> All();

        TEntity Find(TKey key);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TKey key);
    }
}
