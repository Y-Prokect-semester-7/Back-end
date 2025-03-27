namespace TweetManagement.Models
{
    public class TweetResponse
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime Timestamp { get; set; }
        public string Visibility { get; set; }
    }
}
