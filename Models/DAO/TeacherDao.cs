using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Teacher;
using Dapper;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiInscripcionMaterias.Models.DAO
{
    public class TeacherDao
    {
        private readonly IDbConnection _db;
        private readonly ILogger<TeacherDao> _logger;
    

            public TeacherDao(ILogger<TeacherDao> logger = null)
        {
            _logger = logger;
            _db = DatabaseConfig.GetConnection();
            _logger?.LogInformation("✅ TeacherDao inicializado");
        }

        public async Task<IEnumerable<TeacherResponseDto>> GetAssignedCourses(int userId)

        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@profesorId", userId, DbType.Int32);


                var resultado = await _db.QueryAsync<TeacherResponseDto>(
                        "sp_ObtenerMateriasPorProfesor",
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

        public async Task<ResultCourseInscriptionDto> UnassignTeacher(RequestUnassignTeacher unassignTeacher)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@ProfesorId", unassignTeacher.ProfesorId, DbType.Int32);
                parameters.Add("@CodigoMateria", unassignTeacher.CodigoMateria, DbType.String);


                var result = await _db.QueryFirstOrDefaultAsync<ResultCourseInscriptionDto>(
                                "sp_EliminarAsignacionMateriaProfesor",
                                parameters,
                                commandType: CommandType.StoredProcedure
                            );


                return new ResultCourseInscriptionDto
                {
                    Resultado = result.Resultado,
                    Mensaje = result.Mensaje,
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error al desasignar al profesor");
                return new ResultCourseInscriptionDto
                {
                    Resultado = false,
                    Mensaje = "Ocurrió un error al desasignar al profesor"
                };
            }
        }
    }
}
