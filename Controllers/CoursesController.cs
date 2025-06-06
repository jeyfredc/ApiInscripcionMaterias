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

        [HttpDelete("remove-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<ResultCourseInscriptionDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCourse([FromBody] List<FormCourseRequestDto> request)
        {
            try
            {
                if (request == null || !request.Any())
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "La lista de materias a remover no puede estar vacía"
                    });
                }

                var results = new List<ResultCourseInscriptionDto>();

                foreach (var course in request)
                {
                    var response = await _courseService.RemoveInscription(course);
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
                        ? "Se removieron correctamente las materias inscritas"
                        : "Se removio la materia correctamente",
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
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al remover las inscripciones",
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtiene las materias que no han sido asignadas a ningún profesor
        /// </summary>
        /// <returns>Lista de materias no asignadas</returns>
        [HttpGet("unassigned-courses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<CourseWithoutAssignDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUnassignedCourses()
        {
            try
            {
                var result = await _courseService.GetUnassignedCourses();

                if (!result.Success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
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
                _logger.LogError(ex, "Error al obtener las materias no asignadas");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al obtener las materias no asignadas",
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Registra una nueva materia en el sistema
        /// </summary>
        /// <param name="newCourse">Datos de la nueva materia</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("register-new-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ResultCourseInscriptionDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegisterNewCourse([FromBody] RequestRegisterCourseDto newCourse)
        {
            try
            {
                if (newCourse == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Los datos de la materia son requeridos"
                    });
                }

                var result = await _courseService.RegisterNewCourse(newCourse);

                if (!result.Success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
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
                _logger.LogError(ex, "Error al registrar la nueva materia");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al registrar la materia",
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Asigna una materia a un profesor
        /// </summary>
        /// <param name="registerCourse">Datos de la asignación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("assign-course-teacher")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ResultCourseInscriptionDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignCourseToTeacher([FromBody] RegisterCourseTeacherDto registerCourse)
        {
            try
            {
                if (registerCourse == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Los datos de asignación son requeridos"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Datos de asignación inválidos",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var result = await _courseService.AssignCourseTeacher(registerCourse);

                if (!result.Success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
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
                _logger.LogError(ex, "Error al asignar la materia al profesor");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor al asignar la materia al profesor",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}
