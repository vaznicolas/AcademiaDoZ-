// Nicolas Vaz

using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static void ConfigureServices(
        IServiceCollection services)
    {
        var databaseType = AppDatabaseType.SqlServer;

        const string dbServer = "localhost,1433";
        const string dbDatabase = "db_academia_do_ze";
        const string dbUser = "academia";

        var dbPassword = Environment.GetEnvironmentVariable(
            "ACADEMIA_DB_PASSWORD");

        if (string.IsNullOrWhiteSpace(dbPassword))
        {
            throw new InvalidOperationException(
                "A variável de ambiente ACADEMIA_DB_PASSWORD não foi configurada.");
        }

        var connectionString =
            $"Server={dbServer};" +
            $"Database={dbDatabase};" +
            $"User Id={dbUser};" +
            $"Password={dbPassword};" +
            "TrustServerCertificate=True;" +
            "Encrypt=True;" +
            "Connect Timeout=5;";

        services.AddSingleton(new RepositoryConfig
        {
            ConnectionString = connectionString,
            DatabaseType = databaseType.ToInfrastructure()
        });

        services.AddApplicationServices();
    }
}