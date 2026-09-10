// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Tests.Entities;

public class MatriculaTests
{
    [Fact]
    public void Deve_Criar_Matricula_Com_Dados_Validos()
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
        var emailResult = Email.Criar("aluno@email.com");
        var senhaResult = Senha.Criar("Senha123");
        var arquivoResult = Arquivo.Criar("foto.jpg");

        var aluno = Aluno.Criar(
            1,
            "Nicolas Vaz",
            cpfResult.Value!,
            new DateOnly(2000, 1, 1),
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            arquivoResult.Value!);

        var matricula = Matricula.Criar(
            1,
            aluno,
            MatriculaPlano.Mensal,
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 2, 10),
            "Ganhar massa muscular",
            MatriculaRestricoes.None,
            "",
            arquivoResult.Value!);

        Assert.NotNull(matricula);
    }

    [Fact]
    public void Deve_Armazenar_Dados_Da_Matricula_Corretamente()
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
        var emailResult = Email.Criar("aluno@email.com");
        var senhaResult = Senha.Criar("Senha123");
        var arquivoResult = Arquivo.Criar("foto.jpg");

        var aluno = Aluno.Criar(
            1,
            "Nicolas Vaz",
            cpfResult.Value!,
            new DateOnly(2000, 1, 1),
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            arquivoResult.Value!);

        var matricula = Matricula.Criar(
            1,
            aluno,
            MatriculaPlano.Mensal,
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 2, 10),
            "Ganhar massa muscular",
            MatriculaRestricoes.None,
            "",
            arquivoResult.Value!);

        Assert.Equal(1, matricula.Id);
        Assert.Equal(aluno, matricula.Aluno);
        Assert.Equal(MatriculaPlano.Mensal, matricula.Plano);
        Assert.Equal(new DateOnly(2026, 1, 10), matricula.DataInicio);
        Assert.Equal(new DateOnly(2026, 2, 10), matricula.DataFinal);
        Assert.Equal("Ganhar massa muscular", matricula.Objetivo);
        Assert.Equal(MatriculaRestricoes.None, matricula.Restricoes);
        Assert.Equal("", matricula.ObservacoesRestricoes);
        Assert.Equal(arquivoResult.Value, matricula.LaudoMedico);
    }
}