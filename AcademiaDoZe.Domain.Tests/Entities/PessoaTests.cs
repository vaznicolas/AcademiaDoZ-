// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Tests.Entities;

public class PessoaTests
{
    private class PessoaTeste : Pessoa
    {
        public PessoaTeste(
            int id,
            string nome,
            Cpf cpf,
            DateOnly dataNascimento,
            Telefone telefone,
            Email email,
            Endereco endereco,
            Senha senha,
            Arquivo foto)
            : base(
                id,
                nome,
                cpf,
                dataNascimento,
                telefone,
                email,
                endereco,
                senha,
                foto)
        {
        }
    }

    [Fact]
    public void Deve_Armazenar_Dados_Da_Pessoa_Corretamente()
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
        var emailResult = Email.Criar("pessoa@email.com");
        var senhaResult = Senha.Criar("Senha123");
        var arquivoResult = Arquivo.Criar("foto.jpg");

        var dataNascimento = new DateOnly(2000, 1, 1);

        var pessoa = new PessoaTeste(
            1,
            "Nicolas Vaz",
            cpfResult.Value!,
            dataNascimento,
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            arquivoResult.Value!);

        Assert.Equal(1, pessoa.Id);
        Assert.Equal("Nicolas Vaz", pessoa.Nome);
        Assert.Equal(cpfResult.Value, pessoa.Cpf);
        Assert.Equal(dataNascimento, pessoa.DataNascimento);
        Assert.Equal(telefoneResult.Value, pessoa.Telefone);
        Assert.Equal(emailResult.Value, pessoa.Email);
        Assert.Equal(enderecoResult.Value, pessoa.Endereco);
        Assert.Equal(senhaResult.Value, pessoa.Senha);
        Assert.Equal(arquivoResult.Value, pessoa.Foto);
    }
}