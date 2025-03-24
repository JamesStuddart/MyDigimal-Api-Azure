using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities.System;

namespace MyDigimal.Data.Repositories.Implementations.System
{
    public class NewsRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<NewsEntity>(context, encryptor, "SystemNews")
    {
        public async Task<IEnumerable<NewsEntity>> GetTopAsync(int topCount = 5)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(NewsEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var results = await QueryAsync($"{selectQuery} FROM {TableName} ORDER BY created DESC limit {topCount}");

            return results;
        }
    }
}