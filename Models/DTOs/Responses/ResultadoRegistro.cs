namespace ApiInscripcionMaterias.Models.DTOs.Responses
{
    public class ResultadoRegistro
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UsuarioDto Data { get; set; }
        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new();
    }
}
