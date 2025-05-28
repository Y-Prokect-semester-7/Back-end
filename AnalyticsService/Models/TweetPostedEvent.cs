namespace AnalyticsService.Models
{
    public class TweetPostedEvent
    {
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
