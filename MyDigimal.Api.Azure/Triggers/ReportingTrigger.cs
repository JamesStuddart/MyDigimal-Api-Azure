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
using MyDigimal.Core.LogEntries;
using MyDigimal.Core.Serialization;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers;

public class ReportingTrigger(
    IConfiguration configuration,
    ILogger<CreatureTrigger> logger,
    IUnitOfWork unitOfWork,
    IOptions<AppSettings> appSettings,
    IOptions<Auth0Settings> auth0Settings,
    ILogEntryProvider logEntryProvider)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    [Function("GetDueFeeds")]
    public async Task<HttpResponseData> GetDueFeeds(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "reporting/feeds/due")]
        HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var userId = await GetUserId(req);
            var report = (await unitOfWork.ReportingLogEntries.GetNextFeedsByOwnerIdAsync(userId)).ToList();

            foreach (var feed in report)
            {
                var latestEntry = await logEntryProvider.GetLatestLogEntryExtendedAsync(
                    feed.CreatureId,
                    userId,
                    feed.SchemaEntryId,
                    null,
                    feed.SchemaEntryId
                );
                feed.LogEntry = latestEntry.Entries.FirstOrDefault();
            }

            await unitOfWork.AbortAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(report.OrderByDescending(x => x.NextFeedDate));
            return response;
        });
    }
}