using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Student;

namespace ApiInscripcionMaterias.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponse<IEnumerable<CoursesResponseDto>>> GetCourses();
        Task<ApiResponse<ResultCourseInscriptionDto>> CourseInscription(FormCourseRequestDto course);
        Task<ApiResponse<ResultCourseInscriptionDto>> RemoveInscription(FormCourseRequestDto course);
        Task<ApiResponse<ResultCourseInscriptionDto>> RegisterNewCourse(RequestRegisterCourseDto newCourse);
        Task<ApiResponse<IEnumerable<CourseWithoutAssignDto>>> GetUnassignedCourses();
        Task<ApiResponse<ResultCourseInscriptionDto>> AssignCourseTeacher(RegisterCourseTeacherDto registerCourse);
    }
}
