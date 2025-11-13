using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universe.Api.Data;
using Universe.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FilesController(AppDbContext context) { _context = context; }

        [HttpPost("upload")]
        [Authorize]
        public IActionResult Upload([FromBody] ClassFile file)
        {
            _context.Files.Add(file);
            _context.SaveChanges();
            return Ok(new { message = "File uploaded" });
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id) => Ok(_context.Files.Include(f => f.Uploader).FirstOrDefault(f => f.FileID == id));

        [HttpGet("uploader/{userId}")]
        [Authorize]
        public IActionResult GetByUploader(int userId) => Ok(_context.Files.Include(f => f.Uploader).Where(f => f.UploaderID == userId).ToList());
    }
}
