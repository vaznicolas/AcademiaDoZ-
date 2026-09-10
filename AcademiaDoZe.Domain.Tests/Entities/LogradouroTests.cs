// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Tests.Entities;

public class LogradouroTests
{
    [Fact]
    public void Deve_Criar_Logradouro_Com_Dados_Validos()
    {
        var resultado = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "Brasil");

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);

        var logradouro = resultado.Value!;

        Assert.Equal(1, logradouro.Id);
        Assert.Equal("88500000", logradouro.Cep.Valor);
        Assert.Equal("Rua das Flores", logradouro.Nome);
        Assert.Equal("Centro", logradouro.Bairro);
        Assert.Equal("Lages", logradouro.Cidade);
        Assert.Equal("SC", logradouro.Estado);
        Assert.Equal("Brasil", logradouro.Pais);
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Nome_For_Vazio()
    {
        var resultado = Logradouro.Criar(
            1,
            "88500000",
            "",
            "Centro",
            "Lages",
            "SC",
            "Brasil");

        Assert.False(resultado.IsSuccess);

        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "NOME_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Bairro_For_Vazio()
    {
        var resultado = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "",
            "Lages",
            "SC",
            "Brasil");

        Assert.False(resultado.IsSuccess);

        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "BAIRRO_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Cidade_For_Vazia()
    {
        var resultado = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "",
            "SC",
            "Brasil");

        Assert.False(resultado.IsSuccess);

        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "CIDADE_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Estado_For_Vazio()
    {
        var resultado = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "",
            "Brasil");

        Assert.False(resultado.IsSuccess);

        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "ESTADO_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Pais_For_Vazio()
    {
        var resultado = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "");

        Assert.False(resultado.IsSuccess);

        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "PAIS_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Cep_For_Invalido()
    {
        var resultado = Logradouro.Criar(
            1,
            "123",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "Brasil");

        Assert.False(resultado.IsSuccess);

        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "CEP_DIGITOS");
    }

    [Fact]
    public void Deve_Limpar_Espacos_Dos_Dados_Textuais()
    {
        var resultado = Logradouro.Criar(
            1,
            "88500000",
            "   Rua     das     Flores   ",
            "   Centro   ",
            "   Lages   ",
            "   SC   ",
            "   Brasil   ");

        Assert.True(resultado.IsSuccess);

        var logradouro = resultado.Value!;

        Assert.Equal("Rua das Flores", logradouro.Nome);
        Assert.Equal("Centro", logradouro.Bairro);
        Assert.Equal("Lages", logradouro.Cidade);
        Assert.Equal("SC", logradouro.Estado);
        Assert.Equal("Brasil", logradouro.Pais);
    }
}