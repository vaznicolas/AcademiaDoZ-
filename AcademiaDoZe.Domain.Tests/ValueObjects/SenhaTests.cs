// Nicolas Vaz

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class SenhaTests
{
    [Fact(DisplayName = "Senha válida deve ser criada com sucesso")]
    public void Deve_Criar_Senha_Quando_Valida()
    {
        // Act
        var result = Senha.Criar("Senha123");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Senha123", result.Value!.Valor);
    }

    [Theory(DisplayName = "Senha nula/vazia -> SENHA_OBRIGATORIO")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_Falhar_Quando_Senha_For_Nula_Ou_Vazia(string? input)
    {
        // Act
        var result = Senha.Criar(input!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "SENHA_OBRIGATORIO");
    }

    [Theory(DisplayName = "Senha menor que 8 caracteres -> SENHA_TAMANHO")]
    [InlineData("Senha1")]
    [InlineData("Ab123")]
    public void Deve_Falhar_Quando_Senha_For_Muito_Curta(string input)
    {
        // Act
        var result = Senha.Criar(input);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "SENHA_TAMANHO");
    }

    [Fact(DisplayName = "Senha sem letra maiúscula -> SENHA_MAIUSCULA")]
    public void Deve_Falhar_Quando_Nao_Possuir_Maiuscula()
    {
        // Act
        var result = Senha.Criar("senha123");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "SENHA_MAIUSCULA");
    }

    [Fact(DisplayName = "Senha sem letra minúscula -> SENHA_MINUSCULA")]
    public void Deve_Falhar_Quando_Nao_Possuir_Minuscula()
    {
        // Act
        var result = Senha.Criar("SENHA123");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "SENHA_MINUSCULA");
    }

    [Fact(DisplayName = "Senha sem número -> SENHA_NUMERO")]
    public void Deve_Falhar_Quando_Nao_Possuir_Numero()
    {
        // Act
        var result = Senha.Criar("SenhaAbc");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "SENHA_NUMERO");
    }
}