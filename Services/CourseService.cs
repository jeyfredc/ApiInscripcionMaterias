using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DAO;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Student;
using ApiInscripcionMaterias.Models.DTOs.Teacher;
using Microsoft.Data.SqlClient;

namespace ApiInscripcionMaterias.Services
{
    public class CourseService : ICourseService
    {
        private readonly IConfiguration _configuration;
        private readonly CourseDao _courseDao;

        public CourseService(
            IConfiguration configuration,
            CourseDao courseDao)
        {
            _configuration = configuration;
            _courseDao = courseDao;
        }

        public async Task<ApiResponse<IEnumerable<CoursesResponseDto>>> GetCourses()
        {
            try
            {
                var assignedCourses = await _courseDao.GetAvailableCourses();

                if (assignedCourses == null || !assignedCourses.Any())
                {
                    return new ApiResponse<IEnumerable<CoursesResponseDto>>
                    {
                        Success = false,
                        Message = "No se encontraron materias disponibles",
                        Data = Enumerable.Empty<CoursesResponseDto>()
                    };
                }

                return new ApiResponse<IEnumerable<CoursesResponseDto>>
                {
                    Success = true,
                    Message = "Materias obtenidas exitosamente",
                    Data = assignedCourses
                };
            }
            catch (SqlException sqlEx)
            {
                return new ApiResponse<IEnumerable<CoursesResponseDto>>
                {
                    Success = false,
                    Message = "Ocurrió un error en la base de datos",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CoursesResponseDto>>
                {
                    Success = false,
                    Message = "Ha ocurrido un error inesperado",
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<ResultCourseInscriptionDto>> CourseInscription(FormCourseRequestDto course)
        {
            try
            {
                var result = await _courseDao.CourseInscriptionStudent(course);

                if (result == null)
                {
                    return new ApiResponse<ResultCourseInscriptionDto>
                    {
                        Success = false,
                        Message = "No se pudo procesar la inscripción",
                        Data = null
                    };
                }

                return new ApiResponse<ResultCourseInscriptionDto>
                {
                    Success = result.Resultado,
                    Message = result.Mensaje,
                    Data = result
                };
            }
            catch (SqlException sqlEx)
            {
                return new ApiResponse<ResultCourseInscriptionDto>
                {
                    Success = false,
                    Message = "Ocurrió un error en la base de datos al procesar la inscripción",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResultCourseInscriptionDto>
                {
                    Success = false,
                    Message = "Ha ocurrido un error inesperado al procesar la inscripción",
                    Errors = new[] { ex.Message }
                };
            }
        }
    }
}
