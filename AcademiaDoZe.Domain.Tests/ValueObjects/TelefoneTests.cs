// Nicolas Vaz

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class TelefoneTests
{
    [Theory(DisplayName = "Telefone: formatos válidos")]
    [InlineData("(49) 99999-9999")]
    [InlineData("49999999999")]
    public void Deve_Criar_Telefone_Quando_Valido(string input)
    {
        // Act
        var result = Telefone.Criar(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("49999999999", result.Value!.Valor);
    }

    [Theory(DisplayName = "Telefone: nulo/vazio/espaços -> TELEFONE_OBRIGATORIO")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_Falhar_Quando_Telefone_Nulo_Ou_Vazio(string? input)
    {
        // Act
        var result = Telefone.Criar(input!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "TELEFONE_OBRIGATORIO");
    }

    [Theory(DisplayName = "Telefone: quantidade de dígitos inválida -> TELEFONE_DIGITOS")]
    [InlineData("123")]
    [InlineData("4999999999")]
    [InlineData("499999999999")]
    public void Deve_Falhar_Quando_Telefone_Possuir_Digitos_Invalidos(
        string input)
    {
        // Act
        var result = Telefone.Criar(input);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "TELEFONE_DIGITOS");
    }
}