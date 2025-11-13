using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universe.Api.Data;
using Universe.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NotificationsController(AppDbContext context) { _context = context; }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var notifications = _context.Notifications
                .Include(n => n.User)
                .Select(n => new NotificationDTO
                {
                    NotificationID = n.NotificationID,
                    Type = n.Type,
                    Message = n.Message,
                    Priority = n.Priority,
                    Status = n.Status,
                    UserName = n.User.LastName 
                })
                .ToList();

            return Ok(notifications);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create([FromBody] Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return Ok(new { message = "Notification created" });
        }
    }
}
