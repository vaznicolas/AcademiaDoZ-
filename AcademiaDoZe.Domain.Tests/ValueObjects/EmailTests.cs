// Nicolas Vaz

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory(DisplayName = "Email: formatos válidos")]
    [InlineData("teste@email.com")]
    [InlineData("nicolas.vaz@email.com")]
    public void Deve_Criar_Email_Quando_Valido(string input)
    {
        // Act
        var result = Email.Criar(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(input, result.Value!.Valor);
    }

    [Theory(DisplayName = "Email: nulo/vazio/espaços -> EMAIL_OBRIGATORIO")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_Falhar_Quando_Email_Nulo_Ou_Vazio(string? input)
    {
        // Act
        var result = Email.Criar(input!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "EMAIL_OBRIGATORIO");
    }

    [Theory(DisplayName = "Email: formato inválido -> EMAIL_INVALIDO")]
    [InlineData("teste")]
    [InlineData("teste@")]
    [InlineData("@email.com")]
    [InlineData("teste@email")]
    public void Deve_Falhar_Quando_Email_For_Invalido(string input)
    {
        // Act
        var result = Email.Criar(input);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "EMAIL_INVALIDO");
    }
}