using Microsoft.Data.SqlClient;
using System.Data;

public static class DatabaseConfig
{
    private static string _connectionString;
    private static readonly ILogger _logger;

    static DatabaseConfig()
    {
        // Configurar logger
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        _logger = loggerFactory.CreateLogger("DatabaseConfig");
    }

    public static IDbConnection GetConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
            }

            _logger?.LogInformation("✅ Cadena de conexión configurada");
        }

        var connection = new SqlConnection(_connectionString);
        return connection;
    }
}