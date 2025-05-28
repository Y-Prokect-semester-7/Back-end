using Backend.Tests.TweetManagement.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using TweetManagement.Controllers;
using TweetManagement.Models;
using TweetManagement.Repositories;

using Xunit;
//dotnet test --no-build

namespace TweetManagement.Tests.Controllers
{
    public class TweetServerControllerTests
    {
        private readonly Mock<ITweetRepository> _mockTweetRepository;
        private readonly Mock<ILogger<TweetServerController>> _mockLogger;
        private readonly Mock<IKafkaProducer> _mockKafkaProducer;
        private readonly TweetServerController _controller;

        public TweetServerControllerTests()
        {
            _mockTweetRepository = new Mock<ITweetRepository>();
            _mockLogger = new Mock<ILogger<TweetServerController>>();
            _mockKafkaProducer = new Mock<IKafkaProducer>();

            _controller = new TweetServerController(
                _mockTweetRepository.Object,
                _mockLogger.Object,
                _mockKafkaProducer.Object
            );
        }

        [Fact]
        public async Task CreateTweet_ValidRequest_ReturnsOk()
        {
            // Arrange
            var mockUserId = "12345";
            var mockContent = "This is a valid tweet.";
            var mockRequest = new TweetRequest
            {
                UserId = mockUserId,
                Content = mockContent,
                Visibility = true
            };

            _mockTweetRepository
                .Setup(repo => repo.AddTweetAsync(It.IsAny<TweetRequest>()))
                .Returns(Task.CompletedTask);

            _mockKafkaProducer
                .Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, mockUserId)
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _controller.CreateTweet(mockRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<TweetResponse>(json);

            Assert.NotNull(response);
            Assert.Equal("Tweet stored in MongoDB", response.status);
            Assert.Equal(mockUserId, response.userId);
            Assert.Equal(mockContent, response.content);
        }

        [Fact]
        public async Task CreateTweet_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Content", "Content is required.");

            var mockRequest = new TweetRequest
            {
                UserId = "12345",
                Content = "",
                Visibility = true
            };

            // Act
            var result = await _controller.CreateTweet(mockRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateTweet_UnauthorizedUser_ReturnsForbid()
        {
            // Arrange
            var mockRequest = new TweetRequest
            {
                UserId = "12345",
                Content = "This is a valid tweet.",
                Visibility = true
            };

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "67890") 
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _controller.CreateTweet(mockRequest);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CreateTweet_InvalidContent_ReturnsBadRequest()
        {
            // Arrange
            var mockRequest = new TweetRequest
            {
                UserId = "12345",
                Content = new string('a', 281), 
                Visibility = true
            };

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "12345")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _controller.CreateTweet(mockRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateTweet_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var mockRequest = new TweetRequest
            {
                UserId = "12345",
                Content = "This is a valid tweet.",
                Visibility = true
            };

            _mockTweetRepository.Setup(repo => repo.AddTweetAsync(It.IsAny<TweetRequest>()))
                .Throws(new Exception("Database error"));

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "12345")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _controller.CreateTweet(mockRequest);

            // Assert 
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        private class TweetResponse
        {
            public string status { get; set; }
            public string userId { get; set; }
            public string content { get; set; }
            public DateTime timestamp { get; set; }
        }
    }
}


