using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RozgarNowAPIs.Models
{
    public class BidModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string JobId { get; set; }
        public string ClientId { get; set; }
        public string WorkerId { get; set; }

        public string BidAmount { get; set; }
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}