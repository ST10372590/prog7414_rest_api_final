using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universe.Api.Data;
using Universe.Api.Models;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("progress")]
    public class ProgressController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProgressController(AppDbContext context) { _context = context; }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll() => Ok(_context.ProgressRecords.ToList());

        [HttpPost("update")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Update([FromBody] Progress progress)
        {
            _context.ProgressRecords.Add(progress);
            _context.SaveChanges();
            return Ok(new { message = "Progress updated" });
        }
    }
}
