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

        public TweetServerController(ITweetRepository tweetRepository, ILogger<TweetServerController> logger, IKafkaProducer kafkaProducert)
        {
            _tweetRepository = tweetRepository;
            _logger = logger;
            _kafkaProducer = kafkaProducert;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTweet([FromBody] TweetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdFromToken == null || userIdFromToken != request.UserId)
                {
                    _logger.LogWarning("Unauthorized attempt by {UserId}", userIdFromToken);
                    return Forbid();
                }

                if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 280)
                {
                    return BadRequest("Invalid tweet content.");
                }

                if (request.Content.Contains("$") || request.Content.Contains("{") || request.Content.Contains("}"))
                {
                    return BadRequest("Potentially unsafe content.");
                }

                await _tweetRepository.AddTweetAsync(request);

                var kafkaEvent = JsonSerializer.Serialize(new
                {
                    userId = request.UserId,
                    content = request.Content,
                    timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Preparing to send Kafka message: {Event}", kafkaEvent);

                await _kafkaProducer.PublishAsync("tweets.posted", kafkaEvent);

                _logger.LogInformation("Kafka message successfully sent to topic 'tweets.posted'");

                return Ok(new
                {
                    status = "Tweet stored in MongoDB",
                    userId = request.UserId,
                    content = request.Content,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create tweet");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}

//using Confluent.Kafka;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using System.Text.Json;
//using TweetManagement.Models;
//using TweetManagement.Repositories;
//using static System.Net.Mime.MediaTypeNames;
//using TweetManagement.Models;


//namespace TweetManagement.Controllers
//{
//    [ApiController]
//    [Route("api/tweetserver")]
//    public class TweetServerController : ControllerBase
//    {
//        private readonly ITweetRepository _tweetRepository;

//        private readonly ILogger<TweetServerController> _logger;

//        public TweetServerController(ITweetRepository tweetRepository, ILogger<TweetServerController> logger)
//        {
//            _tweetRepository = tweetRepository;
//            _logger = logger;
//        }

//        [Authorize]
//        [HttpPost]
//        public async Task<IActionResult> CreateTweet([FromBody] TweetRequest request)
//        {
//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//                if (userIdFromToken == null || userIdFromToken != request.UserId)
//                {
//                    _logger.LogWarning("Unauthorized attempt by {UserId}", userIdFromToken);
//                    return Forbid();
//                }

//                if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 280)
//                {
//                    return BadRequest("Invalid tweet content.");
//                }

//                if (request.Content.Contains("$") || request.Content.Contains("{") || request.Content.Contains("}"))
//                {
//                    return BadRequest("Potentially unsafe content.");
//                }

//                await _tweetRepository.AddTweetAsync(request);

//                var config = new ProducerConfig { BootstrapServers = "kafka:9092" };

//                using var producer = new ProducerBuilder<Null, string>(config).Build();

//                var kafkaEvent = JsonSerializer.Serialize(new
//                {
//                    userId = request.UserId,
//                    content = request.Content,
//                    timestamp = DateTime.UtcNow
//                });

//                await producer.ProduceAsync("tweets.posted", new Message<Null, string> { Value = kafkaEvent });

//                return Ok(new
//                {
//                    status = "Tweet stored in MongoDB",
//                    userId = request.UserId,
//                    content = request.Content,
//                    timestamp = DateTime.UtcNow
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to create tweet");
//                return StatusCode(500, "An unexpected error occurred. Please try again later.");
//            }
//        }
//    }
//}
