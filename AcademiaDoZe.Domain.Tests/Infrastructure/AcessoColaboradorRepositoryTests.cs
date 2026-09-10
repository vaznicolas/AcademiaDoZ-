// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Infrastructure;

public class AcessoColaboradorRepositoryTests
{
    private static Colaborador CriarColaborador(int id)
    {
        var logradouro = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "Brasil").Value!;

        var endereco = Endereco.Criar(
            logradouro,
            "123",
            "Apto 101").Value!;

        return Colaborador.Criar(
            id,
            $"Colaborador {id}",
            Cpf.Criar("12345678909").Value!,
            new DateOnly(2000, 1, 1),
            Telefone.Criar("(49) 99999-9999").Value!,
            Email.Criar($"colaborador{id}@email.com").Value!,
            endereco,
            Senha.Criar("Senha123").Value!,
            Arquivo.Criar("foto.jpg").Value!,
            new DateOnly(2026, 1, 10),
            ColaboradorTipo.Instrutor,
            ColaboradorVinculo.CLT);
    }

    [Fact]
    public async Task Deve_Adicionar_Acesso_Colaborador()
    {
        var repository = new AcessoColaboradorRepository();
        var colaborador = CriarColaborador(1);

        var acesso = AcessoColaborador.Criar(
            1,
            colaborador,
            DateTime.Now,
            null);

        var resultado = await repository.Adicionar(acesso);

        Assert.Equal(acesso, resultado);
    }

    [Fact]
    public async Task Deve_Obter_Acessos_Por_Colaborador()
    {
        var repository = new AcessoColaboradorRepository();
        var colaborador = CriarColaborador(1);

        await repository.Adicionar(
            AcessoColaborador.Criar(
                1,
                colaborador,
                DateTime.Now,
                null));

        await repository.Adicionar(
            AcessoColaborador.Criar(
                2,
                colaborador,
                DateTime.Now.AddHours(-1),
                DateTime.Now));

        var resultado =
            await repository.ObterPorColaborador(colaborador.Id);

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task Deve_Obter_Acessos_Por_Periodo()
    {
        var repository = new AcessoColaboradorRepository();
        var colaborador = CriarColaborador(1);

        var inicio = new DateTime(2026, 1, 1);
        var fim = new DateTime(2026, 1, 31);

        await repository.Adicionar(
            AcessoColaborador.Criar(
                1,
                colaborador,
                new DateTime(2026, 1, 10),
                null));

        var resultado =
            await repository.ObterPorPeriodo(inicio, fim);

        Assert.Single(resultado);
    }

    [Fact]
    public async Task Deve_Obter_Acesso_Aberto()
    {
        var repository = new AcessoColaboradorRepository();
        var colaborador = CriarColaborador(1);

        var acesso = AcessoColaborador.Criar(
            1,
            colaborador,
            DateTime.Now,
            null);

        await repository.Adicionar(acesso);

        var resultado =
            await repository.ObterAberto(colaborador.Id);

        Assert.Equal(acesso, resultado);
    }

    [Fact]
    public async Task Deve_Obter_Acesso_Por_Data()
    {
        var repository = new AcessoColaboradorRepository();
        var colaborador = CriarColaborador(1);

        var data = new DateTime(2026, 1, 10, 8, 0, 0);

        var acesso = AcessoColaborador.Criar(
            1,
            colaborador,
            data,
            null);

        await repository.Adicionar(acesso);

        var resultado =
            await repository.ObterPorData(
                colaborador.Id,
                new DateTime(2026, 1, 10));

        Assert.Equal(acesso, resultado);
    }

    [Fact]
    public async Task Deve_Retornar_Nulo_Quando_Nao_Houver_Acesso_Aberto()
    {
        var repository = new AcessoColaboradorRepository();
        var colaborador = CriarColaborador(1);

        await repository.Adicionar(
            AcessoColaborador.Criar(
                1,
                colaborador,
                DateTime.Now,
                DateTime.Now.AddHours(1)));

        var resultado =
            await repository.ObterAberto(colaborador.Id);

        Assert.Null(resultado);
    }
}