// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;
    private readonly MatriculaRepository _matriculaRepo;

    public MatriculaInfrastructureTests()
    {
        _logradouroRepo =
            new LogradouroRepository(
                ConnectionString,
                DatabaseType);

        _alunoRepo =
            new AlunoRepository(
                ConnectionString,
                DatabaseType);

        _matriculaRepo =
            new MatriculaRepository(
                ConnectionString,
                DatabaseType);
    }

    private async Task<Matricula> CriarEInserirMatriculaAsync(
        Aluno aluno,
        MatriculaPlano plano = MatriculaPlano.Mensal,
        DateOnly? dataInicio = null,
        MatriculaRestricoes restricoes = MatriculaRestricoes.None,
        string obsRestricao = "")
    {
        var inicio =
            dataInicio ??
            DateOnly.FromDateTime(DateTime.Today);

        var dataFinal =
            plano switch
            {
                MatriculaPlano.Mensal =>
                    inicio.AddMonths(1),

                MatriculaPlano.Trimestral =>
                    inicio.AddMonths(3),

                MatriculaPlano.Semestral =>
                    inicio.AddMonths(6),

                MatriculaPlano.Anual =>
                    inicio.AddYears(1),

                _ => inicio.AddMonths(1)
            };

        var laudo =
            Arquivo.Criar(
                "Laudo medico - Nicolas Vaz")
            .Value!;

        var matricula =
            Matricula.Criar(
                id: 0,
                aluno: aluno,
                plano: plano,
                dataInicio: inicio,
                dataFinal: dataFinal,
                objetivo: "Nicolas Vaz",
                restricoes: restricoes,
                observacoesRestricoes:
                    string.IsNullOrWhiteSpace(obsRestricao)
                        ? "SQLServer"
                        : obsRestricao,
                laudoMedico: laudo);

        return await _matriculaRepo.Adicionar(
            matricula);
    }

    [Fact]
    public async Task Matricula_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        var inserida =
            await CriarEInserirMatriculaAsync(
                aluno,
                MatriculaPlano.Mensal,
                restricoes:
                    MatriculaRestricoes.Diabetes |
                    MatriculaRestricoes.PressaoAlta,
                obsRestricao: "SQLServer");

        Assert.NotNull(inserida);
        Assert.True(inserida.Id > 0);
        Assert.Equal(aluno.Id, inserida.Aluno.Id);
        Assert.Equal(
            MatriculaPlano.Mensal,
            inserida.Plano);

        var obtida =
            await _matriculaRepo.ObterPorId(
                inserida.Id);

        Assert.NotNull(obtida);
        Assert.Equal(
            inserida.Id,
            obtida.Id);

        Assert.Equal(
            aluno.Id,
            obtida.Aluno.Id);

        Assert.Equal(
            MatriculaPlano.Mensal,
            obtida.Plano);

        Assert.Equal(
            "Nicolas Vaz",
            obtida.Objetivo);

        Assert.Equal(
            "SQLServer",
            obtida.ObservacoesRestricoes);

        Assert.NotNull(
            obtida.LaudoMedico);

        Assert.Equal(
            inserida.LaudoMedico.Valor,
            obtida.LaudoMedico.Valor);
    }

    [Fact]
    public async Task Matricula_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtida =
            await _matriculaRepo.ObterPorId(
                999999);

        Assert.Null(obtida);
    }

    [Fact]
    public async Task Matricula_ObterTodos_Sucesso()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        await CriarEInserirMatriculaAsync(aluno);

        var todas =
            await _matriculaRepo.ObterTodos();

        Assert.NotNull(todas);
        Assert.NotEmpty(todas);
    }

    [Fact]
    public async Task Matricula_Atualizar_Sucesso()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        var inserida =
            await CriarEInserirMatriculaAsync(
                aluno,
                MatriculaPlano.Mensal,
                restricoes:
                    MatriculaRestricoes.Alergias);

        var atualizada =
            Matricula.Criar(
                id: inserida.Id,
                aluno: aluno,
                plano: MatriculaPlano.Anual,
                dataInicio: inserida.DataInicio,
                dataFinal:
                    inserida.DataInicio.AddYears(1),
                objetivo: "Nicolas Vaz",
                restricoes:
                    MatriculaRestricoes.Alergias |
                    MatriculaRestricoes.Diabetes |
                    MatriculaRestricoes.Labirintite,
                observacoesRestricoes:
                    "SQLServer",
                laudoMedico:
                    Arquivo.Criar(
                        "Laudo atualizado - Nicolas Vaz")
                    .Value!);

        var resultado =
            await _matriculaRepo.Atualizar(
                atualizada);

        Assert.NotNull(resultado);

        Assert.Equal(
            MatriculaPlano.Anual,
            resultado.Plano);

        Assert.Equal(
            "Nicolas Vaz",
            resultado.Objetivo);

        var noBanco =
            await _matriculaRepo.ObterPorId(
                inserida.Id);

        Assert.NotNull(noBanco);

        Assert.Equal(
            MatriculaPlano.Anual,
            noBanco.Plano);

        Assert.Equal(
            "Nicolas Vaz",
            noBanco.Objetivo);

        Assert.True(
            noBanco.Restricoes.HasFlag(
                MatriculaRestricoes.Alergias));

        Assert.True(
            noBanco.Restricoes.HasFlag(
                MatriculaRestricoes.Diabetes));

        Assert.True(
            noBanco.Restricoes.HasFlag(
                MatriculaRestricoes.Labirintite));
    }

    [Fact]
    public async Task Matricula_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        var matricula =
            Matricula.Criar(
                id: 999999,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                dataFinal:
                    DateOnly.FromDateTime(
                        DateTime.Today.AddMonths(1)),
                objetivo: "Nicolas Vaz",
                restricoes:
                    MatriculaRestricoes.None,
                observacoesRestricoes:
                    "SQLServer",
                laudoMedico:
                    Arquivo.Criar(
                        "Laudo medico - Nicolas Vaz")
                    .Value!);

        var ex =
            await Assert.ThrowsAsync<
                InfrastructureException>(
                () =>
                    _matriculaRepo.Atualizar(
                        matricula));

        Assert.Equal(
            "REGISTRO_NAO_ENCONTRADO",
            ex.ErrorCode);
    }

    [Fact]
    public async Task Matricula_Remover_Sucesso()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        var inserida =
            await CriarEInserirMatriculaAsync(aluno);

        var removida =
            await _matriculaRepo.Remover(
                inserida.Id);

        Assert.True(removida);

        var noBanco =
            await _matriculaRepo.ObterPorId(
                inserida.Id);

        Assert.Null(noBanco);
    }

    [Fact]
    public async Task Matricula_Remover_RetornaFalseQuandoInexistente()
    {
        var removida =
            await _matriculaRepo.Remover(
                999999);

        Assert.False(removida);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_FiltragemCorreta()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        await CriarEInserirMatriculaAsync(aluno);

        var matriculas =
            await _matriculaRepo.ObterPorAluno(
                aluno.Id);

        Assert.NotNull(matriculas);
        Assert.NotEmpty(matriculas);

        Assert.All(
            matriculas,
            m =>
                Assert.Equal(
                    aluno.Id,
                    m.Aluno.Id));
    }

    [Fact]
    public async Task Matricula_ObterMatriculaAtivaPorAluno_E_PossuiMatriculaAtiva()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        var possuiAntes =
            await _matriculaRepo
                .PossuiMatriculaAtiva(
                    aluno.Id);

        Assert.False(possuiAntes);

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Mensal);

        var possuiDepois =
            await _matriculaRepo
                .PossuiMatriculaAtiva(
                    aluno.Id);

        Assert.True(possuiDepois);

        var ativa =
            await _matriculaRepo
                .ObterMatriculaAtivaPorAluno(
                    aluno.Id);

        Assert.NotNull(ativa);

        Assert.Equal(
            aluno.Id,
            ativa.Aluno.Id);
    }

    [Fact]
    public async Task Matricula_ObterAtivas_FiltragemCorreta()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Semestral);

        var ativasGeral =
            await _matriculaRepo.ObterAtivas();

        Assert.NotNull(ativasGeral);
        Assert.NotEmpty(ativasGeral);

        var ativasPorAluno =
            await _matriculaRepo.ObterAtivas(
                aluno.Id);

        Assert.NotNull(ativasPorAluno);
        Assert.NotEmpty(ativasPorAluno);

        Assert.All(
            ativasPorAluno,
            m =>
                Assert.Equal(
                    aluno.Id,
                    m.Aluno.Id));
    }

    [Fact]
    public async Task Matricula_ObterVencendoEmDias_RetornaMatriculasProximasDoVencimento()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        var inicio =
            DateOnly.FromDateTime(
                DateTime.Today.AddDays(-25));

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Mensal,
            inicio);

        var vencendo =
            await _matriculaRepo
                .ObterVencendoEmDias(30);

        Assert.NotNull(vencendo);

        Assert.Contains(
            vencendo,
            m =>
                m.Aluno.Id == aluno.Id);
    }

    [Fact]
    public async Task Matricula_ObterPorPlano_FiltragemCorreta()
    {
        var aluno =
            await AlunoInfrastructureTests
                .CriarEInserirAlunoAsync(
                    _alunoRepo,
                    _logradouroRepo);

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Trimestral);

        var trimestrais =
            await _matriculaRepo
                .ObterPorPlano(
                    MatriculaPlano.Trimestral);

        Assert.NotNull(trimestrais);

        Assert.Contains(
            trimestrais,
            m =>
                m.Aluno.Id == aluno.Id &&
                m.Plano ==
                    MatriculaPlano.Trimestral);
    }
}