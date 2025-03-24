using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using MyDigimal.Core.Authentication.Models;

namespace MyDigimal.Core.Authentication
{
    public interface IAccountService
    {
        Task<UserResponse> LoginAsync(JwtSecurityToken token);
        Task<bool> ValidateASync(JwtSecurityToken token);
    }
}