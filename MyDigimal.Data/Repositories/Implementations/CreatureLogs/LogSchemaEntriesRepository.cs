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
    public class LogSchemaEntriesRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<LogSchemaEntryEntity>(context, encryptor, "LogSchemaEntries")
    {
        public async Task<IEnumerable<LogSchemaEntryEntity>> GetBySchemaIdAsync(Guid id)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogSchemaEntryEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE schemaid = @id",
                new { id });

            return result;
        }

        public async Task<IEnumerable<LogSchemaEntryEntity>> GetByParentEntryIdAsync(Guid id)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogSchemaEntryEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE parentid = @id",
                new { id });

            return result;
        }
        public async Task<IEnumerable<LogSchemaEntryEntity>> GetCreatureLinkEntriesAsync(Guid schemaId)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogSchemaEntryEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE schemaId = @schemaId AND type = 'CreatureLink'",
                new { schemaId });

            return result;
        }
        
        public async Task<IEnumerable<LogSchemaEntryEntity>> GetByParentEntryIdsAsync(IEnumerable<Guid> ids)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogSchemaEntryEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE parentid = ANY(@ids)",
                new { ids = ids.ToList() });

            return result;
        }
    }
}