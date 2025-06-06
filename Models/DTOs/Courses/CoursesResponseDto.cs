namespace ApiInscripcionMaterias.Models.DTOs.Student
{
    public class CoursesResponseDto
    {
        public string CodigoMateria { get; set; }
        public string Materia { get; set; }
        public int Creditos { get; set; }
        public string NombreProfesor { get; set; }
        public string Horario { get; set; }

        public int CupoMaximo { get; set; }
        public int CupoDisponible { get; set; }

        public int ProfesorId { get; set; }
    }
}
