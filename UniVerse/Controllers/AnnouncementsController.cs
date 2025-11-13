using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Data;
using Universe.Api.Models;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/announcements")]
public class AnnouncementsController : ControllerBase
{
    private readonly AppDbContext _context;
    public AnnouncementsController(AppDbContext context) => _context = context;

    // GET all announcements
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var announcements = await _context.Announcements
            .OrderByDescending(a => a.Date)
            .ToListAsync();
        return Ok(announcements);
    }

    // ✅ GET announcements by module
    [HttpGet("module/{moduleId}")]
    public async Task<IActionResult> GetByModule(string moduleId)
    {
        var announcements = await _context.Announcements
            .Where(a => a.ModuleId == moduleId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(announcements);
    }

    // ✅ POST create new announcement
    [HttpPost]
    public async Task<IActionResult> CreateAnnouncement([FromBody] Announcement announcement)
    {
        if (string.IsNullOrEmpty(announcement.ModuleId))
            return BadRequest("ModuleId is required.");

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
        return Ok(announcement);
    }
}
