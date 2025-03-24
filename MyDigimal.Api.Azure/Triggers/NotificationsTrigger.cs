using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Common;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Core.Handlers;
using MyDigimal.Data;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers;

public class NotificationsTrigger(
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    ILogger<CreatureTrigger> logger,
    INotificationHandler notificationHandler,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    [Function("GetUserNotifications")]
    public async Task<HttpResponseData> GetUserNotifications(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "notifications")]
        HttpRequestData req)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            var userId = await GetUserId(req);
            var notifications = await notificationHandler.GetNotificationsByUserIdAsync(userId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(
                JsonConvert.SerializeObject(notifications.OrderByDescending(x => x.Created)));
            return response;
        });
    }

    [Function("MarkNotificationAsRead")]
    public async Task<HttpResponseData> MarkNotificationAsRead(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "notifications/{id}")]
        HttpRequestData req, Guid id)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            try
            {
                var userId = await GetUserId(req);
                await notificationHandler.MarkAsReadASync(id, userId);
            }
            catch (Exception ex)
            {
                logger.LogCritical("Unable to mark notification as read", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return req.CreateResponse(HttpStatusCode.OK);
        });
    }

    [Function("ProcessNotification")]
    public async Task<HttpResponseData> ProcessNotification(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "notifications/{id}/{processType}")]
        HttpRequestData req, Guid id, string processType)
    {
        return await ValidateUserRequestAsync<object>(req, async _ =>
        {
            try
            {
                var userId = await GetUserId(req);
                var process = Enum.TryParse<NotificationProcessingType>(processType, true, out var parsed)
                    ? parsed
                    : (NotificationProcessingType?)null;

                if (process == null)
                    return req.CreateResponse(HttpStatusCode.BadRequest);

                switch (process)
                {
                    case NotificationProcessingType.Accept:
                        await notificationHandler.AcceptNotificationAsync(id, userId);
                        return req.CreateResponse(HttpStatusCode.Accepted);
                    case NotificationProcessingType.Decline:
                        await notificationHandler.DeclineNotificationAsync(id, userId);
                        return req.CreateResponse(HttpStatusCode.Accepted);
                    default:
                        return req.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical("Unable to process notification", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        });
    }
}