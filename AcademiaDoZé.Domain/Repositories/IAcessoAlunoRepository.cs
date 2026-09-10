// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoAlunoRepository : IRepository<AcessoAluno>
{
    Task<IEnumerable<AcessoAluno>> ObterPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AcessoAluno>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default);

    Task<AcessoAluno?> ObterAberto(
        int alunoId,
        CancellationToken cancellationToken = default);

    Task<AcessoAluno?> ObterPorData(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default);
}
