using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetFunctions.Models;

namespace TweetFunctions.Services
{
    public class TweetService
    {
        private readonly IMongoCollection<Tweet> _tweets;

        public TweetService()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("MongoDbConnectionString"));
            var database = client.GetDatabase("TweetDatabase");
            _tweets = database.GetCollection<Tweet>("Tweets");
        }

        public async Task<List<Tweet>> GetTweetsByUserAsync(string userId)
        {
            return await _tweets.Find(t => t.UserId == userId).ToListAsync();
        }
    }
}
