
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Domain.Tests.Infrastructure;

public class AcessoAlunoRepositoryEmMemoria
    : IAcessoAlunoRepository
{
    private readonly List<AcessoAluno> _acessos = new();

    public Task<AcessoAluno?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _acessos.FirstOrDefault(a => a.Id == id));
    }

    public Task<IEnumerable<AcessoAluno>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoAluno>>(
            _acessos.ToList());
    }

    public Task<AcessoAluno> Adicionar(
        AcessoAluno entity,
        CancellationToken cancellationToken = default)
    {
        _acessos.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<AcessoAluno> Atualizar(
        AcessoAluno entity,
        CancellationToken cancellationToken = default)
    {
        var indice = _acessos.FindIndex(
            a => a.Id == entity.Id);

        if (indice < 0)
            throw new InvalidOperationException(
                "Acesso não encontrado.");

        _acessos[indice] = entity;

        return Task.FromResult(entity);
    }

    public Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _acessos.RemoveAll(a => a.Id == id) > 0);
    }

    public Task<IEnumerable<AcessoAluno>> ObterPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoAluno>>(
            _acessos
                .Where(a => a.Aluno.Id == alunoId)
                .ToList());
    }

    public Task<IEnumerable<AcessoAluno>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoAluno>>(
            _acessos
                .Where(a => a.DataHoraEntrada >= inicio &&
                            a.DataHoraEntrada <= fim)
                .ToList());
    }

    public Task<AcessoAluno?> ObterAberto(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _acessos
                .Where(a => a.Aluno.Id == alunoId &&
                            a.DataHoraSaida == null)
                .OrderByDescending(a => a.DataHoraEntrada)
                .FirstOrDefault());
    }

    public Task<AcessoAluno?> ObterPorData(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _acessos.FirstOrDefault(a =>
                a.Aluno.Id == alunoId &&
                a.DataHoraEntrada.Date == data.Date));
    }

    public Task<IEnumerable<AcessoAluno>> ObterTodosPorData(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoAluno>>(
            _acessos
                .Where(a =>
                    a.Aluno.Id == alunoId &&
                    a.DataHoraEntrada.Date == data.Date)
                .OrderBy(a => a.DataHoraEntrada)
                .ToList());
    }
}
