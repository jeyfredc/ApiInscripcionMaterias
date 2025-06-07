namespace ApiInscripcionMaterias.Models.DTOs.Courses
{
    public class ListCoursesAndSchedulesDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Materia { get; set; }
        public string Descripcion { get; set; }
        public int Creditos { get; set; }
        public int Cupo_Maximo { get; set; }
        public int Cupo_Disponible { get; set; }
        public string Profesor_Asignado { get; set; }

        public int ProfesorId { get; set; }
    
        public String Horarios { get; set; }
    }
}
