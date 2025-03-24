using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;

namespace MyDigimal.Data.Repositories
{
    public abstract class BaseRepository<TEntity>(IDbContext context, IEncryptor encryptor, string tableName)
        : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly IEncryptor Encryptor = encryptor;
        internal readonly IDbContext Context = context;
        protected readonly string TableName = tableName;

        public async Task InsertAsync(TEntity entity, bool? includeReturnId = true)
        {
            var insertQuery = GenerateInsertQuery(includeReturnId);
            await Context.ExecuteAsync(insertQuery, entity);
        }
        
        public async Task<TEntity> InsertAndReturnAsync(TEntity entity, bool? includeReturnId = true)
        {
            var insertQuery = GenerateInsertQuery(includeReturnId);
            var id = await Context.ExecuteScalarAsync(insertQuery, entity);
            return await GetByIdAsync(id);
        }

        internal string GenerateInsertQuery(bool? includeReturnId = true)
        {
            var insertQuery = new StringBuilder($"INSERT INTO {TableName} ");
            
            insertQuery.Append("(");

            var properties = typeof(TEntity).GenerateListOfProperties();
            
            var insertProps = new List<string>();
            
            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    insertProps.Add($"{property}");
                }
            });

            insertQuery.Append(string.Join(", ", insertProps.ToArray()));

            insertQuery
                .Append(") VALUES (");

            var insertValues = new List<string>();
            
            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    insertValues.Add($"@{property}");
                }
            });

            insertQuery.Append(string.Join(", ", insertValues.ToArray()));

            if (includeReturnId.HasValue && includeReturnId.Value)
            {
                insertQuery
                    .Append(") RETURNING id");
            }
            else
            {
                insertQuery
                    .Append(")");
            }

            return insertQuery.ToString();
        }
        
        public async Task UpdateAsync(TEntity entity)
        {
            var updateQuery = GenerateUpdateQuery();
            await Context.ExecuteAsync(updateQuery, entity);
        }

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {TableName} SET ");
            var properties =  typeof(TEntity).GenerateListOfProperties();

            var updateProps = new List<string>();
            
            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateProps.Add($"{property}=@{property}");
                }
            });

            updateQuery.Append(string.Join(", ", updateProps.ToArray()));
            
            updateQuery.Append(" WHERE id = @id");

            return updateQuery.ToString();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(string query, object param = null)
        {
            return await Context.QueryAsync<TEntity>(query, param);
        }
        
        public async Task<TEntity> GetByIdAsync<TId>(TId id)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(TEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var result = await Context.QueryAsync<TEntity>($"{selectQuery} FROM {TableName} WHERE id = @id", new { Id = id });
            return result.FirstOrDefault();
        }

        public async Task DeleteAsync<TId>(TId id)
        {
            await Context.ExecuteAsync($"DELETE FROM {TableName} WHERE id = @id", new { Id = id });
        }
    }
}