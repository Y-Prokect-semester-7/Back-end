using System;

namespace TweetFunctions.Models
{
    public class Tweet
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Visibility { get; set; }
    }
}
