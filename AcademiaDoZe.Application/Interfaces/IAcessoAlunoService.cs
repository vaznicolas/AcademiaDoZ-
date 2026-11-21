
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAcessoAlunoService
{
    Task<(bool Sucesso, string Mensagem)> RegistrarEntrada(
        int alunoId,
        CancellationToken cancellationToken = default);

    Task<(bool Sucesso, string Mensagem)> RegistrarSaida(
        int alunoId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AcessoAluno>> ObterAcessosDoDia(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default);
}
