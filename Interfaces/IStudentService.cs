using ApiInscripcionMaterias.Models.DTOs.Student;

namespace ApiInscripcionMaterias.Interfaces
{
    public interface IStudentService
    {
        Task<ApiResponse<StudentCreditsDto>> GetStudentCredits(int userId);
        Task<ApiResponse<IEnumerable<StudentCoursesDto>>> CoursesByStudent(int StudentId);
    }
}
