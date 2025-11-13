using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Universe.Api.Data;
using Universe.Api.Models;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ GET: /users
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                var users = _context.Users.ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return Problem("An error occurred while fetching users.");
            }
        }

        // ✅ GET: /users/{id}
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Courses)
                    .Include(u => u.Notifications)
                    .Include(u => u.SentMessages)
                    .Include(u => u.ReceivedMessages)
                    .FirstOrDefault(u => u.UserID == id);

                if (user == null)
                    return NotFound(new { message = "User not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with ID {id}");
                return Problem("An error occurred while fetching the user.");
            }
        }

        // ✅ GET: /users/me
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                // Extract UserID from JWT claim
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Missing or invalid token claim." });

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "Token contains invalid user ID." });

                var user = _context.Users
                    .Include(u => u.Courses)
                    .Include(u => u.Notifications)
                    .Include(u => u.SentMessages)
                    .Include(u => u.ReceivedMessages)
                    .FirstOrDefault(u => u.UserID == userId);

                if (user == null)
                    return NotFound(new { message = "User not found." });

                var profile = new
                {
                    user.UserID,
                    user.Username,
                    user.Email,
                    user.Role,
                    user.FirstName,
                    user.LastName
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user profile");
                return Problem("An error occurred while retrieving your profile.");
            }
        }

        // ✅ GET: /users/students
        [HttpGet("students")]
        [Authorize]
        public IActionResult GetAllStudents()
        {
            try
            {
                var students = _context.Users
                    .Where(u => u.Role == "Student")
                    .Select(u => new
                    {
                        u.UserID,
                        u.Username,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.Role
                    })
                    .ToList();

                if (!students.Any())
                    return NotFound(new { message = "No students found." });

                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student list");
                return Problem("An error occurred while fetching students.");
            }
        }

        // ✅ POST: /users/device-token
        [HttpPost("device-token")]
        [Authorize]
        public IActionResult RegisterDeviceToken([FromBody] UniVerse.Models.Requests.DeviceTokenRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Token))
                    return BadRequest(new { message = "Device token cannot be empty." });

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "Invalid or missing token claim." });

                var user = _context.Users.FirstOrDefault(u => u.UserID == userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                user.DeviceToken = request.Token;
                _context.SaveChanges();

                _logger.LogInformation($"✅ Device token updated for user {user.Username} ({user.UserID})");

                return Ok(new { message = "Device token registered successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering device token");
                return Problem("An error occurred while registering device token.");
            }
        }
    }
}
