// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoColaboradorRepository
    : Repository<AcessoColaborador>, IAcessoColaboradorRepository
{
    public Task<IEnumerable<AcessoColaborador>> ObterPorColaborador(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoColaborador>>(
            _entities
                .Where(x => x.Colaborador.Id == colaboradorId)
                .ToList());
    }

    public Task<IEnumerable<AcessoColaborador>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoColaborador>>(
            _entities
                .Where(x => x.DataHoraEntrada >= inicio &&
                            x.DataHoraEntrada <= fim)
                .ToList());
    }

    public Task<AcessoColaborador?> ObterAberto(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _entities.FirstOrDefault(x =>
                x.Colaborador.Id == colaboradorId &&
                x.DataHoraSaida == null));
    }

    public Task<AcessoColaborador?> ObterPorData(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _entities.FirstOrDefault(x =>
                x.Colaborador.Id == colaboradorId &&
                x.DataHoraEntrada.Date == data.Date));
    }
}