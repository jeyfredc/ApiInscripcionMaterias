using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace ApiInscripcionMaterias.Data
{
    public static class DbConnectionHelper
    {
        private static string _connectionString;

        public static void Initialize(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection' en la configuración.");
                }
            }
        }

        public static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no ha sido inicializada. Llame a Initialize() primero.");
            }
            return _connectionString;
        }

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }
}