using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universe.Api.Data;
using Universe.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace UniVerse.Controllers
{
    [ApiController]
    [Route("settings")]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /settings/me
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMySettings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var settings = _context.UserSetting
                .AsNoTracking()
                .FirstOrDefault(s => s.UserID == userId);

            if (settings == null)
            {
                // Create default settings and return them
                settings = new UserSettings
                {
                    UserID = userId,
                    PreferredLanguage = "English",
                    TimeZone = "UTC",
                    EmailNotifications = true,
                    PushNotifications = true,
                    InAppNotifications = true,
                    ProfilePublic = true,
                    ShareUsageData = false,
                    Theme = "light",
                    ItemsPerPage = 10
                };
                _context.UserSetting.Add(settings);
                _context.SaveChanges();
            }

            var dto = new SettingsDto
            {
                PreferredLanguage = settings.PreferredLanguage,
                TimeZone = settings.TimeZone,
                EmailNotifications = settings.EmailNotifications,
                PushNotifications = settings.PushNotifications,
                InAppNotifications = settings.InAppNotifications,
                ProfilePublic = settings.ProfilePublic,
                ShareUsageData = settings.ShareUsageData,
                Theme = settings.Theme,
                ItemsPerPage = settings.ItemsPerPage,

                // optional: include canonical user fields
                FirstName = settings.User?.FirstName,
                LastName = settings.User?.LastName,
                PhoneNumber = settings.User?.PhoneNumber
            };

            return Ok(dto);
        }

        // PUT: /settings/me
        [HttpPut("me")]
        [Authorize]
        public IActionResult UpdateMySettings([FromBody] SettingsDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var settings = _context.UserSetting.FirstOrDefault(s => s.UserID == userId);
            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);
            if (settings == null)
            {
                settings = new UserSettings { UserID = userId };
                _context.UserSetting.Add(settings);
            }

            // Update settings
            settings.PreferredLanguage = dto.PreferredLanguage ?? settings.PreferredLanguage;
            settings.TimeZone = dto.TimeZone ?? settings.TimeZone;
            settings.EmailNotifications = dto.EmailNotifications;
            settings.PushNotifications = dto.PushNotifications;
            settings.InAppNotifications = dto.InAppNotifications;
            settings.ProfilePublic = dto.ProfilePublic;
            settings.ShareUsageData = dto.ShareUsageData;
            settings.Theme = dto.Theme ?? settings.Theme;
            settings.ItemsPerPage = dto.ItemsPerPage > 0 ? dto.ItemsPerPage : settings.ItemsPerPage;

            // Optional: update user profile fields together
            if (user != null)
            {
                if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName;
                if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
            }

            _context.SaveChanges();

            return Ok(new { message = "Settings updated" });
        }

        // Optional: endpoints for admins to read other users' settings
        [HttpGet("{userId}")]
        [Authorize(Roles = "Lecturer,Admin")]
        public IActionResult GetUserSettings(int userId)
        {
            var settings = _context.UserSetting
                .Include(s => s.User)
                .FirstOrDefault(s => s.UserID == userId);

            if (settings == null) return NotFound();
            return Ok(settings);
        }
    }
}
