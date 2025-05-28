using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TweetManagement.Models;
using TweetManagement.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace TweetManagement.Controllers
{
    [ApiController]
    [Route("api/tweets")]
    public class TweetsServerController : ControllerBase
    {
        private readonly ITweetRepository _tweetRepository;

        private readonly ILogger<TweetsServerController> _logger;

        public TweetsServerController(ITweetRepository tweetRepository, ILogger<TweetsServerController> logger)
        {
            _tweetRepository = tweetRepository;
            _logger = logger;
        }

        // GET: api/tweets/user/{userId}
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<TweetResponse>>> GetTweetsByUserAsync(string userId)
        {
            try
            {
                var tweets = await _tweetRepository.GetTweetsAsync(userId);

                if (tweets == null || !tweets.Any())
                {
                    return NotFound("No tweets found for this user.");
                }


                // Convert to TweetResponse format
                var response = tweets.Select(tweet => new TweetResponse
                {
                    UserId = Guid.TryParse(tweet.UserId, out var guid) ? guid : Guid.Empty,
                    Content = tweet.Content,
                    MediaUrl = tweet.MediaUrl,
                    Timestamp = tweet.Timestamp,
                    Visibility = tweet.Visibility ? "Public" : "Private"
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get tweets for user {UserId}", userId);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}
