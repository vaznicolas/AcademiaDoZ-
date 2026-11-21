
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class AcessoColaboradorService : IAcessoColaboradorService
{
    private readonly IColaboradorRepository _colaboradorRepository;
    private readonly IAcessoColaboradorRepository _acessoRepository;

    public AcessoColaboradorService(
        IColaboradorRepository colaboradorRepository,
        IAcessoColaboradorRepository acessoRepository)
    {
        _colaboradorRepository = colaboradorRepository;
        _acessoRepository = acessoRepository;
    }

    public async Task<(bool Sucesso, string Mensagem)> RegistrarEntrada(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        var colaborador = await _colaboradorRepository.ObterPorId(
            colaboradorId, cancellationToken);

        if (colaborador == null)
            return (false, "Colaborador não encontrado.");

        var acessoAberto = await _acessoRepository.ObterAberto(
            colaboradorId, cancellationToken);

        if (acessoAberto != null)
            return (false,
                "O colaborador já possui uma entrada aberta.");

        var agora = DateTime.Now;

        var limite = ObterLimiteDiario(colaborador.Vinculo);

        var acessos = await _acessoRepository.ObterTodosPorData(
            colaboradorId, agora.Date, cancellationToken);

        var tempoTrabalhado = SomarPermanencias(acessos);

        if (tempoTrabalhado >= limite)
        {
            return (false,
                $"Entrada negada para {colaborador.Nome}. " +
                $"A jornada diária de {limite.TotalHours:0} horas " +
                "já foi cumprida.");
        }

        var acesso = AcessoColaborador.Criar(
            0, colaborador, agora, null);

        await _acessoRepository.Adicionar(
            acesso, cancellationToken);

        return (true,
            $"Entrada registrada: {colaborador.Nome}. " +
            $"Vínculo: {colaborador.Vinculo}. " +
            $"Jornada diária: {limite.TotalHours:0} horas.");
    }

    public async Task<(bool Sucesso, string Mensagem)> RegistrarSaida(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        var acesso = await _acessoRepository.ObterAberto(
            colaboradorId, cancellationToken);

        if (acesso == null)
            return (false,
                "O colaborador não possui entrada aberta.");

        acesso.RegistrarSaida(DateTime.Now);

        await _acessoRepository.Atualizar(
            acesso, cancellationToken);

        var acessos = await _acessoRepository.ObterTodosPorData(
            colaboradorId,
            acesso.DataHoraEntrada.Date,
            cancellationToken);

        var tempoTotal = SomarPermanencias(acessos);

        var limite = ObterLimiteDiario(
            acesso.Colaborador.Vinculo);

        var tempoSessao = acesso.TempoPermanencia!.Value;

        var mensagem =
            $"Saída registrada: {acesso.Colaborador.Nome}. " +
            $"Permanência nesta sessão: {FormatarTempo(tempoSessao)}. " +
            $"Total trabalhado no dia: {FormatarTempo(tempoTotal)}. " +
            $"Jornada prevista: {FormatarTempo(limite)}.";

        if (tempoTotal > limite)
        {
            mensagem +=
                $" Jornada excedida em " +
                $"{FormatarTempo(tempoTotal - limite)}.";
        }
        else if (tempoTotal < limite)
        {
            mensagem +=
                $" Restam {FormatarTempo(limite - tempoTotal)} " +
                "para completar a jornada.";
        }
        else
        {
            mensagem += " Jornada diária completa.";
        }

        return (true, mensagem);
    }

    public Task<IEnumerable<AcessoColaborador>> ObterAcessosDoDia(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return _acessoRepository.ObterTodosPorData(
            colaboradorId, data, cancellationToken);
    }

    private static TimeSpan ObterLimiteDiario(
        ColaboradorVinculo vinculo)
    {
        return vinculo switch
        {
            ColaboradorVinculo.CLT =>
                TimeSpan.FromHours(8),

            ColaboradorVinculo.Estagio =>
                TimeSpan.FromHours(6),

            _ => throw new InvalidOperationException(
                "Vínculo do colaborador inválido.")
        };
    }

    private static TimeSpan SomarPermanencias(
        IEnumerable<AcessoColaborador> acessos)
    {
        var total = TimeSpan.Zero;

        foreach (var acesso in acessos)
        {
            if (acesso.TempoPermanencia.HasValue)
            {
                total += acesso.TempoPermanencia.Value;
            }
        }

        return total;
    }

    private static string FormatarTempo(TimeSpan tempo)
    {
        return $"{(int)tempo.TotalHours}h {tempo.Minutes:D2}min";
    }
}
