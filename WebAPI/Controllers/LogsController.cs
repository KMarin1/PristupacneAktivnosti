using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public LogsController(ApplicationDbContext db) { _db = db; }

        [HttpGet("get/{n:int}")]
        public async Task<IActionResult> GetLast(int n)
        {
            var logs = await _db.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();
            return Ok(logs);
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _db.Logs.LongCountAsync();
            return Ok(count);
        }
    }
}
