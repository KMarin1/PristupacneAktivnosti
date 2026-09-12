namespace WebAPI.Services
{
    public interface ILogService
    {
        Task LogAsync(string poruka, string level = "INFO");
    }
}
