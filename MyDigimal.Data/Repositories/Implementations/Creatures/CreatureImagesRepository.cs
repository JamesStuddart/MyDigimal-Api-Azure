using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities.Creatures;

namespace MyDigimal.Data.Repositories.Implementations.Creatures
{
    public class CreatureImagesRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<CreatureImageEntity>(context, encryptor, "CreatureImages")
    {
        public async Task<CreatureImageEntity> GetByIdAsync(Guid id)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureImageEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE creatureid = @id",
                new {id});

            return result?.FirstOrDefault();
        }
    }
}