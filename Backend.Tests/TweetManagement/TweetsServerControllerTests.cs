using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TweetManagement.Controllers;
using TweetManagement.Models;
using TweetManagement.Repositories;
using Xunit;

namespace TweetManagement.Tests.Controllers
{
    public class TweetsServerControllerTests
    {
        private readonly Mock<ITweetRepository> _mockTweetRepository;
        private readonly Mock<ILogger<TweetsServerController>> _mockLogger;
        private readonly TweetsServerController _controller;

        public TweetsServerControllerTests()
        {
            _mockTweetRepository = new Mock<ITweetRepository>();
            _mockLogger = new Mock<ILogger<TweetsServerController>>();

            _controller = new TweetsServerController(
                _mockTweetRepository.Object,
                _mockLogger.Object
            );

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };
        }

        [Fact]
        public async Task GetTweetsByUserAsync_UserHasTweets_ReturnsOkWithTweets()
        {
            // Arrange
            var userId = "12345";
            var tweets = new List<TweetModel>
            {
                new TweetModel
                {
                    UserId = userId,
                    Content = "Test tweet",
                    Timestamp = DateTime.UtcNow,
                    Visibility = true,
                    MediaUrl = "http://example.com/image.jpg"
                }
            };

            _mockTweetRepository.Setup(r => r.GetTweetsAsync(userId))
                .ReturnsAsync(tweets);

            // Act
            var result = await _controller.GetTweetsByUserAsync(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsAssignableFrom<List<TweetResponse>>(okResult.Value);
            Assert.Single(response);
            Assert.Equal("Test tweet", response[0].Content);
            Assert.Equal("Public", response[0].Visibility);
        }

        [Fact]
        public async Task GetTweetsByUserAsync_UserHasNoTweets_ReturnsNotFound()
        {
            // Arrange
            var userId = "";
            _mockTweetRepository.Setup(r => r.GetTweetsAsync(userId))
                .ReturnsAsync(new List<TweetModel>());

            // Act
            var result = await _controller.GetTweetsByUserAsync(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No tweets found for this user.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetTweetsByUserAsync_ExceptionThrown_ReturnsServerError()
        {
            // Arrange
            var userId = "";
            _mockTweetRepository.Setup(r => r.GetTweetsAsync(userId))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.GetTweetsByUserAsync(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
