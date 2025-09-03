using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPiChallenge.Core.DTOs;
using PPiChallenge.Core.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PPiChallenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Constructor
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Validar usuario y generar un token JWT para acceder a esta API.
        /// </summary>
        /// <remarks>
        /// Endpoint que permite iniciar sesión pasandole el <c>Usuario</c> y <c>Password</c>.
        /// Si las credenciales son correctas y la cuenta está habilitada, devuelve un token JWT valido por 1 hora.
        ///
        /// Ejemplo de request:
        ///
        ///     POST /api/auth/login
        ///     {
        ///        "usuario": "mario",
        ///        "password": "MiPassword123"
        ///     }
        ///
        /// Ejemplo de response 200 OK:
        ///
        ///     {
        ///         "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///         "expiration": "2025-09-02T18:30:00Z"
        ///     }
        ///
        /// </remarks>
        /// <param name="dto">DTO que contiene las credenciales del usuario.</param>
        /// <returns>LoginResDto con el token JWT y fecha de expiración si la autenticación es correcta.</returns>
        /// <response code="200">Login exitoso. Devuelve token JWT y expiración.</response>
        /// <response code="400">Datos inválidos. Por ejemplo, si faltan campos requeridos.</response>
        /// <response code="401">Usuario o contraseña incorrectos, o cuenta deshabilitada.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginReqDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(token);
        }

        /// <summary>
        /// Endpoint para registrar una nueva cuenta.
        /// </summary>
        /// <param name="dto">Datos del nuevo usuario a registrar</param>
        /// <returns>Cuenta creada (sin password)</returns>
        /// <response code="200">Cuenta registrada correctamente</response>
        /// <response code="400">Email o usuario ya registrado</response>
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarCuenta([FromBody] RegistroCuentaDto dto)
        {
            var cuenta = await _authService.RegistrarCuentaAsync(dto);
            return Ok(cuenta);
        }

        /// <summary>
        /// Endpoint protegido para probar que el token JWT es válido.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere un token válido enviado en el header Authorization:
        /// 
        ///     GET /api/auth/probar-token
        ///     Header:
        ///         Authorization: Bearer {token}
        ///
        /// Response:
        /// 200 OK
        ///     {
        ///         "mensaje": "Token válido. Usuario: miusuario"
        ///     }
        ///
        /// 401 Unauthorized si el token es inválido o no se envía.
        /// </remarks>
        /// <returns>Mensaje confirmando que el token es válido y el usuario autenticado.</returns>
        [HttpGet("probar-token")]
        [Authorize] // Requiere token válido
        public IActionResult ProbarToken()
        {
            var usuario = User.Identity?.Name ?? "desconocido";
            return Ok(new { mensaje = $"Token válido. Usuario: {usuario}" });
        }
    }
}
