using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.Entities;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;


namespace ApiInscripcionMaterias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] 
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UsuarioDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Register([FromBody] RegisterRequest usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse("Datos de entrada inválidos", ModelState));
                }

                var result = await _authService.RegisterAsync(usuario);

                if (!result.Success)
                {
                    return BadRequest(result); 
                }

                return Ok(result); 
            }
            catch (ApplicationException appEx)
            {
            
                return BadRequest(new
                {
                    Success = false,
                    Message = appEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("🔐 Intento de inicio de sesión para: {Email}", request.Email);

                var result = await _authService.AuthenticateAsync(request.Email, request.Password);

                if (!result.Success)
                {
                    _logger.LogWarning("❌ Autenticación fallida para: {Email}", request.Email);
                    return Unauthorized(result);
                }

                _logger.LogInformation("✅ Inicio de sesión exitoso para: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en inicio de sesión para: {Email}", request.Email);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new[] { ex.Message }
                });
            }
        }


    }
}



