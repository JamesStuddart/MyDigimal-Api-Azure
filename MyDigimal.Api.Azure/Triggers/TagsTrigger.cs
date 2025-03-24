using System.Net;
using MyDigimal.Core.AccountPlans;
using MyDigimal.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Core.Authentication.Models;
using Newtonsoft.Json;


namespace MyDigimal.Api.Azure.Triggers;

public class TagsTrigger (
    IConfiguration configuration,
    ILogger<CreatureTrigger> logger,
    IUnitOfWork unitOfWork,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    [Function("GetUserTags")]
    public async Task<HttpResponseData> GetUserTags(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tags")] HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var userId = await GetUserId(req);
            var creatures = await unitOfWork.Creatures.GetByOwnerIdAsync(userId);

            var tags = creatures?
                .SelectMany(c => string.IsNullOrWhiteSpace(c.Tags)
                    ? []
                    : c.Tags.Split(',').Select(t => t.Trim()))
                .Distinct()
                .OrderBy(t => t);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonConvert.SerializeObject(new { tags = tags ?? Enumerable.Empty<string>() }));
            return response;
        });
    }
}