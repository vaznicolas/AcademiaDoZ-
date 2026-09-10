// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Tests.Entities;

public class AcessoAlunoTests
{
    [Fact]
    public void Deve_Criar_AcessoAluno_Com_Dados_Validos()
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

        var entrada = new DateTime(2026, 8, 17, 8, 30, 0);

        var acesso = AcessoAluno.Criar(
            1,
            aluno,
            entrada,
            null);

        Assert.NotNull(acesso);
    }

    [Fact]
    public void Deve_Armazenar_Dados_Do_AcessoAluno_Corretamente()
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

        var entrada = new DateTime(2026, 8, 17, 8, 30, 0);
        var saida = new DateTime(2026, 8, 17, 10, 15, 0);

        var acesso = AcessoAluno.Criar(
            1,
            aluno,
            entrada,
            saida);

        Assert.Equal(1, acesso.Id);
        Assert.Equal(aluno, acesso.Aluno);
        Assert.Equal(entrada, acesso.DataHoraEntrada);
        Assert.Equal(saida, acesso.DataHoraSaida);
    }
}