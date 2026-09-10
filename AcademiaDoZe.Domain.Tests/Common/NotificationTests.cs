// Nicolas Vaz

using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Tests.Common;

public class NotificationTests
{
    [Fact]
    public void Deve_Criar_Notification_Com_Propriedade_E_Mensagem()
    {
        var notification = new Notification(
            "Nome",
            "NOME_OBRIGATORIO");

        Assert.Equal("Nome", notification.Propriedade);
        Assert.Equal("NOME_OBRIGATORIO", notification.Mensagem);
    }

    [Fact]
    public void Deve_Considerar_Notifications_Iguais_Quando_Tiverem_Mesmos_Dados()
    {
        var primeira = new Notification(
            "Nome",
            "NOME_OBRIGATORIO");

        var segunda = new Notification(
            "Nome",
            "NOME_OBRIGATORIO");

        Assert.Equal(primeira, segunda);
    }

    [Fact]
    public void Deve_Considerar_Notifications_Diferentes_Quando_Tiverem_Dados_Diferentes()
    {
        var primeira = new Notification(
            "Nome",
            "NOME_OBRIGATORIO");

        var segunda = new Notification(
            "Email",
            "EMAIL_OBRIGATORIO");

        Assert.NotEqual(primeira, segunda);
    }
}