using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
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
    [Authorize]
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

        /// <summary>
        /// Desasigna un profesor de una materia
        /// </summary>
        /// <param name="request">Datos para la desasignación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("unassign-teacher")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ResultCourseInscriptionDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnassignTeacher([FromBody] RequestUnassignTeacher request)
        {
            try
            {
                _logger.LogInformation("🔄 Iniciando desasignación de profesor para la materia: {CodigoMateria}",
                    request?.CodigoMateria);

                if (request == null)
                {
                    _logger.LogWarning("⚠️ La solicitud de desasignación no puede ser nula");
                    return BadRequest(new ApiResponse("La solicitud no puede estar vacía"));
                }

                if (request.ProfesorId <= 0 || string.IsNullOrWhiteSpace(request.CodigoMateria))
                {
                    _logger.LogWarning("⚠️ Datos de entrada inválidos: ProfesorId: {ProfesorId}, CodigoMateria: {CodigoMateria}",
                        request.ProfesorId, request.CodigoMateria);
                    return BadRequest(new ApiResponse("Los datos de entrada no son válidos"));
                }

                var result = await _teacherService.UnassignTeacherSubject(request);

                if (!result.Success)
                {
                    _logger.LogWarning("⚠️ No se pudo desasignar al profesor: {Mensaje}", result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("✅ Profesor desasignado exitosamente de la materia: {CodigoMateria}",
                    request.CodigoMateria);
                return Ok(result);
            }
            catch (ApplicationException appEx)
            {
                _logger.LogWarning(appEx, "⚠️ Error de aplicación al desasignar profesor");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = appEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al desasignar al profesor de la materia: {CodigoMateria}",
                    request?.CodigoMateria);
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al procesar la desasignación",
                    Errors = new[] { ex.Message }
                });
            }

        }
    }
}