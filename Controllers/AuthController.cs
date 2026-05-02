using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RozgarNowAPIs.Models;
using RozgarNowAPIs.Services;

namespace RozgarNowAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbService _mongo;

        public AuthController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        // ================= SIGNUP =================
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromForm] UserModel user, IFormFile? logo, IFormFile? businessDoc, IFormFile? ntn, IFormFile? profileImage)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Email and Password are required");

            var existingUser = await _mongo.Users
                .Find(x => x.Email == user.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
                return BadRequest("User already exists");

            // ================= FILE UPLOAD LOGIC =================
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string SaveFile(IFormFile file)
            {
                if (file == null) return null;

                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return fileName;
            }

            // CLIENT FILES
            user.LogoUrl = SaveFile(logo);
            user.BusinessDocUrl = SaveFile(businessDoc);
            user.NtnUrl = SaveFile(ntn);

            // WORKER FILES
            user.ProfileImageUrl = SaveFile(profileImage);

            user.VerificationStatus = "Pending";

            await _mongo.Users.InsertOneAsync(user);

            return Ok(new
            {
                message = "Account created successfully",
                user
            });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            // ✅ validation (PHONE + PASSWORD)
            if (string.IsNullOrEmpty(login.Phone) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Phone and Password required");

            // 🔥 login using phone
            var user = await _mongo.Users
                .Find(x => x.Phone == login.Phone && x.Password == login.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized("Invalid phone number or password");

            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    user.Phone
                }
            });
        }
    }
}