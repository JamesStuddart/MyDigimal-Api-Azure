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
using MyDigimal.Core.Serialization;
using MyDigimal.Data;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers;

public class ReferenceDataTrigger (
    IConfiguration configuration, 
    IUnitOfWork unitOfWork,
    ILogger<CreatureTrigger> logger,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    
    [Function("GetCreatureReferenceData")]
    public async Task<HttpResponseData> GetCreatureReferenceData(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ref/creature")] HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var model = new
            {
                creatureStatuses = default(CreatureStatus).ToList(),
                creatureSexes = default(Sex).ToList(),
                feedingCadenceType = default(FeedingCadenceType).ToList()
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(model);
            return response;
        });
    }

    [Function("GetAuthenticationReferenceData")]
    public async Task<HttpResponseData> GetAuthenticationReferenceData(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ref/authentication")] HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var model = AppSettings.AvailableLoginTypes.ToDescriptiveList();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(model);
            return response;
        });
    }
}