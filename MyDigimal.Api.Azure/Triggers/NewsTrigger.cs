using System.Net;
using MyDigimal.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Data.Entities.System;
using Newtonsoft.Json;

namespace MyDigimal.Api.Azure.Triggers;

public class NewsTrigger (IConfiguration configuration, 
    ILogger<CreatureTrigger> logger,
    IUnitOfWork unitOfWork,
    IOptions<Auth0Settings> auth0Settings,
    IOptions<AppSettings> appSettings)
    : BaseTriggerFunction(configuration, unitOfWork, logger, appSettings, auth0Settings)
{
    
        [Function("GetLatestNews")]
        public async Task<HttpResponseData> GetLatestNews(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "newsfeed")] HttpRequestData req)
        {
            var news = await unitOfWork.News.GetTopAsync();
            await unitOfWork.AbortAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(JsonConvert.SerializeObject(news.OrderByDescending(x => x.Created)));
            return response;
        }

        [Function("InsertNews")]
        public async Task<HttpResponseData> InsertNews(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "newsfeed")] HttpRequestData req)
        {
            return await ValidateAdminRequestAsync<NewsEntity>(req, async (news) =>
            {
                if (string.IsNullOrWhiteSpace(news.Title) || string.IsNullOrWhiteSpace(news.Description))
                {
                    logger.LogInformation("Invalid News", news);
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }

                var result = await unitOfWork.News.InsertAndReturnAsync(news);
                await unitOfWork.CommitAsync();

                var response = req.CreateResponse(HttpStatusCode.Created);
                response.Headers.Add("Location", $"{req.Url}{result.Id}");
                await response.WriteStringAsync(JsonConvert.SerializeObject(new { result.Id }));
                return response;
            });
        }

        [Function("DeleteNews")]
        public async Task<HttpResponseData> DeleteNews(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "newsfeed")] HttpRequestData req)
        {
            return await ValidateAdminRequestAsync<object>(req, async (_) =>
            {
                var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
                if (!Guid.TryParse(query["id"], out var id) || id == Guid.Empty)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }

                var existing = await unitOfWork.News.GetByIdAsync(id);
                if (existing == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                await unitOfWork.News.DeleteAsync(existing);
                await unitOfWork.CommitAsync();

                return req.CreateResponse(HttpStatusCode.OK);
            });
        }
}