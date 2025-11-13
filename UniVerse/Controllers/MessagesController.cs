using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Data;
using Universe.Api.Models;

[ApiController]
[Route("messages")]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _context;
    public MessagesController(AppDbContext context) => _context = context;

    // Get all messages for a specific user
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMessagesForUser(int userId)
    {
        var messages = await _context.Messages
            .Where(m => m.SenderID == userId || m.ReceiverID == userId)
            .OrderByDescending(m => m.MessageID)
            .Select(m => new MessageResponseDto
            {
                MessageID = m.MessageID,
                SenderID = m.SenderID,
                SenderName = m.Sender.LastName,
                ReceiverID = m.ReceiverID,
                ReceiverName = m.Receiver.LastName,
                Content = m.Content,
                ReadStatus = m.ReadStatus,
                FileAttachment = m.FileAttachment
            })
            .ToListAsync();

        return Ok(messages);
    }

    // Get conversation between two users
    [HttpGet("conversation/{senderId}/{receiverId}")]
    public async Task<IActionResult> GetConversation(int senderId, int receiverId)
    {
        var conversation = await _context.Messages
            .Where(m =>
                (m.SenderID == senderId && m.ReceiverID == receiverId) ||
                (m.SenderID == receiverId && m.ReceiverID == senderId))
            .OrderBy(m => m.MessageID)
            .Select(m => new MessageResponseDto
            {
                MessageID = m.MessageID,
                SenderID = m.SenderID,
                SenderName = m.Sender.LastName,
                ReceiverID = m.ReceiverID,
                ReceiverName = m.Receiver.LastName,
                Content = m.Content,
                ReadStatus = m.ReadStatus,
                FileAttachment = m.FileAttachment
            })
            .ToListAsync();

        if (!conversation.Any())
            return NotFound(new { message = "No messages found between these users." });

        return Ok(conversation);
    }

    // Send a new message
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request cannot be null" });

        // Validate that sender and receiver exist
        var senderExists = await _context.Users.AnyAsync(u => u.UserID == request.SenderID);
        var receiverExists = await _context.Users.AnyAsync(u => u.UserID == request.ReceiverID);

        if (!senderExists || !receiverExists)
            return BadRequest(new { message = "Sender or receiver does not exist." });

        var message = new Message
        {
            SenderID = request.SenderID,
            ReceiverID = request.ReceiverID,
            Content = request.Content,
            FileAttachment = request.FileAttachment,
            ReadStatus = string.IsNullOrEmpty(request.ReadStatus) ? "Unread" : request.ReadStatus
        };

        try
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Map to DTO to return safely
            var responseDto = new MessageResponseDto
            {
                MessageID = message.MessageID,
                SenderID = message.SenderID,
                SenderName = (await _context.Users.FindAsync(message.SenderID))?.LastName ?? "Unknown",
                ReceiverID = message.ReceiverID,
                ReceiverName = (await _context.Users.FindAsync(message.ReceiverID))?.LastName ?? "Unknown",
                Content = message.Content,
                ReadStatus = message.ReadStatus,
                FileAttachment = message.FileAttachment
            };

            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error saving message", detail = ex.Message });
        }
    }

    // Mark message as read
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message == null) return NotFound();

        message.ReadStatus = "Read";

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Message marked as read" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating message", detail = ex.Message });
        }
    }
}
