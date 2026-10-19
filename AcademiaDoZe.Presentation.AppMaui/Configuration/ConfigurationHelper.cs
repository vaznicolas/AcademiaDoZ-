
// Nicolas Vaz
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var config = new RepositoryConfig
        {
            ConnectionString = ObterConnectionString(),
            DatabaseType = AppDatabaseType.SqlServer.ToInfrastructure()
        };

        services.AddSingleton(config);
        services.AddApplicationServices();

        WeakReferenceMessenger.Default.Register<RepositoryConfig, BancoPreferencesUpdatedMessage>(
            config,
            (r, m) =>
            {
                r.ConnectionString = ObterConnectionString();
                r.DatabaseType = AppDatabaseType.SqlServer.ToInfrastructure();
            });
    }

    private static string ObterConnectionString()
    {
        const string servidorPadrao = "localhost,1433";
        const string bancoPadrao = "db_academia_do_ze";
        const string usuarioPadrao = "academia";

        string servidor = Preferences.Get("SqlServer_Servidor", servidorPadrao);
        string banco = Preferences.Get("SqlServer_Banco", bancoPadrao);
        string usuario = Preferences.Get("SqlServer_Usuario", usuarioPadrao);

        string senhaPadrao =
            Environment.GetEnvironmentVariable("ACADEMIA_DB_PASSWORD") ?? string.Empty;

        string senha = Preferences.Get("SqlServer_Senha", senhaPadrao);

        if (string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "Configure a senha do SQL Server nas configurações ou na variável de ambiente ACADEMIA_DB_PASSWORD.");
        }

        return $"Server={servidor};" +
               $"Database={banco};" +
               $"User Id={usuario};" +
               $"Password={senha};" +
               "TrustServerCertificate=True;" +
               "Encrypt=True;" +
               "Connect Timeout=5;";
    }
}
