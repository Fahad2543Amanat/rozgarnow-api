using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RozgarNowAPIs.Models;
using RozgarNowAPIs.Services;

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
                // 🔹 Get all jobs of this client
                var jobs = await _mongo.Jobs
                    .Find(x => x.ClientId == clientId)
                    .ToListAsync();

                var jobIds = jobs.Select(j => j.Id).ToList();

                // 🔹 Get bids for those jobs
                var bids = await _mongo.Bids
                    .Find(x => jobIds.Contains(x.JobId))
                    .ToListAsync();

                // 🔹 Get all users (workers)
                var users = await _mongo.Users
                    .Find(_ => true)
                    .ToListAsync();

                // 🔥 JOIN DATA (OLD LOGIC SAME — ONLY EXTENDED)
                var result = bids.Select(b => new
                {
                    b.Id,
                    b.JobId,
                    b.WorkerId,
                    b.BidAmount,
                    b.Status,
                    b.CreatedAt,

                    // ✅ Job Info (same as before)
                    Job = jobs.FirstOrDefault(j => j.Id == b.JobId),

                    // ✅ NEW: Worker Info
                    WorkerName = users.FirstOrDefault(u => u.Id == b.WorkerId)?.Name,
                    WorkerLocation = users.FirstOrDefault(u => u.Id == b.WorkerId)?.Location
                });

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
    }
}