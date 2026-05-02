using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RozgarNowAPIs.Models
{
    public class JobModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // 🔥 OWNER (CLIENT WHO CREATED JOB)
        public string ClientId { get; set; }

        // ================= JOB FIELDS =================
        public string Title { get; set; }
        public string Description { get; set; }

        public string BudgetMin { get; set; }
        public string BudgetMax { get; set; }

        public string Deadline { get; set; }
        public string Time { get; set; }

        public string Skills { get; set; }
        public string Location { get; set; }

        public string Phone { get; set; }
        public string Category { get; set; }

        public string Priority { get; set; }
        public string Notes { get; set; }

        // ================= SYSTEM =================
        public string Status { get; set; } = "Pending";
        public int Progress { get; set; } = 0;

        public string CreatedAt { get; set; } = DateTime.Now.ToString("s");
    }
}