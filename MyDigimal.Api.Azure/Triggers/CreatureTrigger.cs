using System.Net;
using MyDigimal.Core.AccountPlans;
using MyDigimal.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Common;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Core.Serialization;
using MyDigimal.Data.Entities.Creatures;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers
{
    public class CreatureTrigger(
        IConfiguration configuration,
        ILogger<CreatureTrigger> logger,
        IUnitOfWork unitOfWork,
        IAccountPlanFactory accountPlanFactory,
        IOptions<Auth0Settings> auth0Settings,
        IOptions<AppSettings> appSettings)
        : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
    {
        [Function("GetCreatures")]
        public async Task<HttpResponseData> GetCreatures(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "creature")]
            HttpRequestData req)
        {
            return await ValidateUserRequestAsync<object>(req, async (_) =>
            {
                var userId = await GetUserId(req);
                var creatures = await unitOfWork.Creatures.GetByOwnerIdAsync(userId, includeArchived: false);
                await unitOfWork.AbortAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(creatures);
                return response;
            });
        }

        [Function("GetCreatureByShortCode")]
        public async Task<HttpResponseData> GetCreatureByShortCode(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "creature/sc/{shortCode}")]
            HttpRequestData req, string shortCode)
        {
            return await ValidatePublicRequestAsync<object>(req, async (_) =>
            {
                var creature = await unitOfWork.Creatures.GetByShortCodeAsync(shortCode, includeArchived: false);
                await unitOfWork.AbortAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new { creatureId = creature?.Id });
                return response;
            });
        }

        [Function("GetCreature")]
        public async Task<HttpResponseData> GetCreature(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "creature/{id}")]
            HttpRequestData req, Guid id)
        {
            return await ValidateUserRequestAsync<object>(req, async (_) =>
            {
                if (id == Guid.Empty) return req.CreateResponse(HttpStatusCode.BadRequest);

                var userId = await GetUserId(req);
                var creature = await unitOfWork.Creatures.GetByIdAsync(id, userId, includeArchived: false);
                await unitOfWork.AbortAsync();

                if (creature == null)
                    return req.CreateResponse(HttpStatusCode.NotFound);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(creature);
                return response;
            });
        }

        [Function("CreateCreature")]
        public async Task<HttpResponseData> CreateCreature(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "creatures")]
            HttpRequestData req)
        {
            return await ValidateUserRequestAsync<object>(req, async (_) =>
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var creature = JsonConvert.DeserializeObject<CreatureViewModel>(requestBody);

                if (!creature.IsValid)
                    return req.CreateResponse(HttpStatusCode.BadRequest);

                var userId = await GetUserId(req);
                creature.Owner = userId;

                if (creature.Id.HasValue && await unitOfWork.Creatures.GetByIdAsync(creature.Id.Value) != null)
                {
                    return req.CreateResponse(HttpStatusCode.Conflict);
                }

                var user = await unitOfWork.Users.GetByIdAsync(userId);
                var plan = accountPlanFactory.GetModel(user.AccountPlan);
                var currentDigimalCount = await unitOfWork.Creatures.GetCountByOwnerIdAsync(userId);

                if (plan.MaxCreatures != -1 && plan.MaxCreatures == currentDigimalCount)
                {
                    return req.CreateResponse(HttpStatusCode.Conflict);
                }

                try
                {
                    var result = await unitOfWork.Creatures.InsertAndReturnAsync(creature);
                    await unitOfWork.CommitAsync();

                    if (result.Id.HasValue)
                    {
                        await unitOfWork.CreatureEvents.InsertAsync(new CreatureEventEntity
                        {
                            CreatureId = result.Id.Value,
                            Event = (int)CreatureEventType.Created
                        }, false);
                        await unitOfWork.CommitAsync();
                    }

                    var response = req.CreateResponse(HttpStatusCode.Created);
                    response.Headers.Add("Location", $"{req.Url}{result.Id}");
                    await response.WriteAsJsonAsync(new { result.Id });
                    return response;
                }
                catch
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }

        [Function("UpdateCreature")]
        public async Task<HttpResponseData> UpdateCreature(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "creature/{id}")]
            HttpRequestData req, Guid id)
        {
            return await ValidateUserRequestAsync<CreatureViewModel>(req, async creature =>
            {
                if (!creature.Id.HasValue || id != creature.Id || !creature.IsValid)
                    return req.CreateResponse(HttpStatusCode.BadRequest);

                var userId = await GetUserId(req);
                var existingCreature = await unitOfWork.Creatures.GetByIdAsync(creature.Id.Value, userId);

                if (existingCreature == null)
                    return req.CreateResponse(HttpStatusCode.Conflict);

                try
                {
                    await unitOfWork.Creatures.UpdateAsync(creature);
                    await unitOfWork.CommitAsync();

                    return req.CreateResponse(HttpStatusCode.Created);
                }
                catch
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }

        [Function("DeleteCreature")]
        public async Task<HttpResponseData> DeleteCreature(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "creature/{id}")]
            HttpRequestData req, Guid id)
        {
            return await ValidateUserRequestAsync<object>(req, async (_) =>
            {
                if (id == Guid.Empty)
                    return req.CreateResponse(HttpStatusCode.BadRequest);

                var userId = await GetUserId(req);
                var creature = await unitOfWork.Creatures.GetByIdAsync(id, userId);

                if (creature == null || creature.Owner != userId)
                    return req.CreateResponse(HttpStatusCode.NotFound);

                creature.Status = CreatureStatus.Archived;

                await unitOfWork.Creatures.UpdateAsync(creature);
                await unitOfWork.CommitAsync();

                return req.CreateResponse(HttpStatusCode.OK);
            });
        }
    }
}