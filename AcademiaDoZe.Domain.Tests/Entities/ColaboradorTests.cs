// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Tests.Entities;

public class ColaboradorTests
{
    [Fact]
    public void Deve_Criar_Colaborador_Com_Dados_Validos()
    {
        var logradouroResult = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "Brasil");

        var enderecoResult = Endereco.Criar(
            logradouroResult.Value!,
            "123",
            "Apto 101");

        var cpfResult = Cpf.Criar("12345678909");
        var telefoneResult = Telefone.Criar("(49) 99999-9999");
        var emailResult = Email.Criar("colaborador@email.com");
        var senhaResult = Senha.Criar("Senha123");
        var arquivoResult = Arquivo.Criar("foto.jpg");

        var colaborador = Colaborador.Criar(
            1,
            "Nicolas Vaz",
            cpfResult.Value!,
            new DateOnly(2000, 1, 1),
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            arquivoResult.Value!,
            new DateOnly(2026, 1, 10),
            ColaboradorTipo.Administrador,
            ColaboradorVinculo.CLT);

        Assert.NotNull(colaborador);
    }

    [Fact]
    public void Deve_Armazenar_Dados_Do_Colaborador_Corretamente()
    {
        var logradouroResult = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "Brasil");

        var enderecoResult = Endereco.Criar(
            logradouroResult.Value!,
            "123",
            "Apto 101");

        var cpfResult = Cpf.Criar("12345678909");
        var telefoneResult = Telefone.Criar("(49) 99999-9999");
        var emailResult = Email.Criar("colaborador@email.com");
        var senhaResult = Senha.Criar("Senha123");
        var arquivoResult = Arquivo.Criar("foto.jpg");

        var colaborador = Colaborador.Criar(
            1,
            "Nicolas Vaz",
            cpfResult.Value!,
            new DateOnly(2000, 1, 1),
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            arquivoResult.Value!,
            new DateOnly(2026, 1, 10),
            ColaboradorTipo.Administrador,
            ColaboradorVinculo.CLT);

        Assert.Equal(1, colaborador.Id);
        Assert.Equal("Nicolas Vaz", colaborador.Nome);
        Assert.Equal(new DateOnly(2000, 1, 1), colaborador.DataNascimento);
        Assert.Equal(cpfResult.Value, colaborador.Cpf);
        Assert.Equal(telefoneResult.Value, colaborador.Telefone);
        Assert.Equal(emailResult.Value, colaborador.Email);
        Assert.Equal(enderecoResult.Value, colaborador.Endereco);
        Assert.Equal(senhaResult.Value, colaborador.Senha);
        Assert.Equal(arquivoResult.Value, colaborador.Foto);
        Assert.Equal(new DateOnly(2026, 1, 10), colaborador.DataAdmissao);
        Assert.Equal(ColaboradorTipo.Administrador, colaborador.Tipo);
        Assert.Equal(ColaboradorVinculo.CLT, colaborador.Vinculo);
    }
}