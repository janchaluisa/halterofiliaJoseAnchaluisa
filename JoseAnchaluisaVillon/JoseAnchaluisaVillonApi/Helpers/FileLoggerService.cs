using JoseAnchaluisaVillonApi.Services;

namespace JoseAnchaluisaVillonApi.Helpers
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logFilePath;

        public FileLoggerService(IConfiguration configuration)
        {
            _logFilePath = configuration["LoggingLog:FilePath"] ?? "logs/log.txt";

            // Asegura que el directorio y el archivo existan
            VerificaRuta();
        }

        private void VerificaRuta()
        {
            var logDirectory = Path.GetDirectoryName(_logFilePath);

            // Si el directorio no existe, créalo
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory!);
            }

            // Si el archivo no existe, créalo
            if (!File.Exists(_logFilePath))
            {
                using var _ = File.Create(_logFilePath);
            }
        }

        public void Log(string message)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(_logFilePath, true);
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el log: {ex.Message}");
            }
        }
    }
}
