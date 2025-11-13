using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universe.Api.Data;
using Universe.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("submissions")]
    public class SubmissionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SubmissionsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ GET: /submissions
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var submissions = _context.Submissions
                .Include(s => s.Student)          // Ensure Student is loaded
                .Include(s => s.Assessment)       // Ensure Assessment is loaded
                .Select(s => new
                {
                    s.SubmissionID,
                    s.AssessmentID,
                    AssessmentTitle = s.Assessment != null ? s.Assessment.Title : "N/A",
                    s.UserID,
                    StudentFirst = s.Student != null ? s.Student.FirstName : "Unknown",
                    StudentLast = s.Student != null ? s.Student.LastName : "User",
                    s.FileLink,
                    s.SubmittedAt,
                    s.Grade,
                    s.Feedback
                })
                .ToList();

            return Ok(submissions);
        }

        // ✅ POST: /submissions/submit (Student upload)
        [HttpPost("submit")]
        [Authorize]
        [RequestSizeLimit(50_000_000)] // optional: 50MB limit
        public async Task<IActionResult> Submit([FromForm] SubmissionRequest request)
        {
            var assessment = await _context.Assessments.FindAsync(request.AssessmentID);
            if (assessment == null)
                return BadRequest(new { message = "Invalid AssessmentID" });

            var student = await _context.Users.FindAsync(request.UserID);
            if (student == null)
                return BadRequest(new { message = "Invalid UserID" });

            string? fileLink = null;

            if (request.File != null && request.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "UploadedSubmissions");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                fileLink = $"/UploadedSubmissions/{uniqueFileName}";
            }

            var submission = new Submission
            {
                AssessmentID = request.AssessmentID,
                UserID = request.UserID,
                FileLink = fileLink ?? string.Empty,
                SubmittedAt = DateTime.UtcNow,
                Grade = 0m,
                Feedback = null
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Submission recorded successfully",
                submissionId = submission.SubmissionID,
                fileLink = submission.FileLink
            });
        }

        // ✅ GET: /submissions/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize]
        public IActionResult GetByUser(int userId)
        {
            var submissions = _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assessment)
                .Where(s => s.UserID == userId)
                .Select(s => new
                {
                    submissionID = s.SubmissionID,
                    assessmentID = s.AssessmentID,
                    assessmentTitle = s.Assessment != null ? s.Assessment.Title : "N/A",
                    studentName = s.Student != null
                        ? $"{s.Student.FirstName} {s.Student.LastName}"
                        : "Unknown Student",
                    fileLink = s.FileLink,
                    submittedAt = s.SubmittedAt,
                    grade = s.Grade,
                    feedback = s.Feedback
                })
                .ToList();

            if (!submissions.Any())
                return NotFound(new { message = "No submissions found for this user" });

            return Ok(submissions);
        }

        // ✅ GET: /submissions/assessment/{assessmentId}
        // Lecturer retrieves all submissions for a given assessment
        [HttpGet("assessment/{assessmentId}")]
        [Authorize(Roles = "Lecturer,Admin")]
        public IActionResult GetByAssessment(int assessmentId)
        {
            var assessment = _context.Assessments.Find(assessmentId);
            if (assessment == null)
                return NotFound(new { message = "Assessment not found" });

            var submissions = _context.Submissions
                .Include(s => s.Student)
                .Where(s => s.AssessmentID == assessmentId)
                .Select(s => new
                {
                    s.SubmissionID,
                    s.AssessmentID,
                    AssessmentTitle = assessment.Title,
                    s.UserID,
                    StudentFirst = s.Student != null ? s.Student.FirstName : "Unknown",
                    StudentLast = s.Student != null ? s.Student.LastName : "User",
                    StudentFullName = s.Student != null
                        ? $"{s.Student.FirstName} {s.Student.LastName}"
                        : "Unknown Student",
                    s.FileLink,
                    s.SubmittedAt,
                    s.Grade,
                    s.Feedback
                })
                .ToList();

            return Ok(submissions);
        }

        // ✅ PUT: /submissions/{submissionId}/feedback
        [HttpPut("{submissionId}/feedback")]
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> UpdateFeedback(int submissionId, [FromForm] FeedbackUpdateRequest request)
        {
            var submission = await _context.Submissions.FindAsync(submissionId);
            if (submission == null)
                return NotFound(new { message = "Submission not found" });

            if (request.Grade < 0 || request.Grade > 100)
                return BadRequest(new { message = "Grade must be between 0 and 100" });

            submission.Grade = request.Grade;
            submission.Feedback = request.Feedback;
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedback and grade updated successfully" });
        }
    }

    // DTOs
    public class SubmissionRequest
    {
        public int AssessmentID { get; set; }
        public int UserID { get; set; }
        public IFormFile? File { get; set; }
    }

    public class FeedbackUpdateRequest
    {
        public decimal Grade { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
