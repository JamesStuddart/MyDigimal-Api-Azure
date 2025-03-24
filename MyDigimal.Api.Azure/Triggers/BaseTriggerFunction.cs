using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using MyDigimal.Data.Entities;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Common;
using MyDigimal.Common.Extensions;
using MyDigimal.Core.Authentication;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Data;

namespace MyDigimal.Api.Azure.Triggers;

public abstract class BaseTriggerFunction(
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    ILogger Logger,
    IOptions<AppSettings> appSettings,
    IOptions<Auth0Settings> auth0Settings)
{
    internal readonly AppSettings AppSettings = appSettings.Value;
    internal readonly Auth0Settings Auth0Settings = auth0Settings.Value;

    private async Task<UserEntity> GetUserAsync(string emailAddress)
    {
        //get this role from the user
        var user = await unitOfWork.Users.GetByEmailAddressAsync(emailAddress);
        await unitOfWork.AbortAsync();
        return user;
    }

    protected async Task<Guid> GetUserId(HttpRequestData req)
    {
        var token = req.Headers.GetValues("Authorization").FirstOrDefault();

        if (!string.IsNullOrEmpty(token))
        {
            var claims = await ValidateTokenAsync(token);
            return Guid.TryParse(claims.FindFirst("custom:user_id").Value, out var userId)
                ? userId
                : Guid.Empty;
        }

        return Guid.Empty;
    }

    private async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(Auth0Settings.ActionSecret ?? string.Empty);

        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{Auth0Settings.AuthorityEndpoint}.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever());
        var config = await configManager.GetConfigurationAsync();

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Auth0Settings.Issuer ?? string.Empty,
            ValidateAudience = true,
            ValidAudience = Auth0Settings.Audience ?? string.Empty,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = config.SigningKeys
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        //
        //         
        // var validatedToken = new ValidatedToken();
        // var jwtToken = token != null ? token.Replace("Bearer ", string.Empty) : null;
        //         
        // var handler = new JwtSecurityTokenHandler();
        //     `
        // if (!string.IsNullOrWhiteSpace(jwtToken) && handler.CanReadToken(jwtToken))
        // {
        //     var decodedToken = handler.ReadJwtToken(jwtToken);
        //
        //     if (!string.IsNullOrWhiteSpace(decodedToken.Issuer))
        //     {
        //         validatedToken.Platform = Enum<SocialPlatform>.GetByDescription(decodedToken.Issuer, SocialPlatform.Undefined);
        //         validatedToken.Token = jwtToken;
        //         validatedToken.DecodedToken = decodedToken;
        //         validatedToken.IsValid = true;
        //     }
        // }
        //
        if (validatedToken == null)
        {
            return null;
        }
        
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            throw new SecurityTokenException("Invalid token");
        }

        var user = await GetUserAsync(email);
        if (user == null)
        {
            throw new SecurityTokenException("User role not found");
        }

        var identity = new ClaimsIdentity(principal.Identity);
        identity.AddClaim(new Claim("custom:user_id", user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Role, ((int)user.Role).ToString()));
        return new ClaimsPrincipal(identity);
    }

    protected async Task<HttpResponseData> ValidateAdminRequestAsync<TRequest>(
        HttpRequestData request,
        Func<TRequest, Task<HttpResponseData>> action) where TRequest : class
        => await ValidateAsync(AccountRoleType.Public, request, action);

    protected async Task<HttpResponseData> ValidateUserRequestAsync<TRequest>(
        HttpRequestData request,
        Func<TRequest, Task<HttpResponseData>> action) where TRequest : class
        => await ValidateAsync(AccountRoleType.User, request, action);

    protected async Task<HttpResponseData> ValidatePublicRequestAsync<TRequest>(
        HttpRequestData request,
        Func<TRequest, Task<HttpResponseData>> action) where TRequest : class
        => await ValidateAsync(AccountRoleType.Admin, request, action);

    private async Task<HttpResponseData> ValidateAsync<TRequest>(
        AccountRoleType requiredRole,
        HttpRequestData request,
        Func<TRequest, Task<HttpResponseData>> action) where TRequest : class
    {
        var response = request.CreateResponse();

        try
        {
            var isAuthorized = await ValidateAuthorizationAsync(requiredRole, request, response);
            if (!isAuthorized) return response;

            var requestModel = await DeserializeRequestAsync<TRequest>(request);
            return await action(requestModel);
        }
        catch (Exception ex)
        {
            return HandleException(ex, response);
        }
    }

    private async Task<bool> ValidateAuthorizationAsync(
        AccountRoleType requiredRole,
        HttpRequestData request,
        HttpResponseData response)
    {
        if (requiredRole == AccountRoleType.Public) return true;

        var token = request.Headers.GetValues("Authorization").FirstOrDefault()?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            response.StatusCode = HttpStatusCode.Unauthorized;
            return false;
        }

        try
        {
            var principal = await ValidateTokenAsync(token);
            var userRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (userRole != ((int)requiredRole).ToString())
            {
                response.StatusCode = HttpStatusCode.Forbidden;
                return false;
            }

            return true;
        }
        catch (SecurityTokenException)
        {
            response.StatusCode = HttpStatusCode.Unauthorized;
            return false;
        }
    }

    private async Task<TRequest> DeserializeRequestAsync<TRequest>(HttpRequestData request) where TRequest : class
    {
        var requestModel = await request.ReadFromJsonAsync<TRequest>();

        if (requestModel == null)
        {
            Logger.LogCritical("EmptyRequest");
            throw new ArgumentNullException(nameof(requestModel), "Request body is empty.");
        }

        return requestModel;
    }

    private HttpResponseData HandleException(Exception ex, HttpResponseData response)
    {
        switch (ex)
        {
            case KeyNotFoundException _:
                Logger.LogWarning(ex, ex.Message ?? "No exception mesage found.");
                response.StatusCode = HttpStatusCode.NoContent;
                break;
            case UnauthorizedAccessException _:
                Logger.LogWarning(ex, ex.Message ?? "No exception mesage found.");
                response.StatusCode = HttpStatusCode.Forbidden;
                break;
            case ConstraintException _:
                Logger.LogWarning(ex, ex.Message ?? "No exception mesage found.");
                response.StatusCode = HttpStatusCode.Conflict;
                break;
            default:
                Logger.LogCritical(ex, ex.Message ?? "No exception mesage found.");
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Logger.LogCritical(ex, ex.Message ?? "No exception mesage found.");
                }

                response.StatusCode = HttpStatusCode.BadRequest;
                break;
        }

        return response;
    }

    public async Task<ValidatedToken> ValidateAsync(string token)
    {
        var response = new ValidatedToken();
        var jwtToken = token != null ? token.Replace("Bearer ", string.Empty) : null;

        var handler = new JwtSecurityTokenHandler();

        if (!string.IsNullOrWhiteSpace(jwtToken) && handler.CanReadToken(jwtToken))
        {
            var decodedToken = handler.ReadJwtToken(jwtToken);

            if (!string.IsNullOrWhiteSpace(decodedToken.Issuer))
            {
                response.Platform =
                    Enum<SocialPlatform>.GetByDescription(decodedToken.Issuer, SocialPlatform.Undefined);
                response.Token = jwtToken;
                response.DecodedToken = decodedToken;
                response.IsValid = true;
            }
        }

        return response;
    }
}