namespace ApiInscripcionMaterias.Models.DTOs
{
    public class RegisterCourseTeacherDto
    {
        public int ProfesorId { get; set; }
        public string CodigoMateria{ get; set; }
        public string Horario { get; set; }
        public string Grupo { get; set; }
    }
}
