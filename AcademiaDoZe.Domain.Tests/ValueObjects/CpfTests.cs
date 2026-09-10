// Nicolas Vaz

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class CpfTests
{
    [Theory(DisplayName = "Cpf: formatos válidos")]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void Deve_Criar_Cpf_Quando_Valido(string input)
    {
        // Act
        var result = Cpf.Criar(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("52998224725", result.Value!.Valor);
    }

    [Theory(DisplayName = "Cpf: nulo/vazio/espaços -> CPF_OBRIGATORIO")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_Falhar_Criacao_Quando_CpfNuloOuVazio(string? input)
    {
        // Act
        var result = Cpf.Criar(input!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CPF_OBRIGATORIO");
    }

    [Theory(DisplayName = "Cpf: quantidade de dígitos inválida -> CPF_DIGITOS")]
    [InlineData("123")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    public void Deve_Falhar_Criacao_Quando_CpfDigitosInvalidos(string input)
    {
        // Act
        var result = Cpf.Criar(input);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CPF_DIGITOS");
    }

    [Theory(DisplayName = "Cpf: CPF matematicamente inválido -> CPF_INVALIDO")]
    [InlineData("11111111111")]
    [InlineData("12345678901")]
    public void Deve_Falhar_Criacao_Quando_CpfInvalido(string input)
    {
        // Act
        var result = Cpf.Criar(input);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CPF_INVALIDO");
    }
}