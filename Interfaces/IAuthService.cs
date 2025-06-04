using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Responses;
using ApiInscripcionMaterias.Models.Entities;

namespace ApiInscripcionMaterias.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<UsuarioDto>> RegisterAsync(RegisterRequest usuario);
        Task<ApiResponse<UsuarioAutenticadoDto>> AuthenticateAsync(string email, string password);
        //Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        //Task<AuthResultDto> GetUserProfileAsync(string userId);
    }
}
