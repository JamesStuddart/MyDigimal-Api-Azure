using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MyDigimal.Core.Authentication
{
    public interface IRequestValidator
    {
        Task<AuthenticateResult> AuthenticateRequestAsync(AuthenticationScheme scheme, HttpRequest request, bool checkDatabase = false);
    }
}