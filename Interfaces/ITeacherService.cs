using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Teacher;

namespace ApiInscripcionMaterias.Interfaces
{
    public interface ITeacherService
    {
        Task<ApiResponse<IEnumerable<TeacherResponseDto>>> GetAssignedCourses(int userId);
    }
}
