// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<IEnumerable<Aluno>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default);
}