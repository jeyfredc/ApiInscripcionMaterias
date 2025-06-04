namespace ApiInscripcionMaterias.Models.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string? Matricula { get; set; } 
        public int? Creditos_Totales { get; set; }
    }
}
