// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Infrastructure;

public class MatriculaRepositoryTests
{
    private static Aluno CriarAluno(int id)
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

        return Aluno.Criar(
            id,
            $"Aluno {id}",
            Cpf.Criar("12345678909").Value!,
            new DateOnly(2000, 1, 1),
            Telefone.Criar("(49) 99999-9999").Value!,
            Email.Criar($"aluno{id}@email.com").Value!,
            endereco,
            Senha.Criar("Senha123").Value!,
            Arquivo.Criar("foto.jpg").Value!);
    }

    private static Matricula CriarMatricula(
        int id,
        Aluno aluno,
        MatriculaPlano plano,
        DateOnly inicio,
        DateOnly final)
    {
        return Matricula.Criar(
            id,
            aluno,
            plano,
            inicio,
            final,
            "Ganhar massa muscular",
            MatriculaRestricoes.None,
            "",
            Arquivo.Criar("laudo.jpg").Value!);
    }

    [Fact]
    public async Task Deve_Obter_Matriculas_Por_Aluno()
    {
        var repository = new MatriculaRepository();
        var aluno = CriarAluno(1);

        await repository.Adicionar(
            CriarMatricula(
                1,
                aluno,
                MatriculaPlano.Mensal,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 2, 1)));

        var resultado =
            await repository.ObterPorAluno(aluno.Id);

        Assert.Single(resultado);
    }

    [Fact]
    public async Task Deve_Obter_Matriculas_Por_Plano()
    {
        var repository = new MatriculaRepository();
        var aluno = CriarAluno(1);

        await repository.Adicionar(
            CriarMatricula(
                1,
                aluno,
                MatriculaPlano.Mensal,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 2, 1)));

        var resultado =
            await repository.ObterPorPlano(MatriculaPlano.Mensal);

        Assert.Single(resultado);
    }

    [Fact]
    public async Task Deve_Obter_Matriculas_Por_Periodo()
    {
        var repository = new MatriculaRepository();
        var aluno = CriarAluno(1);

        await repository.Adicionar(
            CriarMatricula(
                1,
                aluno,
                MatriculaPlano.Mensal,
                new DateOnly(2026, 1, 10),
                new DateOnly(2026, 2, 10)));

        var resultado =
            await repository.ObterPorPeriodo(
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 31));

        Assert.Single(resultado);
    }

    [Fact]
    public async Task Deve_Obter_Matriculas_Ativas()
    {
        var repository = new MatriculaRepository();
        var aluno = CriarAluno(1);

        var hoje = DateOnly.FromDateTime(DateTime.Today);

        await repository.Adicionar(
            CriarMatricula(
                1,
                aluno,
                MatriculaPlano.Mensal,
                hoje.AddDays(-10),
                hoje.AddDays(10)));

        var resultado = await repository.ObterAtivas();

        Assert.Single(resultado);
    }

    [Fact]
    public async Task Deve_Verificar_Se_Aluno_Possui_Matricula_Ativa()
    {
        var repository = new MatriculaRepository();
        var aluno = CriarAluno(1);

        var hoje = DateOnly.FromDateTime(DateTime.Today);

        await repository.Adicionar(
            CriarMatricula(
                1,
                aluno,
                MatriculaPlano.Mensal,
                hoje.AddDays(-10),
                hoje.AddDays(10)));

        var resultado =
            await repository.AlunoPossuiMatriculaAtiva(aluno.Id);

        Assert.True(resultado);
    }

    [Fact]
    public async Task Deve_Retornar_Falso_Quando_Aluno_Nao_Possuir_Matricula_Ativa()
    {
        var repository = new MatriculaRepository();
        var aluno = CriarAluno(1);

        var hoje = DateOnly.FromDateTime(DateTime.Today);

        await repository.Adicionar(
            CriarMatricula(
                1,
                aluno,
                MatriculaPlano.Mensal,
                hoje.AddDays(-30),
                hoje.AddDays(-10)));

        var resultado =
            await repository.AlunoPossuiMatriculaAtiva(aluno.Id);

        Assert.False(resultado);
    }
}