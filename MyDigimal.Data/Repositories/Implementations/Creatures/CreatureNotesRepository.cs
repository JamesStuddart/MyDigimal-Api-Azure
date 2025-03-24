using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities.Creatures;
using MyDigimal.Common.Cryptography;

namespace MyDigimal.Data.Repositories.Implementations.Creatures
{
    public class CreatureNotesRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<CreatureNoteEntity>(context, encryptor, "CreatureNotes")
    {
        public async Task<IEnumerable<CreatureNoteEntity>> GetByCreatureIdAsync(Guid creatureId)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var result = await QueryAsync($"{selectQuery} FROM {TableName} WHERE creatureId = @id",
                new { creatureId });
          
            if (result == null)
                throw new KeyNotFoundException($"CreatureNotes for the creature with id {creatureId} could not be found.");

            return result;
        }
    }
}