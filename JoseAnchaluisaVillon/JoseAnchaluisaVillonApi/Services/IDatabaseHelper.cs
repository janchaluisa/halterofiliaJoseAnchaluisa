using System.Data;
using Microsoft.Data.SqlClient;

namespace JoseAnchaluisaVillonApi.Services
{
    public interface IDatabaseHelper
    {
        DataTable ExecuteQuery(string query, params SqlParameter[] parameters);
    }
}
