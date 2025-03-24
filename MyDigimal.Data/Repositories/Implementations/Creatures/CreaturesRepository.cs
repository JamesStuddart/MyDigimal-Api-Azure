using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities.Creatures;
using MyDigimal.Common.Cryptography;

namespace MyDigimal.Data.Repositories.Implementations.Creatures
{
    public class CreaturesRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<CreatureEntity>(context, encryptor, "Creatures")
    {
        public async Task<CreatureEntity> GetByIdAsync(Guid id, Guid ownerId, bool includeArchived = false)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var includeArchivedSql = includeArchived ? string.Empty :  "AND status != -1 ";
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE id = @id AND owner = @ownerId {includeArchivedSql}",
                new {id, ownerId});

            return result?.FirstOrDefault();
        }
        
        public async Task<CreatureEntity> GetByShortCodeAsync(string shortCode, bool includeArchived = false)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var includeArchivedSql = includeArchived ? string.Empty :  "AND status != -1 ";
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE shortcode = @shortCode {includeArchivedSql}",
                new {shortCode});

            return result?.FirstOrDefault();
        }
        
        public async Task<CreatureEntity> GetByIdAsync(Guid id, bool includeArchived = false)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var includeArchivedSql = includeArchived ? string.Empty :  "AND status != -1 ";
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE id = @id {includeArchivedSql}",
                new {id});

            return result?.FirstOrDefault();
        }
        
        public async Task<IEnumerable<CreatureEntity>> GetByOwnerIdAsync(Guid ownerId, bool includeArchived = false)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var includeArchivedSql = includeArchived ? string.Empty : "AND status != -1 ";
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE owner = @ownerId {includeArchivedSql}",
                new {ownerId});

            return result.ToList();
        }
        
        public async Task<int> GetCountByOwnerIdAsync(Guid ownerId, bool includeArchived = false)
        {
            var selectQuery = new StringBuilder("SELECT COUNT(*) AS COUNT");
            
            var includeArchivedSql = includeArchived ? string.Empty : "AND status != -1 ";
            var result = await Context.QueryAsync<int>($"{selectQuery} FROM { TableName } WHERE owner = @ownerId {includeArchivedSql}",
                new {ownerId});
            
            return result.FirstOrDefault();
        }
        
        public async Task<CreatureEntity> GetByShortCodeAsync(Guid ownerId, bool includeArchived = false)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var includeArchivedSql = includeArchived ? string.Empty : "AND status != -1 ";
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE shortCode = @shortCode {includeArchivedSql}",
                new {ownerId});

            return result.FirstOrDefault();
        }
    }
}