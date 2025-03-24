using System.Globalization;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Common;
using MyDigimal.Common.Extensions;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Data;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers;

public class UserTrigger(
    IConfiguration configuration,
    ILogger<CreatureTrigger> logger,
    IUnitOfWork unitOfWork,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    [Function("GetAllUsers")]
    public async Task<HttpResponseData> GetAllUsers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user")]
        HttpRequestData req)
    {
        return await ValidateAdminRequestAsync<object>(req, async _ =>
        {
            var users = await unitOfWork.Users.GetUsersAsync();

            var responseModel = users.Select(user => new
            {
                user.Name,
                Email = user.Email.MaskedEmail(),
                Avatar = string.IsNullOrWhiteSpace(user.Avatar) ? "/assets/digimals/light.jpg" : user.Avatar,
                AccountPlan = Enum.GetName(typeof(AccountPlanType), user.AccountPlan),
                AccountStatus = Enum.GetName(typeof(AccountStatusType), user.AccountStatus),
                PaymentPlan = Enum.GetName(typeof(PaymentPlanType), user.PaymentPlan),
                RenewalMonth = user.RenewalMonth.HasValue
                    ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(user.RenewalMonth.Value)
                    : "-",
                RenewalYear = user.RenewalYear?.ToString() ?? "-",
                Role = Enum.GetName(typeof(AccountRoleType), user.Role),
            });

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonConvert.SerializeObject(responseModel));
            return response;
        });
    }

    [Function("DisableAccount")]
    public async Task<HttpResponseData> DisableAccount(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "user")]
        HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var userId = await GetUserId(req);
            var user = await unitOfWork.Users.GetByIdAsync(userId);
            user.AccountStatus = AccountStatusType.DeactivatedByUser;

            await unitOfWork.Users.UpdateAsync(user);
            await unitOfWork.CommitAsync();

            return req.CreateResponse(HttpStatusCode.Accepted);
        });
    }
}