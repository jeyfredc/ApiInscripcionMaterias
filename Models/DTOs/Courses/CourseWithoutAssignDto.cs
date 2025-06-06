namespace ApiInscripcionMaterias.Models.DTOs.Courses
{
    public class CourseWithoutAssignDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Creditos { get; set; }
        public int Cupo_Maximo { get; set; }
        public bool Activa { get; set; }
        public DateTime Creado_En { get; set; }
        public int Cupo_Disponible { get; set; }
    }
}
