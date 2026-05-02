using MongoDB.Driver;
using RozgarNowAPIs.Models;
using System.Security.Authentication;

namespace RozgarNowAPIs.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _db;

        public MongoDbService(IConfiguration config)
        {
            var connectionString = config["MongoDB:ConnectionString"];

            var settings = MongoClientSettings.FromConnectionString(connectionString);

            // 🔥 PRODUCTION SAFE TLS CONFIG
            settings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            };

            // Optional stability boost (recommended for Azure)
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
            settings.ConnectTimeout = TimeSpan.FromSeconds(10);

            var client = new MongoClient(settings);

            _db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        }

        public IMongoCollection<UserModel> Users =>
            _db.GetCollection<UserModel>("Users");

        public IMongoCollection<JobModel> Jobs =>
         _db.GetCollection<JobModel>("Jobs");
    }
}