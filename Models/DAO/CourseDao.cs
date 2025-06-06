using ApiInscripcionMaterias.Models.DTOs;
using ApiInscripcionMaterias.Models.DTOs.Courses;
using ApiInscripcionMaterias.Models.DTOs.Student;
using Dapper;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public async Task<ResultCourseInscriptionDto> DeleteCourseByStudent(FormCourseRequestDto course)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@estudianteId", course.IdEstudiante, DbType.Int32);
                parameters.Add("@codigoMateria", course.CodigoMateria, DbType.String);

                parameters.Add("@resultado", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                parameters.Add("@mensaje", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                await _db.ExecuteAsync(
                    "sp_DesmatricularMateria",
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


        public async Task<ResultCourseInscriptionDto> RegisterNewCourse(RequestRegisterCourseDto newCourse)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Codigo", newCourse.Codigo, DbType.String);
                parameters.Add("@Nombre", newCourse.Nombre, DbType.String);
                parameters.Add("@Descripcion", newCourse.Descripcion, DbType.String);
                parameters.Add("@Creditos", newCourse.Creditos, DbType.Int32);
                parameters.Add("@Cupo_Maximo", newCourse.Cupo_Maximo, DbType.Int32);
                parameters.Add("@Activa", newCourse.Activa, DbType.Boolean);

                parameters.Add("@ReturnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                await _db.ExecuteAsync(
                    "sp_CrearMateria",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );


                var returnValue = parameters.Get<int>("@ReturnVal");

                var resultado = (int)returnValue;

                if(resultado < 0)
                {
                    return new ResultCourseInscriptionDto
                    {
                        Resultado = false,
                        Mensaje = "Error al registrar la nueva materia. Código ya existe o cupo máximo alcanzado."
                    };
                }
                else
                {
                    return new ResultCourseInscriptionDto
                    {
                        Resultado = true,
                        Mensaje = "La materia fue registrada exitosamente"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error al registrar la nueva materia");
                return new ResultCourseInscriptionDto
                {
                    Resultado = false,
                    Mensaje = "Ocurrió un error al registrar la nueva materia"
                };
            }
        }

        public async Task<IEnumerable<CourseWithoutAssignDto>> GetCoursesWithouthAssign()

        {
            try
            {
                var parameters = new DynamicParameters();
                var resultado = await _db.QueryAsync<CourseWithoutAssignDto>(
                        "sp_ObtenerMateriasNoAsignadas",
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


        public async Task<ResultCourseInscriptionDto> AssignCourseTeacher(RegisterCourseTeacherDto registerCourse)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProfesorId", registerCourse.ProfesorId, DbType.Int32);
                parameters.Add("@CodigoMateria", registerCourse.CodigoMateria, DbType.String);
                parameters.Add("@Horario", registerCourse.Horario, DbType.String);
                parameters.Add("@Grupo", registerCourse.Grupo, DbType.String);

                // Usamos QueryFirstOrDefaultAsync para capturar el resultado del SELECT
                var result = await _db.QueryFirstOrDefaultAsync<ResultCourseInscriptionDto>(
                    "sp_AsignarMateriaProfesor",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return new ResultCourseInscriptionDto
                    {
                        Resultado = false,
                        Mensaje = "No se recibió respuesta del servidor"
                    };
                }

                return new ResultCourseInscriptionDto
                {
                    Resultado = result.Resultado,
                    Mensaje = result.Mensaje
                };
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción que pueda ocurrir
                return new ResultCourseInscriptionDto
                {
                    Resultado = false,
                    Mensaje = ex.InnerException?.Message ?? ex.Message
                };
            }
        }


    }
}
