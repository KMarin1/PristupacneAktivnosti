namespace WebAPI.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = "INFO";
        public string Poruka { get; set; } = string.Empty;
    }
}