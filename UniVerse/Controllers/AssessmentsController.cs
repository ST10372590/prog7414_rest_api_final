using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Universe.Api.Data;
using Universe.Api.Models;
using Microsoft.AspNetCore.Http;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("assessments")]
    public class AssessmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AssessmentsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ Get all assessments by course
        [HttpGet("course/{courseId}")]
        [Authorize]
        public IActionResult GetByCourse(string courseId)
        {
            var assessments = _context.Assessments
                .Include(a => a.Course)
                .Include(a => a.Submissions)
                .Where(a => a.CourseID == courseId)
                .ToList();

            if (assessments.Count == 0)
                return NotFound(new { message = "No assessments found for this course" });

            var result = assessments.ConvertAll(a => new
            {
                a.AssessmentID,
                a.Title,
                a.Description,
                a.DueDate,
                a.MaxMarks,
                a.CourseID,
                a.ModuleID,
                fileUrl = string.IsNullOrEmpty(a.FilePath)
                    ? null
                    : Url.Content($"~/uploads/{Path.GetFileName(a.FilePath)}")
            });

            return Ok(result);
        }

        // ✅ Get all assessments by module
        [HttpGet("module/{moduleId}")]
        [Authorize]
        public IActionResult GetByModule(string moduleId)
        {
            try
            {
                var assessments = _context.Assessments
                    .Include(a => a.Course)
                    .Include(a => a.Submissions)
                    .Where(a => a.ModuleID == moduleId)
                    .ToList();

                if (assessments.Count == 0)
                    return NotFound(new { message = $"No assessments found for moduleId '{moduleId}'" });

                var result = assessments.ConvertAll(a => new
                {
                    a.AssessmentID,
                    a.Title,
                    a.Description,
                    a.DueDate,
                    a.MaxMarks,
                    a.CourseID,
                    a.ModuleID,
                    fileUrl = string.IsNullOrEmpty(a.FilePath)
                        ? null
                        : Url.Content($"~/uploads/{Path.GetFileName(a.FilePath)}")
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching assessments",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // ✅ NEW: Get assessment by ID
        [HttpGet("{assessmentId}")]
        [Authorize]
        public IActionResult GetById(int assessmentId)
        {
            var assessment = _context.Assessments
                .Include(a => a.Course)
                .Include(a => a.Submissions)
                .FirstOrDefault(a => a.AssessmentID == assessmentId);

            if (assessment == null)
                return NotFound(new { message = $"Assessment with ID {assessmentId} not found" });

            var result = new
            {
                assessment.AssessmentID,
                assessment.Title,
                assessment.Description,
                assessment.DueDate,
                assessment.MaxMarks,
                assessment.CourseID,
                assessment.ModuleID,
                fileUrl = string.IsNullOrEmpty(assessment.FilePath)
                    ? null
                    : Url.Content($"~/uploads/{Path.GetFileName(assessment.FilePath)}")
            };

            return Ok(result);
        }

        // ✅ Create new assessment (with module support)
        [HttpPost("create")]
        [Authorize(Roles = "Lecturer")]
        [RequestSizeLimit(50_000_000)] // limit ~50MB
        public async Task<IActionResult> Create([FromForm] AssessmentCreateDto dto, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == dto.CourseID);
            if (course == null)
                return BadRequest(new { message = $"CourseID {dto.CourseID} not found" });

            if (!string.IsNullOrEmpty(dto.ModuleID))
            {
                var moduleExists = await _context.Modules
                    .AnyAsync(m => m.ModuleID == dto.ModuleID && m.CourseID == dto.CourseID);
                if (!moduleExists)
                    return BadRequest(new { message = $"ModuleID {dto.ModuleID} does not belong to CourseID {dto.CourseID}" });
            }

            string filePath = null;
            if (file != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            var assessment = new Assessment
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc),
                MaxMarks = dto.MaxMarks,
                CourseID = dto.CourseID,
                ModuleID = dto.ModuleID,
                FilePath = filePath
            };

            try
            {
                _context.Assessments.Add(assessment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Error saving assessment", details = ex.InnerException?.Message });
            }

            return Ok(new
            {
                message = "Assessment created successfully",
                assessmentID = assessment.AssessmentID,
                courseID = assessment.CourseID,
                moduleID = assessment.ModuleID,
                fileUrl = string.IsNullOrEmpty(assessment.FilePath)
                    ? null
                    : Url.Content($"~/uploads/{Path.GetFileName(assessment.FilePath)}")
            });
        }
    }
}
