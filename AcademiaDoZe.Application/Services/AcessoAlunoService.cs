
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class AcessoAlunoService : IAcessoAlunoService
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IAcessoAlunoRepository _acessoRepository;

    public AcessoAlunoService(
        IAlunoRepository alunoRepository,
        IMatriculaRepository matriculaRepository,
        IAcessoAlunoRepository acessoRepository)
    {
        _alunoRepository = alunoRepository;
        _matriculaRepository = matriculaRepository;
        _acessoRepository = acessoRepository;
    }

    public async Task<(bool Sucesso, string Mensagem)> RegistrarEntrada(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var aluno = await _alunoRepository.ObterPorId(
            alunoId, cancellationToken);

        if (aluno == null)
            return (false, "Aluno não encontrado.");

        var acessoAberto = await _acessoRepository.ObterAberto(
            alunoId, cancellationToken);

        if (acessoAberto != null)
            return (false, "O aluno já possui uma entrada aberta.");

        var matricula =
            await _matriculaRepository.ObterMatriculaAtivaPorAluno(
                alunoId, cancellationToken);

        if (matricula == null)
            return (false,
                $"Entrada negada para {aluno.Nome}. " +
                "O aluno não possui matrícula ativa.");

        var agora = DateTime.Now;

        if (matricula.DataInicio > DateOnly.FromDateTime(agora) ||
            matricula.DataFinal < DateOnly.FromDateTime(agora))
        {
            return (false,
                $"Entrada negada para {aluno.Nome}. " +
                "A matrícula não está vigente.");
        }

        var diasRestantes =
    matricula.DataFinal.DayNumber -
    DateOnly.FromDateTime(agora).DayNumber;

        var acesso = AcessoAluno.Criar(
            0, aluno, agora, null);

        await _acessoRepository.Adicionar(
            acesso, cancellationToken);

        return (true,
            $"Entrada registrada: {aluno.Nome}. " +
            $"Restam {diasRestantes} dias de plano.");
    }

    public async Task<(bool Sucesso, string Mensagem)> RegistrarSaida(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var acesso = await _acessoRepository.ObterAberto(
            alunoId, cancellationToken);

        if (acesso == null)
            return (false, "O aluno não possui entrada aberta.");

        acesso.RegistrarSaida(DateTime.Now);

        await _acessoRepository.Atualizar(
            acesso, cancellationToken);

        var tempo = acesso.TempoPermanencia!.Value;

        return (true,
            $"Saída registrada: {acesso.Aluno.Nome}. " +
            $"Permanência: {(int)tempo.TotalHours}h " +
            $"{tempo.Minutes:D2}min.");
    }

    public Task<IEnumerable<AcessoAluno>> ObterAcessosDoDia(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return _acessoRepository.ObterTodosPorData(
            alunoId, data, cancellationToken);
    }
}
