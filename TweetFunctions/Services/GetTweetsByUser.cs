using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TweetFunctions.Services;
using System.Net;
using System.Text.Json;
using Azure;

namespace TweetFunctions
{
    public class GetTweetsByUser
    {
        [Function("GetTweetsByUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tweets/user/{userId}")] HttpRequestData req,
            string userId,
            FunctionContext context)
        {
            var logger = context.GetLogger("GetTweetsByUser");
            var service = new TweetService();

            try
            {
                var tweets = await service.GetTweetsByUserAsync(userId);

                var response = req.CreateResponse();

                if (tweets == null || !tweets.Any())
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    await response.WriteStringAsync("No tweets found for this user.");
                    return response;
                }

                var mapped = tweets.Select(tweet => new
                {
                    tweet.UserId,
                    tweet.Content,
                    tweet.MediaUrl,
                    tweet.Timestamp,
                    Visibility = tweet.Visibility ? "Public" : "Private"
                });

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteAsJsonAsync(mapped);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"🔥 Error in fallback function for userId={userId}: {ex.Message}");
                var response = req.CreateResponse();
                response.StatusCode = HttpStatusCode.InternalServerError;
                await response.WriteStringAsync("Internal error: " + ex.Message);
                return response;
            }
        }
    }
}
