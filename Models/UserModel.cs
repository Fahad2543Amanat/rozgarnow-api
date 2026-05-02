using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace RozgarNowAPIs.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // ================= COMMON =================
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // client / worker

        public string Phone { get; set; }

        // ================= CLIENT FIELDS =================
        public string Company { get; set; }
        public string Category { get; set; }
        public string Address { get; set; }

        // ================= WORKER FIELDS =================
        public string Cnic { get; set; }
        public string City { get; set; }

        public List<string> Skills { get; set; }
        public string Experience { get; set; }
        public string Location { get; set; }
        public string Radius { get; set; }

        public string VerificationStatus { get; set; } = "Pending";
    }
}