using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DAO;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Student;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ApiInscripcionMaterias.Services
{
    public class StudentService : IStudentService
    {
        private readonly IConfiguration _configuration;
        private readonly StudentDao _studentDao;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IConfiguration configuration,
            StudentDao studentDao,
            ILogger<StudentService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _studentDao = studentDao ?? throw new ArgumentNullException(nameof(studentDao));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<StudentCreditsDto>> GetStudentCredits(int userId)
        {
            try
            {
                _logger.LogInformation("Obteniendo créditos para el estudiante con ID: {UserId}", userId);

                if (userId <= 0)
                {
                    _logger.LogWarning("ID de usuario no válido: {UserId}", userId);
                    return new ApiResponse<StudentCreditsDto>
                    {
                        Success = false,
                        Message = "El ID del estudiante no es válido"
                    };
                }

                var studentCredits = await _studentDao.GetStudentCredits(userId);

                if (studentCredits == null)
                {
                    _logger.LogWarning("No se encontró el estudiante con ID: {UserId}", userId);
                    return new ApiResponse<StudentCreditsDto>
                    {
                        Success = false,
                        Message = "Estudiante no encontrado"
                    };
                }

                _logger.LogInformation("Créditos obtenidos exitosamente para el estudiante ID: {UserId}", userId);
                return new ApiResponse<StudentCreditsDto>
                {
                    Success = true,
                    Message = "Créditos obtenidos exitosamente",
                    Data = new StudentCreditsDto
                    {
                        EstudianteId = studentCredits.EstudianteId,
                        CreditosDisponibles = studentCredits.CreditosDisponibles
                    }
                };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error de base de datos al obtener créditos del estudiante ID: {UserId}", userId);
                return new ApiResponse<StudentCreditsDto>
                {
                    Success = false,
                    Message = "Error al acceder a la base de datos",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener créditos del estudiante ID: {UserId}", userId);
                return new ApiResponse<StudentCreditsDto>
                {
                    Success = false,
                    Message = "Error inesperado al procesar la solicitud",
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<StudentCoursesDto>>> CoursesByStudent(int StudentId)
        {
            try
            {
                if (StudentId <= 0)
                {
                    _logger.LogWarning("ID de estudiante no válido: {StudentId}", StudentId);
                    return new ApiResponse<IEnumerable<StudentCoursesDto>>
                    {
                        Success = false,
                        Message = "El ID del estudiante no es válido"
                    };
                }

                var studentCourses = await _studentDao.GetCoursesByStudentId(StudentId);

                if (studentCourses == null || !studentCourses.Any())
                {
                    _logger.LogInformation("No se encontraron cursos para el estudiante ID: {StudentId}", StudentId);
                    return new ApiResponse<IEnumerable<StudentCoursesDto>>
                    {
                        Success = true,
                        Message = "No se encontraron cursos para el estudiante",
                        Data = new List<StudentCoursesDto>()
                    };
                }

                _logger.LogInformation("Cursos obtenidos exitosamente para el estudiante ID: {StudentId}", StudentId);
                return new ApiResponse<IEnumerable<StudentCoursesDto>>
                {
                    Success = true,
                    Message = "Cursos obtenidos exitosamente",
                    Data = studentCourses
                };
            }
            catch (Exception ex)
            {

                return new ApiResponse<IEnumerable<StudentCoursesDto>>
                {
                    Success = false,
                    Message = "Error al obtener los cursos del estudiante",
                    Errors = new[] { ex.Message }
                };
            }
        }
    }
}