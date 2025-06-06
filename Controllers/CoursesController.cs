using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Student;
using ApiInscripcionMaterias.Models.DTOs.Teacher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiInscripcionMaterias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController: ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ICourseService courseService,
            ILogger<CoursesController> logger)
        {
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene las materias asignadas a un profesor
        /// </summary>
        /// <param name="userId">ID del profesor</param>
        /// <returns>Lista de materias asignadas</returns>
        [HttpGet("available-courses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CoursesResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableCourses( )
        {
            try
            {


                var result = await _courseService.GetCourses();

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (ApplicationException appEx)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = appEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al procesar la solicitud",
                    Errors = new[] { ex.Message }
                });
            }
        }

        [HttpPost("inscription-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<ResultCourseInscriptionDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostInscriptionCourse([FromBody] List<FormCourseRequestDto> request)
        {
            try
            {
                if (request == null || !request.Any())
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "La lista de materias a inscribir no puede estar vacía"
                    });
                }

                var results = new List<ResultCourseInscriptionDto>();

                foreach (var course in request)
                {
                    var response = await _courseService.CourseInscription(course);
                    if (response.Data != null) 
                    {
                        results.Add(response.Data);
                    }
                }

                var allSuccess = results.All(r => r.Resultado);

                return Ok(new ApiResponse<List<ResultCourseInscriptionDto>>
                {
                    Success = allSuccess,
                    Message = allSuccess
                        ? "Todas las inscripciones se procesaron correctamente"
                        : "Algunas inscripciones no pudieron ser procesadas",
                    Data = results
                });
            }
            catch (ApplicationException appEx)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = appEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar inscripciones");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al procesar las inscripciones",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}
