using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities.Creatures;

namespace MyDigimal.Data.Repositories.Implementations.Creatures
{
    public class CreatureGroupRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<CreatureGroupEntity>(context, encryptor, "CreatureGroups")
    {
        public async Task<IEnumerable<CreatureGroupEntity>> GetByCreatedById(Guid id)
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(CreatureGroupEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));
            
            var result = await QueryAsync($"{selectQuery} FROM { TableName } WHERE createdby = @id",
                new {id});

            return result;
        }
    }
}