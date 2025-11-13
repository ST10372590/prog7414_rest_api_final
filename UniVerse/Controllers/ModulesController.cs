using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Data;
using Universe.Api.Models;
using System.Linq;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("modules")]
    public class ModulesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModulesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all modules (includes course info)
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var modules = _context.Modules
                .Include(m => m.Course)
                .ToList();

            return Ok(modules);
        }

        // ✅ Get module by ID
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(string id)
        {
            var module = _context.Modules
                .Include(m => m.Course)
                .FirstOrDefault(m => m.ModuleID == id);

            if (module == null)
                return NotFound();

            return Ok(module);
        }

        // ✅ Create module (Lecturer only)
        [HttpPost("create")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create([FromBody] Module module)
        {
            if (module == null)
                return BadRequest("Invalid module data.");

            _context.Modules.Add(module);
            _context.SaveChanges();

            return Ok(new { message = "Module created successfully." });
        }

        [HttpGet("byCourse/{courseId}")]
        [Authorize]
        public IActionResult GetByCourseId(string courseId)
        {
            var modules = _context.Modules
                .Where(m => m.CourseID == courseId)
                .Select(m => new ModuleDto
                {
                    ModuleID = m.ModuleID,
                    CourseID = m.CourseID,
                    ModuleTitle = m.ModuleTitle,
                    ContentType = m.ContentType,
                    ContentLink = m.ContentLink,
                    CompletionStatus = m.CompletionStatus
                })

                .ToList();

            return Ok(modules);
        }
    }
}
