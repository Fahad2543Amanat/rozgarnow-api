using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RozgarNowAPIs.Models;
using RozgarNowAPIs.Services;

namespace RozgarNowAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly MongoDbService _mongo;

        public JobController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        // ================= CREATE JOB =================
        [HttpPost("create")]
        public async Task<IActionResult> CreateJob([FromBody] JobModel job)
        {
            try
            {
                if (job == null)
                    return BadRequest("Job data is required");

                if (string.IsNullOrEmpty(job.ClientId))
                    return BadRequest("ClientId required");

                if (string.IsNullOrEmpty(job.Title) || string.IsNullOrEmpty(job.Description))
                    return BadRequest("Title and Description required");

                await _mongo.Jobs.InsertOneAsync(job);

                return Ok(new
                {
                    message = "Job created successfully",
                    job
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating job",
                    error = ex.Message
                });
            }
        }

        // ================= GET ALL JOBS =================
        [HttpGet("all")]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var jobs = await _mongo.Jobs.Find(_ => true).ToListAsync();

                return Ok(new
                {
                    message = "Jobs fetched successfully",
                    data = jobs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching jobs",
                    error = ex.Message
                });
            }
        }

        // ================= GET CLIENT JOBS =================
        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientJobs(string clientId)
        {
            try
            {
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId required");

                var jobs = await _mongo.Jobs
                    .Find(x => x.ClientId == clientId)
                    .ToListAsync();

                return Ok(new
                {
                    message = "Client jobs fetched successfully",
                    data = jobs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching client jobs",
                    error = ex.Message
                });
            }
        }

        // ================= GET SINGLE JOB =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Job Id required");

                var job = await _mongo.Jobs
                    .Find(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (job == null)
                    return NotFound("Job not found");

                return Ok(new
                {
                    message = "Job fetched successfully",
                    data = job
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching job",
                    error = ex.Message
                });
            }
        }
    }
}