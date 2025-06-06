namespace ApiInscripcionMaterias.Models.DTOs.Student
{
    public class StudentCoursesDto
    {
        public string Codigomateria { get; set; }

        public string Materia { get; set; }
        public string Profesor { get; set; }
        public string Horario { get; set; }
        public DateTime FehaInscripcion { get; set; }

    }
}
