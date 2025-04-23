using System;
using System.Linq;
using System.Threading.Tasks;
using MyDigimal.Common.Cryptography;
using MyDigimal.Data.Entities;

namespace MyDigimal.Data.Repositories.Implementations;

public class UserExternalAuthRepository(IDbContext context, IEncryptor encryptor)
    : BaseRepository<UserExternalAuthEntity>(context, encryptor, "userexternalauth")
{
    public new async Task InsertAsync(UserExternalAuthEntity entity)
    {
        var insertQuery = GenerateInsertQuery(false);
        await Context.ExecuteAsync(insertQuery, entity);
    }
        
    public async Task<bool> IsValidUserAuthAsync(string email, string providerKey)
    {
        var encryptedEmail = Encryptor.Encrypt(email);
        try
        {
            var result =
                await Context.QueryAsync<UserExternalAuth>(
                    "SELECT uae.userId as UserId, uea.providerkey as ProviderKey FROM public.users AS u LEFT JOIN userexternalauth AS uea ON uae.userid = u.id WHERE u.email = @email AND uae.providerkey = @providerkey;",
                    new {email = encryptedEmail, providerkey = providerKey});

            return result.Any();
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private class UserExternalAuth
    {
        public Guid UserId { get; set; }
        public string ProviderKey { get; set; }
    }

    public async Task<Guid?> GetByProviderKeyAsync(string providerKey)
    {
        try
        {
            var result =
                await Context.QueryAsync<UserExternalAuth>(
                    "SELECT uea.userId as UserId, uea.providerkey as ProviderKey FROM public.users AS u LEFT JOIN userexternalauth AS uea ON uea.userid = u.id WHERE uea.providerkey = @providerkey;",
                    new {providerkey = providerKey});

            return result.FirstOrDefault()?.UserId;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}