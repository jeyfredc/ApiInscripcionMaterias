using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Student;
using ApiInscripcionMaterias.Models.DTOs.Teacher;
using Dapper;
using System.Data;

namespace ApiInscripcionMaterias.Models.DAO
{
    public class CourseDao
    {
        private readonly IDbConnection _db;
        private readonly ILogger<CourseDao> _logger;


        public CourseDao(ILogger<CourseDao> logger = null)
        {
            _logger = logger;
            _db = DatabaseConfig.GetConnection();
            _logger?.LogInformation("✅ courseDao inicializado");
        }
        public async Task<IEnumerable<CoursesResponseDto>> GetAvailableCourses()

        {


            try
            {
                var parameters = new DynamicParameters();
                var resultado = await _db.QueryAsync<CoursesResponseDto>(
                        "sp_ObtenerMateriasDisponibles",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                if (resultado == null)
                {
                    throw new ApplicationException("No se pudo completar la busqueda de materias disponibles");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ No se pudo completar la busqueda de materias disponibles");
                throw;
            }

        }

        public async Task<ResultCourseInscriptionDto> CourseInscriptionStudent(FormCourseRequestDto course)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@usuarioId", course.IdEstudiante, DbType.Int32);
                parameters.Add("@codigoMateria", course.CodigoMateria, DbType.String);

                parameters.Add("@resultado", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                parameters.Add("@mensaje", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                await _db.ExecuteAsync(
                    "sp_MatricularMateria",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var resultado = parameters.Get<bool>("@resultado");
                var mensaje = parameters.Get<string>("@mensaje");
                var returnValue = parameters.Get<int>("@ReturnValue");

                return new ResultCourseInscriptionDto
                {
                    Resultado = resultado,
                    Mensaje = $"{mensaje} Codigo Materia: {course.CodigoMateria}",
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error al realizar la inscripción");
                return new ResultCourseInscriptionDto
                {
                    Resultado = false,
                    Mensaje = "Ocurrió un error al procesar la inscripción"
                };
            }
        }
    }
}
