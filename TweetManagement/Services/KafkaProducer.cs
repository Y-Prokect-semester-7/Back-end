using Backend.Tests.TweetManagement.Interfaces;
using Confluent.Kafka;

namespace TweetManagement.Services
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer()
        {
            var config = new ProducerConfig { BootstrapServers = "kafka:9092" };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync(string topic, string message)
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
    }
}
//using Backend.Tests.TweetManagement.Interfaces;
//using Confluent.Kafka;
//using Microsoft.Extensions.Configuration;

//namespace TweetManagement.Services
//{
//    public class KafkaProducer : IKafkaProducer
//    {
//        private readonly IProducer<Null, string> _producer;
//        private readonly string _topic;

//        public KafkaProducer(IConfiguration configuration)
//        {
//            var config = new ProducerConfig
//            {
//                BootstrapServers = configuration["Kafka:BootstrapServers"],
//                SecurityProtocol = SecurityProtocol.SaslSsl,
//                SaslMechanism = SaslMechanism.Plain,
//                SaslUsername = configuration["Kafka:SaslUsername"],
//                SaslPassword = configuration["Kafka:SaslPassword"]
//            };

//            _topic = configuration["Kafka:Topic"];
//            _producer = new ProducerBuilder<Null, string>(config).Build();
//        }

//        public async Task PublishAsync(string topic, string message)
//        {
//            // fallback to default topic if none provided
//            var useTopic = string.IsNullOrEmpty(topic) ? _topic : topic;

//            await _producer.ProduceAsync(useTopic, new Message<Null, string> { Value = message });
//        }
//    }
//}
