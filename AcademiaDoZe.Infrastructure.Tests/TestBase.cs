// Nicolas Vaz

using AcademiaDoZe.Infrastructure.Data;

[assembly: CollectionBehavior(
    CollectionBehavior.CollectionPerAssembly,
    DisableTestParallelization = true)]

namespace AcademiaDoZe.Infrastructure.Tests;

public abstract class TestBase
{
    private const DatabaseType SelectedDatabaseType =
        DatabaseType.SqlServer;

    protected string ConnectionString { get; }

    protected DatabaseType DatabaseType { get; }

    protected TestBase()
    {
        DatabaseType =
            SelectedDatabaseType;

        ConnectionString =
     "Server=localhost,1433;" +
     "Database=db_academia_do_ze;" +
     "User Id=academia;" +
     $"Password={Environment.GetEnvironmentVariable("ACADEMIA_DB_PASSWORD")};" +
     "TrustServerCertificate=True;" +
     "Encrypt=True;";
    }

    #region Geradores de dados aleatórios

    private static int _counter = 10000;

    protected static string GerarCep() =>
        (
            80000000 +
            ((int)(DateTime.UtcNow.Ticks % 8000000)) +
            Interlocked.Increment(ref _counter)
        )
        .ToString("D8")[..8];

    protected static string GerarCpf() =>
        (
            10000000000L +
            ((DateTime.UtcNow.Ticks % 8000000000L)) +
            Interlocked.Increment(ref _counter)
        )
        .ToString("D11")[..11];

    protected static string GerarEmail() =>
        $"user_{Guid.NewGuid().ToString("N")[..8]}@test.com";

    protected static string GerarTelefone() =>
        (
            49990000000L +
            ((DateTime.UtcNow.Ticks % 8000000000L)) +
            Interlocked.Increment(ref _counter)
        )
        .ToString("D11")[..11];

    #endregion
}