using WebAPI.Models;

namespace WebAPI.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string poruka, string level = "INFO")
        {
            var log = new Log
            {
                Poruka = poruka,
                Level = level,
                Timestamp = DateTime.UtcNow
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
