// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Tests.Entities;

public class AcessoColaboradorTests
{
    [Fact]
    public void Deve_Criar_AcessoColaborador_Com_Dados_Validos()
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

        var entrada = new DateTime(2026, 8, 17, 8, 30, 0);

        var acesso = AcessoColaborador.Criar(
            1,
            colaborador,
            entrada,
            null);

        Assert.NotNull(acesso);
    }

    [Fact]
    public void Deve_Armazenar_Dados_Do_AcessoColaborador_Corretamente()
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

        var entrada = new DateTime(2026, 8, 17, 8, 30, 0);
        var saida = new DateTime(2026, 8, 17, 17, 30, 0);

        var acesso = AcessoColaborador.Criar(
            1,
            colaborador,
            entrada,
            saida);

        Assert.Equal(1, acesso.Id);
        Assert.Equal(colaborador, acesso.Colaborador);
        Assert.Equal(entrada, acesso.DataHoraEntrada);
        Assert.Equal(saida, acesso.DataHoraSaida);
    }
}