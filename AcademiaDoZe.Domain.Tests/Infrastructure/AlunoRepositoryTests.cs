// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Domain.Tests.Infrastructure;

public class AlunoRepositoryTests
{
    private static Aluno CriarAluno(
        int id,
        string nome)
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
            nome,
            Cpf.Criar("12345678909").Value!,
            new DateOnly(2000, 1, 1),
            Telefone.Criar("(49) 99999-9999").Value!,
            Email.Criar($"{id}@email.com").Value!,
            endereco,
            Senha.Criar("Senha123").Value!,
            Arquivo.Criar("foto.jpg").Value!);
    }

    [Fact]
    public async Task Deve_Adicionar_Aluno()
    {
        var repository = new AlunoRepository();
        var aluno = CriarAluno(1, "Nicolas Vaz");

        var resultado = await repository.Adicionar(aluno);

        Assert.Equal(aluno, resultado);
    }

    [Fact]
    public async Task Deve_Obter_Aluno_Por_Id()
    {
        var repository = new AlunoRepository();
        var aluno = CriarAluno(1, "Nicolas Vaz");

        await repository.Adicionar(aluno);

        var resultado = await repository.ObterPorId(1);

        Assert.Equal(aluno, resultado);
    }

    [Fact]
    public async Task Deve_Obter_Alunos_Por_Nome()
    {
        var repository = new AlunoRepository();

        var primeiro = CriarAluno(1, "Nicolas Vaz");
        var segundo = CriarAluno(2, "João Silva");

        await repository.Adicionar(primeiro);
        await repository.Adicionar(segundo);

        var resultado = await repository.ObterPorNome("Nicolas");

        Assert.Single(resultado);
        Assert.Equal("Nicolas Vaz", resultado.First().Nome);
    }

    [Fact]
    public async Task Deve_Obter_Todos_Os_Alunos()
    {
        var repository = new AlunoRepository();

        await repository.Adicionar(CriarAluno(1, "Nicolas Vaz"));
        await repository.Adicionar(CriarAluno(2, "João Silva"));

        var resultado = await repository.ObterTodos();

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task Deve_Remover_Aluno()
    {
        var repository = new AlunoRepository();
        var aluno = CriarAluno(1, "Nicolas Vaz");

        await repository.Adicionar(aluno);

        var resultado = await repository.Remover(1);

        Assert.True(resultado);
        Assert.Null(await repository.ObterPorId(1));
    }
}
