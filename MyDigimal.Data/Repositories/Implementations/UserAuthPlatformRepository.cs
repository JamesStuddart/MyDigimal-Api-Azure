using System;
using System.Linq;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Data.Entities;

namespace MyDigimal.Data.Repositories.Implementations
{
    public class UserAuthPlatformRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<UserAuthPlatformEntity>(context, encryptor, "userauthplatform")
    {
        public new async Task InsertAsync(UserAuthPlatformEntity entity)
        {
            var insertQuery = GenerateInsertQuery(false);
            await Context.ExecuteAsync(insertQuery, entity);
        }
        
        public async Task<bool> IsValidUserAuthPlatformAsync(string email, string platform)
        {
            var encryptedEmail = Encryptor.Encrypt(email);
            try
            {
                var result =
                    await Context.QueryAsync<AvailablePlatforms>(
                        "SELECT uap.platform FROM public.users AS u LEFT JOIN userauthplatform AS uap ON uap.userid = u.id WHERE u.email = @email AND uap.platform = @platform;",
                        new {email = encryptedEmail, platform = platform});

                return result.Any();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private class AvailablePlatforms
        {
            public string Platform { get; set; }
        }
    }
}