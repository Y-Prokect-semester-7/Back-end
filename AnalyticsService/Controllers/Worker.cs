using System.Text.Json;
using Confluent.Kafka;
using AnalyticsService.Models;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "analytics-service",
            BootstrapServers = "kafka:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("tweets.posted");

        _logger.LogInformation("AnalyticsService is consuming...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var tweet = JsonSerializer.Deserialize<TweetPostedEvent>(result.Message.Value);

                _logger.LogInformation("Received tweet: {Content} by {UserId} at {Timestamp}", tweet.Content, tweet.UserId, tweet.Timestamp);

                // TODO: Save or analyze tweet (e.g., hashtag detection)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }
}
