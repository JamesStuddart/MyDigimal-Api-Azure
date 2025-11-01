using System.Net;
using MyDigimal.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Core.LogEntries;
using MyDigimal.Core.Serialization;
using MyDigimal.Data.Entities.CreatureLogs;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers;

public class LogTrigger(
    IConfiguration configuration,
    ILogger<LogTrigger> logger,
    IUnitOfWork unitOfWork,
    ILogEntryProvider logEntryProvider,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    [Function("CreateLogEntry")]
    public async Task<HttpResponseData> CreateLogEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "creature/{id}/log")]
        HttpRequestData req, Guid id)
    {
        return await ValidateUserRequestAsync<LogEntriesViewModel>(req, async logEntries =>
        {
            var userId = await GetUserId(req);
            var correlationId = Guid.NewGuid();

            var creature = await unitOfWork.Creatures.GetByIdAsync(id, userId);
            if (creature == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var linkEntryIds = (await unitOfWork.LogSchemaEntries
                .GetCreatureLinkEntriesAsync(creature.LogSchemaId)).Select(x => x.Id).ToList();

            var logs = logEntries.Entries.Select(x => new LogEntryEntity
            {
                CreatureId = x.CreatureId,
                LogSchemaEntryId = x.LogSchemaEntryId,
                Date = x.Date,
                Notes = x.Notes,
                Value = x.Value,
            });

            var inserted = new List<LogEntryEntity>();

            foreach (var entry in logs)
            {
                try
                {
                    entry.CreatureId = id;
                    entry.CorrelationId = correlationId;
                    entry.Owner = userId;
                    inserted.Add(await unitOfWork.LogEntries.InsertAndReturnAsync(entry));
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Unable to create log", entry);
                }
            }

            await unitOfWork.CommitAsync();

            var linkedId = logEntries.Entries.FirstOrDefault(x => linkEntryIds.Contains(x.LogSchemaEntryId))?.Value;
            if (!string.IsNullOrEmpty(linkedId))
            {
                foreach (var entry in logs)
                {
                    try
                    {
                        entry.Id = Guid.NewGuid();
                        entry.CreatureId = Guid.Parse(linkedId);
                        if (linkEntryIds.Contains(entry.LogSchemaEntryId))
                            entry.Value = id.ToString();

                        inserted.Add(await unitOfWork.LogEntries.InsertAndReturnAsync(entry));
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, "Unable to create linked log", entry);
                    }
                }

                await unitOfWork.CommitAsync();
            }

            return req.CreateResponse(HttpStatusCode.Accepted);
        });
    }
    
    [Function("DuplicateLogEntry")]
    public async Task<HttpResponseData> DuplicateLogEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "creature/{id}/log/duplicate/{entryId}")]
        HttpRequestData req, Guid id, Guid entryId)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var userId = await GetUserId(req);
            var correlationId = Guid.NewGuid();

            var creature = await unitOfWork.Creatures.GetByIdAsync(id, userId);
            if (creature == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var logView = await logEntryProvider.GetLatestLogEntryExtendedAsync(id, userId, entryId, null, entryId);
            if (logView?.Entries == null || !logView.Entries.Any())
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var latest = logView.Entries.First();
            var toInsert = new List<LogEntryEntity>();

            toInsert.Add(await unitOfWork.LogEntries.InsertAndReturnAsync(new LogEntryEntity
            {
                CreatureId = latest.CreatureId.Value,
                LogSchemaEntryId = latest.LogSchemaEntryId.Value,
                Date = DateTime.UtcNow,
                Notes = latest.Notes,
                Value = latest.Value,
                CorrelationId = correlationId,
                Owner = userId
            }));

            foreach (var entry in latest.LogEntries.Where(x => x.Value != null))
            {
                try
                {
                    toInsert.Add(await unitOfWork.LogEntries.InsertAndReturnAsync(new LogEntryEntity
                    {
                        CreatureId = id,
                        LogSchemaEntryId = entry.LogSchemaEntryId.Value,
                        Date = DateTime.UtcNow,
                        Notes = entry.Notes,
                        Value = entry.Value,
                        CorrelationId = correlationId,
                        Owner = userId
                    }));
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Unable to duplicate child log", entry);
                }
            }

            await unitOfWork.CommitAsync();
            return req.CreateResponse(HttpStatusCode.Accepted);
        });
    }

    [Function("GetLogEntry")]
    public async Task<HttpResponseData> GetLogEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "creature/{id}/log/-/{fromYear}/{toYear}")]
        HttpRequestData req,
        Guid id, int fromYear, int toYear)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
            await BuildLogResponse(req, id, await GetUserId(req), fromYear, toYear, null));
    }

    [Function("GetUserLogs")]
    public async Task<HttpResponseData> GetUserLogs(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "creature/-/log/-/{fromYear}/{toYear}")]
        HttpRequestData req,
        int fromYear, int toYear)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
            await BuildUserLogResponse(req, await GetUserId(req), fromYear, toYear, null));
    }

    [Function("GetSpecificLog")]
    public async Task<HttpResponseData> GetSpecificLog(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "creature/{id}/log/{logSchemaEntryId}/{fromYear}/{toYear}")]
        HttpRequestData req,
        Guid id, Guid logSchemaEntryId, int fromYear, int toYear)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
            await BuildLogResponse(req, id, await GetUserId(req), fromYear, toYear, logSchemaEntryId));
    }

    [Function("DeleteLogEntry")]
    public async Task<HttpResponseData> DeleteLogEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "creature/{id}/log/{entryId}")]
        HttpRequestData req,
        Guid id, Guid entryId)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var userId = await GetUserId(req);
            var creature = await unitOfWork.Creatures.GetByIdAsync(id, userId);
            if (creature == null || creature.Owner != userId)
                return req.CreateResponse(HttpStatusCode.Unauthorized);

            var logEntry = await unitOfWork.LogEntries.GetByIdAsync(entryId);
            var entries = (await unitOfWork.LogEntries.GetByCorrelationId(logEntry.CorrelationId)).ToList();

            if (entries.Any(x => x.Owner != userId))
                return req.CreateResponse(HttpStatusCode.Unauthorized);

            foreach (var entry in entries)
                await unitOfWork.LogEntries.DeleteAsync(entry.Id);

            await unitOfWork.CommitAsync();
            return req.CreateResponse(HttpStatusCode.Accepted);
        });
    }

    private async Task<HttpResponseData> BuildLogResponse(HttpRequestData req, Guid creatureId, Guid userId,
        int fromYear, int toYear, Guid? entryId)
    {
        try
        {
            var creature = await unitOfWork.Creatures.GetByIdAsync(creatureId, userId);
            if (creature == null || creature.Owner != userId)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var from = new DateTime(fromYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(toYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var logs = await unitOfWork.LogEntries.GetExtendedByCreatureIdAsync(creatureId, from, to, entryId);
            var years = await unitOfWork.LogEntries.GetDistinctYearsForExtendedByCreatureIdAsync(creatureId, entryId);

            var result = logEntryProvider.MapLogsToReturnModel(userId, logs, years);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error building creature log view", new { creatureId, fromYear, toYear });
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }

    private async Task<HttpResponseData> BuildUserLogResponse(HttpRequestData req, Guid userId, int fromYear,
        int toYear, Guid? entryId)
    {
        try
        {
            var from = new DateTime(fromYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(toYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var logs = await unitOfWork.LogEntries.GetExtendedByUserIdAsync(userId, from, to, entryId);
            var years = await unitOfWork.LogEntries.GetDistinctYearsForExtendedByUserIdAsync(userId, entryId);

            var result = logEntryProvider.MapLogsToReturnModel(userId, logs, years);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error building user log view", new { userId, fromYear, toYear });
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}