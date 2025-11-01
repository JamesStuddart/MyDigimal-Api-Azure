using System.Net;
using MyDigimal.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Data.Entities.Creatures;

namespace MyDigimal.Api.Azure.Triggers;

public class GroupsTrigger(
    IConfiguration configuration,
    ILogger<GroupsTrigger> logger,
    IUnitOfWork unitOfWork,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    [Function("GetGroups")]
    public async Task<HttpResponseData> GetGroups(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "groups")]
        HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async (_) =>
        {
            var userId = await GetUserId(req);
            var groups = await unitOfWork.CreatureGroups.GetByCreatedById(userId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                groups = groups.Select(x => new CreatureGroupViewModel { Id = x.Id, Name = x.Name })
            });

            return response;
        });
    }

    [Function("CreateGroup")]
    public async Task<HttpResponseData> CreateGroup(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "groups/{groupName}")]
        HttpRequestData req, string groupName)
    {
        return await ValidateUserRequestAsync<object>(req, async (_) =>
        {
            var userId = await GetUserId(req);
            var groups = await unitOfWork.CreatureGroups.GetByCreatedById(userId);

            if (groups.Any(x => x.Name == groupName))
            {
                logger.LogCritical("Unable to create group, already exists", new { name = groupName });
                var conflictResponse = req.CreateResponse(HttpStatusCode.Conflict);
                await conflictResponse.WriteAsJsonAsync(new { name = groupName });
                return conflictResponse;
            }

            var created = await unitOfWork.CreatureGroups.InsertAndReturnAsync(new CreatureGroupEntity
            {
                Name = groupName,
                CreatedBy = userId
            });

            if (created.Id == Guid.Empty)
            {
                logger.LogCritical("Failed to create group", created);
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            await unitOfWork.CommitAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
                { group = new CreatureGroupViewModel { Id = created.Id, Name = created.Name } });
            return response;
        });
    }

    [Function("DeleteGroup")]
    public async Task<HttpResponseData> DeleteGroup(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "groups/{groupName}")]
        HttpRequestData req, string groupName)
    {
        return await ValidateUserRequestAsync<object>(req, async (_) =>
        {
            var userId = await GetUserId(req);
            var groups = await unitOfWork.CreatureGroups.GetByCreatedById(userId);

            foreach (var group in groups.Where(x => x.Name == groupName))
            {
                await unitOfWork.CreatureGroups.DeleteAsync(group.Id);
            }

            await unitOfWork.CommitAsync();

            var updatedGroups = await unitOfWork.CreatureGroups.GetByCreatedById(userId);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                groups = updatedGroups.Select(x => new CreatureGroupViewModel { Id = x.Id, Name = x.Name })
            });

            return response;
        });
    }
}