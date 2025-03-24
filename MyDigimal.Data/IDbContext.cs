using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDigimal.Data
{
    public interface IDbContext : IDisposable
    {
        Task SaveChangesAsync();
        Task AbortChangesAsync();
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, object param = null);
        Task ExecuteAsync<TEntity>(string query, TEntity entity) where TEntity : class;
        Task<object> ExecuteScalarAsync<TEntity>(string query, TEntity entity) where TEntity : class;
        Task<TEntity> QuerySingleOrDefaultAsync<TEntity>(string query, object param);
    }
}