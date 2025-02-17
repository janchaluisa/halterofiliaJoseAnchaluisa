using JoseAnchaluisaVillonApi.Services;
using Microsoft.Data.SqlClient;

namespace JoseAnchaluisaVillonApi.Helpers
{
    public class DatabaseLoggerService : ILoggerService
    {
        private readonly IDatabaseHelper _dbHelper;

        public DatabaseLoggerService(IDatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public void Log(string message)
        {
            string query = "INSERT INTO Logs (Mensaje, Fecha) VALUES (@Mensaje, GETDATE())";
            var parameters = new[]
            {
            new SqlParameter("@Mensaje", message)
        };
            _dbHelper.ExecuteQuery(query, parameters);
        }
    }
}
