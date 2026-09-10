// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoColaboradorRepository : IRepository<AcessoColaborador>
{
    Task<IEnumerable<AcessoColaborador>> ObterPorColaborador(
        int colaboradorId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AcessoColaborador>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default);

    Task<AcessoColaborador?> ObterAberto(
        int colaboradorId,
        CancellationToken cancellationToken = default);

    Task<AcessoColaborador?> ObterPorData(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default);
}