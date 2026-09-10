// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class EnderecoTests
{
    [Theory(DisplayName = "Endereco: criação válida com número e complemento")]
    [InlineData("10", "Bloco A")]
    [InlineData("1", "")]
    public void Deve_Criar_Endereco_Quando_Valido(
        string numero,
        string complemento)
    {
        var logradouro = Logradouro
            .Criar(
                1,
                "12345-678",
                "Rua Teste",
                "Bairro",
                "Cidade",
                "SP",
                "Brasil")
            .Value!;

        var result = Endereco.Criar(
            logradouro,
            numero,
            complemento);

        Assert.True(result.IsSuccess);
        Assert.Equal(logradouro.Id, result.Value!.LogradouroId);
        Assert.Equal(numero, result.Value.Numero);
        Assert.Equal(complemento, result.Value.Complemento);
    }

    [Theory(DisplayName = "Endereco: valida obrigatoriedade do logradouro e número")]
    [InlineData(null, "1", "LOGRADOURO_OBRIGATORIO")]
    [InlineData("valid", "", "NUMERO_OBRIGATORIO")]
    public void Deve_Falhar_Criacao_Quando_EnderecoInvalido(
        string logradouroCase,
        string numero,
        string expected)
    {
        Logradouro? logradouro = null;

        if (logradouroCase == "valid")
        {
            logradouro = Logradouro
                .Criar(
                    1,
                    "12345-678",
                    "Rua Teste",
                    "Bairro",
                    "Cidade",
                    "SP",
                    "Brasil")
                .Value!;
        }

        var result = Endereco.Criar(
            logradouro!,
            numero,
            "");

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == expected);
    }
}