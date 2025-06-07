namespace ApiInscripcionMaterias.Models.DTOs
{
    public class RequestUpdateSubject
    {
        public int MateriaId { get; set; }

        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public int Creditos { get; set; }

        public int Cupo_Maximo { get; set; }

        public string? Horarios { get; set; }

    }
}
