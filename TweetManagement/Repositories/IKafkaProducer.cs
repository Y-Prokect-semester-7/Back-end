using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Tests.TweetManagement.Interfaces
{
    public interface IKafkaProducer
    {
        Task PublishAsync(string topic, string message);
    }
}
