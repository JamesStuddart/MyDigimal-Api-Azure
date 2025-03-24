using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Data.Entities.CreatureLogs.Reporting;

namespace MyDigimal.Data.Repositories.Implementations.CreatureLogs
{
    public class ReportingLogEntryRepository(IDbContext context)
    {

        public async Task<IEnumerable<DueFeedingEntity>> GetNextFeedsByOwnerIdAsync(Guid ownerId)
        { 
            var selectQuery = new StringBuilder();
            selectQuery.AppendLine("SELECT DISTINCT ON (creatureId) feeds.creatureId,");
            selectQuery.AppendLine("                                cr.name,");
            selectQuery.AppendLine("                                feeds.lastFeedDate,");
            selectQuery.AppendLine("                                cr.feedingcadence,");
            selectQuery.AppendLine("                                cr.feedingcadencetype,");
            selectQuery.AppendLine("                                schemaEntryId");
            selectQuery.AppendLine("FROM (");
            selectQuery.AppendLine("         SELECT le.creatureId, (le.date) AS lastFeedDate, lse.title, lse.id AS schemaEntryId");
            selectQuery.AppendLine("         FROM LogEntries AS le");
            selectQuery.AppendLine("                  LEFT JOIN logschemaentries AS lse ON lse.id = le.logschemaentryid");
            selectQuery.AppendLine("         WHERE le.owner = @ownerId");
            selectQuery.AppendLine("           AND title = 'Feeding'");
            selectQuery.AppendLine("         ORDER BY le.creatureId, le.date DESC");
            selectQuery.AppendLine("     ) AS feeds");
            selectQuery.AppendLine("         LEFT JOIN creatures AS cr ON cr.id = feeds.creatureId");
            selectQuery.AppendLine("WHERE cr.feedingcadence IS NOT NULL AND cr.owner = @ownerId AND cr.status != -1 AND cr.status != 9000;");
            
            
            var results = await context.QueryAsync<DueFeedingEntity>($"{selectQuery}", new {ownerId});


            return results.Where(x => x.NextFeedDate < DateTime.UtcNow.AddDays(1));
        }
    }
}