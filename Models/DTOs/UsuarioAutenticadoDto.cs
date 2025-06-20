﻿namespace ApiInscripcionMaterias.Models.DTOs
{
    public class UsuarioAutenticadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public int? Creditos_Disponibles { get; set; }
        public string Token { get; set; }

        public int? Id_Profesor { get; set; }
        public int? Id_Estudiante { get; set; }

    }
}
