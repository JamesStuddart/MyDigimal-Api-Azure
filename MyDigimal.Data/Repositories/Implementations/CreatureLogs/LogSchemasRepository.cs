using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Data.Entities.CreatureLogs;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;

namespace MyDigimal.Data.Repositories.Implementations.CreatureLogs
{
    public class LogSchemasRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<LogSchemaEntity>(context, encryptor, "LogSchemas")
    {
        public async Task<IEnumerable<LogSchemaEntity>> GetAsync(Guid ownerId, bool includePublic = false)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogSchemaEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var includePublicSql = includePublic ? "OR ispublic = true " : string.Empty;
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE author = @ownerId {includePublicSql}",
                new {ownerId});

            return result.ToList();
        }
        
        public async Task<LogSchemaEntity> GetByIdAsync(Guid id, Guid ownerId, bool includePublic = false)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogSchemaEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var includePublicSql = includePublic ? " OR (id = @id AND ispublic = true)" : string.Empty;
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE (id = @id AND author = @ownerId){includePublicSql}",
                new {id, ownerId});

            return result?.FirstOrDefault();
        }
    }
}