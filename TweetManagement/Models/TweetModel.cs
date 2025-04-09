using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TweetManagement.Models
{
    public class TweetModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("mediaUrl")]
        public string MediaUrl { get; set; }

        [BsonElement("visibility")]
        public bool Visibility { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
