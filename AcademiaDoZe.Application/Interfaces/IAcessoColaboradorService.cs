
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAcessoColaboradorService
{
    Task<(bool Sucesso, string Mensagem)> RegistrarEntrada(
        int colaboradorId,
        CancellationToken cancellationToken = default);

    Task<(bool Sucesso, string Mensagem)> RegistrarSaida(
        int colaboradorId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AcessoColaborador>> ObterAcessosDoDia(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default);
}
