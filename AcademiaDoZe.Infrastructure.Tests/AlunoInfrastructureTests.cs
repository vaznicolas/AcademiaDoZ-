// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AlunoInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;

    public AlunoInfrastructureTests()
    {
        _logradouroRepo =
            new LogradouroRepository(ConnectionString, DatabaseType);

        _alunoRepo =
            new AlunoRepository(ConnectionString, DatabaseType);
    }

    internal static async Task<Aluno> CriarEInserirAlunoAsync(
        AlunoRepository alunoRepo,
        LogradouroRepository logradouroRepo,
        string? nome = null)
    {
        var logradouro =
            await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(
                logradouroRepo);

        var aluno =
            Aluno.Criar(
                id: 0,
                nome: nome ?? "Aluno Teste " + Guid.NewGuid().ToString("N")[..5],
                cpf: Cpf.Criar(GerarCpf()).Value!,
                dataNascimento: new DateOnly(2000, 3, 20),
                telefone: Telefone.Criar(GerarTelefone()).Value!,
                email: Email.Criar(GerarEmail()).Value!,
                endereco: Endereco.Criar(logradouro, "100", "Casa").Value!,
                senha: Senha.Criar("SenhaValida123").Value!,
                foto: Arquivo.Criar("foto.jpg").Value!);

        return await alunoRepo.Adicionar(aluno);
    }

    [Fact]
    public async Task Aluno_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        Assert.NotNull(aluno);
        Assert.True(aluno.Id > 0);

        var obtido =
            await _alunoRepo.ObterPorId(aluno.Id);

        Assert.NotNull(obtido);
        Assert.Equal(aluno.Id, obtido.Id);
        Assert.Equal(aluno.Cpf.Valor, obtido.Cpf.Valor);
        Assert.Equal(aluno.Nome, obtido.Nome);
        Assert.Equal(aluno.Email.Valor, obtido.Email.Valor);
        Assert.NotNull(obtido.Endereco);
        Assert.Equal(aluno.Endereco.LogradouroId, obtido.Endereco.LogradouroId);
    }

    [Fact]
    public async Task Aluno_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtido =
            await _alunoRepo.ObterPorId(999999);

        Assert.Null(obtido);
    }

    [Fact]
    public async Task Aluno_ObterTodos_Sucesso()
    {
        await CriarEInserirAlunoAsync(
            _alunoRepo,
            _logradouroRepo);

        var todos =
            await _alunoRepo.ObterTodos();

        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task Aluno_Atualizar_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var novoLogradouro =
            await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(
                _logradouroRepo);

        var novoNome =
            "Aluno Editado " + Guid.NewGuid().ToString("N")[..5];

        var alunoAtualizado =
            Aluno.Criar(
                id: aluno.Id,
                nome: novoNome,
                cpf: aluno.Cpf,
                dataNascimento: aluno.DataNascimento,
                telefone: aluno.Telefone,
                email: aluno.Email,
                endereco: Endereco.Criar(novoLogradouro, "999", "Bloco B").Value!,
                senha: aluno.Senha,
                foto: aluno.Foto);

        var resultado =
            await _alunoRepo.Atualizar(alunoAtualizado);

        Assert.NotNull(resultado);
        Assert.Equal(novoNome, resultado.Nome);

        var noBanco =
            await _alunoRepo.ObterPorId(aluno.Id);

        Assert.NotNull(noBanco);
        Assert.Equal(novoNome, noBanco.Nome);
        Assert.Equal("999", noBanco.Endereco.Numero);
    }

    [Fact]
    public async Task Aluno_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var logradouro =
            await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(
                _logradouroRepo);

        var alunoInexistente =
            Aluno.Criar(
                id: 999999,
                nome: "Inexistente",
                cpf: Cpf.Criar(GerarCpf()).Value!,
                dataNascimento: new DateOnly(1990, 1, 1),
                telefone: Telefone.Criar(GerarTelefone()).Value!,
                email: Email.Criar(GerarEmail()).Value!,
                endereco: Endereco.Criar(logradouro, "1", "").Value!,
                senha: Senha.Criar("SenhaValida123").Value!,
                foto: Arquivo.Criar("foto.jpg").Value!);

        var ex =
            await Assert.ThrowsAsync<InfrastructureException>(
                () => _alunoRepo.Atualizar(alunoInexistente));

        Assert.Equal("REGISTRO_NAO_ENCONTRADO", ex.ErrorCode);
    }

    [Fact]
    public async Task Aluno_Remover_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var removido =
            await _alunoRepo.Remover(aluno.Id);

        Assert.True(removido);

        var noBanco =
            await _alunoRepo.ObterPorId(aluno.Id);

        Assert.Null(noBanco);
    }

    [Fact]
    public async Task Aluno_Remover_RetornaFalseQuandoInexistente()
    {
        var removido =
            await _alunoRepo.Remover(999999);

        Assert.False(removido);
    }

    [Fact]
    public async Task Aluno_ObterPorNome_FiltragemCorreta()
    {
        var nomeUnico =
            "AlunoUnico_" + Guid.NewGuid().ToString("N")[..8];

        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo,
                nome: nomeUnico);

        var resultados =
            await _alunoRepo.ObterPorNome(nomeUnico);

        Assert.NotNull(resultados);
        Assert.Contains(resultados, a => a.Id == aluno.Id);

        var resultadosVazio =
            await _alunoRepo.ObterPorNome(
                "NomeCompletamenteInexistente_" + Guid.NewGuid().ToString("N"));

        Assert.Empty(resultadosVazio);
    }
}