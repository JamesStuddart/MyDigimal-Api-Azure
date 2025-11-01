using System.Net;
using MyDigimal.Common;
using MyDigimal.Core.AccountPlans;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Api.Azure.Models.Authentication;
using MyDigimal.Data.Entities;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers
{
    public class AuthenticationTrigger(
        IConfiguration configuration,
        ILogger<AuthenticationTrigger> logger,
        IUnitOfWork unitOfWork,
        IAccountPlanFactory accountPlanFactory,
        IOptions<Auth0Settings> auth0Settings,
        IOptions<AppSettings> appSettings)
        : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
    {
        /// <summary>
        /// Public endpoint to check if authentication is required.
        /// </summary>
        [Function("IsAuthed")]
        public async Task<HttpResponseData> IsAuthed(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth")]
            HttpRequestData req)
        {
            return await ValidateUserRequestAsync<object>(req,
                _ => Task.FromResult(req.CreateResponse(HttpStatusCode.OK)));
        }

        /// <summary>
        /// Publicly accessible user registration.
        /// No authentication required.
        /// </summary>
        [Function("Register")]
        public async Task<HttpResponseData> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/register")]
            HttpRequestData req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<RegisterUserViewViewModel>(requestBody);

            if (request is not { IsValid: true })
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var authToken = req.Headers.Contains("Authorization")
                ? req.Headers.GetValues("Authorization").ToString()
                : string.Empty;
            if (string.IsNullOrWhiteSpace(authToken) || !authToken.Equals(Auth0Settings.ActionSecret))
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            var providerKey = string.Empty; // TODO: get provider key from token 

            var user = await unitOfWork.Users.GetByEmailAddressAsync(request.Email);

            if (user == null)
            {
                await unitOfWork.Users.RegisterUserAsync(request.Email, request.Username,
                    AccountStatusType.Preregister);
                await unitOfWork.CommitAsync();

                user = await unitOfWork.Users.GetByEmailAddressAsync(request.Email);
                await unitOfWork.UserAuthPlatforms.InsertAsync(new UserAuthPlatformEntity
                {
                    Platform = request.Platform,
                    UserId = user.Id
                });
                
                await unitOfWork.UserExternalAuth.InsertAsync(new UserExternalAuthEntity
                {
                    UserId = user.Id,
                    ProviderKey = providerKey,
                });

                await unitOfWork.CommitAsync();
            }

            var response = req.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Location", $"{req.Url}/{user.Id}");
            await response.WriteAsJsonAsync(user.Id.ToString());
            return response;
        }

        /// <summary>
        /// Secured endpoint to get user details.
        /// Requires authentication.
        /// </summary>
        [Function("GetUserDetails")]
        public async Task<HttpResponseData> GetDetails(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/details")]
            HttpRequestData req)
        {
            return await ValidateUserRequestAsync<UserViewModel>(req, async (request) =>
            {
                if (!request.IsValid)
                    return req.CreateResponse(HttpStatusCode.BadRequest);

                try
                {
                    var user = await unitOfWork.Users.GetByEmailAddressAsync(request.Email);
                    await unitOfWork.AbortAsync();

                    if (user == null)
                        return req.CreateResponse(HttpStatusCode.Unauthorized);

                    var userDetails = new
                    {
                        user.Name,
                        user.Avatar,
                        AccountPlan = Enum.GetName(typeof(AccountPlanType), user.AccountPlan),
                        AccountStatus = Enum.GetName(typeof(AccountStatusType), user.AccountStatus),
                        PaymentPlan = Enum.GetName(typeof(PaymentPlanType), user.PaymentPlan),
                        user.RenewalMonth,
                        user.RenewalYear,
                        Role = Enum.GetName(typeof(AccountRoleType), user.Role),
                        AccountPlanSettings = accountPlanFactory.GetModel(user.AccountPlan)
                    };

                    var response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteAsJsonAsync(userDetails);
                    return response;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error retrieving user details: {ex.Message}");
                    return req.CreateResponse(HttpStatusCode.Unauthorized);
                }
            });
        }
    }
}