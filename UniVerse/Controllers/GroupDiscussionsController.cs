using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Data;
using Universe.Api.Models;

[ApiController]
[Route("api/groups")]
public class GroupDiscussionsController : ControllerBase
{
    private readonly AppDbContext _context;
    public GroupDiscussionsController(AppDbContext context) => _context = context;

    [HttpGet("{groupId}/messages")]
    public async Task<IActionResult> GetGroupMessages(int groupId)
    {
        var messages = await _context.GroupMessages
            .Where(m => m.GroupID == groupId)
            .Include(m => m.Sender)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("{groupId}/messages")]
    public async Task<IActionResult> SendGroupMessage(int groupId, [FromBody] GroupMessage message)
    {
        message.GroupID = groupId;
        _context.GroupMessages.Add(message);
        await _context.SaveChangesAsync();
        return Ok(message);
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _context.Groups.Include(g => g.Members).ToListAsync();
        return Ok(groups);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] Group group)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return Ok(group);
    }
}
