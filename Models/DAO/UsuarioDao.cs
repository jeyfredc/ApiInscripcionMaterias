using ApiInscripcionMaterias.Data;
using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ApiInscripcionMaterias.Models.DAO
{
    public class UsuarioDAO : IDisposable
    {
        private readonly IDbConnection _db;
        private readonly ILogger<UsuarioDAO> _logger;

        public UsuarioDAO(ILogger<UsuarioDAO> logger = null)
        {
            _logger = logger;
            _db = DatabaseConfig.GetConnection();
            _logger?.LogInformation("✅ UsuarioDAO inicializado");
        }

        public async Task<UsuarioDto> RegistrarUsuario(RegisterRequest usuario)
        {
            _logger?.LogInformation("🔹 Iniciando registro de usuario: {Email}", usuario.Email);

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Nombre", usuario.Nombre, DbType.String);
                parameters.Add("@Email", usuario.Email, DbType.String);
                parameters.Add("@PasswordHash", usuario.Password, DbType.String);
                parameters.Add("@RolId", usuario.RolId, DbType.Int32);


                var resultado = await _db.QueryFirstOrDefaultAsync<UsuarioDto>(
                    "sp_RegistrarUsuario",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (resultado == null)
                {
                    throw new ApplicationException("No se pudo completar el registro del usuario");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error inesperado al registrar usuario");
                throw;
            }

        }

        public async Task<Usuario> ObtenerUsuarioPorEmail(string email)
        {
            _logger?.LogInformation("🔍 Buscando usuario con email: {Email}", email);

            try
            {
                using var connection = _db;


                var parameters = new DynamicParameters();
                parameters.Add("@Email", email, DbType.String);

                using var command = connection.CreateCommand();
                command.CommandText = "sp_ObtenerUsuarioCompletoPorEmail";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Email", email));

                using var adapter = new SqlDataAdapter((SqlCommand)command);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    return null;

                // Mapear Usuario
                var usuario = new Usuario();
                var usuarioRow = dataSet.Tables[0].Rows[0];
                DataRow StudentRow = null;
                if (dataSet.Tables[1].Rows.Count != 0)
                {
                    StudentRow = dataSet.Tables[1].Rows[0];

                }

                usuario.RolId = Convert.ToInt32(usuarioRow["Id"]);
                usuario.Nombre = usuarioRow["Nombre"].ToString();
                usuario.Email = usuarioRow["Email"].ToString();
                usuario.Password = usuarioRow["password_hash"].ToString();
                usuario.Estudiante ??= new Estudiante();
                usuario.Rol ??= new Rol();

                // Verificar si el rol existe en el DataRow
                if (usuarioRow["rol_nombre"] != DBNull.Value)
                {
                    // Asegurarse de que el objeto Rol esté inicializado
                    usuario.Rol.Nombre = usuarioRow["rol_nombre"].ToString();
                }

                if (usuarioRow["rol_nombre"]?.ToString() == "Estudiante")
                {
                    // Initialize the Estudiante object if it's null

                    // Now it's safe to set the property
                    if (StudentRow["creditos_disponibles"] != DBNull.Value)
                    {
                        usuario.Estudiante.CreditosDisponibles = Convert.ToInt32(StudentRow["creditos_disponibles"]);
                    }
                }

                return usuario;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error al buscar usuario por email: {Email}", email);
                throw;
            }
        }

        public void Dispose()
        {
            // No cerramos la conexión aquí, solo liberamos recursos si los hay
            _logger?.LogInformation("♻️ UsuarioDAO liberando recursos");
            // No llamar a _db.Dispose() aquí si estás usando una conexión compartida
        }
    }
}