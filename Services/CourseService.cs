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

        public async Task<ApiResponse<ResultCourseInscriptionDto>> RemoveInscription(FormCourseRequestDto course)
        {
            try
            {
                var result = await _courseDao.DeleteCourseByStudent(course);

                if (result == null)
                {
                    return new ApiResponse<ResultCourseInscriptionDto>
                    {
                        Success = false,
                        Message = "No se pudo procesar la desubscripcion del curso",
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

        public async Task<ApiResponse<ResultCourseInscriptionDto>> RegisterNewCourse(RequestRegisterCourseDto newCourse)
        {
            try
            {
                if (newCourse == null)
                {
                    return new ApiResponse<ResultCourseInscriptionDto>
                    {
                        Success = false,
                        Message = "Los datos de la materia son requeridos",
                        Data = null
                    };
                }

                var result = await _courseDao.RegisterNewCourse(newCourse);

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
                    Message = "Error en la base de datos al registrar la materia",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResultCourseInscriptionDto>
                {
                    Success = false,
                    Message = "Error inesperado al registrar la materia",
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CourseWithoutAssignDto>>> GetUnassignedCourses()
        {
            try
            {
                var unassignedCourses = await _courseDao.GetCoursesWithouthAssign();

                if (unassignedCourses == null || !unassignedCourses.Any())
                {
                    return new ApiResponse<IEnumerable<CourseWithoutAssignDto>>
                    {
                        Success = true,
                        Message = "No se encontraron materias sin asignar",
                        Data = Enumerable.Empty<CourseWithoutAssignDto>()
                    };
                }

                return new ApiResponse<IEnumerable<CourseWithoutAssignDto>>
                {
                    Success = true,
                    Message = "Materias sin asignar obtenidas exitosamente",
                    Data = unassignedCourses
                };
            }
            catch (SqlException sqlEx)
            {
                return new ApiResponse<IEnumerable<CourseWithoutAssignDto>>
                {
                    Success = false,
                    Message = "Error en la base de datos al obtener las materias sin asignar",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CourseWithoutAssignDto>>
                {
                    Success = false,
                    Message = "Error inesperado al obtener las materias sin asignar",
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<ResultCourseInscriptionDto>> AssignCourseTeacher(RegisterCourseTeacherDto registerCourse)
        {
            try
            {
                if (registerCourse == null)
                {
                    return new ApiResponse<ResultCourseInscriptionDto>
                    {
                        Success = false,
                        Message = "Los datos de asignación son requeridos",
                        Data = null
                    };
                }

                var result = await _courseDao.AssignCourseTeacher(registerCourse);

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
                    Message = "Error en la base de datos al asignar la materia al profesor",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResultCourseInscriptionDto>
                {
                    Success = false,
                    Message = "Error inesperado al asignar la materia al profesor",
                    Errors = new[] { ex.Message }
                };
            }
        }
    }
}
