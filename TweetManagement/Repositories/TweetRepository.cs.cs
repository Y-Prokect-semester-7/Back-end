using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TweetManagement.Models;

namespace TweetManagement.Repositories
{
    public class TweetRepository : ITweetRepository
    {
        private readonly IMongoCollection<TweetModel> _tweets;

        public TweetRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _tweets = database.GetCollection<TweetModel>("tweets");
        }

        public async Task AddTweetAsync(TweetRequest request)
        {
            var tweet = new TweetModel
            {
                UserId = request.UserId,
                Content = request.Content,
                MediaUrl = request.MediaUrl,
                Visibility = request.Visibility,
                Timestamp = DateTime.UtcNow
            };

            await _tweets.InsertOneAsync(tweet);
        }

        public async Task<List<TweetModel>> GetTweetsAsync(string userId)
        {
            return await _tweets.Find(tweet => tweet.UserId == userId).ToListAsync();
        }
    }
}


