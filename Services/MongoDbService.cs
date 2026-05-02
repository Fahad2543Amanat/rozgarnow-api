using MongoDB.Driver;
using RozgarNowAPIs.Models;

namespace RozgarNowAPIs.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _db;

        public MongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            _db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        }

        public IMongoCollection<UserModel> Users =>
            _db.GetCollection<UserModel>("Users");
    }
}