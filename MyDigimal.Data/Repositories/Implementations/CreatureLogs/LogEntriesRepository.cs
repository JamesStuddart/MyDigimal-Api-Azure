using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Data.Entities.CreatureLogs;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;

namespace MyDigimal.Data.Repositories.Implementations.CreatureLogs
{
    public class LogEntriesRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<LogEntryEntity>(context, encryptor, "LogEntries")
    {
        public async Task<IEnumerable<LogEntryEntityExtendedEntity>> GetExtendedByCreatureIdAsync(Guid id, DateTime? fromDateTime = null, DateTime? toDateTime = null, Guid? schemaEntryId = null, Guid? schemaId = null, Guid? parentLogSchemaEntryId = null)
        {
            var selectQuery = new StringBuilder("SELECT le.id, le.creatureId, le.LogSchemaEntryId, le.date, le.notes, le.value, lse.title, lse.type, le.correlationId, le.owner, (CASE WHEN lse.parentId IS NULL THEN lse.id ELSE lse.parentId END) AS parentLogSchemaEntryId, (CASE WHEN lse.parentid IS NULL THEN true ELSE false END) AS isParent");
            selectQuery.AppendLine($" FROM {TableName} AS le");
            selectQuery.AppendLine(" LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid");

            var includeDateTimeFilter = fromDateTime.HasValue || toDateTime.HasValue ? $"AND ({(fromDateTime.HasValue ? "date >= @fromDateTime" : string.Empty)} {(toDateTime.HasValue ? "AND date < @toDateTime" : string.Empty)})" : string.Empty;
            var includeSchemaEntryIdFilter = schemaEntryId.HasValue ? "AND (lse.parentid = @schemaEntryId OR lse.id = @schemaEntryId)" : string.Empty;
            var includeSchemaIdFilter = schemaId.HasValue ? "AND schemaId = @schemaId" : string.Empty;
            var includeParentLogSchemaEntryIdFilter = parentLogSchemaEntryId.HasValue ? "AND (CASE WHEN lse.parentId IS NULL THEN lse.id = @parentLogSchemaEntryId ELSE lse.parentId = @parentLogSchemaEntryId END)" : string.Empty;
            
            selectQuery.AppendLine($" WHERE le.creatureid = @id {includeDateTimeFilter} {includeSchemaEntryIdFilter} {includeSchemaIdFilter} {includeParentLogSchemaEntryIdFilter}");     
            
            return await Context.QueryAsync<LogEntryEntityExtendedEntity>($"{selectQuery}", new {id, fromDateTime, toDateTime, schemaEntryId, schemaId, parentLogSchemaEntryId});
        }
        public async Task<IEnumerable<LogEntryEntityExtendedEntity>> GetLatestExtendedByCreatureIdAsync(Guid id, Guid? schemaEntryId = null, Guid? schemaId = null, Guid? parentLogSchemaEntryId = null)
        {
            var selectQuery = new StringBuilder("SELECT le.id, le.creatureId, le.LogSchemaEntryId, le.date, le.notes, le.value, lse.title, lse.type, le.correlationId, le.owner, (CASE WHEN lse.parentId IS NULL THEN lse.id ELSE lse.parentId END) AS parentLogSchemaEntryId, (CASE WHEN lse.parentid IS NULL THEN true ELSE false END) AS isParent");
            selectQuery.AppendLine($" FROM {TableName} AS le");
            selectQuery.AppendLine(" LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid");

            var includeSchemaEntryIdFilter = schemaEntryId.HasValue ? "AND (lse.parentid = @schemaEntryId OR lse.id = @schemaEntryId)" : string.Empty;
            var includeSchemaIdFilter = schemaId.HasValue ? "AND schemaId = @schemaId" : string.Empty;
            var includeParentLogSchemaEntryIdFilter = parentLogSchemaEntryId.HasValue ? "AND (CASE WHEN lse.parentId IS NULL THEN lse.id = @parentLogSchemaEntryId ELSE lse.parentId = @parentLogSchemaEntryId END)" : string.Empty;

            var includeDateTimeSubQueryFilter = $"(SELECT date from (SELECT  DISTINCT  le.date  FROM LogEntries AS le LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid WHERE le.creatureid = @id {includeSchemaEntryIdFilter} {includeSchemaIdFilter} {includeParentLogSchemaEntryIdFilter} ORDER BY le.date DESC) as x LIMIT 1)";

            var includeDateTimeFilter = $"AND (date >= {includeDateTimeSubQueryFilter} AND date <= {includeDateTimeSubQueryFilter})";
                
            selectQuery.AppendLine($" WHERE le.creatureid = @id {includeDateTimeFilter} {includeSchemaEntryIdFilter} {includeSchemaIdFilter} {includeParentLogSchemaEntryIdFilter}");     
            
            return await Context.QueryAsync<LogEntryEntityExtendedEntity>($"{selectQuery}", new {id, schemaEntryId, schemaId, parentLogSchemaEntryId});
        }
        
        public async Task<IEnumerable<LogEntryEntityExtendedEntity>> GetExtendedByCreatureIdAndEntryIdAsync(Guid id, Guid entryId)
        {
            var selectQuery = new StringBuilder();
            
            selectQuery.AppendLine("SELECT le.id, le.creatureId, le.LogSchemaEntryId, le.date, le.notes, le.value, lse.title, lse.type, le.correlationId, le.owner, (CASE WHEN lse.parentId IS NULL THEN lse.id ELSE lse.parentId END) AS parentLogSchemaEntryId, (CASE WHEN lse.parentid IS NULL THEN true ELSE false END) AS isParent FROM LogEntries AS le");
            selectQuery.AppendLine("LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid");
            selectQuery.AppendLine("WHERE le.correlationid IN (SELECT le.correlationId FROM  LogEntries AS le WHERE le.creatureid = @id AND le.id = @entryId)");
      
            return await Context.QueryAsync<LogEntryEntityExtendedEntity>($"{selectQuery}", new {id, entryId});
        }

        public async Task<IEnumerable<int>> GetDistinctYearsForExtendedByCreatureIdAsync(Guid id, Guid? schemaEntryId = null)
        {
            var selectQuery = new StringBuilder("SELECT DISTINCT EXTRACT(YEAR FROM le.date) as year");
            selectQuery.AppendLine($" FROM {TableName} AS le");
            selectQuery.AppendLine(" LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid");
            selectQuery.AppendLine(" LEFT JOIN logschemaentries AS lsep ON lsep.id = lse.parentid");
            
            var includeSchemaEntryIdFilter = schemaEntryId.HasValue ? "AND (lse.parentid = @schemaEntryId OR lse.id = @schemaEntryId)" : string.Empty;
            
            selectQuery.AppendLine($" WHERE le.creatureid = @id {includeSchemaEntryIdFilter}");     
            
            return await Context.QueryAsync<int>($"{selectQuery}", new {id, schemaEntryId});
        }
        
        public async Task<IEnumerable<LogEntryEntityExtendedEntity>> GetExtendedByUserIdAsync(Guid userId, DateTime? fromDateTime, DateTime? toDateTime, Guid? schemaEntryId = null)
        {
            var selectQuery = new StringBuilder("SELECT le.id, le.creatureId, le.LogSchemaEntryId, le.date, le.notes, le.value, lse.title, lse.type, le.correlationId, (CASE WHEN lse.parentId IS NULL THEN lse.id ELSE lse.parentId END) AS parentLogSchemaEntryId, (CASE WHEN lse.parentid IS NULL THEN true ELSE false END) AS isParent");
            selectQuery.AppendLine($" FROM {TableName} AS le");
            selectQuery.AppendLine(" LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid");
            selectQuery.AppendLine(" LEFT JOIN creatures AS cre ON cre.id = le.creatureid");
            
            var includeDateTimeFilter = $"AND ({(fromDateTime.HasValue ? "date >= @fromDateTime" : string.Empty)} {(toDateTime.HasValue ? "AND date < @toDateTime" : string.Empty)})";
            var includeSchemaEntryIdFilter = schemaEntryId.HasValue ? "AND (lse.parentid = @schemaEntryId OR lse.id = @schemaEntryId)" : string.Empty;
      
            selectQuery.AppendLine($" WHERE cre.owner = @userId {includeDateTimeFilter} {includeSchemaEntryIdFilter}");     
            
            return await Context.QueryAsync<LogEntryEntityExtendedEntity>($"{selectQuery}", new {userId, fromDateTime, toDateTime});
        }
        
        public async Task<IEnumerable<int>> GetDistinctYearsForExtendedByUserIdAsync(Guid userId, Guid? schemaEntryId = null)
        {
            var selectQuery = new StringBuilder("SELECT DISTINCT EXTRACT(YEAR FROM le.date) as year");
            selectQuery.AppendLine($" FROM {TableName} AS le");
            selectQuery.AppendLine(" LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid");
            selectQuery.AppendLine(" LEFT JOIN logschemaentries AS lsep ON lsep.id = lse.parentid");
            selectQuery.AppendLine(" LEFT JOIN creatures AS cre ON cre.id = le.creatureid");
            
            var includeSchemaEntryIdFilter = schemaEntryId.HasValue ? "AND (lse.parentid = @schemaEntryId OR lse.id = @schemaEntryId)" : string.Empty;
            
            selectQuery.AppendLine($" WHERE cre.owner = @userId {includeSchemaEntryIdFilter}");     
            
            return await Context.QueryAsync<int>($"{selectQuery}", new {userId, schemaEntryId});
        }


        public async Task<IEnumerable<LogEntryEntity>> GetByCorrelationId(Guid correlationId)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogEntryEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            return await QueryAsync($"{selectQuery} FROM { TableName } WHERE correlationId = @correlationId",
                new { correlationId });
        }

        public async Task<IEnumerable<LogEntryEntity>> GetByOwnerIdAsync(Guid userId)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(LogEntryEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            return await QueryAsync($"{selectQuery} FROM { TableName } WHERE owner = @userId",
                new { userId });
        }
    }
}