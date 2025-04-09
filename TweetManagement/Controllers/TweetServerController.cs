using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetManagement.Models;
using TweetManagement.Repositories;

namespace TweetManagement.Controllers
{
    [ApiController]
    [Route("api/tweetserver")]
    public class TweetServerController : ControllerBase
    {
        private readonly ITweetRepository _tweetRepository;

        public TweetServerController(ITweetRepository tweetRepository)
        {
            _tweetRepository = tweetRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTweet([FromBody] TweetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _tweetRepository.AddTweetAsync(request);

            return Ok(new
            {
                status = "Tweet stored in MongoDB",
                userId = request.UserId,
                content = request.Content,
                timestamp = DateTime.UtcNow
            });
        }
    }

    //[HttpPost]
    //public IActionResult CreateTweet([FromBody] TweetRequest request)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        Console.WriteLine("Model validation failed:");
    //        foreach (var entry in ModelState)
    //        {
    //            foreach (var error in entry.Value.Errors)
    //            {
    //                Console.WriteLine($" {entry.Key}: {error.ErrorMessage}");
    //            }
    //        }

    //        return BadRequest(ModelState);
    //    }

    //    return Ok(new { status = "Tweet accepted" });
    //}
}

