// Nicolas Vaz

using System.Runtime.ConstrainedExecution;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.ValueObjects;

public class CepTests
{
    [Fact(DisplayName = "CEP válido com hífen deve ser criado com sucesso")]
    public void Deve_Criar_Cep_Valido_Com_Hifen()
    {
        // Arrange
        var valor = "12345-678";

        // Act
        var result = Cep.Criar(valor);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("12345678", result.Value!.Valor);
    }

    [Fact(DisplayName = "CEP válido sem hífen deve ser criado com sucesso")]
    public void Deve_Criar_Cep_Valido_Sem_Hifen()
    {
        // Arrange
        var valor = "12345678";

        // Act
        var result = Cep.Criar(valor);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("12345678", result.Value!.Valor);
    }

    [Theory(DisplayName = "CEP inválido deve retornar erro de quantidade de dígitos")]
    [InlineData("123")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    public void Deve_Falhar_Quando_Cep_Nao_Possuir_8_Digitos(string valor)
    {
        // Act
        var result = Cep.Criar(valor);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CEP_DIGITOS");
    }

    [Fact(DisplayName = "CEP vazio deve retornar erro")]
    public void Deve_Falhar_Quando_Cep_For_Vazio()
    {
        // Act
        var result = Cep.Criar("");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CEP_OBRIGATORIO");
    }

    [Fact(DisplayName = "CEP nulo deve retornar erro")]
    public void Deve_Falhar_Quando_Cep_For_Nulo()
    {
        // Act
        var result = Cep.Criar(null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CEP_OBRIGATORIO");
    }
}