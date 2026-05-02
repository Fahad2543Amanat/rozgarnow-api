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
        public string Role { get; set; }
        public string Phone { get; set; }

        // ================= CLIENT =================
        public string? Company { get; set; }
        public string? Category { get; set; }
        public string? Address { get; set; }

        // FILES (CLIENT)
        public string? LogoUrl { get; set; }
        public string? BusinessDocUrl { get; set; }
        public string? NtnUrl { get; set; }

        // ================= WORKER =================
        public string? Cnic { get; set; }
        public string? City { get; set; }

        public List<string>? Skills { get; set; }
        public string? Experience { get; set; }
        public string? Location { get; set; }
        public string? Radius { get; set; }

        // FILES (WORKER)
        public string? CnicFrontUrl { get; set; }
        public string? CnicBackUrl { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string VerificationStatus { get; set; } = "Pending";
    }
}