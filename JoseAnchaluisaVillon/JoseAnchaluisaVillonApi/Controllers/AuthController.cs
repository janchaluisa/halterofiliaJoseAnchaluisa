using JoseAnchaluisaVillonApi.Helpers;
using JoseAnchaluisaVillonApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace JoseAnchaluisaVillonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly JwtService _jwtService;

        public AuthController(ILoggerService logger, JwtService jwtService)
        {
            _logger = logger;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            _logger.Log("Intento de inicio de sesión para el usuario: " + request.Username);
            if (request.Username == "admin" && request.Password == "password123")
            {
                var token = _jwtService.GenerateToken(request.Username);
                _logger.Log("Token generado exitosamente para: " + request.Username);
                return Ok(new { token });
            }
            _logger.Log("Credenciales inválidas para el usuario: " + request.Username);
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}
