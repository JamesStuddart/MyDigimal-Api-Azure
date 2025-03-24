using MyDigimal.Data.Entities.CreatureLogs;
using MyDigimal.Common.Cryptography;

namespace MyDigimal.Data.Repositories.Implementations.CreatureLogs
{
    public class LogSchemaSuggestedSpeciesRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<LogSchemaSuggestedSpeciesEntity>(context, encryptor, "LogSchemaSuggestedSpecies");
}