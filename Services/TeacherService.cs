using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DAO;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Teacher;
using Microsoft.Data.SqlClient;

namespace ApiInscripcionMaterias.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IConfiguration _configuration;
        private readonly TeacherDao _teacherDao;

        public TeacherService(
            IConfiguration configuration,
            TeacherDao teacherDao)
        {
            _configuration = configuration;
            _teacherDao = teacherDao;
        }

        public async Task<ApiResponse<IEnumerable<TeacherResponseDto>>> GetAssignedCourses(int userId)
        {
            try
            {
                var assignedCourses = await _teacherDao.GetAssignedCourses(userId);

                if (assignedCourses == null || !assignedCourses.Any())
                {
                    return new ApiResponse<IEnumerable<TeacherResponseDto>>
                    {
                        Success = false,
                        Message = "No se encontraron materias asignadas para el profesor",
                        Data = Enumerable.Empty<TeacherResponseDto>()
                    };
                }

                return new ApiResponse<IEnumerable<TeacherResponseDto>>
                {
                    Success = true,
                    Message = "Materias obtenidas exitosamente",
                    Data = assignedCourses
                };
            }
            catch (SqlException sqlEx)
            {
                return new ApiResponse<IEnumerable<TeacherResponseDto>>
                {
                    Success = false,
                    Message = "Ocurrió un error en la base de datos",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<TeacherResponseDto>>
                {
                    Success = false,
                    Message = "Ha ocurrido un error inesperado",
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<ResultCourseInscriptionDto>> UnassignTeacherSubject(RequestUnassignTeacher unassignTeacher)
        {
            try
            {
                if (unassignTeacher == null)
                {
                    return new ApiResponse<ResultCourseInscriptionDto>
                    {
                        Success = false,
                        Message = "La solicitud no puede estar vacía"
                    };
                }

                var result = await _teacherDao.UnassignTeacher(unassignTeacher);

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
                    Message = "Error de base de datos al desasignar el profesor",
                    Errors = new[] { sqlEx.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResultCourseInscriptionDto>
                {
                    Success = false,
                    Message = "Error inesperado al desasignar el profesor",
                    Errors = new[] { ex.Message }
                };
            }
        }
    }
}