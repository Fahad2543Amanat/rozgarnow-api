using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RozgarNowAPIs.Models;
using RozgarNowAPIs.Services;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace RozgarNowAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        private readonly MongoDbService _mongo;

        public BidController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        // ================= CREATE BID =================
        [HttpPost("create")]
        public async Task<IActionResult> CreateBid([FromBody] BidModel bid)
        {
            try
            {
                if (bid == null)
                    return BadRequest("Bid data required");

                if (string.IsNullOrEmpty(bid.JobId))
                    return BadRequest("JobId required");

                if (string.IsNullOrEmpty(bid.WorkerId))
                    return BadRequest("WorkerId required");

                if (string.IsNullOrWhiteSpace(bid.BidAmount))
                    return BadRequest("Bid amount required");

                bid.Status = "Pending";
                bid.CreatedAt = DateTime.UtcNow;

                await _mongo.Bids.InsertOneAsync(bid);

                return Ok(new
                {
                    message = "Bid submitted successfully",
                    bid
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating bid",
                    error = ex.Message
                });
            }
        }

        // ================= GET BIDS BY JOB =================
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetBidsByJob(string jobId)
        {
            try
            {
                var bids = await _mongo.Bids
                    .Find(x => x.JobId == jobId)
                    .ToListAsync();

                return Ok(new
                {
                    message = "Bids fetched",
                    data = bids
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching bids",
                    error = ex.Message
                });
            }
        }

        // ================= GET BIDS FOR CLIENT =================
        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientBids(string clientId)
        {
            try
            {
                // 🔹 Jobs of client
                var jobs = await _mongo.Jobs
                    .Find(x => x.ClientId == clientId)
                    .ToListAsync();

                var jobIds = jobs.Select(j => j.Id).ToList();

                // 🔹 Bids
                var bids = await _mongo.Bids
                    .Find(x => jobIds.Contains(x.JobId))
                    .ToListAsync();

                // 🔹 Users (workers)
                var users = await _mongo.Users
                    .Find(_ => true)
                    .ToListAsync();

                // 🔥 JOIN DATA
                var result = new List<object>();

                foreach (var b in bids)
                {
                    var job = jobs.FirstOrDefault(j => j.Id == b.JobId);
                    var worker = users.FirstOrDefault(u => u.Id == b.WorkerId);

                    // 🔥 CITY FROM LAT/LONG
                    var city = await GetCityFromCoordinates(worker?.Location);

                    result.Add(new
                    {
                        b.Id,
                        b.JobId,
                        b.WorkerId,
                        b.BidAmount,
                        b.Status,
                        b.CreatedAt,

                        Job = job,

                        WorkerName = worker?.Name,
                        WorkerLocation = worker?.Location,
                        WorkerCity = city   // ✅ NEW FIELD
                    });
                }

                return Ok(new
                {
                    message = "Client bids fetched",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching client bids",
                    error = ex.Message
                });
            }
        }

        // ================= GET WORKER BIDS =================
        [HttpGet("worker/{workerId}")]
        public async Task<IActionResult> GetWorkerBids(string workerId)
        {
            try
            {
                // 🔹 worker ki bids
                var bids = await _mongo.Bids
                    .Find(x => x.WorkerId == workerId)
                    .ToListAsync();

                // 🔹 jobs
                var jobs = await _mongo.Jobs
                    .Find(_ => true)
                    .ToListAsync();

                // 🔥 JOIN DATA
                var result = bids.Select(b => new
                {
                    b.Id,
                    b.JobId,
                    b.ClientId,
                    b.WorkerId,
                    b.BidAmount,
                    b.Status,
                    b.CreatedAt,

                    Job = jobs.FirstOrDefault(j => j.Id == b.JobId)
                });

                return Ok(new
                {
                    message = "Worker jobs fetched",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching worker jobs",
                    error = ex.Message
                });
            }
        }


        // ================= ACCEPT / REJECT =================
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateBidStatus(string id, [FromBody] string status)
        {
            try
            {
                var update = Builders<BidModel>.Update.Set(x => x.Status, status);

                await _mongo.Bids.UpdateOneAsync(x => x.Id == id, update);

                return Ok(new { message = "Bid status updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating bid",
                    error = ex.Message
                });
            }
        }

        // ================= 🔥 NEW: LAT/LONG → CITY =================
        private async Task<string> GetCityFromCoordinates(string coords)
        {
            try
            {
                if (string.IsNullOrEmpty(coords))
                    return "Unknown";

                var parts = coords.Split(',');
                if (parts.Length != 2)
                    return "Unknown";

                var lat = parts[0].Trim();
                var lng = parts[1].Trim();

                string apiKey = "da7ee24e572d4fed9e6aeec1472c6f97"; // 🔴 PUT YOUR KEY HERE

                var url = $"https://api.opencagedata.com/geocode/v1/json?q={lat}+{lng}&key={apiKey}";

                using var client = new HttpClient();
                var response = await client.GetStringAsync(url);

                var json = JObject.Parse(response);

                var components = json["results"]?[0]?["components"];

                return components?["city"]?.ToString()
                    ?? components?["town"]?.ToString()
                    ?? components?["state"]?.ToString()
                    ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    
}
}