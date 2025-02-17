using JoseAnchaluisaVillonApi.Services;

namespace JoseAnchaluisaVillonApi.Helpers
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logFilePath;

        public FileLoggerService(IConfiguration configuration)
        {
            _logFilePath = configuration["LoggingLog:FilePath"] ?? "log.txt";
        }

        public void Log(string message)
        {
            using StreamWriter writer = new StreamWriter(_logFilePath, true);
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
