using JoseAnchaluisaVillonApi.DTO;
using JoseAnchaluisaVillonApi.Helpers;
using JoseAnchaluisaVillonApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JoseAnchaluisaVillonApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AtletasController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly DatabaseHelper _dbHelper;

        public AtletasController(ILoggerService logger, DatabaseHelper dbHelper)
        {
            _logger = logger;
            _dbHelper = dbHelper;
        }


        // Método para convertir un DataTable en una lista de diccionarios
        private List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            var lista = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                lista.Add(dict);
            }

            return lista;
        }

        #region GET
        [HttpGet("resultados")]
        public IActionResult GetResultados([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string query = "EXEC ObtenerResultadosPaginados @Pagina, @TamanioPagina";

            var parameters = new[]
            {
            new SqlParameter("@Pagina", page),
            new SqlParameter("@TamanioPagina", pageSize)
            };

            var dt = _dbHelper.ExecuteQuery(query, parameters);

            // Convierte el DataTable en una lista de objetos
            var listaresultado = DataTableToList(dt);

            return Ok(listaresultado);
        }

        [HttpGet("intentos-deportista")]
        public IActionResult GetIntentosPorDeportista([FromQuery] int idDeportista)
        {
            _logger.Log($"Consulta de intentos para el deportista con ID: {idDeportista}");
            string query = "EXEC IntentoDeportista @idDeportista";
            var parameters = new[] { new SqlParameter("@idDeportista", idDeportista) };
            var dt = _dbHelper.ExecuteQuery(query, parameters);

            // Convierte el DataTable en una lista de objetos
            var listaIntentos = DataTableToList(dt);

            return Ok(listaIntentos);
        }
        #endregion

        #region POST
        [HttpPost("registrar-pais")]
        public IActionResult RegistrarPais([FromBody] PaisDto data)
        {
            string query = "EXEC RegistrarPais @iniciales, @descripcion";
            var parameters = new[]
            {
            new SqlParameter("@iniciales", (string)data.iniciales),
            new SqlParameter("@descripcion", (string)data.descripcion)
            };

            _dbHelper.ExecuteQuery(query, parameters);
            return Ok(new { mensaje = "País registrado con éxito" });
        }

        [HttpPost("registrar-deportista")]
        public IActionResult RegistrarDeportista([FromBody] DeportistaDto data)
        {
            string query = "EXEC RegistrarDepostista @nombre_completo, @idPais";
            var parameters = new[]
            {
            new SqlParameter("@nombre_completo", (string)data.nombre_completo),
            new SqlParameter("@idPais", (int)data.idPais)
            };

            _dbHelper.ExecuteQuery(query, parameters);
            return Ok(new { mensaje = "Deportista registrado con éxito" });
        }

        [HttpPost("registrar-intento")]
        public IActionResult RegistrarIntento([FromBody] IntentoDto data)
        {
            string query = "EXEC RegistrarIntento @idDeportista, @Tipo, @Peso";
            var parameters = new[]
            {
            new SqlParameter("@idDeportista", (int)data.idDeportista),
            new SqlParameter("@Tipo", (string)data.Tipo),
            new SqlParameter("@Peso", (int)data.Peso)
            };

            _dbHelper.ExecuteQuery(query, parameters);
            return Ok(new { mensaje = "Intento registrado con éxito" });
        }
        #endregion
    }
}
