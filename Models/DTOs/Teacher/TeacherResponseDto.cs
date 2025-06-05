namespace ApiInscripcionMaterias.Models.DTOs.Teacher
{
    public class TeacherResponseDto
    {
        public string NombreProfesor { get; set; }
        public string Horario { get; set; }
        public int CupoMaximo { get; set; }
        public int CupoDisponible { get; set; }
        public string NombreMateria { get; set; }

        public string CodigoMateria { get; set; }
    }
}