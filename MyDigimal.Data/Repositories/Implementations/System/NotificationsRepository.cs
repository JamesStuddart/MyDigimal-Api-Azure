using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities.System;

namespace MyDigimal.Data.Repositories.Implementations.System
{
    public class NotificationsRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<NotificationEntity>(context, encryptor, "notifications")
    {
        public async Task<IEnumerable<NotificationEntity>> GetByUserId(Guid userId, bool unreadOnly = true)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(NotificationEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var includeRead = unreadOnly ? " dateread IS NULL AND " : string.Empty;
            
            var results = await QueryAsync($"{selectQuery} FROM {TableName} WHERE {includeRead}recipient = @userId ORDER BY created DESC", new { userId });

            return results;
        }
    }
}