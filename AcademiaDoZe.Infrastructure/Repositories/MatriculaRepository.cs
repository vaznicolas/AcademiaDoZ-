// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class MatriculaRepository
    : Repository<Matricula>, IMatriculaRepository
{
    public Task<IEnumerable<Matricula>> ObterPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Matricula>>(
            _entities
                .Where(x => x.Aluno.Id == alunoId)
                .ToList());
    }

    public Task<IEnumerable<Matricula>> ObterPorPlano(
        MatriculaPlano plano,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Matricula>>(
            _entities
                .Where(x => x.Plano == plano)
                .ToList());
    }

    public Task<IEnumerable<Matricula>> ObterPorPeriodo(
        DateOnly inicio,
        DateOnly fim,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Matricula>>(
            _entities
                .Where(x => x.DataInicio >= inicio &&
                            x.DataInicio <= fim)
                .ToList());
    }

    public Task<IEnumerable<Matricula>> ObterAtivas(
        CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        return Task.FromResult<IEnumerable<Matricula>>(
            _entities
                .Where(x => x.DataInicio <= hoje &&
                            x.DataFinal >= hoje)
                .ToList());
    }

    public Task<bool> AlunoPossuiMatriculaAtiva(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        var existe = _entities.Any(x =>
            x.Aluno.Id == alunoId &&
            x.DataInicio <= hoje &&
            x.DataFinal >= hoje);

        return Task.FromResult(existe);
    }
}