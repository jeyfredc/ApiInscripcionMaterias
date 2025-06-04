using ApiInscripcionMaterias.Interfaces;
using ApiInscripcionMaterias.Models.DAO;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Responses;
using ApiInscripcionMaterias.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UsuarioDAO _userDao;
    private readonly int _workFactor = 12;

    public AuthService(
        IConfiguration configuration,
        UsuarioDAO userDao)
    {
        _configuration = configuration;
        _userDao = userDao;
    }

    private string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía");

        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, _workFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }

    public async Task<ApiResponse<UsuarioDto>> RegisterAsync(RegisterRequest usuario)
    {
        try
        {
            usuario.Password = HashPassword(usuario.Password);

            var createdUser = await _userDao.RegistrarUsuario(usuario);

            return new ApiResponse<UsuarioDto>
            {
                Success = true,
                Message = "Usuario registrado correctamente",
                Data = createdUser
            };
        }
        catch (SqlException sqlEx)
        {
            return new ApiResponse<UsuarioDto>
            {
                Success = false,
                Message = "Error al registrar el usuario",
                Errors = new[] { sqlEx.Message }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UsuarioDto>
            {
                Success = false,
                Message = "Error al crear el usuario",
                Errors = new[] { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<UsuarioAutenticadoDto>> AuthenticateAsync(string email, string password)
    {
        try
        {
            // 1. Validar que el email no esté vacío
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return new ApiResponse<UsuarioAutenticadoDto>
                {
                    Success = false,
                    Message = "El correo y la contraseña son requeridos",
                    Errors = new[] { "Credenciales incompletas" }
                };
            }

            // 2. Buscar el usuario por email
            var usuario = await _userDao.ObtenerUsuarioPorEmail(email);

            // 3. Verificar si el usuario existe
            if (usuario == null)
            {
                // No revelar que el correo no existe por seguridad
                return new ApiResponse<UsuarioAutenticadoDto>
                {
                    Success = false,
                    Message = "Autenticación fallida",
                    Errors = new[] { "Correo o contraseña incorrectos" }
                };
            }

            // 4. Verificar la contraseña
            if (!VerifyPassword(password, usuario.Password))
            {
                return new ApiResponse<UsuarioAutenticadoDto>
                {
                    Success = false,
                    Message = "Autenticación fallida",
                    Errors = new[] { "Correo o contraseña incorrectos" }
                };
            }

            // 6. Generar token JWT (opcional)
            var token = GenerateJwtToken(usuario);

            // 7. Mapear a DTO de respuesta
            var usuarioAutenticado = new UsuarioAutenticadoDto
            {
                Id = usuario.RolId,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol?.Nombre,
                Token = token
            };

            return new ApiResponse<UsuarioAutenticadoDto>
            {
                Success = true,
                Message = "Autenticación exitosa",
                Data = usuarioAutenticado
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<UsuarioAutenticadoDto>
            {
                Success = false,
                Message = "Error en el servidor",
                Errors = new[] { "Ocurrió un error al procesar la autenticación" }
            };
        }
    }


    // Método para generar token JWT (opcional)
    private string GenerateJwtToken(Usuario usuario)
    {
        try
        {
            // 1. Obtener configuración
            var secret = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("JwtSettings:Key", "La clave secreta JWT no está configurada");
            }

            // 2. Crear claims estándar JWT
            var claims = new List<Claim>
        {
            // name = Nombre del usuario
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nombre ?? string.Empty),
            // iat (issued at) = Fecha de emisión
            new Claim(JwtRegisteredClaimNames.Iat,
                     DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                     ClaimValueTypes.Integer64),
            // Agregar email
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty)
        };

            // 3. Agregar rol si existe
            if (!string.IsNullOrEmpty(usuario.Rol?.Nombre))
            {
                claims.Add(new Claim("role", usuario.Rol.Nombre));
            }

            // 4. Configurar credenciales
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. Configurar tiempo de expiración (opcional)
            var expiresInDays = _configuration.GetValue<int>("Jwt:ExpireDays", 7);
            var expires = DateTime.UtcNow.AddDays(expiresInDays);

            // 6. Crear el token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            // 7. Escribir el token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al generar el token de autenticación", ex);
        }
    }



}