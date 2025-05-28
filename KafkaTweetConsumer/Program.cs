using Confluent.Kafka;

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    //BootstrapServers = "kafka:9092",// or "kafka:9092" if inside Docker
    GroupId = "tweet-consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
consumer.Subscribe("tweets.posted");

Console.WriteLine("📡 Listening for tweets on 'tweets.posted'...");

try
{
    while (true)
    {
        var result = consumer.Consume();
        Console.WriteLine($"🟢 Tweet: {result.Message.Value}");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("❌ Consumer stopped.");
    consumer.Close();
}
