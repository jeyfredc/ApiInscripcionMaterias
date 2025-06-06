using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Student;

namespace ApiInscripcionMaterias.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponse<IEnumerable<CoursesResponseDto>>> GetCourses();
        Task<ApiResponse<ResultCourseInscriptionDto>> CourseInscription(FormCourseRequestDto course);
    }
}
