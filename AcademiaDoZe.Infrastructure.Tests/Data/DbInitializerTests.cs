using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Infrastructure.Tests.Data;

public class DbInitializerTests : TestBase
{
    [Fact]
    public void Deve_Encontrar_Script_Do_SQL_Server()
    {
        // Act
        var script = DbInitializer.ObterScript(DatabaseType.SqlServer);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(script));
        Assert.Contains("tb_logradouro", script);
        Assert.Contains("tb_aluno", script);
        Assert.Contains("tb_colaborador", script);
        Assert.Contains("tb_matricula", script);
        Assert.Contains("tb_acesso", script);
    }
    [Fact]
    public async Task Deve_Conectar_E_Inicializar_Banco()
    {
        // Act
        await DbInitializer.InicializarAsync(
            ConnectionString,
            DatabaseType);

        // Assert
        Assert.True(true);
    }
}