using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiInscripcionMaterias.Models.Entities
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }  // Asegúrate de tener esta propiedad
        public Estudiante Estudiante { get; set; }
        public Profesor Profesor { get; set; }
    }

    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public DateTime Creado_En { get; set; } = DateTime.UtcNow;

    }

    public class Estudiante
    {
        public int Id { get; set; }

        public int Usuario_Id { get; set; }
        public string Matricula { get; set; }
        public int CreditosTotales { get; set; }
        public int CreditosDisponibles { get; set; } = 0;

        public bool Activo { get; set; }

        public DateTime Creado_En { get; set; } = DateTime.UtcNow;
    }

    public class Profesor
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public string Especialidad { get; set; }
        public bool Activo { get; set; }

        public DateTime Creado_En { get; set; } = DateTime.UtcNow;
    }


}
