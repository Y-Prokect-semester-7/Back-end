using Backend.Tests.TweetManagement.Interfaces;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TweetManagement.Models;
using TweetManagement.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace TweetManagement.Controllers
{
    [ApiController]
    [Route("api/tweetserver")]
    public class TweetServerController : ControllerBase
    {
        private readonly ITweetRepository _tweetRepository;

        private readonly ILogger<TweetServerController> _logger;

        private readonly IKafkaProducer _kafkaProducer;

        private readonly IBlobUploadService _uploadService;

        public TweetServerController(ITweetRepository tweetRepository, ILogger<TweetServerController> logger, IKafkaProducer kafkaProducert, IBlobUploadService uploadService)
        {
            _tweetRepository = tweetRepository;
            _logger = logger;
            _kafkaProducer = kafkaProducert;
            _uploadService = uploadService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTweet([FromForm] TweetRequest request)
        {
            string? mediaUrl = null;
            bool imageUploaded = false;
            try
            {
                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdFromToken != request.UserId)
                    return Forbid();

                if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 280)
                    return BadRequest("Invalid tweet content.");

                if (request.Image != null && request.Image.Length > 0)
                {
                    mediaUrl = await _uploadService.UploadImageAndGetSasUrlAsync(request.Image);
                    imageUploaded = true;
                    _logger.LogInformation("Image uploaded to blob: {mediaUrl}", mediaUrl);
                }

                var tweet = new TweetRequest
                {
                    UserId = request.UserId,
                    Content = request.Content,
                    MediaUrl = mediaUrl,
                    Visibility = request.Visibility
                };

                await _tweetRepository.AddTweetAsync(tweet);
                _logger.LogInformation("Tweet saved to database");

                var kafkaEvent = JsonSerializer.Serialize(new
                {
                    userId = tweet.UserId,
                    content = tweet.Content,
                    mediaUrl = tweet.MediaUrl,
                    timestamp = DateTime.UtcNow
                });

                //await _kafkaProducer.PublishAsync("tweets.posted", kafkaEvent);
                await _kafkaProducer.PublishAsync("tweets.posted", kafkaEvent);
                _logger.LogInformation("Kafka event published");

                return Ok(new { status = "Tweet created", tweet });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tweet creation failed");

                if (imageUploaded && mediaUrl != null)
                {
                    try
                    {
                        await _uploadService.DeleteImageAsync(mediaUrl);
                        _logger.LogWarning("Image rolled back (deleted from blob): {mediaUrl}", mediaUrl);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogError(deleteEx, "Failed to delete image from blob after failure");
                    }
                }

                return StatusCode(500, "Something went wrong while creating tweet");
            }
        }
    }
}

