using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Data;
using Universe.Api.Models;
using System.Linq;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CalendarController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/calendar
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            // Include the User info if needed
            var events = _context.CalendarEvents
                .Include(e => e.User)
                .OrderBy(e => e.StartTime)
                .ToList();

            return Ok(events);
        }

        // GET: api/calendar/{id}
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(string id)
        {
            var ev = _context.CalendarEvents
                .Include(e => e.User)
                .FirstOrDefault(e => e.EventID == id);

            if (ev == null)
                return NotFound(new { message = "Event not found" });

            return Ok(ev);
        }

        // POST: api/calendar/events
        [HttpPost("events")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create([FromBody] CalendarEvent ev)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Optional: check if the User exists
            var user = _context.Users.Find(ev.UserID);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            ev.EventID = Guid.NewGuid().ToString(); // Generate a unique EventID
            _context.CalendarEvents.Add(ev);
            _context.SaveChanges();

            return Ok(new { message = "Event created successfully!", eventId = ev.EventID });
        }

        // DELETE: api/calendar/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Delete(string id)
        {
            var ev = _context.CalendarEvents.Find(id);
            if (ev == null)
                return NotFound(new { message = "Event not found" });

            _context.CalendarEvents.Remove(ev);
            _context.SaveChanges();

            return Ok(new { message = "Event deleted successfully!" });
        }

        // PUT: api/calendar/{id} (optional: update event)
        [HttpPut("{id}")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Update(string id, [FromBody] CalendarEvent updatedEvent)
        {
            var ev = _context.CalendarEvents.Find(id);
            if (ev == null)
                return NotFound(new { message = "Event not found" });

            ev.Title = updatedEvent.Title;
            ev.EventType = updatedEvent.EventType;
            ev.StartTime = updatedEvent.StartTime;
            ev.EndTime = updatedEvent.EndTime;
            ev.ColorCode = updatedEvent.ColorCode;
            ev.Description = updatedEvent.Description;

            _context.SaveChanges();

            return Ok(new { message = "Event updated successfully!" });
        }
    }
}
