using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Student;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ApiInscripcionMaterias.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            IStudentService studentService,
            ILogger<StudentController> logger)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene los créditos disponibles de un estudiante
        /// </summary>
        /// <param name="userId">ID del usuario estudiante</param>
        /// <returns>Créditos disponibles del estudiante</returns>
        [HttpGet("credits/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<StudentCreditsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStudentCredits(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    _logger.LogWarning("⚠️ ID de usuario no válido: {UserId}", userId);
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "El ID del estudiante no es válido"
                    });
                }

                var result = await _studentService.GetStudentCredits(userId);

                if (!result.Success)
                {
                    _logger.LogWarning("⚠️ No se encontró el estudiante con ID: {UserId}", userId);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Estudiante no encontrado"
                    });
                }

                _logger.LogInformation("✅ Créditos obtenidos exitosamente para el estudiante: {UserId}", userId);
                return Ok(new ApiResponse<StudentCreditsDto>
                {
                    Success = true,
                    Message = "Créditos obtenidos exitosamente",
                    Data = result.Data
                });
            }
            catch (ApplicationException appEx)
            {
                _logger.LogWarning(appEx, "⚠️ Error de aplicación al obtener créditos del estudiante");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = appEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener los créditos del estudiante: {UserId}", userId);
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al procesar la solicitud",
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtiene los cursos de un estudiante por su ID
        /// </summary>
        /// <param name="studentId">ID del estudiante</param>
        /// <returns>Lista de cursos del estudiante</returns>
        [HttpGet("coursesById/{studentId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentCoursesDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentCourses(int studentId)
        {
            try
            {
                if (studentId <= 0)
                {
                    return BadRequest(new ApiResponse<IEnumerable<StudentCoursesDto>>
                    {
                        Success = false,
                        Message = "El ID del estudiante no es válido"
                    });
                }

                var result = await _studentService.CoursesByStudent(studentId);

                if (!result.Success)
                {
                    return NotFound(new { result.Message, result.Errors });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los cursos del estudiante ID: {StudentId}", studentId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<IEnumerable<StudentCoursesDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor al procesar la solicitud",
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtiene los estudiantes inscritos en un curso por su ID
        /// </summary>
        /// <param name="studentId">ID del estudiante</param>
        /// <returns>Lista de estudiantes inscritos en el curso</returns>
        [HttpGet("getClassMatesByStudentId/{studentId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClassMatesDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClassMatesByStudentId(int studentId)
        {
            try
            {
                if (studentId <= 0)
                {
                    return BadRequest(new ApiResponse<IEnumerable<ClassMatesDto>>
                    {
                        Success = false,
                        Message = "El ID del estudiante no es válido"
                    });
                }

                var result = await _studentService.ClassMates(studentId);

                if (!result.Success)
                {
                    return NotFound(new { result.Message, result.Errors });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<IEnumerable<ClassMatesDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor al procesar la solicitud",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}