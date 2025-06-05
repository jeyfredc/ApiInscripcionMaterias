using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Teacher;
using ApiInscripcionMaterias.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ApiInscripcionMaterias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Requiere autenticación
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(
            ITeacherService teacherService,
            ILogger<TeacherController> logger)
        {
            _teacherService = teacherService ?? throw new ArgumentNullException(nameof(teacherService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene las materias asignadas a un profesor
        /// </summary>
        /// <param name="userId">ID del profesor</param>
        /// <returns>Lista de materias asignadas</returns>
        [HttpGet("assigned-courses/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TeacherResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAssignedCourses(int userId)
        {
            try
            {
                _logger.LogInformation("📋 Obteniendo materias asignadas para el profesor: {UserId}", userId);

                if (userId <= 0)
                {
                    return BadRequest(new ApiResponse("El ID del profesor no es válido"));
                }

                var result = await _teacherService.GetAssignedCourses(userId);

                if (!result.Success)
                {
                    _logger.LogWarning("⚠️ No se encontraron materias asignadas para el profesor: {UserId}", userId);
                    return NotFound(result);
                }

                _logger.LogInformation("✅ Materias obtenidas exitosamente para el profesor: {UserId}", userId);
                return Ok(result);
            }
            catch (ApplicationException appEx)
            {
                _logger.LogWarning(appEx, "⚠️ Error de aplicación al obtener materias asignadas");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = appEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener las materias asignadas para el profesor: {UserId}", userId);
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al procesar la solicitud",
                    Errors = new[] { ex.Message }
                });
            }
        }

    }
}