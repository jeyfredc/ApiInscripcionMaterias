using System.ComponentModel.DataAnnotations;

namespace ApiInscripcionMaterias.Models.DTOs
{
    // LoginRequest.cs
    public class LoginRequest
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
    }
}
