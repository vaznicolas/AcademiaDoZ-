// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Tests.Entities;

public class AlunoTests
{
    [Fact]
    public void Deve_Criar_Aluno_Com_Dados_Validos()
    {
        var logradouroResult = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "Brasil");

        Assert.True(logradouroResult.IsSuccess);

        var enderecoResult = Endereco.Criar(
            logradouroResult.Value!,
            "123",
            "Apto 101");

        Assert.True(enderecoResult.IsSuccess);

        var cpfResult = Cpf.Criar("12345678909");
        var telefoneResult = Telefone.Criar("(49) 99999-9999");
        var emailResult = Email.Criar("aluno@email.com");
        var senhaResult = Senha.Criar("Senha123");
        var arquivoResult = Arquivo.Criar("foto.jpg");

        Assert.True(cpfResult.IsSuccess);
        Assert.True(telefoneResult.IsSuccess);
        Assert.True(emailResult.IsSuccess);
        Assert.True(senhaResult.IsSuccess);
        Assert.True(arquivoResult.IsSuccess);

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

        Assert.NotNull(aluno);
    }

    [Fact]
    public void Deve_Armazenar_Dados_Do_Aluno_Corretamente()
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

        Assert.Equal(1, aluno.Id);
        Assert.Equal("Nicolas Vaz", aluno.Nome);
        Assert.Equal(new DateOnly(2000, 1, 1), aluno.DataNascimento);
        Assert.Equal(cpfResult.Value, aluno.Cpf);
        Assert.Equal(telefoneResult.Value, aluno.Telefone);
        Assert.Equal(emailResult.Value, aluno.Email);
        Assert.Equal(enderecoResult.Value, aluno.Endereco);
        Assert.Equal(senhaResult.Value, aluno.Senha);
        Assert.Equal(arquivoResult.Value, aluno.Foto);
    }

}

