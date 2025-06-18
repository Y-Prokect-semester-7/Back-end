using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TweetFunctions.Services;

namespace TweetFunctions
{
    public static class GetTweetsByUser
    {
        [FunctionName("GetTweetsByUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tweets/user/{userId}")] HttpRequest req,
            string userId,
            ILogger log)
        {
            var service = new TweetService();
            var tweets = await service.GetTweetsByUserAsync(userId);

            if (tweets == null || !tweets.Any())
            {
                return new NotFoundObjectResult("No tweets found for this user.");
            }

            var response = tweets.Select(tweet => new
            {
                tweet.UserId,
                tweet.Content,
                tweet.MediaUrl,
                tweet.Timestamp,
                Visibility = tweet.Visibility ? "Public" : "Private"
            });

            return new OkObjectResult(response);
        }
    }
}
