using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Teacher;

namespace ApiInscripcionMaterias.Interfaces
{
    public interface ITeacherService
    {
        Task<ApiResponse<IEnumerable<TeacherResponseDto>>> GetAssignedCourses(int userId);

        Task<ApiResponse<ResultCourseInscriptionDto>> UnassignTeacherSubject(RequestUnassignTeacher unassignTeacher);
    }
}
