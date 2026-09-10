// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoAlunoRepository : Repository<AcessoAluno>, IAcessoAlunoRepository
{
    public Task<IEnumerable<AcessoAluno>> ObterPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoAluno>>(
            _entities.Where(x => x.Aluno.Id == alunoId).ToList());
    }

    public Task<IEnumerable<AcessoAluno>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<AcessoAluno>>(
            _entities
                .Where(x => x.DataHoraEntrada >= inicio &&
                            x.DataHoraEntrada <= fim)
                .ToList());
    }

    public Task<AcessoAluno?> ObterAberto(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _entities.FirstOrDefault(x =>
                x.Aluno.Id == alunoId &&
                x.DataHoraSaida == null));
    }

    public Task<AcessoAluno?> ObterPorData(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _entities.FirstOrDefault(x =>
                x.Aluno.Id == alunoId &&
                x.DataHoraEntrada.Date == data.Date));
    }
}