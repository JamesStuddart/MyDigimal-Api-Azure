using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities.Creatures;

namespace MyDigimal.Data.Repositories.Implementations.Creatures
{
    public class CreatureEventsRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<CreatureEventEntity>(context, encryptor, "CreatureEvents")
    {
        public async Task<IEnumerable<CreatureEventEntity>> GetByCreatureIdAsync(Guid creatureId)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureEventEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var results = await QueryAsync($"{selectQuery} FROM { TableName } WHERE creatureid = @creatureId",
                new {creatureId});

            return results;
            
        } 
    }
}