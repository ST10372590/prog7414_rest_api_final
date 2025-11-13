using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Data;
using Universe.Api.Models;
using Universe.Api.Models.Universe.Api.Models;
using System.Security.Claims;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("courses")]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CoursesController(AppDbContext context) => _context = context;

        // ✅ Get all courses (accessible to authenticated users)
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var courses = _context.Courses
                .Include(c => c.Lecturer)
                .Include(c => c.Modules)
                .Include(c => c.Assessments)
                .AsSplitQuery()
                .Select(c => new CourseDto
                {
                    CourseID = c.CourseID,
                    CourseTitle = c.CourseTitle,
                    CourseDescription = c.CourseDescription,
                    Credits = c.Credits,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    LecturerID = c.LecturerID,
                    Modules = c.Modules.Select(m => new ModuleDto
                    {
                        ModuleID = m.ModuleID,
                        ModuleTitle = m.ModuleTitle,
                        ContentType = m.ContentType,
                        ContentLink = m.ContentLink,
                        CompletionStatus = m.CompletionStatus
                    }).ToList(),
                    Assessments = c.Assessments.Select(a => new AssessmentDto
                    {
                        AssessmentID = a.AssessmentID,
                        Title = a.Title,
                        Description = a.Description,
                        DueDate = a.DueDate,
                        MaxMarks = a.MaxMarks
                    }).ToList()
                })
                .ToList();

            return Ok(courses);
        }

        // ✅ Get course by ID
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(string id)
        {
            var course = _context.Courses
                .Include(c => c.Lecturer)
                .Include(c => c.Modules)
                .Include(c => c.Assessments)
                .AsSplitQuery()
                .Where(c => c.CourseID == id)
                .Select(c => new CourseDto
                {
                    CourseID = c.CourseID,
                    CourseTitle = c.CourseTitle,
                    CourseDescription = c.CourseDescription,
                    Credits = c.Credits,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    LecturerID = c.LecturerID,
                    Modules = c.Modules.Select(m => new ModuleDto
                    {
                        ModuleID = m.ModuleID,
                        ModuleTitle = m.ModuleTitle,
                        ContentType = m.ContentType,
                        ContentLink = m.ContentLink,
                        CompletionStatus = m.CompletionStatus
                    }).ToList(),
                    Assessments = c.Assessments.Select(a => new AssessmentDto
                    {
                        AssessmentID = a.AssessmentID,
                        Title = a.Title,
                        Description = a.Description,
                        DueDate = a.DueDate,
                        MaxMarks = a.MaxMarks
                    }).ToList()
                })
                .FirstOrDefault();

            return course == null ? NotFound() : Ok(course);
        }

        // ✅ Create new course (Lecturer only)
        [HttpPost("create")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create([FromBody] Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
            return Ok(new { message = "Course created" });
        }

        // ✅ Enroll student in a course (fixes UserID issue)
        [HttpPost("{id}/enroll")]
        [Authorize(Roles = "Student")]
        public IActionResult Enroll(string id)
        {
            // Use NameIdentifier claim (set in JwtService)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Invalid User ID in token.");

            var course = _context.Courses.FirstOrDefault(c => c.CourseID == id);
            if (course == null)
                return NotFound("Course not found.");

            var alreadyEnrolled = _context.UserCourses.Any(uc => uc.UserID == userId && uc.CourseID == id);
            if (alreadyEnrolled)
                return BadRequest("Already enrolled in this course.");

            _context.UserCourses.Add(new UserCourse
            {
                UserID = userId,
                CourseID = id,
                EnrolledAt = DateTime.UtcNow
            });
            _context.SaveChanges();

            return Ok(new { message = "Enrolled successfully." });
        }

        // ✅ Get all courses a student is enrolled in
        [HttpGet("enrolled")]
        [Authorize(Roles = "Student")]
        public IActionResult GetEnrolledCourses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID not found in token.");

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid User ID format.");

            var enrolledCourses = _context.UserCourses
                .Where(uc => uc.UserID == userId)
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Lecturer) // ✅ Include Lecturer
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Modules)
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Assessments)
                .AsSplitQuery()
                .Select(uc => new CourseDto
                {
                    CourseID = uc.Course.CourseID,
                    CourseTitle = uc.Course.CourseTitle,
                    CourseDescription = uc.Course.CourseDescription,
                    Credits = uc.Course.Credits,
                    StartDate = uc.Course.StartDate,
                    EndDate = uc.Course.EndDate,
                    LecturerID = uc.Course.LecturerID,
                    Modules = uc.Course.Modules.Select(m => new ModuleDto
                    {
                        ModuleID = m.ModuleID,
                        ModuleTitle = m.ModuleTitle,
                        ContentType = m.ContentType,
                        ContentLink = m.ContentLink,
                        CompletionStatus = m.CompletionStatus
                    }).ToList(),
                    Assessments = uc.Course.Assessments.Select(a => new AssessmentDto
                    {
                        AssessmentID = a.AssessmentID,
                        Title = a.Title,
                        Description = a.Description,
                        DueDate = a.DueDate,
                        MaxMarks = a.MaxMarks
                    }).ToList()
                })
                .ToList();

            return Ok(enrolledCourses);
        }

    }
}
