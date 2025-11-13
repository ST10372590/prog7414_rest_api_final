using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universe.Api.Data;
using Universe.Api.Models;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SchedulesController(AppDbContext context) { _context = context; }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll() => Ok(_context.Schedules.ToList());

        [HttpPost("create")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create([FromBody] Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            _context.SaveChanges();
            return Ok(new { message = "Schedule created" });
        }
    }
}
