using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDigimal.Data.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task InsertAsync(TEntity entity, bool? includeReturnId = true);
        Task<TEntity> InsertAndReturnAsync(TEntity entity, bool? includeReturnId = true);
        Task UpdateAsync(TEntity entity);

        Task<IEnumerable<TEntity>> QueryAsync(string query, object param = null);
        Task<TEntity> GetByIdAsync<TId>(TId id);

        Task DeleteAsync<TId>(TId id);
    }
}