using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Data;

namespace MyDigimal.Api.Azure.Triggers;

public class ExampleTrigger(
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    ILogger<ExampleTrigger> logger,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    // [Function("PublicEndpoint")]
    public async Task<HttpResponseData> PublicEndpoint(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "public")]
        HttpRequestData req)
    {
        return await ValidatePublicRequestAsync<object>(req, async (x) => req.CreateResponse());
    }

    // [Function("UserEndpoint")]
    public async Task<HttpResponseData> UserEndpoint(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user")]
        HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async (x) => req.CreateResponse());
    }

    // [Function("AdminEndpoint")]
    public async Task<HttpResponseData> AdminEndpoint(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "admin")]
        HttpRequestData req)
    {
        return await ValidateAdminRequestAsync<object>(req, async (x) => req.CreateResponse());
    }
}