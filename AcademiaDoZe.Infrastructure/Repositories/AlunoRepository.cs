// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AlunoRepository : Repository<Aluno>, IAlunoRepository
{
    public Task<IEnumerable<Aluno>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default)
    {
        var alunos = _entities
            .Where(x =>
                x.Nome.Contains(
                    nome,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult<IEnumerable<Aluno>>(alunos);
    }
}