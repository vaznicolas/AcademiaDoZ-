// Nicolas Vaz

using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Deve_Criar_Result_Com_Sucesso()
    {
        var resultado = Result<string>.Success("Academia do Zé");

        Assert.True(resultado.IsSuccess);
        Assert.False(resultado.IsFailure);
        Assert.Equal("Academia do Zé", resultado.Value);
        Assert.Empty(resultado.Notifications);
    }

    [Fact]
    public void Deve_Criar_Result_Com_Falha_Usando_Propriedade_E_Mensagem()
    {
        var resultado = Result<string>.Failure(
            "Nome",
            "NOME_OBRIGATORIO");

        Assert.False(resultado.IsSuccess);
        Assert.True(resultado.IsFailure);
        Assert.Null(resultado.Value);
        Assert.Single(resultado.Notifications);

        var notification = resultado.Notifications.First();

        Assert.Equal("Nome", notification.Propriedade);
        Assert.Equal("NOME_OBRIGATORIO", notification.Mensagem);
    }

    [Fact]
    public void Deve_Criar_Result_Com_Falha_Usando_Notification()
    {
        var notification = new Notification(
            "Email",
            "EMAIL_INVALIDO");

        var resultado = Result<string>.Failure(notification);

        Assert.False(resultado.IsSuccess);
        Assert.True(resultado.IsFailure);
        Assert.Null(resultado.Value);
        Assert.Single(resultado.Notifications);

        Assert.Equal(notification, resultado.Notifications.First());
    }

    [Fact]
    public void Deve_Criar_Result_Com_Multiplas_Notifications()
    {
        var notifications = new List<Notification>
        {
            new("Nome", "NOME_OBRIGATORIO"),
            new("Email", "EMAIL_INVALIDO")
        };

        var resultado = Result<string>.Failure(notifications);

        Assert.False(resultado.IsSuccess);
        Assert.True(resultado.IsFailure);
        Assert.Null(resultado.Value);
        Assert.Equal(2, resultado.Notifications.Count);
        Assert.Equal(notifications, resultado.Notifications);
    }

    [Fact]
    public void Deve_Manter_Notifications_Como_Colecao_Somente_Leitura()
    {
        var notifications = new List<Notification>
        {
            new("Nome", "NOME_OBRIGATORIO")
        };

        var resultado = Result<string>.Failure(notifications);

        Assert.IsAssignableFrom<IReadOnlyCollection<Notification>>(
            resultado.Notifications);

        Assert.Single(resultado.Notifications);
    }

}