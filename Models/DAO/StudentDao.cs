using ApiInscripcionMaterias.Models.DTOs.Student;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ApiInscripcionMaterias.Models.DAO
{
    public class StudentDao
    {
        private readonly IDbConnection _db;
        private readonly ILogger<StudentDao> _logger;

        public StudentDao(ILogger<StudentDao> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _db = DatabaseConfig.GetConnection();
            _logger.LogInformation("✅ StudentDao inicializado");
        }

        public async Task<StudentCreditsDto> GetStudentCredits(int userId)
        {
            try
            {
                _logger.LogInformation("Consultando créditos para el estudiante ID: {UserId}", userId);

                var parameters = new DynamicParameters();
                parameters.Add("@usuarioId", userId, DbType.Int32);

                var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_ObtenerCreditosEstudiante",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    _logger.LogWarning("No se encontraron créditos para el estudiante ID: {UserId}", userId);
                    return null;
                }

                // Mapeamos el resultado dinámico a nuestro DTO
                var studentCredits = new StudentCreditsDto
                {
                    EstudianteId = result.EstudianteId,
                    CreditosDisponibles = result.creditos_disponibles
                };

                _logger.LogInformation("Créditos obtenidos exitosamente para el estudiante ID: {UserId}", userId);
                return studentCredits;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener créditos para el estudiante ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<StudentCoursesDto>> GetCoursesByStudentId(int StudentId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@estudianteId", StudentId, DbType.Int32);

                var result = await _db.QueryAsync<dynamic>(
                    "sp_ObtenerHorarioEstudiante",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null || !result.Any())
                {
                    return new List<StudentCoursesDto>();
                }

                var coursesStudent = result.Select(c => new StudentCoursesDto
                {
                    Codigomateria = c.CodigoMateria,
                    Materia = c.Materia,
                    Profesor = c.Profesor,
                    Horario = c.horario, 
                    FehaInscripcion = c.FechaInscripcion
                }).ToList();

                _logger.LogInformation("Cursos obtenidos exitosamente para el estudiante ID: {StudentId}", StudentId);
                return coursesStudent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener cursos para el estudiante ID: {StudentId}", StudentId);
                throw;
            }
        }

    }
}