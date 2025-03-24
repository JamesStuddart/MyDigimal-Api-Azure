using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDigimal.Common;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Extensions;
using MyDigimal.Data.Entities;

namespace MyDigimal.Data.Repositories.Implementations
{
    public class UsersRepository(IDbContext context, IEncryptor encryptor)
        : BaseRepository<UserEntity>(context, encryptor, "Users")
    {
        public async Task<bool> IsValidUserAsync(string username, string password)
        {
            var result =
                await QueryAsync($"SELECT Id FROM {TableName} WHERE username = @username && password = @password",
                    new {username, password});
            return result.Any();
        }

        private UserEntity EncryptUser(UserEntity userEntity)
        {
            if (userEntity == null) return null;
            
            userEntity.Name = Encryptor.Encrypt(userEntity.Name);
            userEntity.Email = Encryptor.Encrypt(userEntity.Email);

            return userEntity;
        }

        private UserEntity DecryptUser(UserEntity userEntity)
        {
            if (userEntity == null) return null;
            
            userEntity.Name = Encryptor.Decrypt(userEntity.Name);
            userEntity.Email = Encryptor.Decrypt(userEntity.Email);

            return userEntity;
        }

        public new async Task UpdateAsync(UserEntity userEntity)
            => await base.UpdateAsync(EncryptUser(userEntity));

        public new async Task<IEnumerable<UserEntity>> QueryAsync(string query, object param = null)
            => (await base.QueryAsync(query, param)).Select(DecryptUser);

        public new async Task<UserEntity> GetByIdAsync<TId>(TId id)
            => DecryptUser(await base.GetByIdAsync(id));
        
        public async Task<UserEntity> GetByEmailAddressAsync(string email)
        {
            var selectQuery = new StringBuilder("SELECT ");

            var properties = typeof(UserEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var encryptedEmail = Encryptor.Encrypt(email.ToLower());
            
            var result = await QueryAsync($"{selectQuery} FROM {TableName} WHERE email = @email",
                new { email = encryptedEmail});

            return result.FirstOrDefault();
        }

        public async Task<UserEntity> GetByNameAsync(string name)
        {
            var selectQuery = new StringBuilder("SELECT ");

            var properties = typeof(UserEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var encryptedName = Encryptor.Encrypt(name);
            
            var result = await QueryAsync($"{selectQuery} FROM {TableName} WHERE name = @name",
                new {name = encryptedName});

            return result.FirstOrDefault();
        }

        public async Task<UserEntity> RegisterUserAsync(string emailAddress, string name, AccountStatusType accountStatusType = AccountStatusType.Active)
        {
            var user = await GetByEmailAddressAsync(emailAddress);

            if (user == null && await GetByNameAsync(name) == null)
            {
                user = new UserEntity { Email = emailAddress.ToLower(), Role = AccountRoleType.User, Name = name, AccountStatus = accountStatusType, PaymentPlan = PaymentPlanType.Monthly, AccountPlan = AccountPlanType.Free };

                await InsertAsync(EncryptUser(user));
            }

            return user;
        }
        
        public async Task<IEnumerable<UserEntity>> GetUsersAsync()
        {
            var selectQuery = new StringBuilder("SELECT ");
                
            var properties = typeof(UserEntity).GenerateListOfProperties();
            selectQuery.Append(string.Join(", ", properties.ToArray()));

            var users = await Context.QueryAsync<UserEntity>($"{selectQuery} FROM {TableName}");

            return users.Select(DecryptUser);
        }
    }
}