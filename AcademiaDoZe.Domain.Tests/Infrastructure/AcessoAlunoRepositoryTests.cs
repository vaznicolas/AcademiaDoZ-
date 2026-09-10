// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Infrastructure;

public class AcessoAlunoRepositoryTests
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

    [Fact]
    public async Task Deve_Adicionar_Acesso_Aluno()
    {
        var repository = new AcessoAlunoRepository();
        var aluno = CriarAluno(1);

        var acesso = AcessoAluno.Criar(
            1,
            aluno,
            DateTime.Now,
            null);

        var resultado = await repository.Adicionar(acesso);

        Assert.Equal(acesso, resultado);
    }

    [Fact]
    public async Task Deve_Obter_Acessos_Por_Aluno()
    {
        var repository = new AcessoAlunoRepository();
        var aluno = CriarAluno(1);

        await repository.Adicionar(
            AcessoAluno.Criar(1, aluno, DateTime.Now, null));

        await repository.Adicionar(
            AcessoAluno.Criar(2, aluno, DateTime.Now.AddHours(-1), DateTime.Now));

        var resultado = await repository.ObterPorAluno(aluno.Id);

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task Deve_Obter_Acessos_Por_Periodo()
    {
        var repository = new AcessoAlunoRepository();
        var aluno = CriarAluno(1);

        var inicio = new DateTime(2026, 1, 1);
        var fim = new DateTime(2026, 1, 31);

        await repository.Adicionar(
            AcessoAluno.Criar(
                1,
                aluno,
                new DateTime(2026, 1, 10),
                null));

        var resultado = await repository.ObterPorPeriodo(inicio, fim);

        Assert.Single(resultado);
    }

    [Fact]
    public async Task Deve_Obter_Acesso_Aberto()
    {
        var repository = new AcessoAlunoRepository();
        var aluno = CriarAluno(1);

        var acesso = AcessoAluno.Criar(
            1,
            aluno,
            DateTime.Now,
            null);

        await repository.Adicionar(acesso);

        var resultado = await repository.ObterAberto(aluno.Id);

        Assert.Equal(acesso, resultado);
    }

    [Fact]
    public async Task Deve_Obter_Acesso_Por_Data()
    {
        var repository = new AcessoAlunoRepository();
        var aluno = CriarAluno(1);

        var data = new DateTime(2026, 1, 10, 8, 0, 0);

        var acesso = AcessoAluno.Criar(
            1,
            aluno,
            data,
            null);

        await repository.Adicionar(acesso);

        var resultado = await repository.ObterPorData(
            aluno.Id,
            new DateTime(2026, 1, 10));

        Assert.Equal(acesso, resultado);
    }

    [Fact]
    public async Task Deve_Retornar_Nulo_Quando_Nao_Houver_Acesso_Aberto()
    {
        var repository = new AcessoAlunoRepository();
        var aluno = CriarAluno(1);

        await repository.Adicionar(
            AcessoAluno.Criar(
                1,
                aluno,
                DateTime.Now,
                DateTime.Now.AddHours(1)));

        var resultado = await repository.ObterAberto(aluno.Id);

        Assert.Null(resultado);
    }
}
