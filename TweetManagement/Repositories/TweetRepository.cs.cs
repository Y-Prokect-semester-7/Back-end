using Microsoft.Extensions.Options;
using TweetManagement.Models;
using MongoDB.Driver;

namespace TweetManagement.Repositories
{
    public class TweetRepository : ITweetRepository
    {
        private readonly IMongoCollection<TweetRequest> _tweets;

        public TweetRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _tweets = database.GetCollection<TweetRequest>("tweets");
        }

        public async Task AddTweetAsync(TweetRequest tweet)
        {
            await _tweets.InsertOneAsync(tweet);
        }
    }
}
