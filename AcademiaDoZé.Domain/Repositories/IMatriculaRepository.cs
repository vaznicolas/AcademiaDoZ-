// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Repositories;

public interface IMatriculaRepository : IRepository<Matricula>
{
    Task<IEnumerable<Matricula>> ObterPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Matricula>> ObterPorPlano(
        MatriculaPlano plano,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Matricula>> ObterPorPeriodo(
        DateOnly inicio,
        DateOnly fim,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Matricula>> ObterAtivas(
        CancellationToken cancellationToken = default);

    Task<bool> AlunoPossuiMatriculaAtiva(
        int alunoId,
        CancellationToken cancellationToken = default);
}
