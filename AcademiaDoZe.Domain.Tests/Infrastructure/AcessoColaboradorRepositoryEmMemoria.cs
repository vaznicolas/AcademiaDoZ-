
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Domain.Tests.Infrastructure;

public class AcessoColaboradorRepositoryEmMemoria
    : IAcessoColaboradorRepository
{
    private readonly List<AcessoColaborador> _acessos = new();

    public Task<AcessoColaborador?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _acessos.FirstOrDefault(a => a.Id == id));
    }

    public Task<IEnumerable<AcessoColaborador>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoColaborador>>(
            _acessos.ToList());
    }

    public Task<AcessoColaborador> Adicionar(
        AcessoColaborador entity,
        CancellationToken cancellationToken = default)
    {
        _acessos.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<AcessoColaborador> Atualizar(
        AcessoColaborador entity,
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

    public Task<IEnumerable<AcessoColaborador>> ObterPorColaborador(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoColaborador>>(
            _acessos
                .Where(a => a.Colaborador.Id == colaboradorId)
                .ToList());
    }

    public Task<IEnumerable<AcessoColaborador>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoColaborador>>(
            _acessos
                .Where(a => a.DataHoraEntrada >= inicio &&
                            a.DataHoraEntrada <= fim)
                .ToList());
    }

    public Task<AcessoColaborador?> ObterAberto(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _acessos
                .Where(a => a.Colaborador.Id == colaboradorId &&
                            a.DataHoraSaida == null)
                .OrderByDescending(a => a.DataHoraEntrada)
                .FirstOrDefault());
    }

    public Task<AcessoColaborador?> ObterPorData(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _acessos.FirstOrDefault(a =>
                a.Colaborador.Id == colaboradorId &&
                a.DataHoraEntrada.Date == data.Date));
    }

    public Task<IEnumerable<AcessoColaborador>> ObterTodosPorData(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoColaborador>>(
            _acessos
                .Where(a =>
                    a.Colaborador.Id == colaboradorId &&
                    a.DataHoraEntrada.Date == data.Date)
                .OrderBy(a => a.DataHoraEntrada)
                .ToList());
    }
}
