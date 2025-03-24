using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MyDigimal.Core.Authentication.Models;
using Newtonsoft.Json;

namespace MyDigimal.Core.Authentication.SocialPlatforms
{
    public class Auth0Service : IAccountService
    {
        private readonly Auth0Settings _settings;

        public Auth0Service(IOptions<Auth0Settings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<UserResponse> LoginAsync(JwtSecurityToken token)
        {
            return await GetUserAsync(token);
        }

        public async Task<bool> ValidateASync(JwtSecurityToken token)
        {
            try
            {
                var openIdConfigurationEndpoint = $"{_settings.AuthorityEndpoint}.well-known/openid-configuration";
                var configurationManager =
                    new ConfigurationManager<OpenIdConnectConfiguration>(openIdConfigurationEndpoint,
                        new OpenIdConnectConfigurationRetriever());
                var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    RequireAudience = true,
                    ValidAudience = _settings.Audience,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

                var handler = new JwtSecurityTokenHandler();
                var user = handler.ValidateToken(token.RawData, validationParameters, out var validatedToken);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<UserResponse> GetUserAsync(JwtSecurityToken token)
        {
            var account = new UserResponse()
            {
                Email = token.Payload["email"]?.ToString(),
                FirstName = token.Payload["given_name"]?.ToString(),
                LastName = token.Payload["family_name"]?.ToString(),
                Picture = token.Payload["picture"]?.ToString()
            };

            return account;
        }
    }
}