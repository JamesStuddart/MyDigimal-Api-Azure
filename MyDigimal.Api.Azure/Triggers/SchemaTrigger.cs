using System.Net;
using MyDigimal.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Core.Schemas;
using Newtonsoft.Json;


namespace MyDigimal.Api.Azure.Triggers;

public class SchemaTrigger(
    IConfiguration configuration,
    ILogger<CreatureTrigger> logger,
    IUnitOfWork unitOfWork,
    ILogSchemaFactory logSchemaFactory,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    [Function("GetSchemas")]
    public async Task<HttpResponseData> GetSchemas(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "schema")]
        HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var userId = await GetUserId(req);
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var includePublic = bool.TryParse(query["includePublic"], out var result) && result;

            var schemas = await unitOfWork.LogSchemas.GetAsync(userId, includePublic);
            await unitOfWork.AbortAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonConvert.SerializeObject(schemas));
            return response;
        });
    }

    [Function("GetSchemaById")]
    public async Task<HttpResponseData> GetSchemaById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "schema/{id}")]
        HttpRequestData req, Guid id)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            if (id == Guid.Empty)
            {
                logger.LogInformation("Invalid schema ID");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = await GetUserId(req);
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var includePublic = bool.TryParse(query["includePublic"], out var result) && result;

            var schema = await logSchemaFactory.BuildSchema(id, userId, includePublic);
            if (schema == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonConvert.SerializeObject(schema));
            return response;
        });
    }

    [Function("GetSchemaByIdAndCreature")]
    public async Task<HttpResponseData> GetSchemaByIdAndCreature(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "schema/{id}/creature/{creatureId}")]
        HttpRequestData req, Guid id, Guid creatureId)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            if (id == Guid.Empty)
            {
                logger.LogInformation("Invalid schema ID");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = await GetUserId(req);
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var includePublic = bool.TryParse(query["includePublic"], out var result) && result;

            var schema = await logSchemaFactory.BuildSchema(id, userId, includePublic, creatureId);
            if (schema == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonConvert.SerializeObject(schema));
            return response;
        });
    }

    [Function("GetGeneticsSchemaById")]
    public async Task<HttpResponseData> GetGeneticsSchemaById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "schema/{id}/genetics")]
        HttpRequestData req, Guid id)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            if (id == Guid.Empty)
            {
                logger.LogInformation("Invalid schema ID");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = await GetUserId(req);
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var includePublic = bool.TryParse(query["includePublic"], out var result) && result;

            var schema = await unitOfWork.LogSchemas.GetByIdAsync(id, userId, includePublic);
            await unitOfWork.AbortAsync();

            if (schema == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonConvert.SerializeObject(new
            {
                Genes = JsonConvert.DeserializeObject<IEnumerable<Gene>>(schema.Genes),
                Morphs = JsonConvert.DeserializeObject<IEnumerable<Morph>>(schema.Morphs)
            }));

            return response;
        });
    }
}