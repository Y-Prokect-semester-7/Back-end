using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetFunctions.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TweetFunctions.Services
{
    public class TweetService
    {
        private readonly IMongoCollection<Tweet> _tweets;

        public TweetService()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("ConnectionString"));
            var database = client.GetDatabase("TweetDb");
            _tweets = database.GetCollection<Tweet>("tweets");
        }

        public async Task<List<Tweet>> GetTweetsByUserAsync(string userId)
        {
            Console.WriteLine($"🔍 Searching for tweets of user: {userId}");

            var result = await _tweets.Find(t => t.UserId == userId).ToListAsync();

            Console.WriteLine($"✅ Found {result.Count} tweets");

            Console.WriteLine($"🧪 First tweet: {_tweets.Find(_ => true).FirstOrDefault()?.Content ?? "none"}");


            return result;
        }

    }
}
